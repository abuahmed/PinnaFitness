using System;
using PinnaFit.Core;
using PinnaFit.Core.Common;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace PinnaFit.WPF.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private string _headerText, _titleText;
        readonly static DashBoardViewModel DashBoardViewModel = new ViewModelLocator().DashBoard;
        readonly static AttendanceListViewModel AttendanceListViewModel = new ViewModelLocator().AttendanceList;

        private ViewModelBase _currentViewModel;

        public MainViewModel()
        {
            CheckRoles();
            TitleText = "PinnaFit V1.0.0.1, Gym and Fitness Management System - " +
                        Singleton.User.UserName + " - " +
                        DateTime.Now.ToString("dd/MM/yyyy") + " - " +
                        ReportUtility.GetEthCalendarFormated(DateTime.Now, "/");
            if (UserRoles.DashboardMgmt == "Visible")
            {
                HeaderText = "Dashboard";
                CurrentViewModel = DashBoardViewModel;
            }
            else
            {
                HeaderText = "ስራዎች ማስተዳደሪያ";
                CurrentViewModel = AttendanceListViewModel;
            }

            DeliveryViewModelViewCommand = new RelayCommand(ExecuteDeliveryViewModelViewCommand);
            FollowUpViewModelViewCommand = new RelayCommand(ExecuteFollowUpViewModelViewCommand);
        }

        public ViewModelBase CurrentViewModel
        {
            get
            {
                return _currentViewModel;
            }
            set
            {
                if (_currentViewModel == value)
                    return;
                _currentViewModel = value;
                RaisePropertyChanged("CurrentViewModel");
            }
        }

        public RelayCommand DeliveryViewModelViewCommand { get; private set; }
        private void ExecuteDeliveryViewModelViewCommand()
        {
            HeaderText = "ስራዎች ማስተዳደሪያ";
            //DeliveryViewModel.LoadData = true;
            CurrentViewModel = AttendanceListViewModel;
        }

        public RelayCommand FollowUpViewModelViewCommand { get; private set; }
        private void ExecuteFollowUpViewModelViewCommand()
        {
            HeaderText = "Followup Managment";
            //FollowUpViewModel.LoadData = true;
            //CurrentViewModel = FollowUpViewModel;
        }

        public string HeaderText
        {
            get
            {
                return _headerText;
            }
            set
            {
                if (_headerText == value)
                    return;
                _headerText = value;
                RaisePropertyChanged("HeaderText");
            }
        }

        public string TitleText
        {
            get
            {
                return _titleText;
            }
            set
            {
                if (_titleText == value)
                    return;
                _titleText = value;
                RaisePropertyChanged("TitleText");
            }
        }

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
            UserRoles.Admin = UserRoles.Company == "Visible" ||
                                UserRoles.UsersMgmt == "Visible" ||
                                UserRoles.BackupRestore == "Visible"
                            ? "Visible" : "Collapsed";

            UserRoles.Files = UserRoles.MemberEntry == "Visible" ||
                                UserRoles.TrainersEntry == "Visible" ||
                                UserRoles.EquipmentsEntry == "Visible" ||
                                UserRoles.SubscriptionEntry == "Visible" ||
                                UserRoles.FacilityEntry == "Visible" ||
                                UserRoles.PackageEntry == "Visible"
                            ? "Visible" : "Collapsed";

            UserRoles.Tasks = UserRoles.AttendanceMgmt == "Visible" ||
                                UserRoles.AssessmentMgmt == "Visible" ||
                                UserRoles.ScheduleMgmt == "Visible"
                            ? "Visible" : "Collapsed";

            UserRoles.Reports = UserRoles.MembersList == "Visible"
                            ? "Visible" : "Collapsed";

        }
        #endregion
    }

    //public class AddressMainViewModel : ViewModelBase
    //{
    //    private string _headerText, _titleText;
    //    readonly static AddressViewModel AddressViewModel = new ViewModelLocator().AddressVm;

    //    private ViewModelBase _currentViewModel;

    //    public AddressMainViewModel()
    //    {
    //        CheckRoles();
    //        TitleText = "PinnaFit V1.0.0.1, Gym and Fitness Management System - " +
    //                    Singleton.User.UserName + " - " +
    //                    DateTime.Now.ToString("dd/MM/yyyy") + " - " +
    //                    ReportUtility.GetEthCalendarFormated(DateTime.Now, "/");

    //        HeaderText = "Address";
    //        CurrentViewModel = AddressViewModel;
    //    }

    //    public ViewModelBase CurrentViewModel
    //    {
    //        get
    //        {
    //            return _currentViewModel;
    //        }
    //        set
    //        {
    //            if (_currentViewModel == value)
    //                return;
    //            _currentViewModel = value;
    //            RaisePropertyChanged("CurrentViewModel");
    //        }
    //    }

    //    public string HeaderText
    //    {
    //        get
    //        {
    //            return _headerText;
    //        }
    //        set
    //        {
    //            if (_headerText == value)
    //                return;
    //            _headerText = value;
    //            RaisePropertyChanged("HeaderText");
    //        }
    //    }

    //    public string TitleText
    //    {
    //        get
    //        {
    //            return _titleText;
    //        }
    //        set
    //        {
    //            if (_titleText == value)
    //                return;
    //            _titleText = value;
    //            RaisePropertyChanged("TitleText");
    //        }
    //    }

    //    #region Previlege Visibility
    //    private UserRolesModel _userRoles;
    //    public UserRolesModel UserRoles
    //    {
    //        get { return _userRoles; }
    //        set
    //        {
    //            _userRoles = value;
    //            RaisePropertyChanged<UserRolesModel>(() => UserRoles);
    //        }
    //    }
    //    private void CheckRoles()
    //    {
    //        UserRoles = Singleton.UserRoles;
    //    }
    //    #endregion
    //}
}