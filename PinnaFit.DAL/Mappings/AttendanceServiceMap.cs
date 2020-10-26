using System.Data.Entity.ModelConfiguration;
using PinnaFit.Core.Models;

namespace PinnaFit.DAL.Mappings
{
    public class AttendanceServiceMap : EntityTypeConfiguration<AttendanceServiceDTO>
    {
        public AttendanceServiceMap()
        {
            // Primary Key
            //this.HasKey(t => {t.RoleId,t.UserId});

            // Properties
            //Property(t => t.RoleDescription)
            // .IsRequired();

            // Table & Column Mappings
            ToTable("AttendanceServices");

            //Relationships
            HasRequired(t => t.Attendance)
                .WithMany(e => e.Services)
                .HasForeignKey(t => t.AttendanceId);

            HasRequired(t => t.Service)
                .WithMany(e => e.Attendances)
                .HasForeignKey(t => t.ServiceId);
        }
    }
}