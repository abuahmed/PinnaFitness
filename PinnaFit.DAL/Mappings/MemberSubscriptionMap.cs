using System.Data.Entity.ModelConfiguration;
using PinnaFit.Core.Models;

namespace PinnaFit.DAL.Mappings
{
    public class MemberSubscriptionMap : EntityTypeConfiguration<MemberSubscriptionDTO>
    {
        public MemberSubscriptionMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            // Properties

            // Table & Column Mappings
            ToTable("MemberSubscriptions");
            Property(t => t.Id).HasColumnName("Id");

            //RelationShips
            HasRequired(t => t.Member)
              .WithMany(t => t.Subscriptions)
              .HasForeignKey(t => new { t.MemberId })
              .WillCascadeOnDelete(false);
        }
    }
}