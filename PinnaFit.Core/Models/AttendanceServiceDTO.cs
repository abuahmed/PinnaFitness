using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinnaFit.Core.Models
{
    public class AttendanceServiceDTO : CommonFieldsA
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Attendance")]
        public int AttendanceId { get; set; }
        public MemberAttendanceDTO Attendance
        {
            get { return GetValue(() => Attendance); }
            set { SetValue(() => Attendance, value); }
        }

        [Key]
        [Column(Order = 2)]
        [ForeignKey("Service")]
        public int ServiceId { get; set; }
        public ServiceDTO Service
        {
            get { return GetValue(() => Service); }
            set { SetValue(() => Service, value); }
        }
    }
}