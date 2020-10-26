using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinnaFit.Core.Models
{
    public class MemberAttendanceDTO : CommonFieldsAttendance
    {
        [ForeignKey("MemberSubscription")]
        public int MemberSubscriptionId { get; set; }
        public MemberSubscriptionDTO MemberSubscription
        {
            get { return GetValue(() => MemberSubscription); }
            set { SetValue(() => MemberSubscription, value); }
        }
        
        //Trained/Followed By
        //List Additional/Extra Services Get (like shampoo,sauna,steam and the like)
        //Days and Hours the facility is given(TimeTable)
        public ICollection<AttendanceServiceDTO> Services
        {
            get { return GetValue(() => Services); }
            set
            {
                SetValue(() => Services, value);
            }
        }
        [NotMapped]
        public string ServicesTook
        {
            get
            {
                string serv = "";
                foreach (var attendanceServiceDTO in Services)
                {
                    serv = serv + " + " + attendanceServiceDTO.Service.DisplayName;
                }
                if (serv.StartsWith(" +"))
                    serv = serv.Substring(3);
                return serv;
            }
            set { SetValue(() => ServicesTook, value); }
        }
    }
}