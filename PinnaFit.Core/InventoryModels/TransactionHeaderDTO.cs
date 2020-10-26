using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Extensions;

namespace PinnaFit.Core.Models
{
    public class TransactionHeaderDTO : CommonFieldsA
    {
        public TransactionHeaderDTO()
        {
            TransactionLines = new List<TransactionLineDTO>();
        }

        public TransactionTypes TransactionType
        {
            get { return GetValue(() => TransactionType); }
            set { SetValue(() => TransactionType, value); }
        }

        [DisplayName("Transaction No.")]
        [MaxLength(255, ErrorMessage = "Exceeded 255 letters")]
        public string TransactionNumber
        {
            get { return GetValue(() => TransactionNumber); }
            set { SetValue(() => TransactionNumber, value); }
        }
        
        [DisplayName("On Date")]
        public DateTime TransactionDate
        {
            get { return GetValue(() => TransactionDate); }
            set { SetValue(() => TransactionDate, value); }
        }

        public TransactionStatus Status
        {
            get { return GetValue(() => Status); }
            set
            {
                SetValue(() => Status, value);
            }
        }

        [ForeignKey("Warehouse")]
        public int WarehouseId { get; set; }
        public WarehouseDTO Warehouse
        {
            get { return GetValue(() => Warehouse); }
            set { SetValue(() => Warehouse, value); }
        }
        
        public string BusinessPartner
        {
            get { return GetValue(() => BusinessPartner); }
            set { SetValue(() => BusinessPartner, value); }
        }

        [ForeignKey("BusinessPartnerD")]
        public int? BusinessPartnerDId { get; set; }
        public BusinessPartnerDTO BusinessPartnerD
        {
            get { return GetValue(() => BusinessPartnerD); }
            set { SetValue(() => BusinessPartnerD, value); }
        }

        public ICollection<TransactionLineDTO> TransactionLines
        {
            get { return GetValue(() => TransactionLines); }
            set
            {
                SetValue(() => TransactionLines, value);
            }
        }

        [DisplayName("Total Amount")]
        public decimal? TotalAmount
        {
            get { return GetValue(() => TotalAmount); }
            set { SetValue(() => TotalAmount, value); }
        }

        [DisplayName("Total Items")]
        public decimal? TotalItems
        {
            get { return GetValue(() => TotalItems); }
            set { SetValue(() => TotalItems, value); }
        }

        [NotMapped]
        [DisplayName("No. of Items")]
        public int CountLines
        {
            get { if (TransactionLines != null) return TransactionLines.Count(l => l.Enabled);
                return 0;
            }
            set { SetValue(() => CountLines, value); }
        }

        [NotMapped]
        [DisplayName("Total Value")]
        public decimal TotalValue
        {
            get
            {
                try
                {
                    if (TransactionLines != null) return TransactionLines.Sum(l => l.LinePrice);
                }catch{}
                return 0;
            }
            set { SetValue(() => TotalValue, value); }
        }

        [NotMapped]
        public string StatusString
        {
            get { return EnumUtil.GetEnumDesc(Status); }
            set { SetValue(() => StatusString, value); }
        }

        [NotMapped]
        public string TransactionDateStringAndAmharic
        {
            get
            {
                return ReportUtility.GetEthCalendarFormated(TransactionDate, "/");// + ")";//TransactionDate.ToString("hh:mm:ss");//dd-MM-yyyy  + "(" +
                       
            }
        }

        /***** Payment Fields ******/
        public ICollection<PaymentDTO> Payments
        {
            get { return GetValue(() => Payments); }
            set
            {
                SetValue(() => Payments, value);
                SetValue(() => PaymentCompleted, value.ToString());
            }
        }

        [NotMapped]
        [DisplayName("Payment Status")]
        public string PaymentCompleted
        {
            get
            {
                if (!Payments.Any(l => l.Enabled))
                    return EnumUtil.GetEnumDesc(PaymentStatus.NoPayment);

                var payment =
                    Payments.Where(l => l.Enabled).FirstOrDefault(
                        p => p.Status == PaymentStatus.NotCleared || p.Status == PaymentStatus.NotDeposited);

                return payment != null ?
                    EnumUtil.GetEnumDesc(PaymentStatus.NotCleared) : EnumUtil.GetEnumDesc(PaymentStatus.Cleared);
            }
            set { SetValue(() => PaymentCompleted, value); }
        }

        [NotMapped]
        [DisplayName("Amount Left")]
        public decimal AmountLeft
        {
            get
            {
                return Status != TransactionStatus.Order ?
                    Payments.Where(l => l.Enabled)
                    .Where(p => p.Status == PaymentStatus.NotCleared && p.PaymentMethod != PaymentMethods.Cash)
                    .Sum(p => p.Amount) : 0;
            }
            set { SetValue(() => AmountLeft, value); }
        }

        /******Transfer Fields******/
        [ForeignKey("ToWarehouse")]
        public int? ToWarehouseId { get; set; }
        public WarehouseDTO ToWarehouse
        {
            get { return GetValue(() => ToWarehouse); }
            set { SetValue(() => ToWarehouse, value); }
        }

        [StringLength(55)]
        public string RequestedBy
        {
            get { return GetValue(() => RequestedBy); }
            set { SetValue(() => RequestedBy, value); }
        }
        public DateTime? RequestedDate
        {
            get { return GetValue(() => RequestedDate); }
            set { SetValue(() => RequestedDate, value); }
        }

        [StringLength(55)]
        public string SentBy
        {
            get { return GetValue(() => SentBy); }
            set { SetValue(() => SentBy, value); }
        }
        public DateTime? SentDate
        {
            get { return GetValue(() => SentDate); }
            set { SetValue(() => SentDate, value); }
        }

        [StringLength(55)]
        public string ReceivedBy
        {
            get { return GetValue(() => ReceivedBy); }
            set { SetValue(() => ReceivedBy, value); }
        }
        public DateTime? ReceivedDate
        {
            get { return GetValue(() => ReceivedDate); }
            set { SetValue(() => ReceivedDate, value); }
        }
    }
}