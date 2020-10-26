using System.Data.Entity.ModelConfiguration;
using PinnaFit.Core.Models;

namespace PinnaFit.DAL.Mappings
{
    public class BusinessPartnerMap : EntityTypeConfiguration<BusinessPartnerDTO>
    {
        public BusinessPartnerMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.DisplayName)
                .IsRequired();
            
            // Table & Column Mappings
            ToTable("BusinessPartners");
            Property(t => t.Id).HasColumnName("Id");
        }
    }
}