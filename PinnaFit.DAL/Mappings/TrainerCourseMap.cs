using System.Data.Entity.ModelConfiguration;
using PinnaFit.Core.Models;

namespace PinnaFit.DAL.Mappings
{
    public class TrainerCourseMap : EntityTypeConfiguration<TrainerCourseDTO>
    {
        public TrainerCourseMap()
        {
            // Primary Key
            //this.HasKey(t => {t.RoleId,t.UserId});

            // Properties
            //Property(t => t.RoleDescription)
            // .IsRequired();

            // Table & Column Mappings
            ToTable("TrainerCourses");

            //Relationships
            HasRequired(t => t.Trainer)
                .WithMany(e => e.Courses)
                .HasForeignKey(t => t.TrainerId);

            HasRequired(t => t.Service)
                .WithMany(e => e.Trainers)
                .HasForeignKey(t => t.ServiceId);
        }
    }
}