using System.Data.Entity.ModelConfiguration;
using PinnaFit.Core.Models;

namespace PinnaFit.DAL.Mappings
{
    public class EquipmentMap : EntityTypeConfiguration<EquipmentDTO>
    {
        public EquipmentMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            // Properties

            // Table & Column Mappings
            ToTable("Equipments");
            Property(t => t.Id).HasColumnName("Id");

            //RelationShips

        }
    }
}