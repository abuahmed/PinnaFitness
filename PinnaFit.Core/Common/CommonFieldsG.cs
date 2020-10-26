using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinnaFit.Core
{
    public class CommonFieldsG : CommonFieldsA
    {
        public DateTime SubscribedDate
        {
            get { return GetValue(() => SubscribedDate); }
            set
            {
                SetValue(() => SubscribedDate, value);
                SetValue<string>(() => SubscribedDateStringAmharic, value.ToLongDateString());

            }
        }
        public DateTime? StartDate
        {
            get { return GetValue(() => StartDate); }
            set
            {
                SetValue(() => StartDate, value);
                if (value != null) SetValue<string>(() => StartDateStringAmharic, value.Value.ToLongDateString());

            }
        }
        public DateTime? EndDate
        {
            get { return GetValue(() => EndDate); }
            set
            {
                SetValue(() => EndDate, value);
                if (value != null) SetValue<string>(() => EndDateStringAmharic, value.Value.ToLongDateString());
                //if (value != null) SetValue<string>(() => CurrentStatus, value.Value.ToLongDateString());
            }
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

        public bool IsBlocked
        {
            get { return GetValue(() => IsBlocked); }
            set { SetValue(() => IsBlocked, value); }
        }

        [StringLength(255)]
        public string Comments
        {
            get { return GetValue(() => Comments); }
            set { SetValue(() => Comments, value); }
        }

        [NotMapped]
        public string SubscribedDateStringAndAmharic
        {
            get
            {
                return SubscribedDate.ToString("dd-MM-yyyy") + "(" + ReportUtility.GetEthCalendarFormated(SubscribedDate, "-") + ")";
            }
            set { SetValue(() => SubscribedDateStringAndAmharic, value); }
        }
        [NotMapped]
        public string SubscribedDateString
        {
            get
            {
                return SubscribedDate.ToString("dd-MM-yyyy");
            }
            set { SetValue(() => SubscribedDateString, value); }
        }
        [NotMapped]
        public string SubscribedDateStringAmharic
        {
            get
            {
                return ReportUtility.GetEthCalendar(SubscribedDate, true);
            }
            set { SetValue(() => SubscribedDateStringAmharic, value); }
        }
        [NotMapped]
        public string SubscribedDateStringAmharicFormatted
        {
            get
            {
              return ReportUtility.GetEthCalendarFormated(SubscribedDate, "/");
            }
            set { SetValue(() => SubscribedDateStringAmharicFormatted, value); }
        }

        [NotMapped]
        public string StartDateStringAndAmharic
        {
            get
            {
                if (StartDate != null)
                    return StartDate.Value.ToString("dd-MM-yyyy") + " (" + ReportUtility.GetEthCalendarFormated(StartDate.Value, "-") + ")";
                return "";
            }
            set { SetValue(() => StartDateStringAndAmharic, value); }
        }
        [NotMapped]
        public string StartDateString
        {
            get
            {
                if (StartDate != null)
                    return StartDate.Value.ToString("dd-MM-yyyy");
                return "";
            }
            set { SetValue(() => StartDateString, value); }
        }
        [NotMapped]
        public string StartDateStringAmharic
        {
            get
            {
                if (StartDate != null)
                    return ReportUtility.GetEthCalendar(StartDate.Value, true);//"-"
                return "";
            }
            set { SetValue(() => StartDateStringAmharic, value); }
        }
        [NotMapped]
        public string StartDateStringAmharicFormatted
        {
            get
            {
                if (StartDate != null)
                    return ReportUtility.GetEthCalendarFormated(StartDate.Value, "-");//"-"
                return "";
            }
            set { SetValue(() => StartDateStringAmharicFormatted, value); }
        }

        [NotMapped]
        public string EndDateStringAndAmharic
        {
            get
            {
                if (EndDate != null)
                    return EndDate.Value.ToString("dd-MM-yyyy") + " (" + ReportUtility.GetEthCalendarFormated(EndDate.Value, "-") + ")";
                return "";
            }
            set { SetValue(() => EndDateStringAndAmharic, value); }
        }
        [NotMapped]
        public string EndDateString
        {
            get
            {
                if (EndDate != null)
                    return EndDate.Value.ToString("dd-MM-yyyy");
                return "";
            }
            set { SetValue(() => EndDateString, value); }
        }
        [NotMapped]
        public string EndDateStringAmharic
        {
            get
            {
                if (EndDate != null)
                    return ReportUtility.GetEthCalendar(EndDate.Value, true);
                return "";
            }
            set { SetValue(() => EndDateStringAmharic, value); }
        }
        [NotMapped]
        public string EndDateStringAmharicFormatted
        {
            get
            {
                if (EndDate != null)
                    return ReportUtility.GetEthCalendarFormated(EndDate.Value, "-");
                return "";
            }
            set { SetValue(() => EndDateStringAmharicFormatted, value); }
        }
    }
}