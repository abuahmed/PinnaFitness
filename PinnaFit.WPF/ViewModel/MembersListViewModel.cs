#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
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
using PinnaFit.WPF.Reports.DataSets;
using PinnaFit.WPF.Views;
using MessageBox = System.Windows.Forms.MessageBox;

#endregion

namespace PinnaFit.WPF.ViewModel
{
    public class MembersListViewModel : ViewModelBase
    {
        #region Fields

        private DateTime _filterStartDate, _filterEndDate;
        private static IMemberService _memberService;
        private IEnumerable<MemberDTO> _members;
        private ObservableCollection<MemberDTO> _filteredMembers;
        private IEnumerable<MemberSubscriptionDTO> _memberSubscriptionList;
        private ObservableCollection<MemberSubscriptionDTO> _filteredMemberSubscriptions;
        private MemberDetail _selectedMember;
        private MemberSubscriptionDTO _selectedMemberSubscription;
        private ICommand _refreshCommand;
        private string _progressBarVisibility, _searchText;

        #endregion

        #region Constructor

        public MembersListViewModel()
        {
            CheckRoles();
            ProgressBarVisibility = "Collapsed";
            FillStatusTypes();
            FillMembershipTypes();
            FillShiftTypes();
            LoadFacilities();
            LoadSubscriptions();

            CurrentPageNumber = 1;
            PreviousEnability = true;
            NextEnability = true;

            GetLiveMembers();
        }

        public static void CleanUp()
        {
            if (_memberService != null)
                _memberService.Dispose();
        }

        public void GetLiveMembers()
        {
            CleanUp();
            _memberService = new MemberService();
            
            ProgressBarVisibility = "Visible";
            

            var worker = new BackgroundWorker();
            worker.DoWork += DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                Members = new ObservableCollection<MemberDTO>(MemberList);
                //MemberSubscriptions = new ObservableCollection<MemberSubscriptionDTO>(MemberSubscriptionList);
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
                GetLiveMembers2();
            }
            catch
            {
            }
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

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                RaisePropertyChanged<string>(() => SearchText);
                if (MemberList != null)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(SearchText))
                        {
                            Members = new ObservableCollection<MemberDTO>
                                (MemberList.Where(c => c.MemberDetail.ToLower().Contains(value.ToLower())).ToList());
                        }
                        else
                            Members = new ObservableCollection<MemberDTO>(MemberList);
                    }
                    catch
                    {
                        System.Windows.MessageBox.Show("Problem searching Member");
                        Members = new ObservableCollection<MemberDTO>(MemberList);
                    }
                }
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

        public ObservableCollection<MemberDTO> Members
        {
            get { return _filteredMembers; }
            set
            {
                _filteredMembers = value;
                RaisePropertyChanged<ObservableCollection<MemberDTO>>(() => Members);

                TotalNumberOfRows = Members.Count;
                TotalNumberOfTransaction = Members.Sum(s => s.LastSubscription.AmountPaid);
            }
        }

        public IEnumerable<MemberSubscriptionDTO> MemberSubscriptionList
        {
            get { return _memberSubscriptionList; }
            set
            {
                _memberSubscriptionList = value;
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
                var numberOfMembers = MemberSubscriptions.GroupBy(m => m.MemberId).Count();
                TotalNumberOfRows = numberOfMembers;// MemberSubscriptions.Count;
                TotalNumberOfTransaction = MemberSubscriptions.Sum(s => s.AmountPaid);
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

        public MemberDetail SelectedMember
        {
            get { return _selectedMember; }
            set
            {
                _selectedMember = value;
                RaisePropertyChanged<MemberDetail>(() => SelectedMember);
                if (SelectedMember != null)
                {
                }
            }
        }

        #endregion

        private int _totalAttendances = 0;
        public void GetLiveMembers2()
        {
            if (CurrentPageNumber < 0)
                CurrentPageNumber = 1;

            var criteria = new SearchCriteria<MemberDTO>
            {
                Page = CurrentPageNumber,
                PageSize = 25,
            };

            criteria.FiList.Add(f => f.LastSubscriptionId != null);

            if (SelectedFacility != null && SelectedFacility.Id != -1)
                criteria.FiList.Add(f => f.LastSubscription.FacilitySubscription.FacilityId == SelectedFacility.Id);
            if (SelectedSubscription != null && SelectedSubscription.Id != -1)
                criteria.FiList.Add(
                    f => f.LastSubscription.FacilitySubscription.SubscriptionId == SelectedSubscription.Id);
            
            if (SelectedMembershipType != null) //&& SelectedMembershipType.Value != 0
            {
                if (SelectedMembershipType.Value == 1)
                {
                    criteria.FiList.Add(f => f.LastSubscription.PreviousSuscrptionId == null);
                }
                else if (SelectedMembershipType.Value == 2)
                {
                    criteria.FiList.Add(f => f.LastSubscription.PreviousSuscrptionId != null);
                }

                //if (!string.IsNullOrEmpty(StartDateText))
                //{
                //    var beginDate = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, 0, 0, 0);
                //    //criteria.FiList.Add(p => p.LastSubscription.StartDate!=null && p.LastSubscription.StartDate.Value >= beginDate);
                //    criteria.FiList.Add(p => p.LastSubscription.SubscribedDate != null && p.LastSubscription.SubscribedDate >= beginDate);
                //}

                //if (!string.IsNullOrEmpty(EndDateText))
                //{
                //    var endDate = new DateTime(EndDate.Year, EndDate.Month, EndDate.Day, 23, 59, 59);
                //    //criteria.FiList.Add(p => p.LastSubscription.StartDate != null && p.LastSubscription.StartDate.Value <= endDate);
                //    criteria.FiList.Add(p => p.LastSubscription.SubscribedDate != null && p.LastSubscription.SubscribedDate <= endDate);
                //}
            }
            PreviousEnability = criteria.Page != 1;

            MemberList = _memberService.GetAll(criteria,out _totalAttendances).OrderByDescending(i => i.Id).ToList();
            TotalNumberOfRows = _totalAttendances;

            if (SelectedStatusType != null && SelectedStatusType.Value != 0)
            {
                var toDayBegin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                //var toDayEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);

                //var toDayBeginMorning = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                //var toDayEndMorning = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 13, 59, 59);
                //var toDayBeginAfter = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 14, 0, 0);
                //var toDayEndAfter = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);


                var after5Days = toDayBegin.AddDays(5);
                var before5Days = toDayBegin.AddDays(-5);

                switch (SelectedStatusType.Value)
                {
                    case 1:
                        criteria.FiList.Add(p => p.LastSubscription.EndDate != null && p.LastSubscription.EndDate >= toDayBegin);
                        MemberList = MemberList.Where(f => f.LastSubscription.DaysLeft >= 0).ToList();
                        break;
                    case 2:
                        MemberList = MemberList.Where(f => f.LastSubscription.DaysLeft < 0).ToList();
                        criteria.FiList.Add(p => p.LastSubscription.EndDate != null && p.LastSubscription.EndDate < toDayBegin);
                        break;
                    case 3:
                        MemberList = MemberList.Where(f => f.LastSubscription.DaysLeft > -5 && f.LastSubscription.DaysLeft <= 5).ToList();
                        break;
                    case 4:
                        MemberList = MemberList.Where(f => f.LastSubscription.DaysLeft > 0 && f.LastSubscription.DaysLeft <= 5).ToList();
                        break;
                    case 5:
                        MemberList = MemberList.Where(f => f.LastSubscription.DaysLeft > -5 && f.LastSubscription.DaysLeft <= 0).ToList();
                        break;

                }
            }
            //if (SelectedShiftType != null && SelectedShiftType.Value != 0)
            //{
            //    //if (criteria.Shift == ShiftTypes.Afternoon)
            //    //    var beginDate = new DateTime(criteria.BeginingDate.Value.Year, criteria.BeginingDate.Value.Month,
            //    //    criteria.BeginingDate.Value.Day, 14, 0, 0);

            //    if (SelectedShiftType.Value == 1)
            //        MemberList = MemberList.Where(f => f.LastSubscription.SubscribedDate.TimeOfDay.Hours < 14).ToList();
            //    else if (SelectedShiftType.Value == 2)
            //        MemberList = MemberList.Where(f => f.LastSubscription.SubscribedDate.TimeOfDay.Hours >= 14).ToList();
            //}

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

            var sNo = (criteria.Page - 1) * criteria.PageSize + 1;
            foreach (var memberDto in MemberList)
            {
                memberDto.SerialNumber = sNo;
                sNo++;
            }

            #endregion

            //var cri = new SearchCriteria<MemberSubscriptionDTO>();
            //cri.FiList.Add(ms=>ms.Member.Enabled);
            //if (!string.IsNullOrEmpty(StartDateText) && !string.IsNullOrEmpty(EndDateText))
            //{
            //    cri.BeginingDate = StartDate;
            //    cri.EndingDate = EndDate;
            //}
            //if (SelectedShiftType != null && SelectedShiftType.Value != 0)
            //    cri.Shift = (ShiftTypes) SelectedShiftType.Value;

            //if (SelectedFacility != null && SelectedFacility.Id != -1)
            //    cri.FiList.Add(f => f.FacilitySubscription.FacilityId == SelectedFacility.Id);
            //if (SelectedSubscription != null && SelectedSubscription.Id != -1)
            //    cri.FiList.Add(f => f.FacilitySubscription.SubscriptionId == SelectedSubscription.Id);
            //if (SelectedMembershipType != null) //&& SelectedMembershipType.Value != 0
            //{
            //    if (SelectedMembershipType.Value == 1)
            //    {
            //        cri.FiList.Add(f => f.PreviousSuscrptionId == null);
            //    }
            //    else if (SelectedMembershipType.Value == 2)
            //    {
            //        cri.FiList.Add(f => f.PreviousSuscrptionId != null);
            //    }
            //}
            //MemberSubscriptionList = new MemberSubscriptionService(true).GetAll(cri).OrderByDescending(m => m.MemberId).ToList();
            //if (SelectedStatusType != null && SelectedStatusType.Value != 0)
            //{
            //    switch (SelectedStatusType.Value)
            //    {
            //        case 1:
            //            MemberSubscriptionList = MemberSubscriptionList.Where(f => f.DaysLeft >= 0).ToList();
            //            break;
            //        case 2:
            //            MemberSubscriptionList = MemberSubscriptionList.Where(f => f.DaysLeft < 0).ToList();
            //            break;
            //        case 3:
            //            MemberSubscriptionList = MemberSubscriptionList.Where(f => f.DaysLeft > -5 && f.DaysLeft <= 5).ToList();
            //            break;
            //        case 4:
            //            MemberSubscriptionList = MemberSubscriptionList.Where(f => f.DaysLeft > 0 && f.DaysLeft <= 5).ToList();
            //            break;
            //        case 5:
            //            MemberSubscriptionList = MemberSubscriptionList.Where(f => f.DaysLeft > -5 && f.DaysLeft <= 0).ToList();
            //            break;
            //    }
            //}

           
        }

        #region Filter List

        private string _startDateText, _endDateText;
        private DateTime _startDate, _endDate;
        private ICommand _memberStartDateViewCommand, _memberEndDateViewCommand;

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

        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                RaisePropertyChanged<DateTime>(() => StartDate);
                if (StartDate.Year > 2000)
                    StartDateText = ReportUtility.GetEthCalendarFormated(StartDate, "-") +
                                    "(" + StartDate.ToString("dd-MM-yyyy") + ")";
            }
        }

        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                RaisePropertyChanged<DateTime>(() => EndDate);

                if (EndDate.Year > 2000)
                    EndDateText = ReportUtility.GetEthCalendarFormated(EndDate, "-") + "(" +
                                  EndDate.ToString("dd-MM-yyyy") + ")";
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
                    StartDate = (DateTime) calConv.DtSelectedDate.SelectedDate;
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
                    EndDate = (DateTime) calConv.DtSelectedDate.SelectedDate;
            }
        }


        public ICommand RefreshCommand
        {
            get { return _refreshCommand ?? (_refreshCommand = new RelayCommand(ExcuteRefreshCommand)); }
        }

        public void ExcuteRefreshCommand()
        {
            GetLiveMembers();
        }

        public DateTime FilterStartDate
        {
            get { return _filterStartDate; }
            set
            {
                _filterStartDate = value;
                RaisePropertyChanged<DateTime>(() => FilterStartDate);
            }
        }

        public DateTime FilterEndDate
        {
            get { return _filterEndDate; }
            set
            {
                _filterEndDate = value;
                RaisePropertyChanged<DateTime>(() => FilterEndDate);
            }
        }

        private List<ListDataItem> _statusTypeList;
        private ListDataItem _selectedStatusType;

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

        private List<ListDataItem> _membershipTypeList;
        private ListDataItem _selectedMembershipType;

        public List<ListDataItem> MembershipTypeList
        {
            get { return _membershipTypeList; }
            set
            {
                _membershipTypeList = value;
                RaisePropertyChanged<List<ListDataItem>>(() => MembershipTypeList);
            }
        }

        public ListDataItem SelectedMembershipType
        {
            get { return _selectedMembershipType; }
            set
            {
                _selectedMembershipType = value;
                RaisePropertyChanged<ListDataItem>(() => SelectedMembershipType);
            }
        }

        public void FillMembershipTypes()
        {
            MembershipTypeList = (List<ListDataItem>) CommonUtility.GetList(typeof (MembershipTypes));
        }

        #endregion

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
            ShiftTypeList = (List<ListDataItem>) CommonUtility.GetList(typeof (ShiftTypes));

            SelectedShiftType = ShiftTypeList.FirstOrDefault(s => s.Value == 0);

            //var nowTime = DateTime.Now.TimeOfDay.Hours;
            //if (nowTime < 14)
            //    SelectedShiftType = ShiftTypeList.FirstOrDefault(s => s.Value == 1);
            //else
            //    SelectedShiftType = ShiftTypeList.FirstOrDefault(s => s.Value == 2);
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

        #endregion

        #region Summary

        private int _totalNumberOfRows;
        private decimal _totalNumberOfTransaction;
        private string _summaryVisibility;

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

        public string SummaryVisibility
        {
            get { return _summaryVisibility; }
            set
            {
                _summaryVisibility = value;
                RaisePropertyChanged<string>(() => SummaryVisibility);
            }
        }

        #endregion

        #region Commands

        private ICommand _printListCommandView, _exportToExcelCommand, _deleteCommand;


        public ICommand PrintListCommandView
        {
            get { return _printListCommandView ?? (_printListCommandView = new RelayCommand<Object>(PrintList)); }
        }

        public void PrintList(object obj)
        {
            int fac = -1, sub = -1;
            if (SelectedFacility != null && SelectedFacility.Id != -1)
                fac = SelectedFacility.Id;
            if (SelectedSubscription != null && SelectedSubscription.Id != -1)
                sub = SelectedSubscription.Id;

            var sumUtil = new SummaryUtility(StartDate, EndDate, SelectedShiftType.Value, fac, sub);
            var list = sumUtil.GetMembers();
            if (SelectedStatusType != null && SelectedStatusType.Value != 0)
            {
                switch (SelectedStatusType.Value)
                {
                    case 1:
                        list = list.Where(f => f.DaysLeft >= 0).ToList();
                        break;
                    case 2:
                        list = list.Where(f => f.DaysLeft < 0).ToList();
                        break;
                    case 3:
                        list = list.Where(f => f.DaysLeft > -5 && f.DaysLeft <= 5).ToList();
                        break;
                    case 4:
                        list = list.Where(f => f.DaysLeft > 0 && f.DaysLeft <= 5).ToList();
                        break;
                    case 5:
                        list = list.Where(f => f.DaysLeft > -5 && f.DaysLeft <= 0).ToList();
                        break;
                }
            }
            sumUtil.PrintDailySummaryList2(list);
            //var myReport4 = new Reports.MembersList2();
            //myReport4.SetDataSource(GetListDataSet());

            //var report = new ReportViewerCommon(myReport4);
            //report.ShowDialog();
        }

        public FitnessDataSet GetListDataSet()
        {
            var myDataSet = new FitnessDataSet();
            var serNo = 1;
            var selectedCompany = new CompanyService(true).GetCompany();
            foreach (var member in Members)
            {
                myDataSet.MembersList.Rows.Add(
                    serNo,
                    member.DisplayName,
                    member.Number,
                    member.SexAmharic,
                    member.Age,
                    member.Address.AddressDetail,
                    member.Address.Mobile,
                    member.LastSubscription.FacilitySubscription.PackageName,
                    member.LastSubscription.FacilitySubscription.Subscription.DisplayName,
                    member.LastSubscription.AmountPaid,
                    member.LastSubscription.SubscribedDateStringAmharicFormatted,
                    member.LastSubscription.EndDateStringAmharicFormatted,
                    member.LastSubscription.CurrentStatus,
                    "",
                    "",
                    0, 0, selectedCompany.Header, "");

                serNo++;
            }

            return myDataSet;
        }


        public ICommand ExportToExcelCommand
        {
            get
            {
                return _exportToExcelCommand ?? (_exportToExcelCommand = new RelayCommand(ExecuteExportToExcelCommand));
            }
        }

        private void ExecuteExportToExcelCommand()
        {
            string[] columnsHeader =
            {
                "መለያ ቁ.", "ስም", "ጾታ", "ዕድሜ", "አድራሻ",
                "ስልክ ቁ.", "ፓኬጅ", "", "ክፍያ",
                "የጀመረበት ቀን", "የሚያበቃበት ቀን", "አሁን ያለበት ሁኔታ"
            };

            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
            var xlApp = new Microsoft.Office.Interop.Excel.Application();

            try
            {
                Microsoft.Office.Interop.Excel.Workbook excelBook = xlApp.Workbooks.Add();
                var excelWorksheet = (Microsoft.Office.Interop.Excel.Worksheet) excelBook.Worksheets[1];
                xlApp.Visible = true;

                int rowsTotal = Members.Count;
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
                    with1.Cells[I + 2, 0 + 1].value = Members[I].Number;
                    with1.Cells[I + 2, 1 + 1].value = Members[I].DisplayName;
                    with1.Cells[I + 2, 2 + 1].value = Members[I].SexAmharic;
                    with1.Cells[I + 2, 3 + 1].value = Members[I].Age.ToString();
                    with1.Cells[I + 2, 4 + 1].value = Members[I].Address.AddressDetail;
                    with1.Cells[I + 2, 5 + 1].value = Members[I].Address.Mobile;
                    with1.Cells[I + 2, 6 + 1].value = Members[I].LastSubscription.FacilitySubscription.PackageName;
                    //with1.Cells[I + 2, 7 + 1].value =Members[I].LastSubscription.FacilitySubscription.Subscription.DisplayName;
                    with1.Cells[I + 2, 8 + 1].value = Members[I].LastSubscription.AmountPaid;
                    with1.Cells[I + 2, 9 + 1].value = Members[I].LastSubscription.StartDateStringAmharicFormatted;
                    with1.Cells[I + 2, 10 + 1].value = Members[I].LastSubscription.EndDateStringAmharicFormatted;
                    with1.Cells[I + 2, 11 + 1].value = Members[I].LastSubscription.CurrentStatus;
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


        public ICommand DeleteCommand
        {
            get { return _deleteCommand ?? (_deleteCommand = new RelayCommand(ExecuteDeleteCommand)); }
        }

        private void ExecuteDeleteCommand()
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

        #region Paging

        private ICommand _firstPageCommand, _previousPageCommand, _nextPageCommand, _lastPageCommand;
        private int _currentPageNumber, _totalPages;
        private string _currentPageNumberString, _totalPagesString;
        private bool _previousEnability, _nextEnability;

        public int CurrentPageNumber
        {
            get { return _currentPageNumber; }
            set
            {
                _currentPageNumber = value;
                RaisePropertyChanged<int>(() => CurrentPageNumber);
                CurrentPageString = "ገጽ " + CurrentPageNumber;
            }
        }

        public string CurrentPageString
        {
            get { return _currentPageNumberString; }
            set
            {
                _currentPageNumberString = value;
                RaisePropertyChanged<string>(() => CurrentPageString);
            }
        }

        public int TotalPages
        {
            get { return _totalPages; }
            set
            {
                _totalPages = value;
                RaisePropertyChanged<int>(() => TotalPages);

                TotalPagesString = TotalPages + " ገጾች";
            }
        }

        public string TotalPagesString
        {
            get { return _totalPagesString; }
            set
            {
                _totalPagesString = value;
                RaisePropertyChanged<string>(() => TotalPagesString);
            }
        }

        public bool PreviousEnability
        {
            get { return _previousEnability; }
            set
            {
                _previousEnability = value;
                RaisePropertyChanged<bool>(() => PreviousEnability);
            }
        }

        public bool NextEnability
        {
            get { return _nextEnability; }
            set
            {
                _nextEnability = value;
                RaisePropertyChanged<bool>(() => NextEnability);
            }
        }

        public ICommand FirstPageCommand
        {
            get { return _firstPageCommand ?? (_firstPageCommand = new RelayCommand(ExcuteFirstPageCommand)); }
        }

        public void ExcuteFirstPageCommand()
        {
            CurrentPageNumber = 1;
            GetLiveMembers();
            Members = new ObservableCollection<MemberDTO>(MemberList);
        }

        public ICommand PreviousPageCommand
        {
            get { return _previousPageCommand ?? (_previousPageCommand = new RelayCommand(ExcutePreviousPageCommand)); }
        }

        public void ExcutePreviousPageCommand()
        {
            CurrentPageNumber--;
            GetLiveMembers();
            Members = new ObservableCollection<MemberDTO>(MemberList);
        }

        public ICommand NextPageCommand
        {
            get { return _nextPageCommand ?? (_nextPageCommand = new RelayCommand(ExcuteNextPageCommand)); }
        }

        public void ExcuteNextPageCommand()
        {
            CurrentPageNumber++;
            GetLiveMembers();
            Members = new ObservableCollection<MemberDTO>(MemberList);
        }

        public ICommand LastPageCommand
        {
            get { return _lastPageCommand ?? (_lastPageCommand = new RelayCommand(ExcuteLastPageCommand)); }
        }

        public void ExcuteLastPageCommand()
        {
            CurrentPageNumber = TotalPages;
            GetLiveMembers();
            Members = new ObservableCollection<MemberDTO>(MemberList);
        }

        #endregion
    }
}