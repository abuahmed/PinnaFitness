using System.Data.Entity.ModelConfiguration;
using PinnaFit.Core.Models;

namespace PinnaFit.DAL.Mappings
{
    public class TimeTableMap : EntityTypeConfiguration<TimeTableDTO>
    {
        public TimeTableMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            // Properties

            // Table & Column Mappings
            ToTable("TimeTables");
            Property(t => t.Id).HasColumnName("Id");

            //RelationShips
            HasRequired(t => t.Service)
                .WithMany(t => t.TimeTable)
                .HasForeignKey(t => new { t.ServiceId })
                .WillCascadeOnDelete(false);
        }
    }
}