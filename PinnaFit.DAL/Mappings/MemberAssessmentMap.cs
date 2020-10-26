using System.Data.Entity.ModelConfiguration;
using PinnaFit.Core.Models;

namespace PinnaFit.DAL.Mappings
{
    public class MemberAssessmentMap : EntityTypeConfiguration<MemberAssessmentDTO>
    {
        public MemberAssessmentMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            // Properties

            // Table & Column Mappings
            ToTable("MemberAssessments");
            Property(t => t.Id).HasColumnName("Id");

            //RelationShips
            HasRequired(t => t.Member)
             .WithMany(t => t.Assessments)
             .HasForeignKey(t => new { t.MemberId })
             .WillCascadeOnDelete(false);
        }
    }
}