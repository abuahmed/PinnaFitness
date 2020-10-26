using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinnaFit.Core.Models
{
    public class MemberAssessmentDTO : CommonFieldsA
    {
        [ForeignKey("Member")]
        public int MemberId { get; set; }
        public MemberDTO Member
        {
            get { return GetValue(() => Member); }
            set { SetValue(() => Member, value); }
        }

        public DateTime AssessmentTime
        {
            get { return GetValue(() => AssessmentTime); }
            set { SetValue(() => AssessmentTime, value); }
        }
        public decimal? Weight
        {
            get { return GetValue(() => Weight); }
            set
            {
                SetValue(() => Weight, value);
                SetValue(() => Bmi, value);
            }

        }
        public decimal? Height
        {
            get { return GetValue(() => Height); }
            set
            {
                SetValue(() => Height, value);
                SetValue(() => Bmi, value);
            }
        }
        public decimal? Fat
        {
            get { return GetValue(() => Fat); }
            set { SetValue(() => Fat, value); }
        }
        [NotMapped]
        public decimal? Bmi
        {
            get
            {
                if(Weight!=null && Height!=null)
                return Weight/(Height*Height);
                return null;
            }
            set { SetValue(() => Bmi, value); }
        }
        public int? Bp
        {
            get { return GetValue(() => Bp); }
            set { SetValue(() => Bp, value); }
        }
        
        public string Notes
        {
            get { return GetValue(() => Notes); }
            set { SetValue(() => Notes, value); }
        }

        [NotMapped]
        public string AssessmentTimeStringAndAmharic
        {
            get
            {
                return AssessmentTime.ToString("MMMM-dd-yyyy") + "(" + ReportUtility.GetEthCalendar(AssessmentTime,true) + ")";
            }
            set { SetValue(() => AssessmentTimeStringAndAmharic, value); }
        }
        
        [NotMapped]
        public string WeightLevel //under,normal,over,obacity
        {
            get { return GetValue(() => WeightLevel); }
            set { SetValue(() => WeightLevel, value); }
        }

        [NotMapped]
        public string IdealWeight
        {
            get
            {
                return Weight.ToString();
            }
            set { SetValue(() => IdealWeight, value); }
        }

        [NotMapped]
        public string OverWeight//Additional weight from the ideal
        {
            get
            {
                return Weight.ToString();
            }
            set { SetValue(() => OverWeight, value); }
        }
    }
}