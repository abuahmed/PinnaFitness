using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinnaFit.Core.Models
{
    public class EquipmentDTO : CommonFieldsA
    {
        [Index(IsUnique = true)]
        [MaxLength(50, ErrorMessage = "Number exceeded 50 letters")]
        [DisplayName("Equipment Number")]
        public string Number
        {
            get { return GetValue(() => Number); }
            set { SetValue(() => Number, value); }
        }

        [ForeignKey("Category")]
        public int? CategoryId { get; set; }
        public CategoryDTO Category
        {
            get { return GetValue(() => Category); }
            set { SetValue(() => Category, value); }
        }

        [MaxLength(255, ErrorMessage = "Name Exceeded 255 letters")]
        [DisplayName("Description")]
        public string Description
        {
            get { return GetValue(() => Description); }
            set { SetValue(() => Description, value); }
        }
    }
}