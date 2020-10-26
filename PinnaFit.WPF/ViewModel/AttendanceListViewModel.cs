#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using CrystalDecisions.CrystalReports.Engine;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PinnaFit.Core;
using PinnaFit.Core.Common;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Models;
using PinnaFit.Service;
using PinnaFit.Service.Interfaces;
using PinnaFit.WPF.Models;
using PinnaFit.WPF.Views;
using MessageBox = System.Windows.Forms.MessageBox;

#endregion

namespace PinnaFit.WPF.ViewModel
{
    public class AttendanceListViewModel : ViewModelBase
    {
        #region Fields

        private static IMemberAttendanceService _memberAttendanceService;
        private static IPervisitSubscriptionService _pervisitAttendanceService;
        private static ITransactionService _transactionService;

        private DateTime _filterStartDate, _filterEndDate;
        private IEnumerable<MemberAttendanceDTO> _memberAttendances;
        private ObservableCollection<MemberAttendanceDTO> _filteredAttendances;
        private MemberAttendanceDTO _selectedAttendance;
        private MemberSubscriptionDTO _selectedMemberSubscription;
        private IEnumerable<PervisitSubscriptionDTO> _pervisitAttendances;
        private ObservableCollection<PervisitSubscriptionDTO> _filteredPervisitAttendances;
        private PervisitSubscriptionDTO _selectedPervisitAttendance;

        private TransactionLineDTO _selectedSales;
        private ObservableCollection<TransactionLineDTO> _sales;
        private IEnumerable<TransactionLineDTO> _salesList;
        private string _progressBarVisibility;

        #endregion

        #region Constructor

        public AttendanceListViewModel()
        {
            FillPeriodCombo();
            SelectedPeriod = FilterPeriods.FirstOrDefault(p => p.Value == 1);
            //Value == 0(show all) Value==1(show today) Value=3(show this week)
            FillStatusTypes();
            FillShiftTypes();
            CheckRoles();
            ProgressBarVisibility = "Collapsed";

            PreviousEnability = true;
            NextEnability = true;
            Load();
        }

        public void Load()
        {
            ProgressBarVisibility = "Visible";
            CurrentPageNumber = 1;
            var worker = new BackgroundWorker();
            worker.DoWork += DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                Attendances = new ObservableCollection<MemberAttendanceDTO>(AttendanceList);
                PervisitAttendances = new ObservableCollection<PervisitSubscriptionDTO>(PervisitAttendanceList);
                Sales = new ObservableCollection<TransactionLineDTO>(SalesList);
                MemberSubscriptions = new ObservableCollection<MemberSubscriptionDTO>(MemberSubscriptionList);
                ProgressBarVisibility = "Collapsed";
            }
            catch
            {
            }
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Load2();
            }
            catch
            {
            }
        }

        public void Load2()
        {
            CleanUp();
            _memberAttendanceService = new MemberAttendanceService();
            _pervisitAttendanceService = new PervisitSubscriptionService();
            _transactionService = new TransactionService();

            GetLiveAttendances();
            GetLivePervisitAttendances();
            GetLiveSales();
            GetLiveMemberSubscriptions();
            GetLiveMembersBk();
        }

        public static void CleanUp()
        {
            if (_memberAttendanceService != null)
                _memberAttendanceService.Dispose();
            if (_pervisitAttendanceService != null)
                _pervisitAttendanceService.Dispose();
            if (_transactionService != null)
                _transactionService.Dispose();
        }

        #endregion

        #region Attendances

        private int _totalAttendances = 0;

        public string ProgressBarVisibility
        {
            get { return _progressBarVisibility; }
            set
            {
                _progressBarVisibility = value;
                RaisePropertyChanged<string>(() => ProgressBarVisibility);
            }
        }

        public IEnumerable<MemberAttendanceDTO> AttendanceList
        {
            get { return _memberAttendances; }
            set
            {
                _memberAttendances = value;
                RaisePropertyChanged<IEnumerable<MemberAttendanceDTO>>(() => AttendanceList);
            }
        }

        public ObservableCollection<MemberAttendanceDTO> Attendances
        {
            get { return _filteredAttendances; }
            set
            {
                _filteredAttendances = value;
                RaisePropertyChanged<ObservableCollection<MemberAttendanceDTO>>(() => Attendances);

                try
                {
                    CalculateSummary();
                }
                catch
                {
                }
                if (Attendances != null)
                    SelectedAttendance = Attendances.FirstOrDefault();
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
                }
            }
        }

        public MemberAttendanceDTO SelectedAttendance
        {
            get { return _selectedAttendance; }
            set
            {
                _selectedAttendance = value;
                RaisePropertyChanged<MemberAttendanceDTO>(() => SelectedAttendance);
                if (SelectedAttendance != null)
                {
                }
            }
        }

        public void GetLiveAttendances()
        {
            var pageSize = 7;
            if (UserRoles != null && UserRoles.AttendanceEdit == "Collapsed")
                pageSize = 10;

            Attendances = new ObservableCollection<MemberAttendanceDTO>();
            if (CurrentPageNumber < 0)
                CurrentPageNumber = 1;

            var criteria = new SearchCriteria<MemberAttendanceDTO>
            {
                BeginingDate = FilterStartDate,
                EndingDate = FilterEndDate,
                Page = CurrentPageNumber,
                PageSize = pageSize,
                IncludePhoto = true
            };

            if (SelectedShiftType != null && SelectedShiftType.Value != 0)
            {
                if (SelectedShiftType.Value == 1)
                    criteria.Shift = ShiftTypes.Morning;
                else if (SelectedShiftType.Value == 2)
                    criteria.Shift = ShiftTypes.Afternoon;
            }

            PreviousEnability = criteria.Page != 1;

            AttendanceList = _memberAttendanceService.GetAll(criteria, out _totalAttendances).ToList();

            #region Paging

            var pages = HelperUtility.GetPages(_totalAttendances, criteria.PageSize);
            TotalPages = pages;

            if (pages == 0)
            {
                criteria.Page = 0;
                CurrentPageNumber = 0;
            }

            NextEnability = criteria.Page != pages;

            #endregion

            #region For Serial Number

            var sNo = (criteria.Page - 1)*criteria.PageSize + 1;
            foreach (var attendanceDto in AttendanceList)
            {
                attendanceDto.SerialNumber = sNo;
                sNo++;
            }

            #endregion
        }

        #endregion

        #region Pervisit Attendances

        public IEnumerable<PervisitSubscriptionDTO> PervisitAttendanceList
        {
            get { return _pervisitAttendances; }
            set
            {
                _pervisitAttendances = value;
                RaisePropertyChanged<IEnumerable<PervisitSubscriptionDTO>>(() => PervisitAttendanceList);
            }
        }

        public ObservableCollection<PervisitSubscriptionDTO> PervisitAttendances
        {
            get { return _filteredPervisitAttendances; }
            set
            {
                _filteredPervisitAttendances = value;
                RaisePropertyChanged<ObservableCollection<PervisitSubscriptionDTO>>(() => PervisitAttendances);

                try
                {
                    CalculateSummary();
                }
                catch
                {
                }

                if (PervisitAttendances != null)
                    SelectedPervisitAttendance = PervisitAttendances.FirstOrDefault();
            }
        }

        public PervisitSubscriptionDTO SelectedPervisitAttendance
        {
            get { return _selectedPervisitAttendance; }
            set
            {
                _selectedPervisitAttendance = value;
                RaisePropertyChanged<PervisitSubscriptionDTO>(() => SelectedPervisitAttendance);
                if (SelectedPervisitAttendance != null)
                {
                }
            }
        }

        public void GetLivePervisitAttendances()
        {
            PervisitAttendances = new ObservableCollection<PervisitSubscriptionDTO>();

            var criteria = new SearchCriteria<PervisitSubscriptionDTO>
            {
                BeginingDate = FilterStartDate,
                EndingDate = FilterEndDate
            };
            if (SelectedShiftType != null && SelectedShiftType.Value != 0)
            {
                if (SelectedShiftType.Value == 1)
                    criteria.Shift = ShiftTypes.Morning;
                else if (SelectedShiftType.Value == 2)
                    criteria.Shift = ShiftTypes.Afternoon;
            }
            PervisitAttendanceList = _pervisitAttendanceService.GetAll(criteria).OrderBy(i => i.Id).ToList();
        }

        #endregion

        #region Member Subscriptions

        public IEnumerable<MemberSubscriptionDTO> MemberSubscriptionList
        {
            get { return _memberSubscriptions; }
            set
            {
                _memberSubscriptions = value;
                RaisePropertyChanged<IEnumerable<MemberSubscriptionDTO>>(() => MemberSubscriptionList);
            }
        }

        public ObservableCollection<MemberSubscriptionDTO> MemberSubscriptions
        {
            get { return _filteredMemberSubscriptions; }
            set
            {
                _filteredMemberSubscriptions = value;
                RaisePropertyChanged<ObservableCollection<MemberSubscriptionDTO>>(() => MemberSubscriptions);

                try
                {
                    CalculateSummary();
                }
                catch
                {
                }
            }
        }

        public void GetLiveMemberSubscriptions()
        {
            MemberSubscriptions = new ObservableCollection<MemberSubscriptionDTO>();
            var criteria = new SearchCriteria<MemberSubscriptionDTO>();
            criteria.FiList.Add(f => f.Member.Enabled);

            criteria.BeginingDate = FilterStartDate;
            criteria.EndingDate = FilterEndDate;
            criteria.Shift = (ShiftTypes) SelectedShiftType.Value;

            MemberSubscriptionList = new MemberSubscriptionService(true).GetAll(criteria).ToList();

            //var cri = new SearchCriteria<MemberDTO>();
            //cri.FiList.Add(f => f.LastSubscriptionId != null);

            //var beginDate = new DateTime(FilterStartDate.Year, FilterStartDate.Month, FilterStartDate.Day, 0, 0, 0);
            //if (SelectedShiftType.Value == 2)
            //    beginDate = new DateTime(FilterStartDate.Year, FilterStartDate.Month, FilterStartDate.Day, 14, 0, 0);
            //cri.FiList.Add(
            //    p => p.LastSubscription.SubscribedDate != null && p.LastSubscription.SubscribedDate >= beginDate);

            //var endDate = new DateTime(FilterEndDate.Year, FilterEndDate.Month, FilterEndDate.Day, 23, 59, 59);
            //if (SelectedShiftType.Value == 1)
            //    endDate = new DateTime(FilterEndDate.Year, FilterEndDate.Month, FilterEndDate.Day, 13, 59, 59);
            //cri.FiList.Add(
            //    p => p.LastSubscription.SubscribedDate != null && p.LastSubscription.SubscribedDate <= endDate);

            //var memberList = new MemberService(true).GetAll(cri).OrderBy(i => i.Id).ToList();

            //MemberSubscriptionList = memberList.Select(m => m.LastSubscription).ToList();
        }

        #endregion

        #region Sales

        public IEnumerable<TransactionLineDTO> SalesList
        {
            get { return _salesList; }
            set
            {
                _salesList = value;
                RaisePropertyChanged<IEnumerable<TransactionLineDTO>>(() => SalesList);
            }
        }

        public ObservableCollection<TransactionLineDTO> Sales
        {
            get { return _sales; }
            set
            {
                _sales = value;
                RaisePropertyChanged<ObservableCollection<TransactionLineDTO>>(() => Sales);

                try
                {
                    CalculateSummary();
                }
                catch
                {
                }

                if (Sales != null)
                    SelectedSales = Sales.FirstOrDefault();
            }
        }

        public TransactionLineDTO SelectedSales
        {
            get { return _selectedSales; }
            set
            {
                _selectedSales = value;
                RaisePropertyChanged<TransactionLineDTO>(() => SelectedSales);
                if (SelectedSales != null)
                {
                }
            }
        }

        public void GetLiveSales()
        {
            var criteria = new SearchCriteria<TransactionHeaderDTO>
            {
                TransactionType = (int) TransactionTypes.SellStock,
                BeginingDate = FilterStartDate,
                EndingDate = FilterEndDate,
                CurrentUserId = Singleton.User.UserId
            };

            if (SelectedShiftType != null && SelectedShiftType.Value != 0)
            {
                if (SelectedShiftType.Value == 1)
                    criteria.Shift = ShiftTypes.Morning;
                else if (SelectedShiftType.Value == 2)
                    criteria.Shift = ShiftTypes.Afternoon;
            }

            var salesHeaderList = _transactionService.GetAll(criteria).OrderBy(i => i.Id).ToList();

            var salesHeaders = new ObservableCollection<TransactionHeaderDTO>(salesHeaderList);

            SalesList = salesHeaders.SelectMany(t => t.TransactionLines).ToList();
        }

        #endregion

        #region Filter List

        #region Fields

        private ObservableCollection<ListDataItem> _filterPeriods;
        private ListDataItem _selectedPeriod;
        private string _filterPeriod;

        private List<ListDataItem> _statusTypeList;
        private ListDataItem _selectedStatusType;

        private List<ListDataItem> _shiftTypeList;
        private ListDataItem _selectedShiftType;

        #endregion

        #region By Period

        public string FilterPeriod
        {
            get { return _filterPeriod; }
            set
            {
                _filterPeriod = value;
                RaisePropertyChanged<string>(() => FilterPeriod);
            }
        }

        public DateTime FilterStartDate
        {
            get { return _filterStartDate; }
            set
            {
                _filterStartDate = value;
                RaisePropertyChanged<DateTime>(() => FilterStartDate);
                if (FilterStartDate.Year > 2000)
                    StartDateText = ReportUtility.GetEthCalendarFormated(FilterStartDate, "-") +
                                    "(" + FilterStartDate.ToString("dd-MM-yyyy") + ")";
            }
        }

        public DateTime FilterEndDate
        {
            get { return _filterEndDate; }
            set
            {
                _filterEndDate = value;
                RaisePropertyChanged<DateTime>(() => FilterEndDate);

                if (FilterEndDate.Year > 2000)
                    EndDateText = ReportUtility.GetEthCalendarFormated(FilterEndDate, "-") + "(" +
                                  FilterEndDate.ToString("dd-MM-yyyy") + ")";
            }
        }

        private void FillPeriodCombo()
        {
            IList<ListDataItem> filterPeriods2 = new List<ListDataItem>();
            filterPeriods2.Add(new ListDataItem {Display = "All", Value = 0});
            filterPeriods2.Add(new ListDataItem {Display = "Today", Value = 1});
            filterPeriods2.Add(new ListDataItem {Display = "Yesterday", Value = 2});
            filterPeriods2.Add(new ListDataItem {Display = "This Week", Value = 3});
            filterPeriods2.Add(new ListDataItem {Display = "Last Week", Value = 4});
            FilterPeriods = new ObservableCollection<ListDataItem>(filterPeriods2);
        }

        public ObservableCollection<ListDataItem> FilterPeriods
        {
            get { return _filterPeriods; }
            set
            {
                _filterPeriods = value;
                RaisePropertyChanged<ObservableCollection<ListDataItem>>(() => FilterPeriods);
            }
        }

        public ListDataItem SelectedPeriod
        {
            get { return _selectedPeriod; }
            set
            {
                _selectedPeriod = value;
                RaisePropertyChanged<ListDataItem>(() => SelectedPeriod);
                if (SelectedPeriod != null)
                {
                    switch (SelectedPeriod.Value)
                    {
                        case 0:
                            FilterStartDate = DateTime.Now.AddYears(-1);
                            FilterEndDate = DateTime.Now;
                            break;
                        case 1:
                            FilterStartDate = DateTime.Now;
                            FilterEndDate = DateTime.Now;
                            break;
                        case 2:
                            FilterStartDate = DateTime.Now.AddDays(-1);
                            FilterEndDate = DateTime.Now.AddDays(-1);
                            break;
                        case 3:
                            FilterStartDate = DateTime.Now.AddDays(-(int) DateTime.Now.DayOfWeek);
                            FilterEndDate = DateTime.Now.AddDays(7 - (int) DateTime.Now.DayOfWeek - 1);
                            break;
                        case 4:
                            FilterStartDate = DateTime.Now.AddDays(-(int) DateTime.Now.DayOfWeek - 7);
                            FilterEndDate = DateTime.Now.AddDays(-(int) DateTime.Now.DayOfWeek - 1);
                            break;
                            //case 5:
                            //    var mon2 = DateTime.Now.Month;
                            //    MessageBox.Show(DateTime.Now.Month.ToString());
                            //    FilterStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                            //    FilterEndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                            //    break;
                            //case 6:
                            //    var mon = DateTime.Now.Month;
                            //    if (mon != 1)
                            //    {
                            //        FilterStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1);
                            //        FilterEndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1,
                            //            DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month - 1));
                            //    }
                            //    else
                            //    {
                            //        FilterStartDate = new DateTime(DateTime.Now.Year - 1, 12, 1);
                            //        FilterEndDate = new DateTime(DateTime.Now.Year - 1, 12, 
                            //            DateTime.DaysInMonth(DateTime.Now.Year, 12));
                            //    }
                            //    break;
                    }
                }
            }
        }

        #endregion

        #region Status Type

        public List<ListDataItem> StatusTypeList
        {
            get { return _statusTypeList; }
            set
            {
                _statusTypeList = value;
                RaisePropertyChanged<List<ListDataItem>>(() => StatusTypeList);
            }
        }

        public ListDataItem SelectedStatusType
        {
            get { return _selectedStatusType; }
            set
            {
                _selectedStatusType = value;
                RaisePropertyChanged<ListDataItem>(() => SelectedStatusType);
            }
        }

        public void FillStatusTypes()
        {
            StatusTypeList = (List<ListDataItem>) CommonUtility.GetList(typeof (MemberStatusTypes));
        }

        #endregion

        #region Shift Type

        public List<ListDataItem> ShiftTypeList
        {
            get { return _shiftTypeList; }
            set
            {
                _shiftTypeList = value;
                RaisePropertyChanged<List<ListDataItem>>(() => ShiftTypeList);
            }
        }

        public ListDataItem SelectedShiftType
        {
            get { return _selectedShiftType; }
            set
            {
                _selectedShiftType = value;
                RaisePropertyChanged<ListDataItem>(() => SelectedShiftType);
            }
        }

        public void FillShiftTypes()
        {
            ShiftTypeList = (List<ListDataItem>) CommonUtility.GetList(typeof (ShiftTypes));

            var nowTime = DateTime.Now.TimeOfDay.Hours;
            if (nowTime < 14)
                SelectedShiftType = ShiftTypeList.FirstOrDefault(s => s.Value == 1);
            else
                SelectedShiftType = ShiftTypeList.FirstOrDefault(s => s.Value == 2);

            //if(DateTime.Now.TimeOfDay)
        }

        #endregion

        private ICommand _memberStartDateViewCommand, _memberEndDateViewCommand;
        private string _startDateText, _endDateText;

        public string StartDateText
        {
            get { return _startDateText; }
            set
            {
                _startDateText = value;
                RaisePropertyChanged<string>(() => StartDateText);
            }
        }

        public string EndDateText
        {
            get { return _endDateText; }
            set
            {
                _endDateText = value;
                RaisePropertyChanged<string>(() => EndDateText);
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
            var calConv = new CalendarConvertor(DateTime.Now);
            calConv.ShowDialog();
            var dialogueResult = calConv.DialogResult;
            if (dialogueResult != null && (bool) dialogueResult)
            {
                if (calConv.DtSelectedDate.SelectedDate != null)
                    FilterStartDate = (DateTime) calConv.DtSelectedDate.SelectedDate;
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
            var calConv = new CalendarConvertor(DateTime.Now);
            calConv.ShowDialog();
            var dialogueResult = calConv.DialogResult;
            if (dialogueResult != null && (bool) dialogueResult)
            {
                if (calConv.DtSelectedDate.SelectedDate != null)
                    FilterEndDate = (DateTime) calConv.DtSelectedDate.SelectedDate;
            }
        }

        #endregion

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
            subs.Insert(0, new SubscriptionDTO()
            {
                Id = -1,
                DisplayName = "All"
            });
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
            subs.Insert(0, new FacilityDTO()
            {
                Id = -1,
                DisplayName = "All"
            });
            Facilities = new ObservableCollection<FacilityDTO>(subs);
        }

        private ObservableCollection<MemberDTO> _members;

        public ObservableCollection<MemberDTO> Members
        {
            get { return _members; }
            set
            {
                _members = value;
                RaisePropertyChanged<ObservableCollection<MemberDTO>>(() => Members);
            }
        }

        private IEnumerable<MemberDTO> _membersList;

        public IEnumerable<MemberDTO> MembersList
        {
            get { return _membersList; }
            set
            {
                _membersList = value;
                RaisePropertyChanged<IEnumerable<MemberDTO>>(() => MembersList);
            }
        }

        private MemberDTO _selectedMember;

        public MemberDTO SelectedMember
        {
            get { return _selectedMember; }
            set
            {
                _selectedMember = value;
                RaisePropertyChanged<MemberDTO>(() => SelectedMember);
            }
        }

        public void LoadMembers()
        {
            var criteria = new SearchCriteria<MemberDTO>();
            var subs = new MemberService(true).GetAll(criteria).ToList();
            subs.Insert(0, new MemberDTO()
            {
                Id = -1,
                DisplayName = "All"
            });
            Members = new ObservableCollection<MemberDTO>(subs);
        }

        private ObservableCollection<ServiceDTO> _services;

        public ObservableCollection<ServiceDTO> Services
        {
            get { return _services; }
            set
            {
                _services = value;
                RaisePropertyChanged<ObservableCollection<ServiceDTO>>(() => Services);
            }
        }

        private ServiceDTO _selectedService;

        public ServiceDTO SelectedService
        {
            get { return _selectedService; }
            set
            {
                _selectedService = value;
                RaisePropertyChanged<ServiceDTO>(() => SelectedService);
            }
        }

        public void LoadServices()
        {
            var criteria = new SearchCriteria<ServiceDTO>();
            var subs = new ServiceService(true).GetAll(criteria).ToList();
            subs.Insert(0, new ServiceDTO()
            {
                Id = -1,
                DisplayName = "All"
            });
            Services = new ObservableCollection<ServiceDTO>(subs);
        }

        #endregion

        #region Summary

        private int _totalNumberOfRows,
            _totalNumberOfMembers,
            _totalNumberOfPervisits,
            _totalNumberOfSales,
            _totalNumberOfMemberSubscriptions,
            _totalNumberOfMales,
            _totalNumberOfFemales;

        private decimal _totalNumberOfTransaction;

        private string _summaryVisibility,
            _totalValueOfPervisitTransaction,
            _totalValueOfSales,
            _totalValueOfMemberSubscriptions,
            _totalShiftAmount;

        public int TotalNumberOfMembers
        {
            get { return _totalNumberOfMembers; }
            set
            {
                _totalNumberOfMembers = value;
                RaisePropertyChanged<int>(() => TotalNumberOfMembers);
            }
        }

        public int TotalNumberOfMales
        {
            get { return _totalNumberOfMales; }
            set
            {
                _totalNumberOfMales = value;
                RaisePropertyChanged<int>(() => TotalNumberOfMales);
            }
        }

        public int TotalNumberOfFemales
        {
            get { return _totalNumberOfFemales; }
            set
            {
                _totalNumberOfFemales = value;
                RaisePropertyChanged<int>(() => TotalNumberOfFemales);
            }
        }

        public int TotalNumberOfPervisits
        {
            get { return _totalNumberOfPervisits; }
            set
            {
                _totalNumberOfPervisits = value;
                RaisePropertyChanged<int>(() => TotalNumberOfPervisits);
            }
        }

        public int TotalNumberOfRows
        {
            get { return _totalNumberOfRows; }
            set
            {
                _totalNumberOfRows = value;
                RaisePropertyChanged<int>(() => TotalNumberOfRows);
            }
        }

        public decimal TotalNumberOfTransaction
        {
            get { return _totalNumberOfTransaction; }
            set
            {
                _totalNumberOfTransaction = value;
                RaisePropertyChanged<decimal>(() => TotalNumberOfTransaction);
            }
        }

        public string TotalValueOfPervisitTransaction
        {
            get { return _totalValueOfPervisitTransaction; }
            set
            {
                _totalValueOfPervisitTransaction = value;
                RaisePropertyChanged<string>(() => TotalValueOfPervisitTransaction);
            }
        }

        public string SummaryVisibility
        {
            get { return _summaryVisibility; }
            set
            {
                _summaryVisibility = value;
                RaisePropertyChanged<string>(() => SummaryVisibility);
            }
        }

        public int TotalNumberOfSales
        {
            get { return _totalNumberOfSales; }
            set
            {
                _totalNumberOfSales = value;
                RaisePropertyChanged<int>(() => TotalNumberOfSales);
            }
        }

        public string TotalValueOfSales
        {
            get { return _totalValueOfSales; }
            set
            {
                _totalValueOfSales = value;
                RaisePropertyChanged<string>(() => TotalValueOfSales);
            }
        }

        public int TotalNumberOfMemberSubscriptions
        {
            get { return _totalNumberOfMemberSubscriptions; }
            set
            {
                _totalNumberOfMemberSubscriptions = value;
                RaisePropertyChanged<int>(() => TotalNumberOfMemberSubscriptions);
            }
        }

        public string TotalValueOfMemberSubscriptions
        {
            get { return _totalValueOfMemberSubscriptions; }
            set
            {
                _totalValueOfMemberSubscriptions = value;
                RaisePropertyChanged<string>(() => TotalValueOfMemberSubscriptions);
            }
        }

        public string TotalShiftAmount
        {
            get { return _totalShiftAmount; }
            set
            {
                _totalShiftAmount = value;
                RaisePropertyChanged<string>(() => TotalShiftAmount);
            }
        }

        public void CalculateSummary()
        {
            TotalNumberOfMemberSubscriptions = MemberSubscriptions.Count;
            TotalValueOfMemberSubscriptions = MemberSubscriptions.Sum(s => s.AmountPaid).ToString("N2");

            TotalNumberOfMembers = _totalAttendances;
            TotalNumberOfPervisits = PervisitAttendances.Count;

            TotalNumberOfRows = Attendances.Count + PervisitAttendances.Count;
            TotalNumberOfTransaction = Attendances.Sum(s => s.MemberSubscription.AmountPaid) +
                                       PervisitAttendances.Sum(s => s.AmountPaid);
            TotalValueOfPervisitTransaction =
                PervisitAttendances.Sum(s => s.AmountPaid).ToString("N2");

            TotalNumberOfSales = Sales.Count;
            TotalValueOfSales = Sales.Sum(s => s.LinePrice).ToString("N2");

            TotalShiftAmount =
                (MemberSubscriptions.Sum(s => s.AmountPaid) +
                 PervisitAttendances.Sum(p => p.AmountPaid)
                 + Sales.Sum(s => s.LinePrice)).ToString("N2");
        }

        #endregion

        #region Commands

        private ICommand _refreshCommand,
            _pervisitCommand,
            _memberSubscriptionCommand,
            _deleteCommand,
            _attendanceEntryCommandView,
            _attendanceDeleteCommandView,
            _pervisitDeleteCommandView,
            _salesEntryCommand,
            _salesDeleteCommand;

        public ICommand RefreshCommand
        {
            get { return _refreshCommand ?? (_refreshCommand = new RelayCommand(ExcuteRefreshCommand)); }
        }

        public void ExcuteRefreshCommand()
        {
            Load();
        }

        public ICommand MemberSubscriptionCommand
        {
            get
            {
                return _memberSubscriptionCommand ?? (_memberSubscriptionCommand = new RelayCommand(ExcuteMemberCommand));
            }
        }

        public void ExcuteMemberCommand()
        {
            var pervisitAttendWindow = new MemberEntry();
            pervisitAttendWindow.ShowDialog();
            //var dialogueResult = pervisitAttendWindow.DialogResult;
            //if (dialogueResult != null && (bool)dialogueResult)
            //{
            GetLiveMemberSubscriptions();
            MemberSubscriptions = new ObservableCollection<MemberSubscriptionDTO>(MemberSubscriptionList);
            //}
        }

        public ICommand PervisitCommand
        {
            get { return _pervisitCommand ?? (_pervisitCommand = new RelayCommand(ExcutePervisitCommand)); }
        }

        public void ExcutePervisitCommand()
        {
            var pervisitAttendWindow = new PervisitSubscriptions();
            pervisitAttendWindow.ShowDialog();
            var dialogueResult = pervisitAttendWindow.DialogResult;
            if (dialogueResult != null && (bool) dialogueResult)
            {
                GetLivePervisitAttendances();
                PervisitAttendances = new ObservableCollection<PervisitSubscriptionDTO>(PervisitAttendanceList);
            }
        }

        public ICommand PervisitDeleteCommandView
        {
            get
            {
                return _pervisitDeleteCommandView ??
                       (_pervisitDeleteCommandView = new RelayCommand<Object>(PervisitDelete));
            }
        }

        public void PervisitDelete(object obj)
        {
            if (SelectedPervisitAttendance == null)
                return;

            if (
                System.Windows.MessageBox.Show("Are you Sure You want to Delete this Record?", "Pinna Fitness",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    if (SelectedPervisitAttendance != null)
                    {
                        SelectedPervisitAttendance.Enabled = false;
                        var stat = _pervisitAttendanceService.Disable(SelectedPervisitAttendance);
                        if (stat == string.Empty)
                        {
                            PervisitAttendances.Remove(SelectedPervisitAttendance);
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("Can't Delete, may be the data is already in use..."
                                                           + Environment.NewLine + stat, "Can't Delete",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Can't Delete, may be the data is already in use..."
                                                   + Environment.NewLine + ex.Message + Environment.NewLine +
                                                   ex.InnerException,
                        "Can't Delete",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public ICommand SalesEntryCommand
        {
            get { return _salesEntryCommand ?? (_salesEntryCommand = new RelayCommand(ExcuteSalesEntryCommand)); }
        }

        public void ExcuteSalesEntryCommand()
        {
            var pervisitAttendWindow = new SellItemEntry(TransactionTypes.SellStock);
            pervisitAttendWindow.ShowDialog();
            var dialogueResult = pervisitAttendWindow.DialogResult;
            if (dialogueResult != null && (bool) dialogueResult)
            {
                GetLiveSales();
                Sales = new ObservableCollection<TransactionLineDTO>(SalesList);
            }
        }

        public ICommand SalesDeleteCommand
        {
            get { return _salesDeleteCommand ?? (_salesDeleteCommand = new RelayCommand(ExcuteSalesDeleteCommand)); }
        }

        public void ExcuteSalesDeleteCommand()
        {
            if (SelectedSales == null)
                return;
            if (
                System.Windows.MessageBox.Show("Are you Sure You want to Delete this Record?", "Pinna Fitness",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    string unpost = _transactionService.UnPost(SelectedSales.Transaction);

                    if (string.IsNullOrEmpty(unpost))
                    {
                        GetLiveSales();
                        Sales = new ObservableCollection<TransactionLineDTO>(SalesList);
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Can't Delete, may be the data is already in use..."
                                                   + Environment.NewLine + ex.Message + Environment.NewLine +
                                                   ex.InnerException,
                        "Can't Delete",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public
            ICommand
            AttendanceEntryCommandView
        {
            get
            {
                return _attendanceEntryCommandView ??
                       (_attendanceEntryCommandView = new RelayCommand<Object>(AttendanceEntry));
            }
        }

        public
            void AttendanceEntry
            (object
                obj)
        {
            var memberAttendWindow = new AttendanceEntry(MembersList);
            memberAttendWindow.ShowDialog();
            var dialogueResult = memberAttendWindow.DialogResult;
            if (dialogueResult != null) //&& (bool) dialogueResult
            {
                Load(); //GetLiveAttendances();
            }
        }

        public
            void GetLiveMembers
            ()
        {
            ProgressBarVisibility = "Visible";

            var worker = new BackgroundWorker();
            worker.DoWork += DoWork2;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted2;
            worker.RunWorkerAsync();
        }

        private
            void Worker_RunWorkerCompleted2
            (object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                ProgressBarVisibility = "Collapsed";
                //MembersList = new ObservableCollection<MemberDTO>(MemberList);
            }
            catch
            {
            }
        }

        private
            void DoWork2
            (object sender, DoWorkEventArgs e)
        {
            try
            {
                GetLiveMembersBk();
            }
            catch
            {
            }
        }

        public
            void GetLiveMembersBk
            ()
        {
            var cri = new SearchCriteria<MemberDTO>();
            cri.FiList.Add(
                m =>
                    m.LastSubscription != null &&
                    !m.LastSubscription.FacilitySubscription.Facility.DisplayName.Contains("ndo"));
            cri.ShortForm = true;

            MembersList = new MemberService(true).GetAll(cri).OrderBy(i => i.Id).ToList();
        }

        //public ICommand AttendanceEditCommandView
        //{
        //    get
        //    {
        //        return _attendanceEditCommandView ??
        //               (_attendanceEditCommandView = new RelayCommand<Object>(AttendanceEdit));
        //    }
        //}

        //public void AttendanceEdit(object obj)
        //{
        //    if (SelectedAttendance != null)
        //    {
        //        int attendId = SelectedAttendance.Id;
        //        var memberAttendWindow = new AttendanceEntry(attendId);
        //        memberAttendWindow.ShowDialog();
        //        var dialogueResult = memberAttendWindow.DialogResult;
        //        if (dialogueResult != null && (bool) dialogueResult)
        //        {
        //            Load(); //GetLiveAttendances();
        //            if (Attendances != null) SelectedAttendance = Attendances.FirstOrDefault(a => a.Id == attendId);
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Select Attendance to Edit");
        //    }
        //}

        public
            ICommand
            AttendanceDeleteCommandView
        {
            get
            {
                return _attendanceDeleteCommandView ??
                       (_attendanceDeleteCommandView = new RelayCommand<Object>(AttendanceDelete));
            }
        }

        public
            void AttendanceDelete
            (object
                obj)
        {
            //int attendId = SelectedAttendance.Id;
            if (
                System.Windows.MessageBox.Show("Are you Sure You want to Delete this Record?", "Pinna Fitness",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    if (SelectedAttendance != null)
                    {
                        SelectedAttendance.Enabled = false;
                        var stat = _memberAttendanceService.Disable(SelectedAttendance);
                        if (stat == string.Empty)
                        {
                            Attendances.Remove(SelectedAttendance);
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("Can't Delete, may be the data is already in use..."
                                                           + Environment.NewLine + stat, "Can't Delete",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Can't Delete, may be the data is already in use..."
                                                   + Environment.NewLine + ex.Message + Environment.NewLine +
                                                   ex.InnerException,
                        "Can't Delete",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public
            ICommand
            DeleteCommand
        {
            get { return _deleteCommand ?? (_deleteCommand = new RelayCommand(ExecuteDeleteCommand)); }
        }

        private
            void ExecuteDeleteCommand
            ()
        {
            if (SelectedMemberSubscription == null) return;
            if (System.Windows.MessageBox.Show("Are you Sure You want to Delete this Record?",
                "Pinna Fitness",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;

            var stat = new MemberSubscriptionService(true).Delete(SelectedMemberSubscription.Id.ToString());
            if (stat == -1)
                MessageBox.Show("Can't Delete Subscription");
            else
                MemberSubscriptions.Remove(SelectedMemberSubscription);
        }

        #endregion

        #region Print List

        private
            ReportTypes _reportType;

        private
            ICommand _printAmountSummaryCommand,
            _printAmountListCommand,
            _printAttendanceSummaryCommand,
            _printAttendanceListCommand;

        public
            ReportTypes
            ReportType
        {
            get { return _reportType; }
            set
            {
                _reportType = value;
                RaisePropertyChanged<ReportTypes>(() => ReportType);
            }
        }

        public
            ICommand
            PrintAmountSummaryCommand
        {
            get
            {
                return _printAmountSummaryCommand ??
                       (_printAmountSummaryCommand = new RelayCommand<Object>(PrintAmountSummary));
            }
        }

        public
            void PrintAmountSummary
            (object
                button)
        {
            ReportType = ReportTypes.AmountSummary;
            PrintSummaryList(button);
        }

        public
            ICommand
            PrintAmountListCommand
        {
            get
            {
                return _printAmountListCommand ??
                       (_printAmountListCommand = new RelayCommand<Object>(PrintAmountList));
            }
        }

        public
            void PrintAmountList
            (object
                button)
        {
            ReportType = ReportTypes.NewRenewedList;
            PrintSummaryList(button);
        }

        public
            ICommand
            PrintAttendanceSummaryCommand
        {
            get
            {
                return _printAttendanceSummaryCommand ??
                       (_printAttendanceSummaryCommand = new RelayCommand<Object>(PrintAttendanceSummary));
            }
        }

        public
            void PrintAttendanceSummary
            (object
                button)
        {
            ReportType = ReportTypes.AttendanceSummarized;
            PrintSummaryList(button);
        }

        public
            ICommand
            PrintAttendanceListCommand
        {
            get
            {
                return _printAttendanceListCommand ??
                       (_printAttendanceListCommand = new RelayCommand<Object>(PrintAttendanceList));
            }
        }

        public
            void PrintAttendanceList
            (object
                button)
        {
            ReportType = ReportTypes.AttendanceList;
            PrintSummaryList(button);
        }


        public
            void PrintSummaryList
            (object
                button)
        {
            if (ReportType == ReportTypes.AmountSummary)
            {
                var sumUtil = new SummaryUtility(FilterStartDate, FilterEndDate, SelectedShiftType.Value);
                var list = sumUtil.GetSummary();
                sumUtil.PrintDailySummaryList(list);
            }
            else if (ReportType == ReportTypes.NewRenewedList)
            {
                PrintList(button);
            }
            else if (ReportType == ReportTypes.AttendanceList || ReportType == ReportTypes.AttendanceSummarized)
            {
                AttendanceList2(button);
            }
        }

        public
            void PrintList
            (object
                obj)
        {
            var sumUtil = new SummaryUtility(FilterStartDate, FilterEndDate, SelectedShiftType.Value);
            var list = sumUtil.GetSubscriptions();
            sumUtil.PrintDailySummaryList2(list);
        }

        public
            void AttendanceList2
            (object
                obj)
        {
            var criteria = new SearchCriteria<MemberAttendanceDTO>
            {
                BeginingDate = FilterStartDate,
                EndingDate = FilterEndDate,
                IncludePhoto = false
            };

            if (SelectedShiftType != null && SelectedShiftType.Value != 0)
            {
                if (SelectedShiftType.Value == 1)
                    criteria.Shift = ShiftTypes.Morning;
                else if (SelectedShiftType.Value == 2)
                    criteria.Shift = ShiftTypes.Afternoon;
            }
            IEnumerable<MemberAttendanceDTO> attendanceList =
                new MemberAttendanceService(true).GetAll(criteria).OrderBy(i => i.Id).ToList();

            var cri = new SearchCriteria<PervisitSubscriptionDTO>
            {
                BeginingDate = FilterStartDate,
                EndingDate = FilterEndDate
            };
            if (SelectedShiftType != null && SelectedShiftType.Value != 0)
            {
                if (SelectedShiftType.Value == 1)
                    cri.Shift = ShiftTypes.Morning;
                else if (SelectedShiftType.Value == 2)
                    cri.Shift = ShiftTypes.Afternoon;
            }
            var pervisitAttendanceList =
                new PervisitSubscriptionService(true).GetAll(cri).OrderBy(i => i.Id).ToList();


            ReportDocument myReport4;
            var summary = new SummaryUtility(FilterStartDate, FilterEndDate, SelectedShiftType.Value);
            if (ReportType == ReportTypes.AttendanceList)
            {
                myReport4 = new Reports.AttendanceListGrouped();
                myReport4.SetDataSource(summary.GetAttendanceListDataSet(attendanceList.ToList(),
                    pervisitAttendanceList.ToList()));
            }
            else
            {
                myReport4 = new Reports.AttendanceList();
                myReport4.SetDataSource(summary.GetAttendanceListSummarizedDataSet(attendanceList.ToList(),
                    pervisitAttendanceList.ToList()));
            }

            var report = new ReportViewerCommon(myReport4);
            report.Show();
        }

        #endregion

        #region Export To Excel

        private
            ICommand _exportToExcelCommand;

        public
            ICommand
            ExportToExcelCommand
        {
            get
            {
                return _exportToExcelCommand ??
                       (_exportToExcelCommand = new RelayCommand(ExecuteExportToExcelCommand));
            }
        }

        private
            void ExecuteExportToExcelCommand
            ()
        {
            string[] columnsHeader =
            {
                "Checked In", "Services Took", "Subscription No", "Number", "Name", "Sex", "Package",
                "Start Date", "End Date", "Current Status"
            };

            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
            var xlApp = new Microsoft.Office.Interop.Excel.Application();

            try
            {
                Microsoft.Office.Interop.Excel.Workbook excelBook = xlApp.Workbooks.Add();
                var excelWorksheet = (Microsoft.Office.Interop.Excel.Worksheet) excelBook.Worksheets[1];
                xlApp.Visible = true;

                int rowsTotal = Attendances.Count;
                int colsTotal = columnsHeader.Count();

                var with1 = excelWorksheet;
                with1.Cells.Select();
                with1.Cells.Delete();

                var iC = 0;
                for (iC = 0; iC <= colsTotal - 1; iC++)
                {
                    with1.Cells[1, iC + 1].Value = columnsHeader[iC];
                }

                var I = 0;
                for (I = 0; I <= rowsTotal - 1; I++)
                {
                    with1.Cells[I + 2, 0 + 1].value = Attendances[I].CheckedInTime;
                    with1.Cells[I + 2, 1 + 1].value = Attendances[I].ServicesTook;
                    with1.Cells[I + 2, 2 + 1].value = Attendances[I].MemberSubscription.SubscriptionNumber;
                    with1.Cells[I + 2, 3 + 1].value = Attendances[I].MemberSubscription.Member.Number;
                    with1.Cells[I + 2, 4 + 1].value = Attendances[I].MemberSubscription.Member.DisplayName;
                    with1.Cells[I + 2, 5 + 1].value = Attendances[I].MemberSubscription.Member.Sex;
                    with1.Cells[I + 2, 6 + 1].value =
                        Attendances[I].MemberSubscription.FacilitySubscription.PackageName;
                    with1.Cells[I + 2, 7 + 1].value = Attendances[I].MemberSubscription.StartDateString;
                    with1.Cells[I + 2, 8 + 1].value = Attendances[I].MemberSubscription.EndDateString;
                    with1.Cells[I + 2, 9 + 1].value = Attendances[I].MemberSubscription.CurrentStatus;
                }

                with1.Rows["1:1"].Font.FontStyle = "Bold";
                with1.Rows["1:1"].Font.Size = 12;

                with1.Cells.Columns.AutoFit();
                with1.Cells.Select();
                with1.Cells.EntireColumn.AutoFit();
                with1.Cells[1, 1].Select();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                //RELEASE ALLOACTED RESOURCES
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
                xlApp = null;
            }
        }

        //public void ImportFromExcel()
        //{

        //}

        #endregion

        #region Paging

        private
            ICommand _firstPageCommand, _previousPageCommand, _nextPageCommand, _lastPageCommand;

        private
            int _currentPageNumber, _totalPages;

        private
            string _currentPageNumberString, _totalPagesString;

        private
            bool _previousEnability, _nextEnability;

        public
            int CurrentPageNumber
        {
            get { return _currentPageNumber; }
            set
            {
                _currentPageNumber = value;
                RaisePropertyChanged<int>(() => CurrentPageNumber);
                CurrentPageString = "ገጽ " + CurrentPageNumber;
            }
        }

        public
            string CurrentPageString
        {
            get { return _currentPageNumberString; }
            set
            {
                _currentPageNumberString = value;
                RaisePropertyChanged<string>(() => CurrentPageString);
            }
        }

        public
            int TotalPages
        {
            get { return _totalPages; }
            set
            {
                _totalPages = value;
                RaisePropertyChanged<int>(() => TotalPages);

                TotalPagesString = TotalPages + " ገጾች";
            }
        }

        public
            string TotalPagesString
        {
            get { return _totalPagesString; }
            set
            {
                _totalPagesString = value;
                RaisePropertyChanged<string>(() => TotalPagesString);
            }
        }

        public
            bool PreviousEnability
        {
            get { return _previousEnability; }
            set
            {
                _previousEnability = value;
                RaisePropertyChanged<bool>(() => PreviousEnability);
            }
        }

        public
            bool NextEnability
        {
            get { return _nextEnability; }
            set
            {
                _nextEnability = value;
                RaisePropertyChanged<bool>(() => NextEnability);
            }
        }

        public
            ICommand
            FirstPageCommand
        {
            get { return _firstPageCommand ?? (_firstPageCommand = new RelayCommand(ExcuteFirstPageCommand)); }
        }

        public
            void ExcuteFirstPageCommand
            ()
        {
            CurrentPageNumber = 1;
            GetLiveAttendances();
            Attendances = new ObservableCollection<MemberAttendanceDTO>(AttendanceList);
        }

        public
            ICommand
            PreviousPageCommand
        {
            get { return _previousPageCommand ?? (_previousPageCommand = new RelayCommand(ExcutePreviousPageCommand)); }
        }

        public
            void ExcutePreviousPageCommand
            ()
        {
            CurrentPageNumber --;
            GetLiveAttendances();
            Attendances = new ObservableCollection<MemberAttendanceDTO>(AttendanceList);
        }

        public
            ICommand
            NextPageCommand
        {
            get { return _nextPageCommand ?? (_nextPageCommand = new RelayCommand(ExcuteNextPageCommand)); }
        }

        public
            void ExcuteNextPageCommand
            ()
        {
            CurrentPageNumber++;
            GetLiveAttendances();
            Attendances = new ObservableCollection<MemberAttendanceDTO>(AttendanceList);
        }

        public
            ICommand
            LastPageCommand
        {
            get { return _lastPageCommand ?? (_lastPageCommand = new RelayCommand(ExcuteLastPageCommand)); }
        }

        public
            void ExcuteLastPageCommand
            ()
        {
            CurrentPageNumber = TotalPages;
            GetLiveAttendances();
            Attendances = new ObservableCollection<MemberAttendanceDTO>(AttendanceList);
        }

        #endregion

        #region Previlege Visibility

        private
            UserRolesModel _userRoles;

        private
            IEnumerable<MemberSubscriptionDTO> _memberSubscriptions;

        private
            ObservableCollection<MemberSubscriptionDTO> _filteredMemberSubscriptions;

        public
            UserRolesModel
            UserRoles
        {
            get { return _userRoles; }
            set
            {
                _userRoles = value;
                RaisePropertyChanged<UserRolesModel>(() => UserRoles);
            }
        }

        private
            void CheckRoles
            ()
        {
            UserRoles = Singleton.UserRoles;
        }

        #endregion
    }
}