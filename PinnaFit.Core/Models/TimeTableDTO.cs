using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinnaFit.Core.Models
{
    public class TimeTableDTO : CommonFieldsA
    {
        [ForeignKey("Service")]
        public int ServiceId { get; set; }
        public ServiceDTO Service
        {
            get { return GetValue(() => Service); }
            set { SetValue(() => Service, value); }
        }
        
        public bool OnMonday
        {
            get { return GetValue(() => OnMonday); }
            set { SetValue(() => OnMonday, value); }
        }
        public bool OnTuesday
        {
            get { return GetValue(() => OnTuesday); }
            set { SetValue(() => OnTuesday, value); }
        }
        public bool OnWednesday
        {
            get { return GetValue(() => OnWednesday); }
            set { SetValue(() => OnWednesday, value); }
        }
        public bool OnThursday
        {
            get { return GetValue(() => OnThursday); }
            set { SetValue(() => OnThursday, value); }
        }
        public bool OnFriday
        {
            get { return GetValue(() => OnFriday); }
            set { SetValue(() => OnFriday, value); }
        }
        public bool OnSaturday
        {
            get { return GetValue(() => OnSaturday); }
            set { SetValue(() => OnSaturday, value); }
        }
        public bool OnSunday
        {
            get { return GetValue(() => OnSunday); }
            set { SetValue(() => OnSunday, value); }
        }
        
        [NotMapped]
        public string DaysHeld
        {
            get
            {
                var days = "";
                if (OnMonday)
                    days = "Mon";
                if (OnTuesday)
                    days = days + "," + "Tue";
                if (OnWednesday)
                    days = days + "," + "Wed";
                if (OnThursday)
                    days = days + "," + "Thr";
                if (OnFriday)
                    days = days + "," + "Fri";
                if (OnSaturday)
                    days = days + "," + "Sat";
                if (OnSunday)
                    days = days + "," + "Sun";
                if (days.StartsWith(","))
                    days = days.Substring(1);
                if (days.Length > 26)
                    days = "Everyday";
                return days;
            }
            set { SetValue(() => DaysHeld, value); }
        }
        [NotMapped]
        public IEnumerable<DayOfWeek> DayOfWeekHeld
        {
            get
            {
                var daysList = new List<DayOfWeek>();
                if (OnMonday)
                    daysList.Add(DayOfWeek.Monday);
                if (OnTuesday)
                    daysList.Add(DayOfWeek.Tuesday);
                if (OnWednesday)
                    daysList.Add(DayOfWeek.Wednesday);
                if (OnThursday)
                    daysList.Add(DayOfWeek.Thursday);
                if (OnFriday)
                    daysList.Add(DayOfWeek.Friday);
                if (OnSaturday)
                    daysList.Add(DayOfWeek.Saturday);
                if (OnSunday)
                    daysList.Add(DayOfWeek.Sunday);
                return daysList;
            }
            set { SetValue(() => DayOfWeekHeld, value); }
        }

        public string ClassBegins
        {
            get { return GetValue(() => ClassBegins); }
            set { SetValue(() => ClassBegins, value); }
        }
        public string ClassEnds
        {
            get { return GetValue(() => ClassEnds); }
            set { SetValue(() => ClassEnds, value); }
        }
        public int ClassCapacity
        {
            get { return GetValue(() => ClassCapacity); }
            set { SetValue(() => ClassCapacity, value); }
        }
        public string Notes
        {
            get { return GetValue(() => Notes); }
            set { SetValue(() => Notes, value); }
        }

        public ICollection<TrainerTimeTableDTO> Trainers
        {
            get { return GetValue(() => Trainers); }
            set { SetValue(() => Trainers, value); }
        }
    }
}