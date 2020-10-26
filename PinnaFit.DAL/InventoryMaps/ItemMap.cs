using System.Data.Entity.ModelConfiguration;
using PinnaFit.Core.Models;

namespace PinnaFit.DAL.Mappings
{
    public class ItemMap : EntityTypeConfiguration<ItemDTO>
    {
        public ItemMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            //Property(t => t.DisplayName)
            //   .IsRequired()
            //   .HasMaxLength(45);
            
            // Table & Column Mappings
            ToTable("Items");
            Property(t => t.Id).HasColumnName("Id");

            //Relationships
            HasOptional(t => t.Category)
              .WithMany()
              .HasForeignKey(t => new { t.CategoryId });

            HasOptional(t => t.UnitOfMeasure)
                .WithMany()
                .HasForeignKey(t => new {t.UnitOfMeasureId});

        }
    }
}
