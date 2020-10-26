using System.Data.Entity.ModelConfiguration;
using PinnaFit.Core.Models;

namespace PinnaFit.DAL.Mappings
{
    public class TrainerMap : EntityTypeConfiguration<TrainerDTO>
    {
        public TrainerMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            // Properties

            // Table & Column Mappings
            ToTable("Trainers");
            Property(t => t.Id).HasColumnName("Id");

            //RelationShips

        }
    }
}