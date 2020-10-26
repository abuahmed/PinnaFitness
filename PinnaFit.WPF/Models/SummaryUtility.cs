#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PinnaFit.Core;
using PinnaFit.Core.Common;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Extensions;
using PinnaFit.Core.Models;
using PinnaFit.Service;
using PinnaFit.Service.Interfaces;
using PinnaFit.WPF.Reports.DataSets;
using PinnaFit.WPF.Views;

#endregion

namespace PinnaFit.WPF.Models
{
    public class SummaryUtility
    {
        private static IMemberService _memberService;
        private static IPervisitSubscriptionService _pervisitSubscriptionService;
        private static ITransactionService _transactionService;

        public IEnumerable<SubscriptionModel> MemberSubscriptionList { get; set; }
        public IEnumerable<MemberDTO> MemberList { get; set; }
        private readonly int? _shiftType;
        private DateTime? _beginDate;
        private DateTime? _endDate;
        private readonly int? _facility;
        private readonly int? _subscription;
        public SummaryUtility()
        {
            Load();
        }
        public SummaryUtility(DateTime beginDate, DateTime endDate, int shiftType)
        {
            _shiftType = shiftType;
            _beginDate = new DateTime(beginDate.Year, beginDate.Month, beginDate.Day, 0, 0, 0);
            _endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);

            Load();
        }
        public SummaryUtility(DateTime beginDate, DateTime endDate, int shiftType, int facility, int subscription)
        {
            _shiftType = shiftType;
            _beginDate = new DateTime(beginDate.Year, beginDate.Month, beginDate.Day, 0, 0, 0);
            _endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);

            _facility = facility;
            _subscription = subscription;
            Load();
        }
        
        public void Load()
        {
            CleanUp();
            _memberService = new MemberService();
            _pervisitSubscriptionService = new PervisitSubscriptionService();
            _transactionService = new TransactionService();
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

        public IEnumerable<SubscriptionModel> GetSummary()
        {
            MemberSubscriptionList = GetSubscriptions().ToList();
            MemberSubscriptionList = MemberSubscriptionList.Union(GetPervisit().ToList()).ToList();
            MemberSubscriptionList = MemberSubscriptionList.Union(GetSales().ToList()).ToList();
           
            return MemberSubscriptionList;
        }

        public IEnumerable<SubscriptionModel> GetMembers()
        {
            var criteria = new SearchCriteria<MemberDTO>();
            criteria.FiList.Add(f => f.LastSubscriptionId!=null);

            if (_beginDate != null && _endDate != null && _beginDate.Value.Year > 2000 && _endDate.Value.Year > 2000)
            {
                var beginDate = new DateTime(_beginDate.Value.Year, _beginDate.Value.Month, _beginDate.Value.Day, 0, 0, 0);
                if (_shiftType == 2)
                    beginDate = new DateTime(_beginDate.Value.Year, _beginDate.Value.Month, _beginDate.Value.Day, 14, 0, 0);
                criteria.FiList.Add(
                    p => p.LastSubscription.SubscribedDate != null && p.LastSubscription.SubscribedDate >= beginDate);

                var endDate = new DateTime(_endDate.Value.Year, _endDate.Value.Month, _endDate.Value.Day, 23, 59, 59);
                if (_shiftType == 1)
                    endDate = new DateTime(_endDate.Value.Year, _endDate.Value.Month, _endDate.Value.Day, 13, 59, 59);
                criteria.FiList.Add(
                    p => p.LastSubscription.SubscribedDate != null && p.LastSubscription.SubscribedDate <= endDate);
            }

            if (_facility != null && _facility != -1)
                criteria.FiList.Add(f => f.LastSubscription.FacilitySubscription.FacilityId == _facility);
            if (_subscription != null && _subscription != -1)
                criteria.FiList.Add(f => f.LastSubscription.FacilitySubscription.SubscriptionId == _subscription);

            var memberList = new MemberService(true).GetAll(criteria).ToList();
            
            var memberSubscriptionList = memberList.Select(member => new SubscriptionModel
            {
                Id = member.Id,
                MemberName = member.DisplayName,
                Sex = member.Sex,
                Age = member.Age ?? 0,
                MobileNumber = member.Address.Mobile,
                Amount = member.LastSubscription.AmountPaid,
                TransactionDate = member.LastSubscription.SubscribedDate,
                StartDate =
                    new DateTime(member.LastSubscription.SubscribedDate.Year,
                        member.LastSubscription.SubscribedDate.Month, member.LastSubscription.SubscribedDate.Day, 0, 0,
                        0),
                EndDate =
                    new DateTime(member.LastSubscription.EndDate.Value.Year, member.LastSubscription.EndDate.Value.Month,
                        member.LastSubscription.EndDate.Value.Day, 23, 59, 59),
                PackageName = member.LastSubscription.FacilitySubscription.PackageName,
                FacilityName = member.LastSubscription.FacilitySubscription.Facility.DisplayName,
                SubscriptionName = member.LastSubscription.FacilitySubscription.Subscription.DisplayName,
                NoOfAttendances = 1, //Convert.ToInt32(member.NoOfAttendances)
                DaysLeft = Convert.ToInt32(member.LastSubscription.DaysLeft),
                IsExpired = member.LastSubscription.SubscriptionExpired
            }).ToList();

            return memberSubscriptionList;
           
        }

        public IEnumerable<SubscriptionModel> GetSubscriptions()
        {
            var criteria = new SearchCriteria<MemberSubscriptionDTO>();
            criteria.FiList.Add(f => f.Member.Enabled);
            if (_beginDate != null && _endDate != null && _beginDate.Value.Year > 2000 && _endDate.Value.Year > 2000)
            {
                criteria.BeginingDate = _beginDate;
                criteria.EndingDate = _endDate;
                if (_shiftType != null)
                    criteria.Shift = (ShiftTypes)_shiftType;
            }
            if (_facility != null && _facility != -1)
                criteria.FiList.Add(f => f.FacilitySubscription.FacilityId == _facility);
            if (_subscription != null && _subscription != -1)
                criteria.FiList.Add(f => f.FacilitySubscription.SubscriptionId == _subscription);

            var memberList = new MemberSubscriptionService(true).GetAll(criteria).ToList();

            var memberSubscriptionList = memberList.Select(member => new SubscriptionModel
            {
                Id = member.Id,
                MemberName = member.Member.DisplayName,
                Sex = member.Member.Sex,
                Age = member.Member.Age ?? 0,
                MobileNumber = member.Member.Address.Mobile,
                Amount = member.AmountPaid,
                TransactionDate = member.SubscribedDate,
                StartDate =
                    new DateTime(member.SubscribedDate.Year,
                        member.SubscribedDate.Month, member.SubscribedDate.Day, 0, 0,
                        0),
                EndDate =
                    new DateTime(member.EndDate.Value.Year, member.EndDate.Value.Month,
                        member.EndDate.Value.Day, 23, 59, 59),
                PackageName = member.FacilitySubscription.PackageName,
                FacilityName = member.FacilitySubscription.Facility.DisplayName,
                SubscriptionName = member.FacilitySubscription.Subscription.DisplayName,
                NoOfAttendances = 1, //Convert.ToInt32(member.NoOfAttendances)
                DaysLeft = Convert.ToInt32(member.DaysLeft),
                IsExpired = member.SubscriptionExpired
            }).ToList();

            return memberSubscriptionList;

        }

        public IEnumerable<SubscriptionModel> GetPervisit()
        {
            var criteria = new SearchCriteria<PervisitSubscriptionDTO>();

            if (_beginDate != null && _endDate != null && _beginDate.Value.Year > 2000 && _endDate.Value.Year > 2000)
            {
                criteria.BeginingDate = _beginDate;
                criteria.EndingDate = _endDate;
                if (_shiftType != null)
                    criteria.Shift = (ShiftTypes)_shiftType;
                //var beginDate = new DateTime(_beginDate.Value.Year, _beginDate.Value.Month, _beginDate.Value.Day, 0, 0, 0);
                //if (_shiftType == 2)
                //    beginDate = new DateTime(_beginDate.Value.Year, _beginDate.Value.Month, _beginDate.Value.Day, 14, 0, 0);
                //criteria.FiList.Add(
                //    p => p.CheckedInTime != null && p.CheckedInTime >= beginDate);

                //var endDate = new DateTime(_endDate.Value.Year, _endDate.Value.Month, _endDate.Value.Day, 23, 59, 59);
                //if (_shiftType == 1)
                //    endDate = new DateTime(_endDate.Value.Year, _endDate.Value.Month, _endDate.Value.Day, 13, 59, 59);
                //criteria.FiList.Add(
                //    p => p.CheckedInTime != null && p.CheckedInTime <= endDate);
               
            }

            var pervistMembers = _pervisitSubscriptionService.GetAll(criteria).ToList();
            IEnumerable<SubscriptionModel> pervisitSubscriptions =
                pervistMembers.Select(pervisit => new SubscriptionModel
                {
                    Id = pervisit.Id,
                    MemberName = pervisit.DisplayName,
                    Sex = pervisit.Sex,
                    Amount = pervisit.AmountPaid,
                    TransactionDate = pervisit.CheckedInTime,
                    StartDate =
                        new DateTime(pervisit.CheckedInTime.Year, pervisit.CheckedInTime.Month,
                            pervisit.CheckedInTime.Day, 0, 0, 0),
                    EndDate =
                        new DateTime(pervisit.CheckedInTime.Year, pervisit.CheckedInTime.Month,
                            pervisit.CheckedInTime.Day, 23, 59, 59),
                    PackageName = pervisit.FacilitySubscription.PackageName,
                    FacilityName = pervisit.FacilitySubscription.Facility.DisplayName,
                    SubscriptionName = pervisit.FacilitySubscription.Subscription.DisplayName,
                    NoOfAttendances = 1,
                    DaysLeft = 0,
                    IsExpired = true
                }).ToList();
            return pervisitSubscriptions;
        }

        public IEnumerable<SubscriptionModel> GetSales()
        {
            var salesLi = GetLiveSales().ToList();
            IEnumerable<SubscriptionModel> sales = salesLi.Select(salesList => new SubscriptionModel
            {
                Id = salesList.Id,
                MemberName = salesList.Transaction.BusinessPartner,
                Amount = salesList.LinePrice,
                TransactionDate = salesList.Transaction.TransactionDate,
                StartDate =
                    new DateTime(salesList.Transaction.TransactionDate.Year, salesList.Transaction.TransactionDate.Month,
                        salesList.Transaction.TransactionDate.Day, 0, 0, 0),
                EndDate =
                    new DateTime(salesList.Transaction.TransactionDate.Year, salesList.Transaction.TransactionDate.Month,
                        salesList.Transaction.TransactionDate.Day, 23, 59, 59),
                PackageName = salesList.Item.DisplayName + salesList.ForFree,
                FacilityName = salesList.Item.DisplayName,
                SubscriptionName = salesList.Item.Number,
                NoOfAttendances = (int) salesList.Unit,
                //DaysLeft = 0,
                //IsExpired = true
            }).ToList();
            return sales;
        }

        public IEnumerable<TransactionLineDTO> GetLiveSales()
        {
            var criteria = new SearchCriteria<TransactionHeaderDTO>
            {
                TransactionType = (int) TransactionTypes.SellStock,
                CurrentUserId = Singleton.User.UserId
            };

            if (_beginDate != null && _endDate != null && _beginDate.Value.Year > 2000 && _endDate.Value.Year > 2000)
            {
           
                criteria.BeginingDate = _beginDate;
                criteria.EndingDate = _endDate;
                if (_shiftType != null)
                    criteria.Shift = (ShiftTypes)_shiftType;
                //var beginDate = new DateTime(_beginDate.Value.Year, _beginDate.Value.Month, _beginDate.Value.Day, 0, 0, 0);
                //if (_shiftType == 2)
                //    beginDate = new DateTime(_beginDate.Value.Year, _beginDate.Value.Month, _beginDate.Value.Day, 14, 0, 0);
                //criteria.FiList.Add(
                //    p => p.TransactionDate != null && p.TransactionDate >= beginDate);

                //var endDate = new DateTime(_endDate.Value.Year, _endDate.Value.Month, _endDate.Value.Day, 23, 59, 59);
                //if (_shiftType == 1)
                //    endDate = new DateTime(_endDate.Value.Year, _endDate.Value.Month, _endDate.Value.Day, 13, 59, 59);
                //criteria.FiList.Add(
                //    p => p.TransactionDate != null && p.TransactionDate <= endDate);
                
            }

            var salesHeader = _transactionService.GetAll(criteria).OrderBy(i => i.Id).ToList();
            return salesHeader.SelectMany(s => s.TransactionLines).ToList();
        }

        public void PrintDailySummaryList(IEnumerable<SubscriptionModel> list)
        {
            var myReport4 = new Reports.Summary.DailySummay();
            var dSet = GetSummaryDataSet(list);
            if (dSet == null)
                return;

            myReport4.SetDataSource(dSet);

            var report = new ReportViewerCommon(myReport4);
            report.Show();
           
        }
        public FitnessDataSet GetSummaryDataSet(IEnumerable<SubscriptionModel> list)
        {
            try
            {
                var myDataSet = new FitnessDataSet();
                var serNo = 1;
                var selectedCompany = new CompanyService(true).GetCompany();

                var memberSubsList = list.ToList();

                if (memberSubsList.Count == 0)
                {
                    MessageBox.Show("No Data found on: ");
                    return null;
                }

                #region Group By Facility
                var querFacility = from memberSubscriptionList in memberSubsList
                                   group memberSubscriptionList by memberSubscriptionList.PackageName
                                       into newGroup
                                       orderby newGroup.Key
                                       select newGroup;

                var memberCategorySubList = querFacility.Select(nameGroup => new SubscriptionSumModel
                {
                    Category = nameGroup.Key,
                    SubscriptionModels = nameGroup
                }).ToList();

                var shift = ShiftTypes.All;
                if (_shiftType != null)
                {
                    shift = (ShiftTypes) _shiftType;
                }
                if (_beginDate == null || _endDate == null) return null;
                string datecaption = ReportUtility.GetEthCalendarFormated(_beginDate.Value, "-") + "(" +
                                     _beginDate.Value.ToShortDateString() + ")";

                if (_beginDate.Value.Day != _endDate.Value.Day || _beginDate.Value.Month != _endDate.Value.Month || _beginDate.Value.Year != _endDate.Value.Year)
                {
                    datecaption = "ከ " + datecaption + " እስከ " + ReportUtility.GetEthCalendarFormated(_endDate.Value, "-") + "(" +
                                     _endDate.Value.ToShortDateString() + ")";
                }

                foreach (var subscriptionSumModel in memberCategorySubList)
                {
                    var subtototal = subscriptionSumModel.TotalAmount / (decimal)1.15;
                    var tax = subtototal * (decimal)0.15;

                    myDataSet.DailySummaryReport.Rows.Add(
                      serNo, datecaption +
                      " - (" + EnumUtil.GetEnumDesc(shift) + ")",
                      subscriptionSumModel.Category,
                      subscriptionSumModel.TotalUnit,
                      subtototal,
                      tax,
                      subscriptionSumModel.TotalAmount,
                      "",
                      "",
                      0, 0, selectedCompany.Header, "");

                    serNo++;
                }



                #endregion


                return myDataSet;
            }
            catch
            {
                return null;
            }
        }

        public void PrintDailySummaryList2(IEnumerable<SubscriptionModel> list)
        {
            var myReport4 = new Reports.MembersListLandscape();
            var dSet = GetSummaryDataSet2(list);
            if (dSet == null)
                return;

            myReport4.SetDataSource(dSet);

            var report = new ReportViewerCommon(myReport4);
            report.Show();

        }
        public FitnessDataSet GetSummaryDataSet2(IEnumerable<SubscriptionModel> list)
        {
            try
            {
                var myDataSet = new FitnessDataSet();
                var serNo = 1;
                var selectedCompany = new CompanyService(true).GetCompany();

                var memberSubsList = list.OrderBy(g=>g.FacilityName).ThenBy(g=>g.SubscriptionName).ToList();

                if (memberSubsList.Count == 0)
                {
                    MessageBox.Show("No Data found on: ");
                    return null;
                }

                #region Group By Facility
        
                var shift = ShiftTypes.All;
                if (_shiftType != null)
                {
                    shift = (ShiftTypes)_shiftType;
                }
                if (_beginDate == null || _endDate == null) return null;

                if (_beginDate.Value.Year < 2000)
                {
                    _beginDate = DateTime.Now;
                    _endDate = DateTime.Now;
                }

                string datecaption = ReportUtility.GetEthCalendarFormated(_beginDate.Value, "-") + "(" +
                                     _beginDate.Value.ToShortDateString() + ")";

                if (_beginDate.Value.Day != _endDate.Value.Day || _beginDate.Value.Month != _endDate.Value.Month || _beginDate.Value.Year != _endDate.Value.Year)
                {
                    datecaption = "ከ " + datecaption + " እስከ " + ReportUtility.GetEthCalendarFormated(_endDate.Value, "-") + "(" +
                                     _endDate.Value.ToShortDateString() + ")";
                }
                
                foreach (var member in memberSubsList)
                {
                    var durAmharic = "1 ወር";
                    if (member.SubscriptionName == "3 Months")
                        durAmharic = "3 ወር";
                    if (member.SubscriptionName == "6 Months")
                        durAmharic = "6 ወር";
                    if (member.SubscriptionName == "1 Year")
                        durAmharic = "1 አመት";

                    myDataSet.MembersList.Rows.Add(
                        serNo,
                        member.MemberName,
                        "አዲስ የተመዘገቡና ያደሱ",
                        EnumUtil.GetEnumDesc(member.Sex),
                        member.Age,
                        member.MobileNumber,
                        "",
                        member.FacilityName,
                        durAmharic,
                        member.Amount,
                        ReportUtility.GetEthCalendarFormated(member.StartDate,"-"),
                        ReportUtility.GetEthCalendarFormated(member.EndDate, "-"),
                        "",
                        datecaption,
                        EnumUtil.GetEnumDesc(shift),
                        0, 0, selectedCompany.Header, "");

                    
                    serNo++;
                }


                #endregion


                return myDataSet;
            }
            catch
            {
                return null;
            }
        }

        public void PrintList(IEnumerable<SubscriptionModel> list, MemberStatusTypes status)
        {
            var myReport4 = new Reports.Summary.NumberSummay();
            myReport4.SetDataSource(GetNumberListDataSet(list.ToList(),status));

            //MenuItem menu = obj as MenuItem;
            //if (menu != null)
            //    new ReportUtility().DirectPrinter(myReport4);
            //else
            //{
            var report = new ReportViewerCommon(myReport4);
            report.Show();
            //}
        }
        public FitnessDataSet GetNumberListDataSet(List<SubscriptionModel> list, MemberStatusTypes status)
        {
            var myDataSet = new FitnessDataSet();
            var serNo = 1;
            var selectedCompany = new CompanyService(true).GetCompany();

            var memberSubscriptionListForNumberSummary = list.ToList();
            switch (status)
            {
                    case MemberStatusTypes.Active:
                    memberSubscriptionListForNumberSummary = list.ToList().Where(m => !m.IsExpired).ToList();
                    break;
                    case MemberStatusTypes.Expired:
                    memberSubscriptionListForNumberSummary = list.ToList().Where(m => m.IsExpired).ToList();
                    break;
            }
            
            #region Group By Facility
            var querFacility = from memberSubscriptionList in memberSubscriptionListForNumberSummary
                               group memberSubscriptionList by memberSubscriptionList.FacilityName
                                   into newGroup
                                   orderby newGroup.Key
                                   select newGroup;

            var memberCategorySubList = querFacility.Select(nameGroup => new SubscriptionSumModel
            {
                Category = nameGroup.Key,
                SubscriptionModels = nameGroup
            }).ToList();


            foreach (var subscriptionSumModel in memberCategorySubList)
            {
                string cat = subscriptionSumModel.Category;
                var subModels = subscriptionSumModel.SubscriptionModels;

                var querSubscription = from memberSubscriptionList in subModels
                                       group memberSubscriptionList by memberSubscriptionList.SubscriptionName
                                           into newGroup
                                           orderby newGroup.Key
                                           select newGroup;

                var memberCategorySubList2 = querSubscription.Select(nameGroup => new SubscriptionSumModel
                {
                    Category = nameGroup.Key,
                    SubscriptionModels = nameGroup
                }).ToList();

                int permonth = 0, threemonth = 0, sixmonth = 0, oneyear = 0;

                var firstOrDefault = memberCategorySubList2.FirstOrDefault(a => a.Category == "Per Month");
                if (firstOrDefault != null)
                {
                    permonth = firstOrDefault.TotalNumber;
                }

                firstOrDefault = memberCategorySubList2.FirstOrDefault(a => a.Category == "3 Months");
                if (firstOrDefault != null)
                {
                    threemonth = firstOrDefault.TotalNumber;
                }

                firstOrDefault = memberCategorySubList2.FirstOrDefault(a => a.Category == "6 Months");
                if (firstOrDefault != null)
                {
                    sixmonth = firstOrDefault.TotalNumber;
                }

                firstOrDefault = memberCategorySubList2.FirstOrDefault(a => a.Category == "1 Year");
                if (firstOrDefault != null)
                {
                    oneyear = firstOrDefault.TotalNumber;
                }

                var tot = oneyear + permonth + threemonth + sixmonth;

                myDataSet.SummaryReport.Rows.Add(
                  serNo,
                  ReportUtility.GetEthCalendarFormated(DateTime.Now, "-") + "(" +
                    DateTime.Now.ToShortDateString() + ")",
                  cat,
                  0,
                  permonth,
                  threemonth,
                  sixmonth,
                  oneyear,
                  tot,
                  "",
                  "",
                  0, 0, selectedCompany.Header, "");

                serNo++;
            }



            #endregion


            return myDataSet;
        }

        public FitnessDataSet GetAttendanceListSummarizedDataSet(List<MemberAttendanceDTO> attendances, List<PervisitSubscriptionDTO> pervisitAttendances)
        {
            var myDataSet = new FitnessDataSet();
            var serNo = 1;
            var selectedCompany = new CompanyService(true).GetCompany();
            //var totalNumberOfRows = attendances.Count();
            attendances = attendances.OrderBy(a => a.MemberSubscription.FacilitySubscription.PackageName).ToList();

            var memberSubscriptionListDaysLeft = attendances.Select(member => new SubscriptionModel
            {
                FacilityName = member.MemberSubscription.FacilitySubscription.PackageName,
                DaysLeft = member.MemberSubscription.DaysLeft
            }).ToList();

            var memberSubscriptionList = attendances.Select(member => new SubscriptionModel
            {
                FacilityName = member.MemberSubscription.FacilitySubscription.PackageName,
                Sex = member.MemberSubscription.Member.Sex
            }).ToList();

            pervisitAttendances = pervisitAttendances.OrderBy(a => a.DisplayName).ToList();
            var pervists = pervisitAttendances.Select(member => new SubscriptionModel
            {
                FacilityName = member.FacilitySubscription.PackageName,
                Sex = member.Sex
            }).ToList();

            memberSubscriptionList = memberSubscriptionList.Union(pervists.ToList()).ToList();

            #region Calculate Sex

            var maleSex = memberSubscriptionList.Count(s => s.Sex == Sex.Male);
            var feMaleSex = memberSubscriptionList.Count(s => s.Sex == Sex.Female);

            #endregion

            #region Calculate DaysLeft

            var withDays = memberSubscriptionListDaysLeft.Count(s => s.DaysLeft >= 0);
            var withNoDays = memberSubscriptionListDaysLeft.Count(s => s.DaysLeft < 0);

            #endregion

            var querFacility = from memberSubscriptionList2 in memberSubscriptionList
                               group memberSubscriptionList2 by memberSubscriptionList2.FacilityName
                                   into newGroup
                                   orderby newGroup.Key
                                   select newGroup;

            var memberCategorySubList = querFacility.Select(nameGroup => new SubscriptionSumModel
            {
                Category = nameGroup.Key,
                SubscriptionModels = nameGroup
            }).ToList();

            var shift = ShiftTypes.All;
            if (_shiftType != null)
            {
                shift = (ShiftTypes)_shiftType;
            }
            if (_beginDate == null || _endDate == null) return null;

            string datecaption = ReportUtility.GetEthCalendarFormated(_beginDate.Value, "-") + "(" +
                                 _beginDate.Value.ToShortDateString() + ")";

            if (_beginDate.Value.Day != _endDate.Value.Day || _beginDate.Value.Month != _endDate.Value.Month ||
                _beginDate.Value.Year != _endDate.Value.Year)
            {
                datecaption = "ከ " + datecaption + " እስከ " + ReportUtility.GetEthCalendarFormated(_endDate.Value, "-") +
                              "(" + _endDate.Value.ToShortDateString() + ")";
            }

            foreach (var member in memberCategorySubList)
            {
                myDataSet.MembersList.Rows.Add(
                    serNo,
                    member.Category,
                    maleSex,
                    feMaleSex,
                    withDays,
                    withNoDays,
                    "",
                    "",
                    "",
                    0,
                    "",
                    "",
                    "",
                    datecaption,
                    EnumUtil.GetEnumDesc(shift),
                    member.TotalNumber, 0, selectedCompany.Header, "");

                serNo++;
            }


            return myDataSet;
        }

        public FitnessDataSet GetAttendanceListDataSet(List<MemberAttendanceDTO> attendances, List<PervisitSubscriptionDTO> pervisitAttendances)
        {
            var myDataSet = new FitnessDataSet();
            var serNo = 1;
            var selectedCompany = new CompanyService(true).GetCompany();
            var totalNumberOfRows = attendances.Count();
            attendances = attendances.OrderBy(a => a.MemberSubscription.FacilitySubscription.PackageName).ToList();

            var shift = ShiftTypes.All;
            if (_shiftType != null)
            {
                shift = (ShiftTypes)_shiftType;
            }
            if (_beginDate == null || _endDate == null) return null;
            foreach (var member in attendances)
            {
                myDataSet.MembersList.Rows.Add(
                    serNo,
                    member.MemberSubscription.Member.DisplayName,
                    member.MemberSubscription.Member.Number,
                    member.MemberSubscription.Member.Sex,
                    "አጠቃላይ: " + totalNumberOfRows,
                    member.MemberSubscription.Member.Address.AddressDetail,
                    member.MemberSubscription.Member.Address.Mobile,
                    member.MemberSubscription.FacilitySubscription.PackageName,
                    member.MemberSubscription.FacilitySubscription.Facility.DisplayName,
                    member.MemberSubscription.AmountPaid,
                    member.MemberSubscription.StartDateString,
                    member.MemberSubscription.EndDateString,
                    member.MemberSubscription.CurrentStatus,
                    ReportUtility.GetEthCalendarFormated(_beginDate.Value, "-") + "(" +
                    _beginDate.Value.ToShortDateString() + ")",
                    EnumUtil.GetEnumDesc(shift),
                    0, 0, selectedCompany.Header, "");

                serNo++;
            }

            pervisitAttendances =
                    pervisitAttendances.OrderBy(a => a.DisplayName).ToList();

            foreach (var member in pervisitAttendances)
            {
                myDataSet.MembersList.Rows.Add(
                    serNo,
                    member.DisplayName,
                    member.VisitNumber,
                    member.Sex,
                    "አጠቃላይ: " + totalNumberOfRows,
                    "",
                    "",
                    member.FacilitySubscription.PackageName,
                    "Per Visit",
                    member.AmountPaid,
                    member.CheckedInTime,
                    member.CheckedInTime,
                    "የዕለት",
                    ReportUtility.GetEthCalendarFormated(_beginDate.Value, "-") + "(" +
                    _beginDate.Value.ToShortDateString() + ")",
                    EnumUtil.GetEnumDesc(shift),
                    0, 0, selectedCompany.Header, "");

                serNo++;
            }

            return myDataSet;
        }

    }
}