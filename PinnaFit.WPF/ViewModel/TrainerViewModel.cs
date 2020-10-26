using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using PinnaFit.Core;
using PinnaFit.Core.Common;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Models;
using PinnaFit.Service;
using PinnaFit.Service.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PinnaFit.WPF.Reports.DataSets;
using PinnaFit.WPF.Views;

namespace PinnaFit.WPF.ViewModel
{
    public class TrainerViewModel : ViewModelBase
    {
        #region Fields
        private static ITrainerService _trainerService;
        private IEnumerable<TrainerDTO> _trainers;
        private ObservableCollection<TrainerDTO> _filteredTrainers;
        private TrainerDTO _selectedTrainer;

        private ICommand _addNewTrainerViewCommand,
            _saveTrainerViewCommand,
            _deleteTrainerViewCommand,
            _trainerAddressViewCommand, _trainerSubscriptionViewCommand, _staffContactAddressViewCommand;
        private string _searchText, _trainerText;
        #endregion

        #region Constructor
        public TrainerViewModel()
        {
            CleanUp();
            _trainerService = new TrainerService();

            //FillTrainerTypes();
            CheckRoles();
            GetLiveTrainers();

            TrainerText = "Trainers/Staffs";
        }
        public static void CleanUp()
        {
            if (_trainerService != null)
                _trainerService.Dispose();
        }
        #endregion

        #region Public Properties
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                RaisePropertyChanged<string>(() => SearchText);
                if (TrainerList != null)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(SearchText))
                        {
                            Trainers = new ObservableCollection<TrainerDTO>
                                (TrainerList.Where(c => c.MemberDetail.ToLower().Contains(value.ToLower())).ToList());
                        }
                        else
                            Trainers = new ObservableCollection<TrainerDTO>(TrainerList);
                    }
                    catch
                    {
                        MessageBox.Show("Problem searching Trainer");
                        Trainers = new ObservableCollection<TrainerDTO>(TrainerList);
                    }
                }
            }
        }
        public string TrainerText
        {
            get { return _trainerText; }
            set
            {
                _trainerText = value;
                RaisePropertyChanged<string>(() => TrainerText);
            }
        }

        public TrainerDTO SelectedTrainer
        {
            get { return _selectedTrainer; }
            set
            {
                _selectedTrainer = value;
                RaisePropertyChanged<TrainerDTO>(() => SelectedTrainer);
                if (SelectedTrainer != null)
                {
                    //EmployeeShortImage = ImageUtil.ToImage(SelectedTrainer.ShortPhoto);// ??
                    //new BitmapImage(new Uri("../Resources/main.jpg",true));

                    //if (TrainerTypeList != null)
                    //    SelectedTrainerType = TrainerTypeList.FirstOrDefault(s => s.Value == (int)SelectedTrainer.Type);

                    TrainerAdressDetail = new ObservableCollection<AddressDTO>
                    {
                        SelectedTrainer.Address
                    };

                    ContactAdressDetail = new ObservableCollection<AddressDTO>();
                    if (SelectedTrainer.ContactPerson != null && SelectedTrainer.ContactPerson.Address != null)
                        ContactAdressDetail.Add(SelectedTrainer.ContactPerson.Address);

                    //LoadSubscriptions();
                }
            }
        }

        public IEnumerable<TrainerDTO> TrainerList
        {
            get { return _trainers; }
            set
            {
                _trainers = value;
                RaisePropertyChanged<IEnumerable<TrainerDTO>>(() => TrainerList);
            }
        }
        public ObservableCollection<TrainerDTO> Trainers
        {
            get { return _filteredTrainers; }
            set
            {
                _filteredTrainers = value;
                RaisePropertyChanged<ObservableCollection<TrainerDTO>>(() => Trainers);

                if (Trainers != null && Trainers.Any())
                    SelectedTrainer = Trainers.FirstOrDefault();
                else
                    AddNewTrainer();
            }
        }

        private ObservableCollection<AddressDTO> _trainerAddressDetail;
        public ObservableCollection<AddressDTO> TrainerAdressDetail
        {
            get { return _trainerAddressDetail; }
            set
            {
                _trainerAddressDetail = value;
                RaisePropertyChanged<ObservableCollection<AddressDTO>>(() => TrainerAdressDetail);
            }
        }

        private ObservableCollection<AddressDTO> _contactAddressDetail;
        public ObservableCollection<AddressDTO> ContactAdressDetail
        {
            get { return _contactAddressDetail; }
            set
            {
                _contactAddressDetail = value;
                RaisePropertyChanged<ObservableCollection<AddressDTO>>(() => ContactAdressDetail);
            }
        }

   
        #endregion

        #region Filter List
        private List<ListDataItem> _trainerTypeList;
        private ListDataItem _selectedTrainerType;

        public List<ListDataItem> TrainerTypeList
        {
            get { return _trainerTypeList; }
            set
            {
                _trainerTypeList = value;
                RaisePropertyChanged<List<ListDataItem>>(() => TrainerTypeList);
            }
        }
        public ListDataItem SelectedTrainerType
        {
            get { return _selectedTrainerType; }
            set
            {
                _selectedTrainerType = value;
                RaisePropertyChanged<ListDataItem>(() => SelectedTrainerType);
            }
        }

        private List<ListDataItem> _trainerTypeListForFilter;
        private ListDataItem _selectedTrainerTypeForFilter;

        public List<ListDataItem> TrainerTypeListForFilter
        {
            get { return _trainerTypeListForFilter; }
            set
            {
                _trainerTypeListForFilter = value;
                RaisePropertyChanged<List<ListDataItem>>(() => TrainerTypeListForFilter);
            }
        }
        public ListDataItem SelectedTrainerTypeForFilter
        {
            get { return _selectedTrainerTypeForFilter; }
            set
            {
                _selectedTrainerTypeForFilter = value;
                RaisePropertyChanged<ListDataItem>(() => SelectedTrainerTypeForFilter);
                GetLiveTrainers();
            }
        }

        //public void FillTrainerTypes()
        //{
        //    var trainerTypes = (List<ListDataItem>)CommonUtility.GetList(typeof(TrainerTypes));

        //    TrainerTypeListForFilter = trainerTypes.ToList();
        //    SelectedTrainerTypeForFilter = TrainerTypeListForFilter.FirstOrDefault();

        //    if (trainerTypes != null && trainerTypes.Count > 1)
        //    {
        //        TrainerTypeList = trainerTypes.Skip(1).ToList();
        //        SelectedTrainerType = TrainerTypeList.FirstOrDefault();
        //    }
        //}


        #endregion

        #region Commands
        public ICommand AddNewTrainerViewCommand
        {
            get { return _addNewTrainerViewCommand ?? (_addNewTrainerViewCommand = new RelayCommand(AddNewTrainer)); }
        }
        private void AddNewTrainer()
        {
            try
            {
                SelectedTrainer = new TrainerDTO
                {
                    //Number = _trainerService.GetTrainerCode(),
                    //Type = TrainerTypes.OfficeStaff,
                    DateOfBirth = DateTime.Now,
                    IsActive = true,
                    Sex = Sex.Male,
                    Address = new AddressDTO
                    {
                        Country = "ኢትዮጲያ",
                        City = "አዲስ አበባ"
                    },
                };
                SelectedTrainer.ContactPerson = new ContactPersonDTO()
                {
                    DisplayName = "-",
                    Address = new AddressDTO
                    {
                        Country = "ኢትዮጲያ",
                        City = "አዲስ አበባ"
                    }
                };
                EmployeeShortImage = new BitmapImage();
            }
            catch (Exception)
            {
                //MessageBox.Show("Problem on adding new Trainer");
            }
        }

        public ICommand SaveTrainerViewCommand
        {
            get { return _saveTrainerViewCommand ?? (_saveTrainerViewCommand = new RelayCommand<Object>(SaveTrainer, CanSave)); }
        }
        private void SaveTrainer(object obj)
        {
            try
            {
                //if (EmployeeShortImage != null && EmployeeShortImage.UriSource != null)
                    //SelectedTrainer.ShortPhoto = ImageUtil.ToBytes(EmployeeShortImage);

                var newObject = SelectedTrainer.Id;
                //SelectedTrainer.Type = (TrainerTypes)SelectedTrainerType.Value;

                var stat = _trainerService.InsertOrUpdate(SelectedTrainer);

                if (stat != string.Empty)
                    MessageBox.Show("Can't save"
                                    + Environment.NewLine + stat, "Can't save", MessageBoxButton.OK,
                        MessageBoxImage.Error);

                else if (newObject == 0)
                {
                    Trainers.Insert(0, SelectedTrainer);
                    SelectedTrainer.Number = _trainerService.GetTrainerNumber(SelectedTrainer.Id);
                    stat = _trainerService.InsertOrUpdate(SelectedTrainer);
                    if (stat != string.Empty)
                        MessageBox.Show("Can't save Number"
                                        + Environment.NewLine + stat, "Can't save", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't save"
                                  + Environment.NewLine + exception.Message, "Can't save", MessageBoxButton.OK,
                      MessageBoxImage.Error);
            }
        }


        public ICommand DeleteTrainerViewCommand
        {
            get { return _deleteTrainerViewCommand ?? (_deleteTrainerViewCommand = new RelayCommand<Object>(DeleteTrainer, CanSave)); }
        }
        private void DeleteTrainer(object obj)
        {
            if (MessageBox.Show("Are you Sure You want to Delete this Record?", "Pinna Fitness", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    SelectedTrainer.Enabled = false;
                    var stat = _trainerService.Disable(SelectedTrainer);
                    if (stat == string.Empty)
                    {
                        Trainers.Remove(SelectedTrainer);
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

        public ICommand TrainerAddressViewCommand
        {
            get { return _trainerAddressViewCommand ?? (_trainerAddressViewCommand = new RelayCommand(TrainerAddress)); }
        }
        public void TrainerAddress()
        {
            new AddressEntry(SelectedTrainer.Address).ShowDialog();
        }
        public ICommand StaffContactAddressViewCommand
        {
            get { return _staffContactAddressViewCommand ?? (_staffContactAddressViewCommand = new RelayCommand(StaffContactAddress)); }
        }
        public void StaffContactAddress()
        {
            if (SelectedTrainer != null && SelectedTrainer.ContactPerson != null && SelectedTrainer.ContactPerson.Address != null)
                new AddressEntry(SelectedTrainer.ContactPerson.Address).ShowDialog();
        }
        
        #endregion

        public void GetLiveTrainers()
        {
            var criteria = new SearchCriteria<TrainerDTO>();
            
            TrainerList = _trainerService.GetAll(criteria).OrderBy(i => i.Id).ToList();

            Trainers = new ObservableCollection<TrainerDTO>(TrainerList);
        }

        #region Short Photo
        private BitmapImage _employeeShortImage;
        private ICommand _showEmployeeShortImageCommand;

        public BitmapImage EmployeeShortImage
        {
            get { return _employeeShortImage; }
            set
            {
                _employeeShortImage = value;
                RaisePropertyChanged<BitmapImage>(() => EmployeeShortImage);

            }
        }
        public ICommand ShowEmployeeShortImageCommand
        {
            get
            {
                return _showEmployeeShortImageCommand ??
                       (_showEmployeeShortImageCommand = new RelayCommand(ExecuteShowEmployeeShortImageViewCommand));
            }
        }
        private void ExecuteShowEmployeeShortImageViewCommand()
        {
            var file = new OpenFileDialog { Filter = "Image Files(*.png;*.jpg; *.jpeg)|*.png;*.jpg; *.jpeg" };
            var result = file.ShowDialog();
            if (result != null && ((bool)result && File.Exists(file.FileName)))
            {
                EmployeeShortImage = new BitmapImage(new Uri(file.FileName, true));
            }
        }

        #endregion

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

            //var myReport4 = new PinnaFit.WPF.Reports.TrainerID();
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
            //var trainerNumberbarcode = ImageToByteArray(brCode.GetBarcode(SelectedTrainer.Id.ToString() + "_ABC850", 620, 50, false), ImageFormat.Bmp);

            //var trainerSub = TrainerSubscriptionDetail.FirstOrDefault();
            ////if (SelectedTrainer.LastSubscription != null)
            //myDataSet.TrainerDetail.Rows.Add(
            //    SelectedTrainer.DisplayName,
            //    SelectedTrainer.ShortPhoto,
            //    SelectedTrainer.Number,
            //    trainerNumberbarcode,
            //    SelectedTrainer.Sex,
            //    SelectedTrainer.Address.AddressDetail,
            //    "", "", "", "",
            //    SelectedTrainer.Address.Mobile,
            //    "", "", "", "",
            //    trainerSub != null ? trainerSub.SubscribedDateStringAndAmharic : null,
            //     trainerSub != null ? trainerSub.EndDateString : null,
            //     trainerSub != null ? trainerSub.FacilitySubscription.PackageName : null,
            //    "");

            ////SelectedTrainer.LastSubscription.SubscribedDateStringAndAmharic,
            ////        SelectedTrainer.LastSubscription.EndDateString,
            ////        SelectedTrainer.LastSubscription.FacilitySubscription.PackageName,

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
