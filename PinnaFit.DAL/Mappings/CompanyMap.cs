using System.Data.Entity.ModelConfiguration;
using PinnaFit.Core.Models;

namespace PinnaFit.DAL.Mappings
{
    public class CompanyMap : EntityTypeConfiguration<CompanyDTO>
    {
        public CompanyMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            // Properties
            
            // Table & Column Mappings
            ToTable("Company");
            Property(t => t.Id).HasColumnName("Id");

            //RelationShips
        }
    }
}