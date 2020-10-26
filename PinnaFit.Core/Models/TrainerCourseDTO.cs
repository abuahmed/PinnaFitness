using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinnaFit.Core.Models
{
    public class TrainerCourseDTO : CommonFieldsA
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Trainer")]
        public int TrainerId { get; set; }
        public TrainerDTO Trainer
        {
            get { return GetValue(() => Trainer); }
            set { SetValue(() => Trainer, value); }
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