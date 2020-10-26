using System.Data.Entity.ModelConfiguration;
using PinnaFit.Core.Models;

namespace PinnaFit.DAL.Mappings
{
    public class AttachmentMap : EntityTypeConfiguration<AttachmentDTO>
    {
        public AttachmentMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            // Properties

            // Table & Column Mappings
            ToTable("Attachments");
            Property(t => t.Id).HasColumnName("Id");
        }
    }
}