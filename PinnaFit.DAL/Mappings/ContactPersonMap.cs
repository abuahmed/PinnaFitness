using System.Data.Entity.ModelConfiguration;
using PinnaFit.Core.Models;

namespace PinnaFit.DAL.Mappings
{
    public class ContactPersonMap : EntityTypeConfiguration<ContactPersonDTO>
    {
        public ContactPersonMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            // Properties

            // Table & Column Mappings
            ToTable("ContactPersons");
            Property(t => t.Id).HasColumnName("Id");
        }
    }
}