using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace PinnaFit.Core.Models
{
    public class CommonFieldsAttendance : CommonFieldsA
    {
        public DateTime CheckedInTime
        {
            get { return GetValue(() => CheckedInTime); }
            set { SetValue(() => CheckedInTime, value); }
        }

        public string LockerNumber//or Key Number
        {
            get { return GetValue(() => LockerNumber); }
            set { SetValue(() => LockerNumber, value); }
        }

        public DateTime? CheckedOutTime
        {
            get { return GetValue(() => CheckedOutTime); }
            set { SetValue(() => CheckedOutTime, value); }
        }

        public string Comments
        {
            get { return GetValue(() => Comments); }
            set { SetValue(() => Comments, value); }
        }
        [NotMapped]
        public string CheckedInTimeStringAndAmharic
        {
            get
            {
                return CheckedInTime.ToString("dd-MM-yyyy hh:mm:ss") + "(" + ReportUtility.GetEthCalendarFormated(CheckedInTime, "/") + ")";
            }
            set { SetValue(() => CheckedInTimeStringAndAmharic, value); }
        }
        [NotMapped]
        public string CheckedInTimeShort
        {
            get
            {
                return CheckedInTime.ToString("hh:mm:ss");// + "(" + ReportUtility.GetEthCalendarFormated(CheckedInTime, "/") + ")";
            }
            set { SetValue(() => CheckedInTimeShort, value); }
        }
           [NotMapped]
        public string CheckedInDateTimeShort
        {
            get
            {
                return ReportUtility.GetEthCalendarFormated(CheckedInTime, "/");
            }
            set { SetValue(() => CheckedInDateTimeShort, value); }
        }
        
        //Trained/Followed By
        //List Additional/Extra Services Get (like shampoo,sauna,steam and the like)
    }
}