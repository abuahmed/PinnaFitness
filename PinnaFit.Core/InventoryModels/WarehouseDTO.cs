using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaFit.Core.Enumerations;

namespace PinnaFit.Core.Models
{
    public class WarehouseDTO : CommonFieldsE
    {
        public WarehouseDTO() 
        {
            //Transactions = new List<TransactionHeaderDTO>();
        }

        public WarehouseTypes Type
        {
            get { return GetValue(() => Type); }
            set { SetValue(() => Type, value); }
        }

        [Index(IsUnique = true)]
        [MaxLength(50, ErrorMessage = "Number exceeded 50 letters")]
        [DisplayName("Number")]
        public string Number
        {
            get { return GetValue(() => Number); }
            set { SetValue(() => Number, value); }
        }
        
        [NotMapped]
        public string DisplayNameShort
        {
            get { return DisplayName != null && DisplayName.Length > 18 ? DisplayName.Substring(0, 15) + "..." : DisplayName; }
            set { SetValue(() => DisplayNameShort, value); }
        }

        public ICollection<TransactionHeaderDTO> Transactions
        {
            get { return GetValue(() => Transactions); }
            set { SetValue(() => Transactions, value); }
        }
   
    }
}
