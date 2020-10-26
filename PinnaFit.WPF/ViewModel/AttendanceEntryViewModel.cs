#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PinnaFit.Core;
using PinnaFit.Core.Common;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Models;
using PinnaFit.DAL;
using PinnaFit.DAL.Interfaces;
using PinnaFit.Service;
using PinnaFit.Service.Interfaces;
using PinnaFit.WPF.Views;

#endregion

namespace PinnaFit.WPF.ViewModel
{
    public class AttendanceEntryViewModel : ViewModelBase
    {
        #region Fields

        private static IDbContext _idbContext;
        private static IMemberAttendanceService _attendanceService;
        //private IEnumerable<MemberSubscriptionDTO> _memberSubscriptionList;
        //private ObservableCollection<MemberSubscriptionDTO> _memberSubscriptions;
        private MemberSubscriptionDTO _selectedMemberSubscription;
        private MemberAttendanceDTO _selectedMemberAttendance;

        private ObservableCollection<MemberDTO> _members;
        private IEnumerable<MemberDTO> _memberList;
        private MemberDTO _selectedMember;
        //private int _attendanceIdForEdit;

        private ICommand _addNewMemberAttendanceViewCommand,_checkMemberAttendanceViewCommand,
            _saveMemberAttendanceViewCommand,_closeWindowViewCommand,
            _deleteMemberAttendanceViewCommand;

        private string _attendanceText, _progressBarVisibility;

        #endregion

        #region Constructor

        public AttendanceEntryViewModel()
        {
            CleanUp();
            _idbContext = DbContextUtil.GetDbContextInstance();
            _attendanceService = new MemberAttendanceService(_idbContext);
           CheckRoles();
            ////AddNewMemberAttendance();
            //Messenger.Default.Register<int>(this, (message) =>
            //{
            //    AttendanceIdForEdit = message;
            //});
            Messenger.Default.Register<string>(this, (message) =>
            {
                BarCodeText = message;
            });
            Messenger.Default.Register<IEnumerable<MemberDTO>>(this, (message) =>
            {
                MemberList = message;
            });

            //GetLiveMembers();
            //ProgressBarVisibility = "Collapsed";
            MemberAttendanceText = "Member Attendance Entry";
        }

        public static void CleanUp()
        {
            if (_idbContext != null)
                _idbContext.Dispose();
        }

        #endregion

        #region Public Properties

        //public string ProgressBarVisibility
        //{
        //    get { return _progressBarVisibility; }
        //    set
        //    {
        //        _progressBarVisibility = value;
        //        RaisePropertyChanged<string>(() => ProgressBarVisibility);
        //    }
        //}

        //public int AttendanceIdForEdit
        //{
        //    get { return _attendanceIdForEdit; }
        //    set
        //    {
        //        _attendanceIdForEdit = value;
        //        RaisePropertyChanged<int>(() => AttendanceIdForEdit);
        //        if (AttendanceIdForEdit != 0)
        //        {
        //            //try
        //            //{
        //            //    var crit = new SearchCriteria<MemberAttendanceDTO>();
        //            //    crit.FiList.Add(a => a.Id == AttendanceIdForEdit);
        //            //    SelectedMemberAttendance = _attendanceService.GetAll(crit).FirstOrDefault();
                        
        //            //    var cri = new SearchCriteria<MemberDTO>();
        //            //    cri.FiList.Add(m => m.Id == SelectedMemberAttendance.MemberSubscription.MemberId);
        //            //    var member = new MemberService(true).GetAll(cri).FirstOrDefault();

        //            //    if (member != null) SelectedMemberSubscription = member.LastSubscription;
        //            //    else MessageBox.Show("Can't Edit Attendance");
        //            //    //SelectedMemberSubscription =
        //            //       // MemberSubscriptions.FirstOrDefault(
        //            //            //m => m.MemberId == SelectedMemberAttendance.MemberSubscription.MemberId);
        //            //}
        //            //catch
        //            //{
        //            //    MessageBox.Show("Can't Edit Attendance");
        //            //}
        //        }
        //    }
        //}

        public string MemberAttendanceText
        {
            get { return _attendanceText; }
            set
            {
                _attendanceText = value;
                RaisePropertyChanged<string>(() => MemberAttendanceText);
            }
        }

        public ObservableCollection<MemberDTO> Members
        {
            get { return _members; }
            set
            {
                _members = value;
                RaisePropertyChanged<ObservableCollection<MemberDTO>>(() => Members);

                
            }
        }

        public IEnumerable<MemberDTO> MemberList
        {
            get { return _memberList; }
            set
            {
                _memberList = value;
                RaisePropertyChanged<IEnumerable<MemberDTO>>(() => MemberList);
                if (MemberList != null)
                {
                    Members = new ObservableCollection<MemberDTO>(MemberList);
                }
            }
        }

        public MemberDTO SelectedMember
        {
            get { return _selectedMember; }
            set
            {
                _selectedMember = value;
                RaisePropertyChanged<MemberDTO>(() => SelectedMember);

                if (SelectedMember != null)
                {
                    var cri = new SearchCriteria<MemberSubscriptionDTO>();
                    cri.FiList.Add(f=>f.Id==SelectedMember.LastSubscriptionId);

                    SelectedMemberSubscription = new MemberSubscriptionService(true).GetAll(cri).FirstOrDefault();
                }
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
                    EmployeeShortImage = new BitmapImage();
                    //if (AttendanceIdForEdit == 0)
                    //{
                        int daysLeft = SelectedMemberSubscription.DaysLeft;

                        if (daysLeft <= 5)
                            new MessageDisplay(daysLeft.ToString()).ShowDialog();

                        AddNewMemberAttendance();
                    //}

                    var photoAttachment =
                        new AttachmentService(true).Find(SelectedMemberSubscription.Member.PhotoId.ToString());
                    if (photoAttachment != null)
                        EmployeeShortImage = ImageUtil.ToImage(photoAttachment.AttachedFile);

                    if (SelectedMemberSubscription.EndDate == null)
                    {
                        SelectedMemberSubscription.StartDate = DateTime.Now;

                        if (SelectedMemberSubscription.FacilitySubscription.Subscription.Type ==
                            SubscriptionTypes.Daily)
                        {
                            SelectedMemberSubscription.EndDate =
                                DateTime.Now.AddDays(
                                    SelectedMemberSubscription.FacilitySubscription.Subscription.Value);
                        }
                        else if (SelectedMemberSubscription.FacilitySubscription.Subscription.Type ==
                                 SubscriptionTypes.Monthy)
                        {
                            SelectedMemberSubscription.EndDate =
                                DateTime.Now.AddMonths(
                                    SelectedMemberSubscription.FacilitySubscription.Subscription.Value);
                        }
                    }
                    //GetLiveServices();
                    //GetLiveFilteredServices();
                    ////}
                }
            }
        }

        public void GetLiveMembers()
        {
            //ProgressBarVisibility = "Visible";
            Members = new ObservableCollection<MemberDTO>(MemberList);

            //var worker = new BackgroundWorker();
            //worker.DoWork += DoWork;
            //worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            //worker.RunWorkerAsync();
        }

        //private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    try
        //    {
        //        ProgressBarVisibility = "Collapsed"; 
        //        Services=new ObservableCollection<ServiceDTO>(ServicesList);
        //        //MemberSubscriptions = new ObservableCollection<MemberSubscriptionDTO>(MemberSubscriptionList);
        //        Members=new ObservableCollection<MemberDTO>(MemberList);
        //    }
        //    catch
        //    {
        //    }
        //}

        //private void DoWork(object sender, DoWorkEventArgs e)
        //{
        //    try
        //    {
        //        GetLiveServices();
        //        CheckRoles();
        //        GetLiveMembersBk();
        //    }
        //    catch
        //    {
        //    }
        //}

        //public void GetLiveMembersBk()
        //{
        //    var cri = new SearchCriteria<MemberDTO>();
        //    cri.FiList.Add(m => m.LastSubscription != null && !m.LastSubscription.FacilitySubscription.Facility.DisplayName.Contains("ndo"));
        //    cri.ShortForm = true;

        //    MemberList = new MemberService(true).GetAll(cri).OrderBy(i => i.Id).ToList();

        //    //MemberSubscriptionList = Members.Select(m => m.LastSubscription).ToList();

        //    //var criteria = new SearchCriteria<MemberSubscriptionDTO>();
        //    //criteria.FiList.Add(f => f.EndDate == null || DateTime.Now <= f.EndDate);

        //    //MemberSubscriptionList = new ObservableCollection<MemberSubscriptionDTO>(
        //    //    new MemberSubscriptionService(_idbContext).GetAll(criteria)
        //    //        .OrderBy(i => i.Id).ToList());
        //}

        public MemberAttendanceDTO SelectedMemberAttendance
        {
            get { return _selectedMemberAttendance; }
            set
            {
                _selectedMemberAttendance = value;
                RaisePropertyChanged<MemberAttendanceDTO>(() => SelectedMemberAttendance);
                if (SelectedMemberAttendance != null)
                {
                    try
                    {
                        //if (SelectedMemberAttendance.Services != null)
                        //{
                        //    IList<ServiceDTO> selectedServicesList =
                        //        SelectedMemberAttendance.Services.Select(userroles => userroles.Service).ToList();
                        //    SelectedServices = new ObservableCollection<ServiceDTO>(selectedServicesList);
                        //}

                        //GetLiveFilteredServices();
                    }
                    catch
                    {
                    }
                }
            }
        }

        #endregion

        #region Commands
        private string _barCodeText;
        public string BarCodeText
        {
            get { return _barCodeText; }
            set
            {
                _barCodeText = value;
                RaisePropertyChanged<string>(() => BarCodeText);
                if (!string.IsNullOrEmpty(BarCodeText) && BarCodeText!="5")
                {
                    if (Members != null)
                        SelectedMember = Members.FirstOrDefault(m => BarCodeText != null && m.Number.Contains(BarCodeText));
                }
            }
        }

        public ICommand CheckMemberAttendanceViewCommand
        {
            get
            {
                return _checkMemberAttendanceViewCommand ??
                       (_checkMemberAttendanceViewCommand = new RelayCommand(ExcuteCheckMemberAttendanceViewCommand));
            }
        }
        private void ExcuteCheckMemberAttendanceViewCommand()
        {
            if (Members != null&&!string.IsNullOrEmpty(BarCodeText))
                SelectedMember = Members.FirstOrDefault(m => BarCodeText != null && m.Number.Contains(BarCodeText));
        }

        public ICommand AddNewMemberAttendanceViewCommand
        {
            get
            {
                return _addNewMemberAttendanceViewCommand ??
                       (_addNewMemberAttendanceViewCommand = new RelayCommand(AddNewMemberAttendanceNew));
            }
        }
        private void AddNewMemberAttendanceNew()
        {
            SelectedMemberSubscription = null;
            AddNewMemberAttendance();
        }
        private void AddNewMemberAttendance()
        {
            EmployeeShortImage = new BitmapImage();

            SelectedMemberAttendance = new MemberAttendanceDTO
            {
                CheckedInTime = DateTime.Now
            };

            //SelectedServices = new ObservableCollection<ServiceDTO>();
            //GetLiveFilteredServices();


            //AllServicesChecked = false;
            //AddServiceEnability = false;
            //RemoveServiceEnability = false;
        }

        public void SaveAttendanceBackGr(object obj)
        {
            _msg = "";
            var worker = new BackgroundWorker();
            worker.DoWork += DoWork2;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted2;
            worker.RunWorkerAsync();
        }
 
        private string _msg;
        private void Worker_RunWorkerCompleted2(object sender, RunWorkerCompletedEventArgs e)
        {
            if(!string.IsNullOrEmpty(_msg))
                MessageBox.Show(_msg, "Can't save", MessageBoxButton.OK, MessageBoxImage.Error);
            AddNewMemberAttendanceNew();
        }
        
        private void DoWork2(object sender, DoWorkEventArgs e)
        {
            SaveMemberAttendance(null);
        }

        public ICommand SaveMemberAttendanceViewCommand
        {
            get
            {
                return _saveMemberAttendanceViewCommand ??
                       (_saveMemberAttendanceViewCommand = new RelayCommand<object>(SaveAttendanceBackGr, CanSave));
            }
        }

        private void SaveMemberAttendance(object obj)
        {
            try
            {
                SelectedMemberAttendance.MemberSubscriptionId = SelectedMemberSubscription.Id;

                ////if (SelectedServices.Count == 0)
                ////{
                ////    MessageBox.Show("Can't save, add at least one service the member will took", "Can't save",
                ////        MessageBoxButton.OK,
                ////        MessageBoxImage.Error);
                ////    return;
                ////}

                //SelectedMemberAttendance.Services = new List<AttendanceServiceDTO>();
                //foreach (var serv in FilteredServices) //SelectedServices is changed to FilteredServices
                //{
                //    SelectedMemberAttendance.Services.Add(new AttendanceServiceDTO
                //    {
                //        AttendanceId = SelectedMemberAttendance.Id,
                //        Service = serv
                //    });
                //}

                var stat = _attendanceService.InsertOrUpdate(SelectedMemberAttendance);
                if (stat != string.Empty)
                    _msg = stat;

                //MessageBox.Show("Can't save"
                //                + Environment.NewLine + stat, "Can't save", MessageBoxButton.OK,
                //    MessageBoxImage.Error);

                //AddNewMemberAttendanceNew();

            }
            catch (Exception exception)
            {
                _msg = exception.Message;
                //MessageBox.Show("Can't save"
                //                + Environment.NewLine + exception.Message, "Can't save", MessageBoxButton.OK,
                //    MessageBoxImage.Error);
            }
        }
        public ICommand CloseWindowViewCommand
        {
            get
            {
                return _closeWindowViewCommand ??
                       (_closeWindowViewCommand = new RelayCommand<Object>(CloseWindow, CanSave));
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

        public ICommand DeleteMemberAttendanceViewCommand
        {
            get
            {
                return _deleteMemberAttendanceViewCommand ??
                       (_deleteMemberAttendanceViewCommand = new RelayCommand<Object>(DeleteMemberAttendance, CanSave));
            }
        }
        private void DeleteMemberAttendance(object obj)
        {
            if (
                MessageBox.Show("Are you Sure You want to Delete this Record?", "PinnaFit",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    SelectedMemberAttendance.Enabled = false;
                    var stat = _attendanceService.Disable(SelectedMemberAttendance);
                    if (stat == string.Empty)
                    {
                        MessageBox.Show("Successfully Deleted");
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

        #region Services

        //private ICommand _addServiceViewCommand, _deleteServiceViewCommand;
        //private ServiceDTO _selectedService, _selectedServiceToAdd;
        //private ObservableCollection<ServiceDTO> _selectedServices;
        //private ObservableCollection<ServiceDTO> _services, _filteredServices;
        //private IEnumerable<ServiceDTO> _servicesList; 
        //private bool _addServiceEnability, _removeServiceEnability, _allServicesChecked;

        //public ServiceDTO SelectedService
        //{
        //    get { return _selectedService; }
        //    set
        //    {
        //        _selectedService = value;
        //        RaisePropertyChanged<ServiceDTO>(() => SelectedService);
        //        RemoveServiceEnability = SelectedService != null;
        //    }
        //}

        //public ServiceDTO SelectedServiceToAdd
        //{
        //    get { return _selectedServiceToAdd; }
        //    set
        //    {
        //        _selectedServiceToAdd = value;
        //        RaisePropertyChanged<ServiceDTO>(() => SelectedServiceToAdd);

        //        AddServiceEnability = SelectedServiceToAdd != null;
        //    }
        //}

        //public ObservableCollection<ServiceDTO> SelectedServices
        //{
        //    get { return _selectedServices; }
        //    set
        //    {
        //        _selectedServices = value;
        //        RaisePropertyChanged<ObservableCollection<ServiceDTO>>(() => SelectedServices);
        //    }
        //}

        //public IEnumerable<ServiceDTO> ServicesList
        //{
        //    get { return _servicesList; }
        //    set
        //    {
        //        _servicesList = value;
        //        RaisePropertyChanged<IEnumerable<ServiceDTO>>(() => ServicesList);
        //    }
        //}

        //public ObservableCollection<ServiceDTO> Services
        //{
        //    get { return _services; }
        //    set
        //    {
        //        _services = value;
        //        RaisePropertyChanged<ObservableCollection<ServiceDTO>>(() => Services);
        //    }
        //}

        //public ObservableCollection<ServiceDTO> FilteredServices
        //{
        //    get { return _filteredServices; }
        //    set
        //    {
        //        _filteredServices = value;
        //        RaisePropertyChanged<ObservableCollection<ServiceDTO>>(() => FilteredServices);
        //    }
        //}

        //public bool AddServiceEnability
        //{
        //    get { return _addServiceEnability; }
        //    set
        //    {
        //        _addServiceEnability = value;
        //        RaisePropertyChanged<bool>(() => AddServiceEnability);
        //    }
        //}

        //public bool RemoveServiceEnability
        //{
        //    get { return _removeServiceEnability; }
        //    set
        //    {
        //        _removeServiceEnability = value;
        //        RaisePropertyChanged<bool>(() => RemoveServiceEnability);
        //    }
        //}

        //public bool AllServicesChecked
        //{
        //    get { return _allServicesChecked; }
        //    set
        //    {
        //        _allServicesChecked = value;
        //        RaisePropertyChanged<bool>(() => AllServicesChecked);

        //        try
        //        {
        //            if (AllServicesChecked)
        //            {
        //                SelectedServices = new ObservableCollection<ServiceDTO>(Services);
        //                GetLiveFilteredServices(); // new ObservableCollection<ServiceDTO>();
        //            }
        //            else
        //            {
        //                SelectedServices = new ObservableCollection<ServiceDTO>();
        //                GetLiveServices();
        //                GetLiveFilteredServices();
        //                // new ObservableCollection<ServiceDTO>(Services.Except(SelectedServices));
        //            }
        //        }
        //        catch
        //        {
        //            MessageBox.Show("Can't Remove Service");
        //        }
        //    }
        //}

        //public ICommand AddServiceViewCommand
        //{
        //    get
        //    {
        //        return _addServiceViewCommand ??
        //               (_addServiceViewCommand = new RelayCommand(ExcuteAddServiceViewCommand, CanSave));
        //    }
        //}

        //private void ExcuteAddServiceViewCommand()
        //{
        //    try
        //    {
        //        SelectedServices.Add(SelectedServiceToAdd);
        //        GetLiveFilteredServices(); // new ObservableCollection<ServiceDTO>(Services.Except(SelectedServices));
        //    }
        //    catch
        //    {
        //        MessageBox.Show("Can't Save Service");
        //    }
        //}

        //public ICommand RemoveServiceViewCommand
        //{
        //    get
        //    {
        //        return _deleteServiceViewCommand ??
        //               (_deleteServiceViewCommand = new RelayCommand(ExecuteRemoveServiceViewCommand));
        //    }
        //}

        //private void ExecuteRemoveServiceViewCommand()
        //{
        //    try
        //    {
        //        SelectedServices.Remove(SelectedService);
        //        GetLiveFilteredServices(); // new ObservableCollection<ServiceDTO>(Services.Except(SelectedServices));
        //    }
        //    catch
        //    {
        //        MessageBox.Show("Can't Remove Service");
        //    }
        //}

        //private void GetLiveServices()
        //{
        //    try
        //    {
        //        _attendanceService.GetAllServices();

        //        var memSubs =
        //            new MemberSubscriptionService(_idbContext)
        //                .GetAll()
        //                .FirstOrDefault(m => SelectedMemberSubscription != null && m.Id == SelectedMemberSubscription.Id);

        //        if (memSubs != null)
        //        {
        //            var servs = memSubs.FacilitySubscription.Facility.Services.Select(s => s.Service).ToList();
        //            servs = IsServiceGivenNow(servs).ToList();

        //            ServicesList = new List<ServiceDTO>(servs);
        //        }
        //        else
        //            ServicesList = new List<ServiceDTO>();
        //    }
        //    catch
        //    {
        //    }
        //}

        //public IEnumerable<ServiceDTO> IsServiceGivenNow(IEnumerable<ServiceDTO> servs)
        //{
        //    var svs = new List<ServiceDTO>();
        //    foreach (var serviceDTO in servs)
        //    {
        //        var timetbls = serviceDTO.TimeTable.ToList();
        //        foreach (var timeTableDTO in timetbls)
        //        {
        //            var currentDateTime = DateTime.Now;

        //            if (timeTableDTO.DayOfWeekHeld.Contains(currentDateTime.DayOfWeek))
        //            {
        //                var currentTime = currentDateTime.TimeOfDay;
        //                var classBegins = Convert.ToDateTime(timeTableDTO.ClassBegins).TimeOfDay;
        //                var classEnds = Convert.ToDateTime(timeTableDTO.ClassEnds).TimeOfDay;

        //                if (currentTime >= classBegins && currentTime <= classEnds)
        //                {
        //                    svs.Add(serviceDTO);
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    return svs;
        //}

        //private void GetLiveFilteredServices()
        //{
        //    IList<ServiceDTO> filtTra = new List<ServiceDTO>();
        //    foreach (var serviceDTO in Services)
        //    {
        //        if (SelectedServices != null)
        //        {
        //            var train = SelectedServices.FirstOrDefault(tr => tr.DisplayName == serviceDTO.DisplayName);
        //            if (train == null)
        //                filtTra.Add(serviceDTO);
        //        }
        //    }
        //    FilteredServices = new ObservableCollection<ServiceDTO>(filtTra);
        //}

        #endregion

        #region short photo

        private BitmapImage _employeeShortImage;

        public BitmapImage EmployeeShortImage
        {
            get { return _employeeShortImage; }
            set
            {
                _employeeShortImage = value;
                RaisePropertyChanged<BitmapImage>(() => EmployeeShortImage);
            }
        }

        #endregion

        #region Validation

        public static int Errors { get; set; }

        public bool CanSave(object parameter)
        {
            return Errors == 0;
        }

        public bool CanSave()
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