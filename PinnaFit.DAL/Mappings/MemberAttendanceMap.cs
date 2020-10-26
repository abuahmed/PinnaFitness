using System.Data.Entity.ModelConfiguration;
using PinnaFit.Core.Models;

namespace PinnaFit.DAL.Mappings
{
    public class MemberAttendanceMap : EntityTypeConfiguration<MemberAttendanceDTO>
    {
        public MemberAttendanceMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            // Properties

            // Table & Column Mappings
            ToTable("MemberAttendances");
            Property(t => t.Id).HasColumnName("Id");

            //RelationShips
            HasRequired(t => t.MemberSubscription)
                .WithMany(t => t.Attendances)
                .HasForeignKey(t => new { t.MemberSubscriptionId })
                .WillCascadeOnDelete(false);
        }
    }
}