using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Extensions;

namespace PinnaFit.Core.Models
{
    public class MemberSubscriptionDTO : CommonFieldsG
    {
        public MemberSubscriptionDTO()
        {
            Attendances = new HashSet<MemberAttendanceDTO>();
        }

        [ForeignKey("Member")]
        public int MemberId { get; set; }
        public MemberDTO Member
        {
            get { return GetValue(() => Member); }
            set { SetValue(() => Member, value); }
        }

        [ForeignKey("FacilitySubscription")]
        public int FacilitySubscriptionId { get; set; }
        public FacilitySubscriptionDTO FacilitySubscription
        {
            get { return GetValue(() => FacilitySubscription); }
            set { SetValue(() => FacilitySubscription, value); }
        }

        //For manually blocking of a subscription

        [NotMapped]
        public string SubscriptionNumber
        {
            get
            {
                return "DFS00" + Id.ToString() + "";
            }
            set { SetValue(() => SubscriptionNumber, value); }
        }
        [NotMapped]
        public string NoOfAttendances
        {
            get
            {
                return Attendances.Count.ToString() ;//+ " Day(s)"
            }
            set { SetValue(() => NoOfAttendances, value); }
        }
        public ICollection<MemberAttendanceDTO> Attendances
        {
            get { return GetValue(() => Attendances); }
            set
            {
                SetValue(() => Attendances, value);
            }
        }

        /**Attributes For Internal Use not for display***
        public bool IsArchived
        {
            get { return GetValue(() => IsArchived); }
            set { SetValue(() => IsArchived, value); }
        }       
        
        *****/
        [NotMapped]
        public bool IsNew//OR is Renewed
        {
            get
            {
                return PreviousSuscrptionId==null;
            }
            set { SetValue(() => IsNew, value); }
        }
        
        public int? PreviousSuscrptionId// To Handle New OR is Renewed
        {
            get { return GetValue(() => PreviousSuscrptionId); }
            set { SetValue(() => PreviousSuscrptionId, value); }
        }

        [NotMapped]
        public bool SubscriptionExpired
        {
            get
            {
                return DaysLeft<0;
            }
            set { SetValue(() => SubscriptionExpired, value); }
        }
        [NotMapped]
        public int DaysLeft
        {
            get
            {
                var today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                return EndDate != null ? EndDate.Value.Subtract(today).Days : 0;//+ " Day(s)"
            }
            set { SetValue(() => DaysLeft, value); }
        }
        [NotMapped]
        public string CurrentStatus
        {
            get
            {
                if (EndDate == null)
                    return EnumUtil.GetEnumDesc(MemberStatusTypes.Active);
                return DateTime.Now >= EndDate ? EnumUtil.GetEnumDesc(MemberStatusTypes.Expired) : EnumUtil.GetEnumDesc(MemberStatusTypes.Active);
            }
            set { SetValue(() => CurrentStatus, value); }
        }
        
       
        //[NotMapped]
        //public string StartDateString
        //{
        //    get
        //    {
        //        if (StartDate != null)
        //            return StartDate.Value.ToString("dd-MM-yyyy");// + "(" + ReportUtility.GetEthCalendarFormated(StartDate, "-") + ")";
        //        else return "";
        //    }
        //    set { SetValue(() => StartDateString, value); }
        //}
        //[NotMapped]
        //public string EndDateString
        //{
        //    get
        //    {
        //        if (EndDate != null)
        //            return EndDate.Value.ToString("dd-MM-yyyy");
        //        // + "(" + ReportUtility.GetEthCalendarFormated(EndDate, "-") + ")";
        //        else return "";
        //    }
        //    set { SetValue(() => EndDateString, value); }
        //}
    }
}