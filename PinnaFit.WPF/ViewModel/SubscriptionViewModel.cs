using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PinnaFit.Core;
using PinnaFit.Core.Common;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Extensions;
using PinnaFit.Core.Models;
using PinnaFit.Service;
using PinnaFit.Service.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace PinnaFit.WPF.ViewModel
{
    public class SubscriptionViewModel : ViewModelBase
    {
        #region Fields
        private static ISubscriptionService _subscriptionService;
        private IEnumerable<SubscriptionDTO> _businessPartners;
        private ObservableCollection<SubscriptionDTO> _filteredSubscriptions;
        private SubscriptionDTO _selectedSubscription;
        private ICommand _addNewSubscriptionViewCommand, _saveSubscriptionViewCommand, _deleteSubscriptionViewCommand;
        private string _searchText, _subscriptionText;
        #endregion

        #region Constructor
        public SubscriptionViewModel()
        {
            CleanUp();
            _subscriptionService = new SubscriptionService();

            FillSubscriptionTypes();
            //LoadDrivers();
            CheckRoles();
            GetLiveSubscriptions();

            SubscriptionText = "Training Durations";
        }
        public static void CleanUp()
        {
            if (_subscriptionService != null)
                _subscriptionService.Dispose();
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
                if (SubscriptionList != null)
                {
                    try
                    {
                        //if (!string.IsNullOrEmpty(SearchText))
                        //{
                        //    Subscriptions = new ObservableCollection<SubscriptionDTO>
                        //        (SubscriptionList.Where(c => c.SubscriptionDetail.ToLower().Contains(value.ToLower())).ToList());
                        //}
                        //else
                            Subscriptions = new ObservableCollection<SubscriptionDTO>(SubscriptionList);
                    }
                    catch
                    {
                        MessageBox.Show("Problem searching subscription");
                        Subscriptions = new ObservableCollection<SubscriptionDTO>(SubscriptionList);
                    }
                }
            }
        }
        public string SubscriptionText
        {
            get { return _subscriptionText; }
            set
            {
                _subscriptionText = value;
                RaisePropertyChanged<string>(() => SubscriptionText);
            }
        }

        public SubscriptionDTO SelectedSubscription
        {
            get { return _selectedSubscription; }
            set
            {
                _selectedSubscription = value;
                RaisePropertyChanged<SubscriptionDTO>(() => SelectedSubscription);
                if (SelectedSubscription != null)
                {
                    //if (SelectedSubscription.AssignedDriverId != null && SelectedSubscription.AssignedDriverId != 0)
                    //    SelectedDriver = Drivers.FirstOrDefault(d => d.Id == SelectedSubscription.AssignedDriverId);
                    //else
                    //    SelectedDriver = null;


                    if (SubscriptionTypeList != null)
                        SelectedSubscriptionType = SubscriptionTypeList.FirstOrDefault(s => s.Value == (int)SelectedSubscription.Type);
                }
            }
        }
        public IEnumerable<SubscriptionDTO> SubscriptionList
        {
            get { return _businessPartners; }
            set
            {
                _businessPartners = value;
                RaisePropertyChanged<IEnumerable<SubscriptionDTO>>(() => SubscriptionList);
            }
        }
        public ObservableCollection<SubscriptionDTO> Subscriptions
        {
            get { return _filteredSubscriptions; }
            set
            {
                _filteredSubscriptions = value;
                RaisePropertyChanged<ObservableCollection<SubscriptionDTO>>(() => Subscriptions);

                if (Subscriptions != null && Subscriptions.Any())
                    SelectedSubscription = Subscriptions.FirstOrDefault();
                else
                    AddNewSubscription();
            }
        }
        #endregion

        #region Filter List
        private List<ListDataItem> _subscriptionTypeList;
        private ListDataItem _selectedSubscriptionType;

        public List<ListDataItem> SubscriptionTypeList
        {
            get { return _subscriptionTypeList; }
            set
            {
                _subscriptionTypeList = value;
                RaisePropertyChanged<List<ListDataItem>>(() => SubscriptionTypeList);
            }
        }
        public ListDataItem SelectedSubscriptionType
        {
            get { return _selectedSubscriptionType; }
            set
            {
                _selectedSubscriptionType = value;
                RaisePropertyChanged<ListDataItem>(() => SelectedSubscriptionType);
            }
        }

        private List<ListDataItem> _subscriptionTypeListForFilter;
        private ListDataItem _selectedSubscriptionTypeForFilter;

        public List<ListDataItem> SubscriptionTypeListForFilter
        {
            get { return _subscriptionTypeListForFilter; }
            set
            {
                _subscriptionTypeListForFilter = value;
                RaisePropertyChanged<List<ListDataItem>>(() => SubscriptionTypeListForFilter);
            }
        }
        public ListDataItem SelectedSubscriptionTypeForFilter
        {
            get { return _selectedSubscriptionTypeForFilter; }
            set
            {
                _selectedSubscriptionTypeForFilter = value;
                RaisePropertyChanged<ListDataItem>(() => SelectedSubscriptionTypeForFilter);
                GetLiveSubscriptions();
            }
        }

        public void FillSubscriptionTypes()
        {
            SubscriptionTypeList = (List<ListDataItem>)CommonUtility.GetList(typeof(SubscriptionTypes));

            //if (subscriptionTypes != null && subscriptionTypes.Count > 1)
                //SubscriptionTypeList = subscriptionTypes.Skip(1).ToList();

            //SubscriptionTypeListForFilter = subscriptionTypes.ToList();
            //SelectedSubscriptionTypeForFilter = SubscriptionTypeListForFilter.FirstOrDefault();
        }


        #endregion

        #region Commands
        public ICommand AddNewSubscriptionViewCommand
        {
            get { return _addNewSubscriptionViewCommand ?? (_addNewSubscriptionViewCommand = new RelayCommand(AddNewSubscription)); }
        }
        private void AddNewSubscription()
        {
            //SelectedDriver = null;
            var firstOrDefault = SubscriptionTypeList.FirstOrDefault();
            if (firstOrDefault != null)
                SelectedSubscription = new SubscriptionDTO
                {
                    Type = (SubscriptionTypes)firstOrDefault.Value,
                    //    Number = _subscriptionService.GetSubscriptionCode()
                };
        }

        public ICommand SaveSubscriptionViewCommand
        {
            get { return _saveSubscriptionViewCommand ?? (_saveSubscriptionViewCommand = new RelayCommand<Object>(SaveSubscription, CanSave)); }
        }
        private void SaveSubscription(object obj)
        {
            try
            {
                //if (SelectedDriver != null)
                //    SelectedSubscription.AssignedDriverId = SelectedDriver.Id;

                var newObject = SelectedSubscription.Id;
                SelectedSubscription.Type = (SubscriptionTypes)SelectedSubscriptionType.Value;

                var stat = _subscriptionService.InsertOrUpdate(SelectedSubscription);
                if (stat != string.Empty)
                    MessageBox.Show("Can't save"
                                    + Environment.NewLine + stat, "Can't save", MessageBoxButton.OK,
                        MessageBoxImage.Error);

                else if (newObject == 0)
                    Subscriptions.Insert(0, SelectedSubscription);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't save"
                                  + Environment.NewLine + exception.Message, "Can't save", MessageBoxButton.OK,
                      MessageBoxImage.Error);
            }
        }

        public ICommand DeleteSubscriptionViewCommand
        {
            get { return _deleteSubscriptionViewCommand ?? (_deleteSubscriptionViewCommand = new RelayCommand<Object>(DeleteSubscription, CanSave)); }
        }
        private void DeleteSubscription(object obj)
        {
            if (MessageBox.Show("Are you Sure You want to Delete this Record?", "Pinna Fitness", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    SelectedSubscription.Enabled = false;
                    var stat = _subscriptionService.Disable(SelectedSubscription);
                    if (stat == string.Empty)
                    {
                        Subscriptions.Remove(SelectedSubscription);
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


        #endregion

        public void GetLiveSubscriptions()
        {
            var criteria = new SearchCriteria<SubscriptionDTO>();

            //var stafType = (SubscriptionTypes)SelectedSubscriptionTypeForFilter.Value;
            //if (stafType != SubscriptionTypes.All)
            //    criteria.FiList.Add(b => b.Type == stafType);

            SubscriptionList = _subscriptionService.GetAll(criteria).OrderBy(i => i.Id).ToList();

            Subscriptions = new ObservableCollection<SubscriptionDTO>(SubscriptionList);
        }

        //#region Assigned Drivers
        //private ObservableCollection<StaffDTO> _drivers;
        //public ObservableCollection<StaffDTO> Drivers
        //{
        //    get { return _drivers; }
        //    set
        //    {
        //        _drivers = value;
        //        RaisePropertyChanged<ObservableCollection<StaffDTO>>(() => Drivers);
        //    }
        //}

        //private StaffDTO _selectedDriver;
        //public StaffDTO SelectedDriver
        //{
        //    get { return _selectedDriver; }
        //    set
        //    {
        //        _selectedDriver = value;
        //        RaisePropertyChanged<StaffDTO>(() => SelectedDriver);
        //    }
        //}

        //public void LoadDrivers()
        //{
        //    var criteria = new SearchCriteria<StaffDTO>();
        //    criteria.FiList.Add(s => s.Type == StaffTypes.Driver || s.Type == StaffTypes.Multi);
        //    //criteria.FiList.Add(s=>s.AssignedSubscriptions.Count==0);
        //    //Can driver assigned on more than one subscription

        //    var drivers = new StaffService(true).GetAll(criteria)
        //        .Where(a => a.AssignedSubscriptions.Count == 0).ToList();

        //    Drivers = new ObservableCollection<StaffDTO>(drivers);
        //}
        //#endregion

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
