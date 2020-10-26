using System.Data.Entity.ModelConfiguration;
using PinnaFit.Core.Models;

namespace PinnaFit.DAL.Mappings
{
    public class TransactionHeaderMap : EntityTypeConfiguration<TransactionHeaderDTO>
    {
        public TransactionHeaderMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            //Property(t => t.TransactionNumber)
            //    .IsRequired()
            //    .HasMaxLength(45);

            // Table & Column Mappings
            ToTable("TransactionHeaders");
            Property(t => t.Id).HasColumnName("Id");

            //Relationships
            HasRequired(t => t.Warehouse)
                .WithMany(t => t.Transactions)
                .HasForeignKey(t => new { t.WarehouseId })
                .WillCascadeOnDelete(false);

        }
    }
}