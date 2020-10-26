#region

using System;
using System.Collections.ObjectModel;
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

#endregion

namespace PinnaFit.WPF.ViewModel
{
    public class PervisitSubscriptionViewModel : ViewModelBase
    {
        #region Fields

        private static IPervisitSubscriptionService _pervisitSubscriptionService;
        private static IMemberService _memberService;
        private PervisitSubscriptionDTO _selectedPervisitSubscription;

        private ICommand _addNewPervisitSubscriptionViewCommand,
            _savePervisitSubscriptionViewCommand,
            _deletePervisitSubscriptionViewCommand;

        private string _subscriptionText;

        #endregion

        #region Constructor

        public PervisitSubscriptionViewModel()
        {
            CleanUp();
            _pervisitSubscriptionService = new PervisitSubscriptionService();
            _memberService = new MemberService();

            LoadPackages();
            CheckRoles();

            PervisitSubscriptionText = "Pervisit Entry";
            //Messenger.Default.Register<MemberDTO>(this, (message) => { SelectedMember = message; });
            AddNewPervisitSubscription();
        }

        public static void CleanUp()
        {
            if (_pervisitSubscriptionService != null)
                _pervisitSubscriptionService.Dispose();
            if (_memberService != null)
                _memberService.Dispose();
        }

        #endregion

        #region Public Properties

        public string PervisitSubscriptionText
        {
            get { return _subscriptionText; }
            set
            {
                _subscriptionText = value;
                RaisePropertyChanged<string>(() => PervisitSubscriptionText);
            }
        }

        public PervisitSubscriptionDTO SelectedPervisitSubscription
        {
            get { return _selectedPervisitSubscription; }
            set
            {
                _selectedPervisitSubscription = value;
                RaisePropertyChanged<PervisitSubscriptionDTO>(() => SelectedPervisitSubscription);
                if (SelectedPervisitSubscription != null)
                {
                    ////SelectedPackage = SelectedPervisitSubscription.FacilitySubscriptionId != 0
                    ////    ? Packages.FirstOrDefault(d => d.Id == SelectedPervisitSubscription.FacilitySubscriptionId)
                    ////    : null;

                    //if (SelectedPervisitSubscription.StartDate != null)
                    //    StartDate = (DateTime) SelectedPervisitSubscription.StartDate.Value;

                    ////SubscribedDate = (DateTime)SelectedPervisitSubscription.SubscribedDate;
                }
            }
        }

        #endregion

        #region Commands

        public ICommand AddNewPervisitSubscriptionViewCommand
        {
            get
            {
                return _addNewPervisitSubscriptionViewCommand ??
                       (_addNewPervisitSubscriptionViewCommand = new RelayCommand(AddNewPervisitSubscription));
            }
        }

        private void AddNewPervisitSubscription()
        {
            SelectedPackage = null;

            SelectedPervisitSubscription = new PervisitSubscriptionDTO()
            {
                Sex = Sex.Male,
                CheckedInTime = DateTime.Now,
                ReceiptDate = DateTime.Now,
                ReceiptNumber = "0000"
            };

            SelectedPackage = Packages.FirstOrDefault();
        }

        public ICommand SavePervisitSubscriptionViewCommand
        {
            get
            {
                return _savePervisitSubscriptionViewCommand ??
                       (_savePervisitSubscriptionViewCommand =
                           new RelayCommand<Object>(SavePervisitSubscription, CanSave));
            }
        }

        private void SavePervisitSubscription(object obj)
        {
            try
            {
                //var newObject = SelectedPervisitSubscription.Id;
                if (SelectedPackage == null)
                    return;
                
                SelectedPervisitSubscription.FacilitySubscriptionId = SelectedPackage.Id;
                SelectedPervisitSubscription.AmountPaid = SelectedPackage.Amount;
                
                var stat = _pervisitSubscriptionService.InsertOrUpdate(SelectedPervisitSubscription);
                if (stat != string.Empty)
                    MessageBox.Show("Can't save"
                                    + Environment.NewLine + stat, "Can't save", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                else
                {
                    CloseWindow(obj);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't save"
                                + Environment.NewLine + exception.Message, "Can't save", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        public void CloseWindow(object obj)
        {
            if (obj == null) return;
            var window = obj as Window;
            if (window != null)
            {
                window.DialogResult = true;
                window.Close();
            }
        }

        public ICommand DeletePervisitSubscriptionViewCommand
        {
            get
            {
                return _deletePervisitSubscriptionViewCommand ??
                       (_deletePervisitSubscriptionViewCommand =
                           new RelayCommand<Object>(DeletePervisitSubscription, CanSave));
            }
        }

        private void DeletePervisitSubscription(object obj)
        {
            if (
                MessageBox.Show("Are you Sure You want to Delete this Record?",
                    "Pinna Fitness",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    SelectedPervisitSubscription.Enabled = false;
                    var stat = _pervisitSubscriptionService.Disable(SelectedPervisitSubscription);
                    if (stat == string.Empty)
                    {
                        //PervisitSubscriptions.Remove(SelectedPervisitSubscription);
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
                                    + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException,
                        "Can't Delete",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        #endregion

        #region Packages

        private ObservableCollection<FacilitySubscriptionDTO> _packages;
        private FacilitySubscriptionDTO _selectedPackage;

        public ObservableCollection<FacilitySubscriptionDTO> Packages
        {
            get { return _packages; }
            set
            {
                _packages = value;
                RaisePropertyChanged<ObservableCollection<FacilitySubscriptionDTO>>(() => Packages);
            }
        }

        public FacilitySubscriptionDTO SelectedPackage
        {
            get { return _selectedPackage; }
            set
            {
                _selectedPackage = value;
                RaisePropertyChanged<FacilitySubscriptionDTO>(() => SelectedPackage);
                if (SelectedPackage != null && SelectedPackage.Subscription != null)
                {
                }
            }
        }

        public void LoadPackages()
        {
            var criteria = new SearchCriteria<FacilitySubscriptionDTO>();
            criteria.FiList.Add(f => f.Subscription.Type == SubscriptionTypes.Daily);

            var subs = new FacilitySubscriptionService(true).GetAll(criteria).ToList(); //.OrderBy(f=>f.PackageName)
            Packages = new ObservableCollection<FacilitySubscriptionDTO>(subs);
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