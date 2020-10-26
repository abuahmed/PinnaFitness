using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinnaFit.Core.Models
{
    public class TrainerTimeTableDTO : CommonFieldsA
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
        [ForeignKey("TimeTable")]
        public int TimeTableId { get; set; }
        public TimeTableDTO TimeTable
        {
            get { return GetValue(() => TimeTable); }
            set { SetValue(() => TimeTable, value); }
        }
    }
}