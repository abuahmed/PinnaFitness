using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaFit.Core.Enumerations;

namespace PinnaFit.Core.Models
{
    public class BusinessPartnerDTO : CommonFieldsD2
    {
        public BusinessPartnerDTO()
        {
            TransactionHeaders = new List<TransactionHeaderDTO>();
        }

        public BusinessPartnerTypes BusinessPartnerType
        {
            get { return GetValue(() => BusinessPartnerType); }
            set { SetValue(() => BusinessPartnerType, value); }
        }

        public BusinessPartnerCategory Category
        {
            get { return GetValue(() => Category); }
            set { SetValue(() => Category, value); }
        }

        [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
        [DisplayName("TIN Number")]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Invalid Tin No, Must be 10 digit")]
        public string TinNumber
        {
            get { return GetValue(() => TinNumber); }
            set { SetValue(() => TinNumber, value); }
        }

        [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
        [DisplayName("VAT Number")]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Invalid VAT No, Must be 10 digit")]
        public string VatNumber
        {
            get { return GetValue(() => VatNumber); }
            set { SetValue(() => VatNumber, value); }
        }

        [MaxLength(30, ErrorMessage = "Code exceeded 50 letters")]
        public string Number
        {
            get { return GetValue(() => Number); }
            set { SetValue(() => Number, value); }
        }
        
        public ICollection<TransactionHeaderDTO> TransactionHeaders
        {
            get { return GetValue(() => TransactionHeaders); }
            set { SetValue(() => TransactionHeaders, value); }
        }

        [NotMapped]
        public string BusinessPartnerDetail
        {
            get
            {
                var clDet = DisplayName + " - " + TinNumber;
                if (Address != null)
                    clDet = clDet + " - " + Address.Mobile;
                if (clDet.Equals(" -  - "))
                    clDet = "";
                return clDet;
            }
            set { SetValue(() => BusinessPartnerDetail, value); }
        }
    }
    
}