#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using CrystalDecisions.CrystalReports.Engine;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PinnaFit.Core;
using PinnaFit.Core.Common;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Extensions;
using PinnaFit.Core.Models;
using PinnaFit.Service;
using PinnaFit.WPF.Models;
using PinnaFit.WPF.Reports.DataSets;
using PinnaFit.WPF.Views;

#endregion

namespace PinnaFit.WPF.ViewModel
{
    public class DurationViewModel : ViewModelBase
    {
        #region Fields
        private ICommand _closeItemViewCommand;
        private ReportTypes _reportType;
        private ICommand _memberStartDateViewCommand, _memberEndDateViewCommand;
        private string _startDateText, _endDateText, _headerText;
        #endregion

        #region Constructor

        public DurationViewModel()
        {
            FillShiftTypes();
            FilterStartDate = DateTime.Now;
            FilterEndDate = DateTime.Now;

            Messenger.Default.Register<ReportTypes>(this, (message) => { ReportType = message; });
        }

        #endregion

        #region Public Properties
        public string HeaderText
        {
            get { return _headerText; }
            set
            {
                _headerText = value;
                RaisePropertyChanged<string>(() => HeaderText);
            }
        }

        public ReportTypes ReportType
        {
            get { return _reportType; }
            set
            {
                _reportType = value;
                RaisePropertyChanged<ReportTypes>(() => ReportType);
                HeaderText = EnumUtil.GetEnumDesc(ReportType) + " ሪፖርት ማውጫ";
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
        
        private ICommand _printSummaryListCommandView;

        public ICommand PrintSummaryListCommandView
        {
            get
            {
                return _printSummaryListCommandView ??
                       (_printSummaryListCommandView = new RelayCommand<Object>(PrintSummaryList));
            }
        }

        public void PrintSummaryList(object obj)
        {
            if (ReportType == ReportTypes.AmountSummary)
            {
                var sumUtil = new SummaryUtility(FilterStartDate, FilterEndDate, SelectedShiftType.Value);
                var list = sumUtil.GetSummary();
                sumUtil.PrintDailySummaryList(list);
            }
            else if (ReportType == ReportTypes.NewRenewedList)
            {
                PrintList(obj);
            }
            else if (ReportType == ReportTypes.AttendanceList || ReportType == ReportTypes.AttendanceSummarized)
            {
                AttendanceList(obj);
            }
        }

        public void PrintList(object obj)
        {
            var sumUtil = new SummaryUtility(FilterStartDate, FilterEndDate, SelectedShiftType.Value);
            var list = sumUtil.GetSubscriptions();
            sumUtil.PrintDailySummaryList2(list);
        }

        public void AttendanceList(object obj)
        {
            var criteria = new SearchCriteria<MemberAttendanceDTO>
            {
                BeginingDate = FilterStartDate,
                EndingDate = FilterEndDate,
                IncludePhoto = false
            };

            //if (SelectedMember != null && SelectedMember.Id != -1)
            //    criteria.FiList.Add(f => f.MemberSubscription.MemberId == SelectedMember.Id);
            //if (SelectedFacility != null && SelectedFacility.Id != -1)
            //    criteria.FiList.Add(f => f.MemberSubscription.FacilitySubscription.FacilityId == SelectedFacility.Id);
            //if (SelectedSubscription != null && SelectedSubscription.Id != -1)
            //    criteria.FiList.Add(
            //        f => f.MemberSubscription.FacilitySubscription.SubscriptionId == SelectedSubscription.Id);

            if (SelectedShiftType != null && SelectedShiftType.Value != 0)
            {
                if (SelectedShiftType.Value == 1)
                    criteria.Shift = ShiftTypes.Morning;
                else if (SelectedShiftType.Value == 2)
                    criteria.Shift = ShiftTypes.Afternoon;
            }
            IEnumerable<MemberAttendanceDTO> attendanceList = new MemberAttendanceService(true).GetAll(criteria).OrderBy(i => i.Id).ToList();
            
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
            var pervisitAttendanceList = new PervisitSubscriptionService(true).GetAll(cri).OrderBy(i => i.Id).ToList();
            

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
        
        public ICommand CloseItemViewCommand
        {
            get { return _closeItemViewCommand ?? (_closeItemViewCommand = new RelayCommand<Object>(CloseWindow)); }
        }

        private void CloseWindow(object obj)
        {
            if (obj != null)
            {
                var window = obj as Window;
                if (window != null)
                {
                    window.DialogResult = true;
                    window.Close();
                }
            }
            //new NotificationWindow().Show();
            //string str = "title", title2 = "title", text2 = "text";

            //var nic = new NotifyIcon
            //{
            //    Text = str,
            //    BalloonTipText = str,
            //    BalloonTipTitle = title2,
            //    Visible = true
            //};
            //nic.Icon = new System.Drawing.Icon("/PinnaFit.WPF;component/Resources/AppIcon.ico");
            //nic.ShowBalloonTip(1000, title2, text2, ToolTipIcon.Info);

            //var tbi = new TaskbarIcon
            //{
            //    Icon = new System.Drawing.Icon("../../Resources/AppIcon.ico"),
            //    ToolTipText = "hello world"
            //};
            //new TaskbarIcon().ShowBalloonTip("hello", "hello world", new System.Drawing.Icon("../../Resources/AppIcon.ico"));
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
        private DateTime _filterStartDate;
        private DateTime _filterEndDate;
        private List<ListDataItem> _shiftTypeList;
        private ListDataItem _selectedShiftType;


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