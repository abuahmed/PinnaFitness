#region

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PinnaFit.Core;
using PinnaFit.Core.Common;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Models;
using PinnaFit.Service;
using PinnaFit.Service.Interfaces;
using PinnaFit.WPF.Views;

#endregion

namespace PinnaFit.WPF.ViewModel
{
    public class MemberSubscriptionViewModel : ViewModelBase
    {
        #region Fields

        private static IMemberSubscriptionService _memberSubscriptionService;
        private static IMemberService _memberService;
        private MemberSubscriptionDTO _selectedMemberSubscription;

        private ICommand _addNewMemberSubscriptionViewCommand,
            _saveMemberSubscriptionViewCommand,
            _deleteMemberSubscriptionViewCommand,
            _memberSubscribedDateViewCommand,
            _memberStartDateViewCommand,
            _memberEndDateViewCommand,
            _selectedSubscribedDateChangedCommand;

        private string _subscriptionText;
        private MemberDTO _selectedMember;
        private bool _isRenewal, _subscriptionDateEnability;
        private DateTime _selectedSubscribedTime;
        #endregion

        #region Constructor

        public MemberSubscriptionViewModel()
        {
            CleanUp();
            _memberSubscriptionService = new MemberSubscriptionService();
            _memberService = new MemberService();

            LoadPackages();
            CheckRoles();

            MemberSubscriptionText = "ፓኬጅ ማስገቢያ/ማስተካከያ";

            Messenger.Default.Register<bool>(this, (message) => { IsRenewal = message; });

            Messenger.Default.Register<MemberDTO>(this, (message) => { SelectedMember = message; });
        }

        public static void CleanUp()
        {
            if (_memberSubscriptionService != null)
                _memberSubscriptionService.Dispose();
            if (_memberService != null)
                _memberService.Dispose();
        }

        #endregion

        #region Public Properties

        public bool IsRenewal
        {
            get { return _isRenewal; }
            set
            {
                _isRenewal = value;
                RaisePropertyChanged<bool>(() => IsRenewal);
            }
        }
        public DateTime SelectedSubscribedTime
        {
            get { return _selectedSubscribedTime; }
            set
            {
                _selectedSubscribedTime = value;
                RaisePropertyChanged<DateTime>(() => SelectedSubscribedTime);
            }
        }
        
        public MemberDTO SelectedMember
        {
            get { return _selectedMember; }
            set
            {
                _selectedMember = value;
                RaisePropertyChanged<MemberDTO>(() => SelectedMember);

                LoadSubscription();
            }
        }

        public void LoadSubscription()
        {
            int? prevSubId = null;
            var criteria = new SearchCriteria<MemberSubscriptionDTO>();
            criteria.FiList.Add(m => m.Id == SelectedMember.LastSubscriptionId);

            var lastSubscription = _memberSubscriptionService.GetAll(criteria).FirstOrDefault();

            if (lastSubscription != null && (IsRenewal))
            {
                prevSubId = lastSubscription.Id;
                lastSubscription = null;
                MemberSubscriptionText = "ፓኬጅ ማደሻ";
            }

            SelectedMemberSubscription = lastSubscription ?? new MemberSubscriptionDTO
            {
                PreviousSuscrptionId = prevSubId,
                MemberId = SelectedMember.Id,
                EndDate = DateTime.Now.AddMonths(1),
                StartDate = DateTime.Now,
                SubscribedDate = DateTime.Now,
                ReceiptDate = DateTime.Now,
            };
            
        }

        public string MemberSubscriptionText
        {
            get { return _subscriptionText; }
            set
            {
                _subscriptionText = value;
                RaisePropertyChanged<string>(() => MemberSubscriptionText);
            }
        }
        public bool SubscriptionDateEnability
        {
            get { return _subscriptionDateEnability; }
            set
            {
                _subscriptionDateEnability = value;
                RaisePropertyChanged<bool>(() => SubscriptionDateEnability);
            }
        }
        
        public MemberSubscriptionDTO SelectedMemberSubscription
        {
            get { return _selectedMemberSubscription; }
            set
            {
                _selectedMemberSubscription = value;
                RaisePropertyChanged<MemberSubscriptionDTO>(() => SelectedMemberSubscription);
                if (SelectedMemberSubscription != null)
                {
                    SelectedPackage = SelectedMemberSubscription.FacilitySubscriptionId != 0
                        ? Packages.FirstOrDefault(d => d.Id == SelectedMemberSubscription.FacilitySubscriptionId)
                        : null;

                    if (SelectedMemberSubscription.StartDate != null)
                        StartDate = (DateTime) SelectedMemberSubscription.StartDate.Value;

                    SelectedSubscribedTime = SelectedMemberSubscription.SubscribedDate;
                    
                    //SubscribedDate = (DateTime)SelectedMemberSubscription.SubscribedDate;
                }
            }
        }

        #endregion

        #region Commands

        public ICommand AddNewMemberSubscriptionViewCommand
        {
            get
            {
                return _addNewMemberSubscriptionViewCommand ??
                       (_addNewMemberSubscriptionViewCommand = new RelayCommand(AddNewMemberSubscription));
            }
        }

        private void AddNewMemberSubscription()
        {
            SelectedPackage = null;
            SelectedMemberSubscription = new MemberSubscriptionDTO();
            SelectedPackage = Packages.FirstOrDefault();
        }

        public ICommand SaveMemberSubscriptionViewCommand
        {
            get
            {
                return _saveMemberSubscriptionViewCommand ??
                       (_saveMemberSubscriptionViewCommand = new RelayCommand<Object>(SaveMemberSubscription, CanSave));
            }
        }

        private void SaveMemberSubscription(object obj)
        {
            try
            {
                //var newObject = SelectedMemberSubscription.Id;
                if (SelectedPackage != null)
                {
                    SelectedMemberSubscription.FacilitySubscriptionId = SelectedPackage.Id;
                    ////SelectedMemberSubscription.AmountPaid = SelectedPackage.Amount;
                    SelectedMemberSubscription.StartDate = StartDate;
                    ////SelectedMemberSubscription.SubscribedDate = StartDate;
                    var subcDate = SelectedMemberSubscription.SubscribedDate;
                    SelectedMemberSubscription.SubscribedDate = new DateTime(subcDate.Year, subcDate.Month, subcDate.Day,
                       SelectedSubscribedTime.Hour, SelectedSubscribedTime.Minute, SelectedSubscribedTime.Second);
                }

                var stat = _memberSubscriptionService.InsertOrUpdate(SelectedMemberSubscription);
                if (stat != string.Empty)
                    MessageBox.Show("Can't save"
                                    + Environment.NewLine + stat, "Can't save", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                else
                {
                    //var meber = _memberService.Find(SelectedMemberSubscription.MemberId.ToString());
                    //meber.LastSubscriptionId = SelectedMemberSubscription.Id;
                    //var sta = _memberService.InsertOrUpdate(meber);
                    //if (sta != string.Empty)
                    //    MessageBox.Show("Can't save"
                    //                    + Environment.NewLine + stat,
                    //        "Can't attach package with the member, try again later", MessageBoxButton.OK,
                    //        MessageBoxImage.Error);
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

        public ICommand DeleteMemberSubscriptionViewCommand
        {
            get
            {
                return _deleteMemberSubscriptionViewCommand ??
                       (_deleteMemberSubscriptionViewCommand =
                           new RelayCommand<Object>(DeleteMemberSubscription, CanSave));
            }
        }

        private void DeleteMemberSubscription(object obj)
        {
            if (
                MessageBox.Show("Are you Sure You want to Delete this Record?", "Pinna Fitness",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    SelectedMemberSubscription.Enabled = false;
                    var stat = _memberSubscriptionService.Disable(SelectedMemberSubscription);
                    if (stat == string.Empty)
                    {
                        //MemberSubscriptions.Remove(SelectedMemberSubscription);
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

        public ICommand MemberSubscribedDateViewCommand
        {
            get
            {
                return _memberSubscribedDateViewCommand ??
                       (_memberSubscribedDateViewCommand = new RelayCommand(MemberSubscription));
            }
        }

        public void MemberSubscription()
        {
            var calConv = new CalendarConvertor(SelectedMemberSubscription.SubscribedDate);
            calConv.ShowDialog();
            var dialogueResult = calConv.DialogResult;
            if (dialogueResult != null && (bool) dialogueResult)
            {
                if (calConv.DtSelectedDate.SelectedDate != null)
                    SelectedMemberSubscription.SubscribedDate = (DateTime) calConv.DtSelectedDate.SelectedDate;
            }
        }

        public ICommand MemberStartDateViewCommand
        {
            get
            {
                return _memberStartDateViewCommand ??
                       (_memberStartDateViewCommand = new RelayCommand(MemberStartDate));
            }
        }

        public void MemberStartDate()
        {
            if (SelectedMemberSubscription.StartDate != null)
            {
                var calConv = new CalendarConvertor(SelectedMemberSubscription.StartDate.Value);
                calConv.ShowDialog();
                var dialogueResult = calConv.DialogResult;
                if (dialogueResult != null && (bool) dialogueResult)
                {
                    if (calConv.DtSelectedDate.SelectedDate != null)
                        StartDate = (DateTime) calConv.DtSelectedDate.SelectedDate;
                }
            }
        }

        public ICommand MemberEndDateViewCommand
        {
            get
            {
                return _memberEndDateViewCommand ??
                       (_memberEndDateViewCommand = new RelayCommand(MemberEndDate));
            }
        }

        public void MemberEndDate()
        {
            if (SelectedMemberSubscription.EndDate != null)
            {
                var calConv = new CalendarConvertor(SelectedMemberSubscription.EndDate.Value);
                calConv.ShowDialog();
                var dialogueResult = calConv.DialogResult;
                if (dialogueResult != null && (bool) dialogueResult)
                {
                    if (calConv.DtSelectedDate.SelectedDate != null)
                        SelectedMemberSubscription.EndDate = (DateTime) calConv.DtSelectedDate.SelectedDate;
                }
            }
        }

        public ICommand SelectedSubscribedDateChangedCommand
        {
            get
            {
                return _selectedSubscribedDateChangedCommand ??
                       (_selectedSubscribedDateChangedCommand = new RelayCommand(SelectedSubscribedDateChanged));
            }
        }

        private void SelectedSubscribedDateChanged()
        {
            //StartDate = SubscribedDate;
        }

        #endregion

        #region Packages

        private ObservableCollection<FacilitySubscriptionDTO> _packages;
        private FacilitySubscriptionDTO _selectedPackage;
        private DateTime _startDate, _subscribedDate;

        public ObservableCollection<FacilitySubscriptionDTO> Packages
        {
            get { return _packages; }
            set
            {
                _packages = value;
                RaisePropertyChanged<ObservableCollection<FacilitySubscriptionDTO>>(() => Packages);
            }
        }

        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                RaisePropertyChanged<DateTime>(() => StartDate);

                //if (StartDate == null) return;
                SelectedMemberSubscription.StartDate = StartDate;
                if (SelectedPackage != null && SelectedPackage.Subscription != null)
                    switch (SelectedPackage.Subscription.Type)
                    {
                        case SubscriptionTypes.Daily:
                            SelectedMemberSubscription.EndDate =
                                StartDate.AddDays(SelectedPackage.Subscription.Value).AddDays(-1);
                            break;
                        case SubscriptionTypes.Monthy:
                            SelectedMemberSubscription.EndDate =
                                StartDate.AddMonths(SelectedPackage.Subscription.Value).AddDays(-1);
                            break;
                    }
            }
        }

        //public DateTime SubscribedDate
        //{
        //    get { return _subscribedDate; }
        //    set
        //    {
        //        _subscribedDate = value;
        //        RaisePropertyChanged<DateTime>(() => SubscribedDate);
        //        SelectedMemberSubscription.SubscribedDate = SubscribedDate;
        //    }
        //}

        public FacilitySubscriptionDTO SelectedPackage
        {
            get { return _selectedPackage; }
            set
            {
                _selectedPackage = value;
                RaisePropertyChanged<FacilitySubscriptionDTO>(() => SelectedPackage);
                if (SelectedPackage != null && SelectedPackage.Subscription != null)
                {
                    if (SelectedMemberSubscription.StartDate != null)
                        StartDate = SelectedMemberSubscription.StartDate.Value;
                    
                    if (SelectedMemberSubscription.Id==0)
                    SelectedMemberSubscription.AmountPaid = SelectedPackage.Amount;
                }
            }
        }

        public void LoadPackages()
        {
            var criteria = new SearchCriteria<FacilitySubscriptionDTO>();
            criteria.FiList.Add(f => f.Subscription.Type != SubscriptionTypes.Daily);

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
            SubscriptionDateEnability = UserRoles.PackageEdit == "Visible";
        }

        #endregion
    }
}