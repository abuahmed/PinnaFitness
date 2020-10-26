using System.Data.Entity.ModelConfiguration;
using PinnaFit.Core.Models;

namespace PinnaFit.DAL.Mappings
{
    public class FacilityServiceMap : EntityTypeConfiguration<FacilityServiceDTO>
    {
        public FacilityServiceMap()
        {

            // Table & Column Mappings
            ToTable("FacilityServices");

            //Relationships
            HasRequired(t => t.Service)
                .WithMany(e => e.Facilities)
                .HasForeignKey(t => t.ServiceId);

            HasRequired(t => t.Facility)
                .WithMany(e => e.Services)
                .HasForeignKey(t => t.FacilityId);
        }
    }
}