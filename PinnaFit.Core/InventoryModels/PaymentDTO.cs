using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Extensions;

namespace PinnaFit.Core.Models
{
    public class PaymentDTO : CommonFieldsA
    {
        [Required]
        public PaymentTypes PaymentType 
        {
            get { return GetValue(() => PaymentType); }
            set { SetValue(() => PaymentType, value); }
        }

        [Required]
        public DateTime PaymentDate
        {
            get { return GetValue(() => PaymentDate); }
            set { SetValue(() => PaymentDate, value); }
        }

        public string ReceiptNumber
        {
            get { return GetValue(() => ReceiptNumber); }
            set { SetValue(() => ReceiptNumber, value); }
        }
        
        public string CheckNumber
        {
            get { return GetValue(() => CheckNumber); }
            set { SetValue(() => CheckNumber, value); }
        }

        [Required]
        public string Reason
        {
            get { return GetValue(() => Reason); }
            set { SetValue(() => Reason, value); }
        }

        [MaxLength(50, ErrorMessage = "Exceeded 50 letters")]
        public string PersonName
        {
            get { return GetValue(() => PersonName); }
            set { SetValue(() => PersonName, value); }
        }

        [NotMapped]
        public string ReceiverName
        {
            get
            {
                if (!String.IsNullOrEmpty(PersonName))
                    return PersonName;
                else if(BusinessPartner!=null)
                    return BusinessPartner.DisplayName;
                
                return "";
            }
            set { SetValue(() => ReceiverName, value); }
        }

        [NotMapped]
        public string Number
        {
            get
            {
                int id = 10000+Id;

                return "DFP0" + id.ToString().Substring(1);
            }
            set { SetValue(() => Number, value); }
        }

        public string PaymentRemark
        {
            get { return GetValue(() => PaymentRemark); }
            set { SetValue(() => PaymentRemark, value); }
        }

        [Required]
        [Range(0.01,10000000)]
        public decimal Amount
        {
            get { return GetValue(() => Amount); }
            set { SetValue(() => Amount, value); }
        }

        public PaymentMethods PaymentMethod
        {
            get { return GetValue(() => PaymentMethod); }
            set { SetValue(() => PaymentMethod, value); }
        }

        public PaymentStatus Status
        {
            get { return GetValue(() => Status); }
            set { SetValue(() => Status, value); }
        }

        public DateTime? DueDate
        {
            get { return GetValue(() => DueDate); }
            set { SetValue(() => DueDate, value); }
        }

        [ForeignKey("BusinessPartner")]
        public int? BusinessPartnerId { get; set; }
        public BusinessPartnerDTO BusinessPartner
        {
            get { return GetValue(() => BusinessPartner); }
            set { SetValue(() => BusinessPartner, value); }
        }

        [ForeignKey("Transaction")]
        public int? TransactionId { get; set; }
        public TransactionHeaderDTO Transaction
        {
            get { return GetValue(() => Transaction); }
            set { SetValue(() => Transaction, value); }
        }
     
        [ForeignKey("Warehouse")]
        public int WarehouseId { get; set; }
        public WarehouseDTO Warehouse
        {
            get { return GetValue(() => Warehouse); }
            set { SetValue(() => Warehouse, value); }
        }

        #region NotMapped Attributes
        [NotMapped]
        [DisplayName("Payment Date")]
        public string PaymentDateString
        {
            get
            {
                return ReportUtility.GetEthCalendarFormated(PaymentDate, "/") + "(" + PaymentDate.ToString("dd/MM/yyyy") + ")";
            }
            set { SetValue(() => PaymentDateString, value); }
        }

        [NotMapped]
        [DisplayName("Amount")]
        public string AmountString
        {
            get { return Amount.ToString("C"); }
            set { SetValue(() => AmountString, value); }
        }

        [NotMapped]
        [DisplayName("Payment Type")]
        public string PaymentTypeString
        {
            get
            {
                return EnumUtil.GetEnumDesc(PaymentType);
            }
            set { SetValue(() => PaymentTypeString, value); }
        }

        [NotMapped]
        [DisplayName("Status")]
        public string StatusString
        {
            get
            {
                return EnumUtil.GetEnumDesc(Status);
            }
            set { SetValue(() => StatusString, value); }
        }

        [NotMapped]
        [DisplayName("Payment Method")]
        public string PaymentMethodString
        {
            get
            {
                return EnumUtil.GetEnumDesc(PaymentMethod);
            }
            set { SetValue(() => StatusString, value); }
        } 
        #endregion
    }
}
