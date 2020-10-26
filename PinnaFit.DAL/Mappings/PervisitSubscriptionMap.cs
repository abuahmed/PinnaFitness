using System.Data.Entity.ModelConfiguration;
using PinnaFit.Core.Models;

namespace PinnaFit.DAL.Mappings
{
    public class PervisitSubscriptionMap : EntityTypeConfiguration<PervisitSubscriptionDTO>
    {
        public PervisitSubscriptionMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            // Properties

            // Table & Column Mappings
            ToTable("PervisitSubscriptions");
            Property(t => t.Id).HasColumnName("Id");

            //RelationShips
        }
    }
}