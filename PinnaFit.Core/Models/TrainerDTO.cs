using System.Collections.Generic;

namespace PinnaFit.Core.Models
{
    public class TrainerDTO : CommonFieldsF
    {
        public ICollection<TrainerCourseDTO> Courses
        {
            get { return GetValue(() => Courses); }
            set { SetValue(() => Courses, value); }
        }
        public ICollection<TrainerTimeTableDTO> TimeTable
        {
            get { return GetValue(() => TimeTable); }
            set { SetValue(() => TimeTable, value); }
        }
    }
}