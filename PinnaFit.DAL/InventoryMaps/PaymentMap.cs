using System.Data.Entity.ModelConfiguration;
using PinnaFit.Core.Models;

namespace PinnaFit.DAL.Mappings
{
    public class PaymentMap : EntityTypeConfiguration<PaymentDTO>
    {
        public PaymentMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.PaymentDate)
               .IsRequired();

            Property(t => t.Amount)
                .IsRequired();

            Property(t => t.Reason)
                .IsRequired();

            Property(t => t.PaymentType)
                .IsRequired();

            // Table & Column Mappings
            ToTable("Payments");
            Property(t => t.Id).HasColumnName("Id");

            //Relationships
            HasOptional(t => t.Transaction)
                .WithMany(t => t.Payments)
                .HasForeignKey(t => new { t.TransactionId });
            
        }
    }
}
