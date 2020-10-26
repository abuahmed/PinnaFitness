using System.Collections.Generic;

namespace PinnaFit.Core.Models
{
    public class ServiceDTO : CommonFieldsB
    {
        public ICollection<FacilityServiceDTO> Facilities
        {
            get { return GetValue(() => Facilities); }
            set { SetValue(() => Facilities, value); }
        }
        //Trainers Entry ...
        public ICollection<TrainerCourseDTO> Trainers
        {
            get { return GetValue(() => Trainers); }
            set { SetValue(() => Trainers, value); }
        }

        //Days and Hours the facility is given(TimeTable)
        public ICollection<TimeTableDTO> TimeTable
        {
            get { return GetValue(() => TimeTable); }
            set { SetValue(() => TimeTable, value); }
        }
        //Days and Hours the facility is given(TimeTable)
        public ICollection<AttendanceServiceDTO> Attendances
        {
            get { return GetValue(() => Attendances); }
            set { SetValue(() => Attendances, value); }
        }
    }
}