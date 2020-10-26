using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Extensions;

namespace PinnaFit.Core.Models
{
    public class PervisitSubscriptionDTO : CommonFieldsAttendance
    {
        [MaxLength(255, ErrorMessage = "Full Name Exceeded 255 letters")]
        [DisplayName("Full Name")]
        [Required]
        public string DisplayName
        {
            get { return GetValue(() => DisplayName); }
            set { SetValue(() => DisplayName, value); }
        }

        [Required]
        public Sex Sex
        {
            get { return GetValue(() => Sex); }
            set { SetValue(() => Sex, value); }
        }

        [ForeignKey("Address")]
        public int? AddressId { get; set; }
        public AddressDTO Address
        {
            get { return GetValue(() => Address); }
            set { SetValue(() => Address, value); }
        }

        [ForeignKey("FacilitySubscription")]
        public int FacilitySubscriptionId { get; set; }
        public FacilitySubscriptionDTO FacilitySubscription
        {
            get { return GetValue(() => FacilitySubscription); }
            set { SetValue(() => FacilitySubscription, value); }
        }

        public decimal AmountPaid
        {
            get { return GetValue(() => AmountPaid); }
            set { SetValue(() => AmountPaid, value); }
        }

        [Required]
        public string ReceiptNumber
        {
            get { return GetValue(() => ReceiptNumber); }
            set { SetValue(() => ReceiptNumber, value); }
        }

        public DateTime ReceiptDate
        {
            get { return GetValue(() => ReceiptDate); }
            set { SetValue(() => ReceiptDate, value); }
        }

        [NotMapped]
        public string SexAmharic
        {
            get
            {
                return EnumUtil.GetEnumDesc(Sex);
            }
            set { SetValue(() => SexAmharic, value); }
        }
        
        [NotMapped]
        public string VisitNumber
        {
            get
            {
                var pref = Id.ToString();
                if (Id < 1000)
                {
                    var id = Id + 10000;
                    pref = id.ToString();
                    pref = pref.Substring(1);
                }

                return "DFPV/" + pref + "/" + ReportUtility.GetEthCalendarFormated(CheckedInTime, "/").Substring(8);
            }
            set { SetValue(() => VisitNumber, value); }
        }
    }
}