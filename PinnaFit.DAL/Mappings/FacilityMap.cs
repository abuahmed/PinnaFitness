using System.Data.Entity.ModelConfiguration;
using PinnaFit.Core.Models;

namespace PinnaFit.DAL.Mappings
{
    public class FacilityMap : EntityTypeConfiguration<FacilityDTO>
    {
        public FacilityMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            // Properties

            // Table & Column Mappings
            ToTable("Facilities");
            Property(t => t.Id).HasColumnName("Id");

            //RelationShips

        }
    }
}