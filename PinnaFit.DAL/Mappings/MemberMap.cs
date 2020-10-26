using System.Data.Entity.ModelConfiguration;
using PinnaFit.Core.Models;

namespace PinnaFit.DAL.Mappings
{
    public class MemberMap : EntityTypeConfiguration<MemberDTO>
    {
        public MemberMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            // Properties

            // Table & Column Mappings
            ToTable("Members");
            Property(t => t.Id).HasColumnName("Id");

            //RelationShips

        }
    }
}