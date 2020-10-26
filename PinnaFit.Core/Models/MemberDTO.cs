using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaFit.Core.CustomValidationAttributes;
using PinnaFit.Core.Enumerations;

namespace PinnaFit.Core.Models
{
    public class MemberDTO : CommonFieldsF
    {
        public MemberTypes Type
        {
            get { return GetValue(() => Type); }
            set { SetValue(() => Type, value); }
        }
        
        [ForeignKey("LastSubscription")]
        public int? LastSubscriptionId { get; set; }
        public MemberSubscriptionDTO LastSubscription
        {
            get { return GetValue(() => LastSubscription); }
            set { SetValue(() => LastSubscription, value); }
        }

        [ForeignKey("LastAssessment")]
        public int? LastAssessmentId { get; set; }
        public MemberAssessmentDTO LastAssessment
        {
            get { return GetValue(() => LastAssessment); }
            set { SetValue(() => LastAssessment, value); }
        }

        [ForeignKey("LastAttendance")]
        public int? LastAttendanceId { get; set; }
        public MemberAttendanceDTO LastAttendance
        {
            get { return GetValue(() => LastAttendance); }
            set { SetValue(() => LastAttendance, value); }
        }
        public DateTime? LastAttendanceTime
        {
            get { return GetValue(() => LastAttendanceTime); }
            set { SetValue(() => LastAttendanceTime, value); }
        }
        
        public ICollection<MemberSubscriptionDTO> Subscriptions
        {
            get { return GetValue(() => Subscriptions); }
            set
            {
                SetValue(() => Subscriptions, value);
            }
        }
        public ICollection<MemberAssessmentDTO> Assessments
        {
            get { return GetValue(() => Assessments); }
            set
            {
                SetValue(() => Assessments, value);
            }
        }

        [NotMapped]
        public string NoOfRenewals
        {
            get
            {
                return (Subscriptions.Count-1).ToString() + " Renewal(s)";
            }
            set { SetValue(() => NoOfRenewals, value); }
        }

        [NotMapped]
        public string NoOfAssessments
        {
            get 
            { if (Assessments != null) 
                return Assessments.Count.ToString()+" ጊዜ";
            return "0 ጊዜ";
            }
            set { SetValue(() => NoOfAssessments, value); }
        }
    }
}