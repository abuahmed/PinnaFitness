using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinnaFit.Core.Models
{
    public class TransactionLineDTO : CommonFieldsA
    {
        [ForeignKey("Transaction")]
        public int TransactionId { get; set; }
        public TransactionHeaderDTO Transaction
        {
            get { return GetValue(() => Transaction); }
            set { SetValue(() => Transaction, value); }
        }

        [ForeignKey("Item")]
        public int ItemId { get; set; }
        public ItemDTO Item
        {
            get { return GetValue(() => Item); }
            set { SetValue(() => Item, value); }
        }

        [Required]
        [Range(0.01,1000000.00)]
        public decimal Unit
        {
            get { return GetValue(() => Unit); }
            set
            {
                SetValue(() => Unit, value);
                SetValue(() => LinePrice, value);
            }
        }
        
        [DisplayName("Each Price")]
        public decimal? EachPrice
        {
            get { return GetValue(() => EachPrice); }
            set
            {
                SetValue(() => EachPrice, value);
                //SetValue(() => LinePrice, value);
            }
        }

        [NotMapped]
        [DisplayName("Line Price")]
        public decimal LinePrice
        {
            get
            {
                if (EachPrice != null)
                return (decimal) (Unit * EachPrice);
                return 0;
            }
            set { SetValue(() => LinePrice, value); }
        }

        public bool IsFree
        {
            get { return GetValue(() => IsFree); }
            set{SetValue(() => IsFree, value);}
        }

        [NotMapped]
        public string ForFree
        {
            get
            {
                if (IsFree)
                    return " (For Trainer) ";
                return "";
            }
        }
        /**Transfer Fields**/
        public decimal? OriginPreviousQuantity
        {
            get { return GetValue(() => OriginPreviousQuantity); }
            set { SetValue(() => OriginPreviousQuantity, value); }
        }

        public decimal? DestinationPreviousQuantity
        {
            get { return GetValue(() => DestinationPreviousQuantity); }
            set { SetValue(() => DestinationPreviousQuantity, value); }
        }


    }
}