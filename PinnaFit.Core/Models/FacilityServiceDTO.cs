using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinnaFit.Core.Models
{
    public class FacilityServiceDTO : CommonFieldsA
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Service")]
        public int ServiceId { get; set; }
        public ServiceDTO Service
        {
            get { return GetValue(() => Service); }
            set { SetValue(() => Service, value); }
        }

        [Key]
        [Column(Order = 2)]
        [ForeignKey("Facility")]
        public int FacilityId { get; set; }
        public FacilityDTO Facility
        {
            get { return GetValue(() => Facility); }
            set { SetValue(() => Facility, value); }
        }
    }
}