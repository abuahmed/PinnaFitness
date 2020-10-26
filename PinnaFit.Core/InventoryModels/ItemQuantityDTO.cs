using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinnaFit.Core.Models
{
    public class ItemQuantityDTO : CommonFieldsA
    {
        [ForeignKey("Warehouse")]
        public int WarehouseId { get; set; }

        public WarehouseDTO Warehouse
        {
            get { return GetValue(() => Warehouse); }
            set { SetValue(() => Warehouse, value); }
        }

        [ForeignKey("Item")]
        public int ItemId { get; set; }
        public ItemDTO Item
        {
            get { return GetValue(() => Item); }
            set { SetValue(() => Item, value); }
        }

        [DisplayName("Qty. On Hand")]
        public decimal? QuantityOnHand
        {
            get { return GetValue(() => QuantityOnHand); }
            set { SetValue(() => QuantityOnHand, value); }
        }

        [NotMapped]
        public decimal? Difference
        {
            get
            {
                if (Item != null && Item.SafeQuantity != null) return QuantityOnHand - (int) Item.SafeQuantity;
                return -1;
            }
            set { SetValue(() => Difference, value); }
        }

        [NotMapped, DisplayName("Total Price")]
        public decimal? TotalPrice
        {
            get
            {
                if (Item != null && Item.SalePrice != null && QuantityOnHand != null) 
                    return (decimal)(QuantityOnHand * Item.SalePrice);
                return null;
            }
            set { SetValue(() => TotalPrice, value); }
        }

        [NotMapped, DisplayName("Total Price")]
        public string TotalPriceString
        {
            get
            {
                return TotalPrice.ToString();//("N");
            }
            set { SetValue(() => TotalPriceString, value); }
        }

        [NotMapped]
        public decimal? TotalPurchasePrice
        {
            get
            {
                if (Item != null && Item.PurchasePrice != null && QuantityOnHand != null) 
                    return (decimal)(QuantityOnHand * Item.PurchasePrice);
                return null;
            }
            set { SetValue(() => TotalPurchasePrice, value); }
        }
    }
}