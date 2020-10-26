using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PinnaFit.Core;
using PinnaFit.Core.Common;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Models;
using PinnaFit.Service;
using PinnaFit.Service.Interfaces;
using PinnaFit.WPF.Models;
using Telerik.Windows.Controls.ChartView;

namespace PinnaFit.WPF.ViewModel
{
    public class DashBoardViewModel : ViewModelBase
    {
        #region Fields
        private static IMemberService _memberService;
        private static IPervisitSubscriptionService _pervisitSubscriptionService;
        private static ITransactionService _transactionService;
        private IEnumerable<MemberDTO> _members;
        private IEnumerable<SubscriptionModel> _memberSubscriptionList, _memberSubscriptionListForNumberSummary;
        private IEnumerable<SubscriptionSumModel> _morningSubList,_afternoonSubList, _memberCategorySubList;
        private object _pieData, _statusPieDate, _facilityPieDate, _subscriptionPieDate, _barData;
        private ChartPalette _palette;
        private int _totalNumberOfMembers;
        private string _totalNumberOfActiveMembers, _totalNumberOfExpiredMembers;
        private string _progressBarVisibility, _totalNumberOfMembersString;
        private readonly System.Timers.Timer _monitorTimer;
        private const int MonitorTimerDelay = 300000; //60000-1 minute
        #endregion

        #region Constructor
        public DashBoardViewModel()
        {
            CheckRoles();
            FillCalendar();
            FillShiftTypes();
            var currentMonth = Convert.ToInt32(ReportUtility.GetEthCalendar(DateTime.Now, false).Substring(2, 2));
            SelectedEthioMonth = EthioMonths.FirstOrDefault(e => e.Value == currentMonth);

            var currentDay = Convert.ToInt32(ReportUtility.GetEthCalendar(DateTime.Now, false).Substring(0, 2));
            SelectedEthioDay = EthioDays.FirstOrDefault(e => e.Value == currentDay);

            var currentYear = Convert.ToInt32(ReportUtility.GetEthCalendar(DateTime.Now, false).Substring(4, 4));
            SelectedEthioYear = EthioYears.FirstOrDefault(e => e.Value == currentYear);
            
            Palette = ChartPalettes.Windows8;//Summer
            Load();

            _monitorTimer = new System.Timers.Timer(MonitorTimerDelay);
            _monitorTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnMonitorTimerElapsed);
            _monitorTimer.Enabled = true;
        }

        public void Load()
        {
             CleanUp();
            _memberService = new MemberService();
            _pervisitSubscriptionService = new PervisitSubscriptionService();
            _transactionService=new TransactionService();

            GetLiveMembers();
        }
        public static void CleanUp()
        {
            if (_memberService != null)
                _memberService.Dispose();
            if (_pervisitSubscriptionService != null)
                _pervisitSubscriptionService.Dispose();
            if (_transactionService != null)
                _transactionService.Dispose();
        } 

        public void GetLiveMembers()
        {
            ProgressBarVisibility = "Visible";

            var worker = new BackgroundWorker();
            worker.DoWork += DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private void OnMonitorTimerElapsed(object source, System.Timers.ElapsedEventArgs e)
        {
            _monitorTimer.Enabled = false;
            
            try
            {
                Load();
            }
            catch
            {
            }

        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                ProgressBarVisibility = "Collapsed";
                _monitorTimer.Enabled = true;
            }
            catch
            { }
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                LoadMembers();
            }
            catch { }
        }
        
        #endregion

        #region Public Properties
        public string ProgressBarVisibility
        {
            get { return _progressBarVisibility; }
            set
            {
                _progressBarVisibility = value;
                RaisePropertyChanged<string>(() => ProgressBarVisibility);
            }
        }
        
        public IEnumerable<MemberDTO> MemberList
        {
            get { return _members; }
            set
            {
                _members = value;
                RaisePropertyChanged<IEnumerable<MemberDTO>>(() => MemberList);
            }
        }
        public IEnumerable<SubscriptionModel> MemberSubscriptionList
        {
            get { return _memberSubscriptionList; }
            set
            {
                _memberSubscriptionList = value;
                RaisePropertyChanged<IEnumerable<SubscriptionModel>>(() => MemberSubscriptionList);
            }
        }
        public IEnumerable<SubscriptionModel> MemberSubscriptionListForNumberSummary
        {
            get { return _memberSubscriptionListForNumberSummary; }
            set
            {
                _memberSubscriptionListForNumberSummary = value;
                RaisePropertyChanged<IEnumerable<SubscriptionModel>>(() => MemberSubscriptionListForNumberSummary);
            }
        }
        public IEnumerable<SubscriptionSumModel> MorningSubList
        {
            get { return _morningSubList; }
            set
            {
                _morningSubList = value;
                RaisePropertyChanged<IEnumerable<SubscriptionSumModel>>(() => MorningSubList);
            }
        }
        public IEnumerable<SubscriptionSumModel> AfternoonSubList
        {
            get { return _afternoonSubList; }
            set
            {
                _afternoonSubList = value;
                RaisePropertyChanged<IEnumerable<SubscriptionSumModel>>(() => AfternoonSubList);
            }
        }
        public IEnumerable<SubscriptionSumModel> MemberCategorySubList
        {
            get { return _memberCategorySubList; }
            set
            {
                _memberCategorySubList = value;
                RaisePropertyChanged<IEnumerable<SubscriptionSumModel>>(() => MemberCategorySubList);
            }
        }
        public object BarData
        {
            get { return _barData; }
            set
            {
                _barData = value;
                RaisePropertyChanged<object>(() => BarData);
            }
        }
        public object PieData
        {
            get { return _pieData; }
            set
            {
                _pieData = value;
                RaisePropertyChanged<object>(() => PieData);
            }
        }
        public object StatusPieData
        {
            get { return _statusPieDate; }
            set
            {
                _statusPieDate = value;
                RaisePropertyChanged<object>(() => StatusPieData);
            }
        }
        public object FacilityPieData
        {
            get { return _facilityPieDate; }
            set
            {
                _facilityPieDate = value;
                RaisePropertyChanged<object>(() => FacilityPieData);
            }
        }
        public object SubscriptionPieData
        {
            get { return _subscriptionPieDate; }
            set
            {
                _subscriptionPieDate = value;
                RaisePropertyChanged<object>(() => SubscriptionPieData);
            }
        }
        public ChartPalette Palette
        {
            get { return _palette; }
            set
            {
                _palette = value;
                RaisePropertyChanged<ChartPalette>(() => Palette);
            }
        }
        public int TotalNumberOfMembers
        {
            get { return _totalNumberOfMembers; }
            set
            {
                _totalNumberOfMembers = value;
                RaisePropertyChanged<int>(() => TotalNumberOfMembers);
                TotalNumberOfMembersString = "አጠቃላይ አባላት: " + TotalNumberOfMembers.ToString();
            }
        }
        public string TotalNumberOfActiveMembers
        {
            get { return _totalNumberOfActiveMembers; }
            set
            {
                _totalNumberOfActiveMembers = value;
                RaisePropertyChanged<string>(() => TotalNumberOfActiveMembers);
            }
        }
        public string TotalNumberOfExpiredMembers
        {
            get { return _totalNumberOfExpiredMembers; }
            set
            {
                _totalNumberOfExpiredMembers = value;
                RaisePropertyChanged<string>(() => TotalNumberOfExpiredMembers);
            }
        }
        public string TotalNumberOfMembersString
        {
            get { return _totalNumberOfMembersString; }
            set
            {
                _totalNumberOfMembersString = value;
                RaisePropertyChanged<string>(() => TotalNumberOfMembersString);
            }
        } 
        #endregion
        
        public void LoadMembers()
        {
            var sutil=new SummaryUtility();
            var membersList = sutil.GetMembers().ToList();
            TotalNumberOfMembers = membersList.Count();

            MemberSubscriptionListForNumberSummary = membersList;
            
            MemberSubscriptionList= sutil.GetSubscriptions().ToList();

            #region Group By Sex

            var maleSex = membersList.Count(s => s.Sex == Sex.Male);
            var feMaleSex = membersList.Count(s => s.Sex == Sex.Female);
            var totalSex = maleSex + feMaleSex;
            maleSex = (maleSex*100)/totalSex;
            feMaleSex = (feMaleSex * 100) / totalSex;

            var pieData2 = new List<Data>
            {
                new Data
                {
                    Category = "ወንድ",
                    Value = maleSex
                },
                new Data
                {
                    Category = "ሴት",
                    Value = feMaleSex
                }
            };
            PieData = pieData2;
            #endregion

            #region Group By Status

            var active = membersList.Count(s => !s.IsExpired);
            TotalNumberOfActiveMembers = "ጊዜ ያላቸው: " + active.ToString();
            var expired = membersList.Count(s => s.IsExpired);
            TotalNumberOfExpiredMembers = "ጊዜ ያለፈባቸው: " + expired.ToString();
            var tot = active + expired;
            active = (active * 100) / tot;
            expired = (expired * 100) / tot;

            var pieDataStatus = new List<Data>
            {
                new Data
                {
                    Category = "ጊዜ ያለው",
                    Value = active
                },
                new Data
                {
                    Category = "ጊዜ ያለፈበት",
                    Value = expired
                }
            };

            StatusPieData = pieDataStatus;
            #endregion
            
            #region Group By Facility
            var querFacility = from memberSubscriptionList in membersList
                              group memberSubscriptionList by memberSubscriptionList.FacilityName 
                                  into newGroup
                                  orderby newGroup.Key
                                  select newGroup;

            MemberCategorySubList = querFacility.Select(nameGroup => new SubscriptionSumModel
            {
                Category = nameGroup.Key,
                SubscriptionModels = nameGroup
            }).ToList();

            var facilityPieData = MemberCategorySubList.Select(fac => new Data(TotalNumberOfMembers)
            {
                Category = fac.Category,
                Value = fac.TotalNumber
            }).ToList();

            FacilityPieData = facilityPieData;

            #endregion

            #region Group By Subscription
            var querSubscription = from memberSubscriptionList in membersList
                               group memberSubscriptionList by memberSubscriptionList.SubscriptionName
                                   into newGroup
                                   orderby newGroup.Key
                                   select newGroup;

            MemberCategorySubList = querSubscription.Select(nameGroup => new SubscriptionSumModel
            {
                Category = nameGroup.Key,
                SubscriptionModels = nameGroup
            }).ToList();

            var subscriptionPieData = MemberCategorySubList.Select(fac => new Data(TotalNumberOfMembers)
            {
                Category = fac.Category,
                Value = fac.TotalNumber
            }).ToList();

            SubscriptionPieData = subscriptionPieData;

            #endregion
            
            MemberSubscriptionList = MemberSubscriptionList.Union(sutil.GetPervisit().ToList()).ToList();
            
            MemberSubscriptionList = MemberSubscriptionList.Union(sutil.GetSales().ToList()).ToList();

            var morningSubList = MemberSubscriptionList.Where(m => m.Shift == ShiftTypes.Morning).ToList();
            var afternoonSubList = MemberSubscriptionList.Where(m => m.Shift == ShiftTypes.Afternoon).ToList();
            
            #region Group By Registration Date
            var queryAmount = from memberSubscriptionList in morningSubList
                              group memberSubscriptionList by memberSubscriptionList.StartDate
                                  into newGroup
                                  orderby newGroup.Key
                                  select newGroup;

            MorningSubList = queryAmount.Select(nameGroup => new SubscriptionSumModel
            {
                TransactionDate = nameGroup.Key,
                StartDate = ReportUtility.GetEthCalendar(nameGroup.Key, false).Substring(0, 2),
                StartMonth = ReportUtility.GetEthCalendar(nameGroup.Key, false).Substring(2, 2),
                SubscriptionModels = nameGroup
            }).ToList();


            var mon = SelectedEthioMonth.Value.ToString();
            if (SelectedEthioMonth.Value < 10)
                mon = "0" + SelectedEthioMonth.Value;
            MorningSubList = MorningSubList.Where(m => m.StartMonth == mon).ToList();
            
            #endregion

            #region Group By Registration Date
            queryAmount = from memberSubscriptionList in afternoonSubList
                              group memberSubscriptionList by memberSubscriptionList.StartDate
                                  into newGroup
                                  orderby newGroup.Key
                                  select newGroup;

            AfternoonSubList = queryAmount.Select(nameGroup => new SubscriptionSumModel
            {
                TransactionDate = nameGroup.Key,
                StartDate = ReportUtility.GetEthCalendar(nameGroup.Key, false).Substring(0, 2),
                StartMonth = ReportUtility.GetEthCalendar(nameGroup.Key, false).Substring(2, 2),
                SubscriptionModels = nameGroup
            }).ToList();


            mon = SelectedEthioMonth.Value.ToString();
            if (SelectedEthioMonth.Value < 10)
                mon = "0" + SelectedEthioMonth.Value;
            AfternoonSubList = AfternoonSubList.Where(m => m.StartMonth == mon).ToList();

            #endregion
        }
        public IEnumerable<TransactionLineDTO> GetLiveSales()
        {
            var criteria = new SearchCriteria<TransactionHeaderDTO>
            {
                TransactionType = (int) TransactionTypes.SellStock,
                CurrentUserId = Singleton.User.UserId
            };
            
            var salesHeader = _transactionService.GetAll(criteria).OrderBy(i => i.Id).ToList();
            return salesHeader.SelectMany(s => s.TransactionLines).ToList();
        }

        #region Shift Type
        private List<ListDataItem> _shiftTypeList;
        private ListDataItem _selectedShiftType;

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
            ShiftTypeList = (List<ListDataItem>)CommonUtility.GetList(typeof(ShiftTypes));

            var nowTime = DateTime.Now.TimeOfDay.Hours;
            if (nowTime < 14)
                SelectedShiftType = ShiftTypeList.FirstOrDefault(s => s.Value == 1);
            else
                SelectedShiftType = ShiftTypeList.FirstOrDefault(s => s.Value == 2);

            //if(DateTime.Now.TimeOfDay)
        }

        #endregion

        #region Commands

        private ICommand _refreshCommand;//, _printListCommandView, _printDailySummaryCommandView;

        public ICommand RefreshCommand
        {
            get { return _refreshCommand ?? (_refreshCommand = new RelayCommand(ExcuteRefreshCommand)); }
        }
        public void ExcuteRefreshCommand()
        {
            Load();
        }

        //public ICommand PrintSummaryCommandView
        //{
        //    get { return _printListCommandView ?? (_printListCommandView = new RelayCommand<Object>(PrintList)); }
        //}
        //public void PrintList(object obj)
        //{
        //    var myReport4 = new PinnaFit.WPF.Reports.Summary.NumberSummay();
        //    myReport4.SetDataSource(GetNumberListDataSet());

        //    //MenuItem menu = obj as MenuItem;
        //    //if (menu != null)
        //    //    new ReportUtility().DirectPrinter(myReport4);
        //    //else
        //    //{
        //    var report = new ReportViewerCommon(myReport4);
        //    report.Show();
        //    //}
        //}
        //public FitnessDataSet GetNumberListDataSet()
        //{
        //    var myDataSet = new FitnessDataSet();
        //    var serNo = 1;
        //    var selectedCompany = new CompanyService(true).GetCompany();

        //    MemberSubscriptionListForNumberSummary = MemberSubscriptionListForNumberSummary.Where(m => !m.IsExpired).ToList();

        //    #region Group By Facility
        //    var querFacility = from memberSubscriptionList in MemberSubscriptionListForNumberSummary
        //                       group memberSubscriptionList by memberSubscriptionList.FacilityName
        //                           into newGroup
        //                           orderby newGroup.Key
        //                           select newGroup;

        //    MemberCategorySubList = querFacility.Select(nameGroup => new SubscriptionSumModel
        //    {
        //        Category = nameGroup.Key,
        //        SubscriptionModels = nameGroup
        //    }).ToList();

          
        //    foreach (var subscriptionSumModel in MemberCategorySubList)
        //    {
        //        string cat = subscriptionSumModel.Category;
        //        var subModels = subscriptionSumModel.SubscriptionModels;

        //        var querSubscription = from memberSubscriptionList in subModels
        //                               group memberSubscriptionList by memberSubscriptionList.SubscriptionName
        //                                   into newGroup
        //                                   orderby newGroup.Key
        //                                   select newGroup;

        //        var memberCategorySubList2 = querSubscription.Select(nameGroup => new SubscriptionSumModel
        //        {
        //            Category = nameGroup.Key,
        //            SubscriptionModels = nameGroup
        //        }).ToList();

        //        int permonth = 0, threemonth = 0, sixmonth = 0, oneyear = 0;

        //        var firstOrDefault = memberCategorySubList2.FirstOrDefault(a => a.Category == "Per Month");
        //        if (firstOrDefault != null)
        //        {
        //            permonth = firstOrDefault.TotalNumber;
        //        }

        //        firstOrDefault = memberCategorySubList2.FirstOrDefault(a => a.Category == "3 Months");
        //        if (firstOrDefault != null)
        //        {
        //            threemonth = firstOrDefault.TotalNumber;
        //        }

        //        firstOrDefault = memberCategorySubList2.FirstOrDefault(a => a.Category == "6 Months");
        //        if (firstOrDefault != null)
        //        {
        //            sixmonth = firstOrDefault.TotalNumber;
        //        }

        //        firstOrDefault = memberCategorySubList2.FirstOrDefault(a => a.Category == "1 Year");
        //        if (firstOrDefault != null)
        //        {
        //            oneyear = firstOrDefault.TotalNumber;
        //        }

        //        var tot = oneyear + permonth + threemonth + sixmonth;

        //        myDataSet.SummaryReport.Rows.Add(
        //          serNo,
        //          ReportUtility.GetEthCalendarFormated(DateTime.Now, "-") + "(" +
        //            DateTime.Now.ToShortDateString() + ")",
        //          cat,
        //          0,
        //          permonth,
        //          threemonth,
        //          sixmonth,
        //          oneyear,
        //          tot,
        //          "",
        //          "",
        //          0, 0, selectedCompany.Header, "");

        //        serNo++;
        //    }



        //    #endregion


        //    return myDataSet;
        //}
        
        //public ICommand PrintDailySummaryCommandView
        //{
        //    get { return _printDailySummaryCommandView ?? (_printDailySummaryCommandView = new RelayCommand<Object>(PrintDailySummaryList)); }
        //}
        //public void PrintDailySummaryList(object obj)
        //{
        //    var myReport4 = new Reports.Summary.DailySummay();
        //    var dSet = GetSummaryDataSet();
        //    if (dSet == null)
        //        return;

        //    myReport4.SetDataSource(dSet);

        //    var report = new ReportViewerCommon(myReport4);
        //    report.Show();
         
        //}
        //public FitnessDataSet GetSummaryDataSet()
        //{
        //    try
        //    {
        //        var selectDate = SelectedEthioDay.Value + "/" + SelectedEthioMonth.Value + "/" + SelectedEthioYear.Display;

        //        var selectedDate = ReportUtility.GetGregorCalendar(SelectedEthioYear.Value, SelectedEthioMonth.Value,
        //            SelectedEthioDay.Value);

        //        var myDataSet = new FitnessDataSet();
        //        var serNo = 1;
        //        var selectedCompany = new CompanyService(true).GetCompany();

        //        var MemberSubsList = MemberSubscriptionList.Where(
        //            m => m.TransactionDate.Day == selectedDate.Day &&
        //            m.TransactionDate.Month == selectedDate.Month &&
        //            m.TransactionDate.Year == selectedDate.Year).ToList();

        //        if (MemberSubsList.Count == 0)
        //        {
        //            MessageBox.Show("No Data found on: " + Environment.NewLine + selectDate + "(" +
        //                selectedDate.ToShortDateString() + ")" + "(" + SelectedShiftType.Display + ")", "No Data",
        //                MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            return null;
        //        }

        //        #region Group By Facility
        //        var querFacility = from memberSubscriptionList in MemberSubsList
        //                           group memberSubscriptionList by memberSubscriptionList.PackageName
        //                               into newGroup
        //                               orderby newGroup.Key
        //                               select newGroup;

        //        MemberCategorySubList = querFacility.Select(nameGroup => new SubscriptionSumModel
        //        {
        //            Category = nameGroup.Key,
        //            SubscriptionModels = nameGroup
        //        }).ToList();

        //        foreach (var subscriptionSumModel in MemberCategorySubList)
        //        {
        //            var subtototal = subscriptionSumModel.TotalAmount / (decimal)1.15;
        //            var tax = subtototal * (decimal)0.15;

        //            myDataSet.DailySummaryReport.Rows.Add(
        //              serNo,
        //              ReportUtility.GetEthCalendarFormated(selectedDate, "-") + "(" +
        //                selectedDate.ToShortDateString() + ") - (" + SelectedShiftType.Display + ")",
        //              subscriptionSumModel.Category,
        //              subscriptionSumModel.TotalUnit,
        //              subtototal,
        //              tax,
        //              subscriptionSumModel.TotalAmount,
        //              "",
        //              "",
        //              0, 0, selectedCompany.Header, "");

        //            serNo++;
        //        }



        //        #endregion


        //        return myDataSet;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //} 

        #endregion

        #region Filter List
        public void FillCalendar()
        {
            #region Initialize
            _ethioDays = new ObservableCollection<ListDataItem>();
            _ethioMonths = new ObservableCollection<ListDataItem>();
            _ethioYears = new ObservableCollection<ListDataItem>();
            _selectedEthioDay = new ListDataItem();
            _selectedEthioMonth = new ListDataItem();
            _selectedEthioYear = new ListDataItem();
            #endregion

            for (var i = 1; i <= 30; i++)
            {
                EthioDays.Add(new ListDataItem { Display = i.ToString(), Value = i });
            }

            for (var i = 1; i <= 12; i++)
            {
                EthioMonths.Add(new ListDataItem { Display = ReportUtility.GetAmhMonth(i - 1), Value = i });
            }
            EthioMonths.Add(new ListDataItem { Display = ReportUtility.GetAmhMonth(12), Value = 13 });

            for (var i = 2008; i <= 2020; i++)
            {
                EthioYears.Add(new ListDataItem { Display = i.ToString(), Value = i });
            }
        }

        private ObservableCollection<ListDataItem> _ethioDays,_ethioMonths, _ethioYears;
        private ListDataItem _selectedEthioDay,_selectedEthioMonth, _selectedEthioYear;

        public ObservableCollection<ListDataItem> EthioDays
        {
            get { return _ethioDays; }
            set
            {
                _ethioDays = value;
                RaisePropertyChanged<ObservableCollection<ListDataItem>>(() => EthioDays);
            }
        }
        public ListDataItem SelectedEthioDay
        {
            get { return _selectedEthioDay; }
            set
            {
                _selectedEthioDay = value;
                RaisePropertyChanged<ListDataItem>(() => SelectedEthioDay);
            }
        }

        public ObservableCollection<ListDataItem> EthioMonths
        {
            get { return _ethioMonths; }
            set
            {
                _ethioMonths = value;
                RaisePropertyChanged<ObservableCollection<ListDataItem>>(() => EthioMonths);
            }
        }
        public ListDataItem SelectedEthioMonth
        {
            get { return _selectedEthioMonth; }
            set
            {
                _selectedEthioMonth = value;
                RaisePropertyChanged<ListDataItem>(() => SelectedEthioMonth);
            }
        }

        public ObservableCollection<ListDataItem> EthioYears
        {
            get { return _ethioYears; }
            set
            {
                _ethioYears = value;
                RaisePropertyChanged<ObservableCollection<ListDataItem>>(() => EthioYears);
            }
        }
        public ListDataItem SelectedEthioYear
        {
            get { return _selectedEthioYear; }
            set
            {
                _selectedEthioYear = value;
                RaisePropertyChanged<ListDataItem>(() => SelectedEthioYear);
            }
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
