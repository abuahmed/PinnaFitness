using System.Data.Entity.ModelConfiguration;
using PinnaFit.Core.Models;

namespace PinnaFit.DAL.Mappings
{
    public class TransactionLineMap : EntityTypeConfiguration<TransactionLineDTO>
    {
        public TransactionLineMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
         
            // Table & Column Mappings
            ToTable("TransactionLines");
            Property(t => t.Id).HasColumnName("Id");

            //Relationships
            HasRequired(t => t.Transaction)
                .WithMany(t => t.TransactionLines)
                .HasForeignKey(t => new { t.TransactionId })
                .WillCascadeOnDelete(false);

            HasRequired(t => t.Item)
                .WithMany(t => t.TransactionLines)
                .HasForeignKey(t => new { t.ItemId })
                .WillCascadeOnDelete(false);

        }
    }
}