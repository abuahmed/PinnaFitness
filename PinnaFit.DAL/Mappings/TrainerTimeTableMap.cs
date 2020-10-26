using System.Data.Entity.ModelConfiguration;
using PinnaFit.Core.Models;

namespace PinnaFit.DAL.Mappings
{
    public class TrainerTimeTableMap : EntityTypeConfiguration<TrainerTimeTableDTO>
    {
        public TrainerTimeTableMap()
        {
            // Primary Key
            //this.HasKey(t => {t.RoleId,t.UserId});

            // Properties
            //Property(t => t.RoleDescription)
            // .IsRequired();

            // Table & Column Mappings
            ToTable("TrainerTimeTables");

            //Relationships
            HasRequired(t => t.Trainer)
                .WithMany(e => e.TimeTable)
                .HasForeignKey(t => t.TrainerId);

            HasRequired(t => t.TimeTable)
                .WithMany(e => e.Trainers)
                .HasForeignKey(t => t.TimeTableId);

        }
    }
}