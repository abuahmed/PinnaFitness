using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PinnaFit.Core;
using PinnaFit.Core.Common;
using PinnaFit.Core.Models;
using PinnaFit.Service;
using PinnaFit.Service.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace PinnaFit.WPF.ViewModel
{
    public class PackagesViewModel : ViewModelBase
    {
        #region Fields
        private static IFacilitySubscriptionService _subscriptionService;
        private IEnumerable<FacilitySubscriptionDTO> _businessPartners;
        private ObservableCollection<FacilitySubscriptionDTO> _filteredFacilitySubscriptions;
        private FacilitySubscriptionDTO _selectedFacilitySubscription;
        private ICommand _addNewFacilitySubscriptionViewCommand, _saveFacilitySubscriptionViewCommand, _deleteFacilitySubscriptionViewCommand;
        private string _subscriptionText;
        #endregion

        #region Constructor
        public PackagesViewModel()
        {
            CleanUp();
            _subscriptionService = new FacilitySubscriptionService();

            LoadFacilities();
            LoadSubscriptions();

            CheckRoles();
            GetLiveFacilitySubscriptions();

            FacilitySubscriptionText = "Packages Entry";
        }
        public static void CleanUp()
        {
            if (_subscriptionService != null)
                _subscriptionService.Dispose();
        }
        #endregion

        #region Public Properties
        public string FacilitySubscriptionText
        {
            get { return _subscriptionText; }
            set
            {
                _subscriptionText = value;
                RaisePropertyChanged<string>(() => FacilitySubscriptionText);
            }
        }

        public FacilitySubscriptionDTO SelectedFacilitySubscription
        {
            get { return _selectedFacilitySubscription; }
            set
            {
                _selectedFacilitySubscription = value;
                RaisePropertyChanged<FacilitySubscriptionDTO>(() => SelectedFacilitySubscription);
                if (SelectedFacilitySubscription != null)
                {
                    SelectedSubscription = SelectedFacilitySubscription.SubscriptionId != 0 ? Subscriptions.FirstOrDefault(d => d.Id == SelectedFacilitySubscription.SubscriptionId) : null;

                    SelectedFacility = SelectedFacilitySubscription.FacilityId != 0 ? Facilities.FirstOrDefault(d => d.Id == SelectedFacilitySubscription.FacilityId) : null;
                }
            }
        }
        public IEnumerable<FacilitySubscriptionDTO> FacilitySubscriptionList
        {
            get { return _businessPartners; }
            set
            {
                _businessPartners = value;
                RaisePropertyChanged<IEnumerable<FacilitySubscriptionDTO>>(() => FacilitySubscriptionList);
            }
        }
        public ObservableCollection<FacilitySubscriptionDTO> FacilitySubscriptions
        {
            get { return _filteredFacilitySubscriptions; }
            set
            {
                _filteredFacilitySubscriptions = value;
                RaisePropertyChanged<ObservableCollection<FacilitySubscriptionDTO>>(() => FacilitySubscriptions);

                //if (FacilitySubscriptions != null && FacilitySubscriptions.Any())
                //    SelectedFacilitySubscription = FacilitySubscriptions.FirstOrDefault();
                //else
                AddNewFacilitySubscription();
            }
        }
        #endregion


        #region Commands
        public ICommand AddNewFacilitySubscriptionViewCommand
        {
            get { return _addNewFacilitySubscriptionViewCommand ?? (_addNewFacilitySubscriptionViewCommand = new RelayCommand(AddNewFacilitySubscription)); }
        }
        private void AddNewFacilitySubscription()
        {
            SelectedSubscription = null;
            SelectedFacility = null;

            SelectedFacilitySubscription = new FacilitySubscriptionDTO();

            SelectedSubscription = Subscriptions.FirstOrDefault();
            SelectedFacility = Facilities.FirstOrDefault();
        }

        public ICommand SaveFacilitySubscriptionViewCommand
        {
            get { return _saveFacilitySubscriptionViewCommand ?? (_saveFacilitySubscriptionViewCommand = new RelayCommand<Object>(SaveFacilitySubscription, CanSave)); }
        }
        private void SaveFacilitySubscription(object obj)
        {
            try
            {
                var newObject = SelectedFacilitySubscription.Id;
                if (SelectedSubscription != null)
                    SelectedFacilitySubscription.SubscriptionId = SelectedSubscription.Id;
                if (SelectedFacility != null)
                    SelectedFacilitySubscription.FacilityId = SelectedFacility.Id;

                var stat = _subscriptionService.InsertOrUpdate(SelectedFacilitySubscription);
                if (stat != string.Empty)
                    MessageBox.Show("Can't save"
                                    + Environment.NewLine + stat, "Can't save", MessageBoxButton.OK,
                        MessageBoxImage.Error);

                else if (newObject == 0)
                    GetLiveFacilitySubscriptions();
                //FacilitySubscriptions.Insert(0, SelectedFacilitySubscription);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't save"
                                  + Environment.NewLine + exception.Message, "Can't save", MessageBoxButton.OK,
                      MessageBoxImage.Error);
            }
        }

        public ICommand DeleteFacilitySubscriptionViewCommand
        {
            get { return _deleteFacilitySubscriptionViewCommand ?? (_deleteFacilitySubscriptionViewCommand = new RelayCommand<Object>(DeleteFacilitySubscription, CanSave)); }
        }
        private void DeleteFacilitySubscription(object obj)
        {
            if (MessageBox.Show("Are you Sure You want to Delete this Record?", "Pinna Fitness", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    SelectedFacilitySubscription.Enabled = false;
                    var stat = _subscriptionService.Disable(SelectedFacilitySubscription);
                    if (stat == string.Empty)
                    {
                        FacilitySubscriptions.Remove(SelectedFacilitySubscription);
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

        public void GetLiveFacilitySubscriptions()
        {
            var criteria = new SearchCriteria<FacilitySubscriptionDTO>();

            FacilitySubscriptionList = _subscriptionService.GetAll(criteria).OrderBy(i => i.Id).ToList();

            FacilitySubscriptions = new ObservableCollection<FacilitySubscriptionDTO>(FacilitySubscriptionList);
        }

        #region Subscriptions and Facilities
        private ObservableCollection<SubscriptionDTO> _subscriptions;
        public ObservableCollection<SubscriptionDTO> Subscriptions
        {
            get { return _subscriptions; }
            set
            {
                _subscriptions = value;
                RaisePropertyChanged<ObservableCollection<SubscriptionDTO>>(() => Subscriptions);
            }
        }
        private SubscriptionDTO _selectedSubscription;
        public SubscriptionDTO SelectedSubscription
        {
            get { return _selectedSubscription; }
            set
            {
                _selectedSubscription = value;
                RaisePropertyChanged<SubscriptionDTO>(() => SelectedSubscription);
            }
        }
        public void LoadSubscriptions()
        {
            var criteria = new SearchCriteria<SubscriptionDTO>();
            var subs = new SubscriptionService(true).GetAll(criteria).ToList();
            Subscriptions = new ObservableCollection<SubscriptionDTO>(subs);
        }

        private ObservableCollection<FacilityDTO> _facilties;
        public ObservableCollection<FacilityDTO> Facilities
        {
            get { return _facilties; }
            set
            {
                _facilties = value;
                RaisePropertyChanged<ObservableCollection<FacilityDTO>>(() => Facilities);
            }
        }
        private FacilityDTO _selectedFacility;
        public FacilityDTO SelectedFacility
        {
            get { return _selectedFacility; }
            set
            {
                _selectedFacility = value;
                RaisePropertyChanged<FacilityDTO>(() => SelectedFacility);
            }
        }


        public void LoadFacilities()
        {
            var criteria = new SearchCriteria<FacilityDTO>();
            var subs = new FacilityService(true).GetAll(criteria).ToList();
            Facilities = new ObservableCollection<FacilityDTO>(subs);
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
