using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using PinnaFit.Core.CustomValidationAttributes;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Extensions;

namespace PinnaFit.Core.Models
{
    public class ItemDTO : CommonFieldsB
    {
        public ItemDTO()
        {
            ItemQuantities = new HashSet<ItemQuantityDTO>();
        }

        public ItemTypes ItemType
        {
            get { return GetValue(() => ItemType); }
            set { SetValue(() => ItemType, value); }
        }

        [Index(IsUnique = true)]
        [MaxLength(50, ErrorMessage = "Number exceeded 50 letters")]
        [DisplayName("Item Number")]
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

        [ForeignKey("UnitOfMeasure")]
        public int? UnitOfMeasureId { get; set; }
        public CategoryDTO UnitOfMeasure
        {
            get { return GetValue(() => UnitOfMeasure); }
            set { SetValue(() => UnitOfMeasure, value); }
        }
        
        //Will have sum current quantity of all stores
        public decimal? TotalCurrentQty 
        {
            get { return ItemQuantities.Sum(it => it.QuantityOnHand); }
            set { SetValue(() => TotalCurrentQty, value); }
        }

        [DisplayName("Purchasing Price")]
        public decimal? PurchasePrice
        {
            get { return GetValue(() => PurchasePrice); }
            set { SetValue(() => PurchasePrice, value); }
        }

        [Display(Name = "Sale Price")]
        public decimal? SalePrice
        {
            get { return GetValue(() => SalePrice); }
            set { SetValue(() => SalePrice, value); }
        }

        public decimal? SafeQuantity
        {
            get { return GetValue(() => SafeQuantity); }
            set { SetValue(() => SafeQuantity, value); }
        }
        public decimal? WarningQuantity
        {
            get { return GetValue(() => WarningQuantity); }
            set { SetValue(() => WarningQuantity, value); }
        }
        public float? Discount
        {
            get { return GetValue(() => Discount); }
            set { SetValue(() => Discount, value); }
        }

        public byte? ItemImage
        {
            get { return GetValue(() => ItemImage); }
            set { SetValue(() => ItemImage, value); }
        }
        
        [NotMapped]
        public string ItemDetail
        {
            get
            {
                var it = Number + " | " + DisplayName;
                if (Category != null)
                {
                    it = it + " | " + Category.DisplayName;
                }
                if (!string.IsNullOrEmpty(Description))
                    it = it + " | " + Description;
                return it;

            }
            set { SetValue(() => ItemDetail, value); }
        }
        [NotMapped]
        public string CategoryString
        {
            get
            {
                return Category != null ? Category.DisplayName : "";
            }
            set { SetValue(() => CategoryString, value); }
        }
        [NotMapped]
        public string UomString
        {
            get
            {
                return UnitOfMeasure != null ? UnitOfMeasure.DisplayName : "";
            }
            set { SetValue(() => UomString, value); }
        }
        [NotMapped]
        public string TypeString
        {
            get
            {
                return EnumUtil.GetEnumDesc(ItemType);
            }
            set { SetValue(() => TypeString, value); }
        }
        
        public ICollection<ItemQuantityDTO> ItemQuantities
        {
            get { return GetValue(() => ItemQuantities); }
            set { SetValue(() => ItemQuantities, value); }
        }
        public ICollection<TransactionLineDTO> TransactionLines
        {
            get { return GetValue(() => TransactionLines); }
            set { SetValue(() => TransactionLines, value); }
        }
    }
}
