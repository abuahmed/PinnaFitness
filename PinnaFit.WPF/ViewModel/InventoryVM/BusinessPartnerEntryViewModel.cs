using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PinnaFit.Core;
using PinnaFit.Core.Common;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Models;
using PinnaFit.Service;
using PinnaFit.Service.Interfaces;
using PinnaFit.WPF.Reports.DataSets;
using PinnaFit.WPF.Views;

namespace PinnaFit.WPF.ViewModel
{
    public class BusinessPartnerEntryViewModel : ViewModelBase
    {
        #region Fields
        private static IBusinessPartnerService _businessPartnerService;
        private IEnumerable<BusinessPartnerDTO> _businessPartners;
        private ObservableCollection<BusinessPartnerDTO> _filteredBusinessPartners;
        private BusinessPartnerDTO _selectedBusinessPartner;

        private ICommand _addNewBusinessPartnerViewCommand,
            _saveBusinessPartnerViewCommand,
            _deleteBusinessPartnerViewCommand,
            _businessPartnerAddressViewCommand;
        private string _businessPartnerText;
        #endregion

        #region Constructor
        public BusinessPartnerEntryViewModel()
        {
            CleanUp();
            _businessPartnerService = new BusinessPartnerService();

            CheckRoles();
            GetLiveBusinessPartners();

            BusinessPartnerText = "አቅራቢዎች";
        }
        public static void CleanUp()
        {
            if (_businessPartnerService != null)
                _businessPartnerService.Dispose();
        }
        #endregion

        #region Public Properties
        
        public string BusinessPartnerText
        {
            get { return _businessPartnerText; }
            set
            {
                _businessPartnerText = value;
                RaisePropertyChanged<string>(() => BusinessPartnerText);
            }
        }

        public BusinessPartnerDTO SelectedBusinessPartner
        {
            get { return _selectedBusinessPartner; }
            set
            {
                _selectedBusinessPartner = value;
                RaisePropertyChanged<BusinessPartnerDTO>(() => SelectedBusinessPartner);
                if (SelectedBusinessPartner != null && !string.IsNullOrEmpty(SelectedBusinessPartner.BusinessPartnerDetail))
                {
                    //SelectedBusinessPartner.BusinessPartnerDetail = "";
                }
            }
        }
        public IEnumerable<BusinessPartnerDTO> BusinessPartnerList
        {
            get { return _businessPartners; }
            set
            {
                _businessPartners = value;
                RaisePropertyChanged<IEnumerable<BusinessPartnerDTO>>(() => BusinessPartnerList);
            }
        }
        public ObservableCollection<BusinessPartnerDTO> BusinessPartners
        {
            get { return _filteredBusinessPartners; }
            set
            {
                _filteredBusinessPartners = value;
                RaisePropertyChanged<ObservableCollection<BusinessPartnerDTO>>(() => BusinessPartners);

                //if (BusinessPartners != null && BusinessPartners.Any())
                //    SelectedBusinessPartner = BusinessPartners.FirstOrDefault();
                //else
                    AddNewBusinessPartner();
            }
        }
        
        #endregion

        #region Filter List
        private List<ListDataItem> _businessPartnerTypeList;
        private ListDataItem _selectedBusinessPartnerType;

        public List<ListDataItem> BusinessPartnerTypeList
        {
            get { return _businessPartnerTypeList; }
            set
            {
                _businessPartnerTypeList = value;
                RaisePropertyChanged<List<ListDataItem>>(() => BusinessPartnerTypeList);
            }
        }
        public ListDataItem SelectedBusinessPartnerType
        {
            get { return _selectedBusinessPartnerType; }
            set
            {
                _selectedBusinessPartnerType = value;
                RaisePropertyChanged<ListDataItem>(() => SelectedBusinessPartnerType);
            }
        }

        private List<ListDataItem> _businessPartnerTypeListForFilter;
        private ListDataItem _selectedBusinessPartnerTypeForFilter;

        public List<ListDataItem> BusinessPartnerTypeListForFilter
        {
            get { return _businessPartnerTypeListForFilter; }
            set
            {
                _businessPartnerTypeListForFilter = value;
                RaisePropertyChanged<List<ListDataItem>>(() => BusinessPartnerTypeListForFilter);
            }
        }
        public ListDataItem SelectedBusinessPartnerTypeForFilter
        {
            get { return _selectedBusinessPartnerTypeForFilter; }
            set
            {
                _selectedBusinessPartnerTypeForFilter = value;
                RaisePropertyChanged<ListDataItem>(() => SelectedBusinessPartnerTypeForFilter);
                GetLiveBusinessPartners();
            }
        }

        //public void FillBusinessPartnerTypes()
        //{
        //    var businessPartnerTypes = (List<ListDataItem>)CommonUtility.GetList(typeof(BusinessPartnerTypes));

        //    BusinessPartnerTypeListForFilter = businessPartnerTypes.ToList();
        //    SelectedBusinessPartnerTypeForFilter = BusinessPartnerTypeListForFilter.FirstOrDefault();

        //    if (businessPartnerTypes != null && businessPartnerTypes.Count > 1)
        //    {
        //        BusinessPartnerTypeList = businessPartnerTypes.Skip(1).ToList();
        //        SelectedBusinessPartnerType = BusinessPartnerTypeList.FirstOrDefault();
        //    }
        //}


        #endregion

        #region Commands
        public ICommand AddNewBusinessPartnerViewCommand
        {
            get { return _addNewBusinessPartnerViewCommand ?? (_addNewBusinessPartnerViewCommand = new RelayCommand(AddNewBusinessPartner)); }
        }
        private void AddNewBusinessPartner()
        {
            try
            {
                SelectedBusinessPartner = new BusinessPartnerDTO
                {
                    BusinessPartnerType = BusinessPartnerTypes.Supplier,
                    Category = BusinessPartnerCategory.Organization,
                    Address = new AddressDTO
                    {
                        Country = "ኢትዮጲያ",
                        City = "አዲስ አበባ"
                    },
                };
                
            }
            catch (Exception)
            {
                //MessageBox.Show("Problem on adding new BusinessPartner");
            }
        }

        public ICommand SaveBusinessPartnerViewCommand
        {
            get { return _saveBusinessPartnerViewCommand ?? (_saveBusinessPartnerViewCommand = new RelayCommand<Object>(SaveBusinessPartner, CanSave)); }
        }
        private void SaveBusinessPartner(object obj)
        {
            try
            {
            
                var newObject = SelectedBusinessPartner.Id;

                var stat = _businessPartnerService.InsertOrUpdate(SelectedBusinessPartner);

                if (stat != string.Empty)
                    MessageBox.Show("Can't save"
                                    + Environment.NewLine + stat, "Can't save", MessageBoxButton.OK,
                        MessageBoxImage.Error);

                else if (newObject == 0)
                {
                    BusinessPartners.Insert(0, SelectedBusinessPartner);
                    SelectedBusinessPartner.Number = _businessPartnerService.GetBusinessPartnerNumber(SelectedBusinessPartner.Id);
                    stat = _businessPartnerService.InsertOrUpdate(SelectedBusinessPartner);
                    if (stat != string.Empty)
                        MessageBox.Show("Can't save Number"
                                        + Environment.NewLine + stat, "Can't save", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    else CloseWindow(obj);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't save"
                                + Environment.NewLine + exception.Message, "Can't save", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        public ICommand DeleteBusinessPartnerViewCommand
        {
            get { return _deleteBusinessPartnerViewCommand ?? (_deleteBusinessPartnerViewCommand = new RelayCommand<Object>(DeleteBusinessPartner, CanSave)); }
        }
        private void DeleteBusinessPartner(object obj)
        {
            if (MessageBox.Show("Are you Sure You want to Delete this Record?", "Pinna Fitness", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    SelectedBusinessPartner.Enabled = false;
                    var stat = _businessPartnerService.Disable(SelectedBusinessPartner);
                    if (stat == string.Empty)
                    {
                        BusinessPartners.Remove(SelectedBusinessPartner);
                    }
                    else
                    {
                        MessageBox.Show("Can't Delete, may be the data is already in use..."
                                        + Environment.NewLine + stat, "Can't Delete",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Can't Delete, may be the data is already in use..."
                                    + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, "Can't Delete",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public ICommand BusinessPartnerAddressViewCommand
        {
            get { return _businessPartnerAddressViewCommand ?? (_businessPartnerAddressViewCommand = new RelayCommand(BusinessPartnerAddress)); }
        }
        public void BusinessPartnerAddress()
        {
            new AddressEntry(SelectedBusinessPartner.Address).ShowDialog();
        }

        public void CloseWindow(object obj)
        {
            if (obj != null)
            {
                var window = obj as Window;
                if (window != null)
                {
                    window.DialogResult = true;
                    window.Close();
                }
            }
        }
        #endregion

        public void GetLiveBusinessPartners()
        {
            var criteria = new SearchCriteria<BusinessPartnerDTO>();

            BusinessPartnerList = _businessPartnerService.GetAll(criteria).OrderBy(i => i.Id).ToList();

            BusinessPartners = new ObservableCollection<BusinessPartnerDTO>(BusinessPartnerList);
        }
        

        #region Print List
        private ICommand _printListCommandView;

        public ICommand PrintListCommandView
        {
            get
            {
                return _printListCommandView ?? (_printListCommandView = new RelayCommand<Object>(PrintList));
            }
        }
        public void PrintList(object obj)
        {

            //var myReport4 = new PinnaFit.WPF.Reports.BusinessPartnerID();
            //myReport4.SetDataSource(GetListDataSet());

            ////MenuItem menu = obj as MenuItem;
            ////if (menu != null)
            ////    new ReportUtility().DirectPrinter(myReport4);
            ////else
            ////{
            //var report = new ReportViewerCommon(myReport4);
            //report.ShowDialog();
            ////}
        }
        public FitnessDataSet GetListDataSet()
        {
            var myDataSet = new FitnessDataSet();

            //var brCode = new BarcodeProcess();
            //var businessPartnerNumberbarcode = ImageToByteArray(brCode.GetBarcode(SelectedBusinessPartner.Id.ToString() + "_ABC850", 620, 50, false), ImageFormat.Bmp);

            //var businessPartnerSub = BusinessPartnerSubscriptionDetail.FirstOrDefault();
            ////if (SelectedBusinessPartner.LastSubscription != null)
            //myDataSet.BusinessPartnerDetail.Rows.Add(
            //    SelectedBusinessPartner.DisplayName,
            //    SelectedBusinessPartner.ShortPhoto,
            //    SelectedBusinessPartner.Number,
            //    businessPartnerNumberbarcode,
            //    SelectedBusinessPartner.Sex,
            //    SelectedBusinessPartner.Address.AddressDetail,
            //    "", "", "", "",
            //    SelectedBusinessPartner.Address.Mobile,
            //    "", "", "", "",
            //    businessPartnerSub != null ? businessPartnerSub.SubscribedDateStringAndAmharic : null,
            //     businessPartnerSub != null ? businessPartnerSub.EndDateString : null,
            //     businessPartnerSub != null ? businessPartnerSub.FacilitySubscription.PackageName : null,
            //    "");

            ////SelectedBusinessPartner.LastSubscription.SubscribedDateStringAndAmharic,
            ////        SelectedBusinessPartner.LastSubscription.EndDateString,
            ////        SelectedBusinessPartner.LastSubscription.FacilitySubscription.PackageName,

            return myDataSet;
        }
        public byte[] ImageToByteArray(Image imageIn, ImageFormat format)
        {
            var ms = new MemoryStream();
            imageIn.Save(ms, format);
            return ms.ToArray();
        }

        #endregion

        #region Validation
        public static int Errors { get; set; }
        public bool CanSave(object parameter)
        {
            return Errors == 0;
        }
        #endregion

        #region Previlege Visibility
        private UserRolesModel _userRoles;

        public UserRolesModel UserRoles
        {
            get { return _userRoles; }
            set
            {
                _userRoles = value;
                RaisePropertyChanged<UserRolesModel>(() => UserRoles);
            }
        }

        private void CheckRoles()
        {
            UserRoles = Singleton.UserRoles;
        }

        #endregion
    }
}