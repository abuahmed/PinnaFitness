using System.Data.Entity.ModelConfiguration;
using PinnaFit.Core.Models;

namespace PinnaFit.DAL.Mappings
{
    public class ServiceMap : EntityTypeConfiguration<ServiceDTO>
    {
        public ServiceMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            // Properties

            // Table & Column Mappings
            ToTable("Services");
            Property(t => t.Id).HasColumnName("Id");

            //RelationShips

        }
    }
}