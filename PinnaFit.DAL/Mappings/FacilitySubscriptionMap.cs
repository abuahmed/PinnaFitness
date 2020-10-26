using System.Data.Entity.ModelConfiguration;
using PinnaFit.Core.Models;

namespace PinnaFit.DAL.Mappings
{
    public class FacilitySubscriptionMap : EntityTypeConfiguration<FacilitySubscriptionDTO>
    {
        public FacilitySubscriptionMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            // Properties

            // Table & Column Mappings
            ToTable("FacilitySubscriptions");
            Property(t => t.Id).HasColumnName("Id");

            //RelationShips

        }
    }
}