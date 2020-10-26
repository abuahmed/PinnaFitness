using System.Data.Entity.ModelConfiguration;
using PinnaFit.Core.Models;

namespace PinnaFit.DAL.Mappings
{
    public class SubscriptionMap : EntityTypeConfiguration<SubscriptionDTO>
    {
        public SubscriptionMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            // Properties

            // Table & Column Mappings
            ToTable("Subscriptions");
            Property(t => t.Id).HasColumnName("Id");

            //RelationShips

        }
    }
}