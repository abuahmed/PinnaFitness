using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PinnaFit.Core.Models
{
    public class CompanyDTO : CommonFieldsE
    {
        [MaxLength(10, ErrorMessage = "Name Exceeded 10 letters")]
        [DisplayName("TinNumber")]
        public string TinNumber
        {
            get { return GetValue(() => TinNumber); }
            set { SetValue(() => TinNumber, value); }
        }
        [MaxLength(10, ErrorMessage = "Name Exceeded 10 letters")]
        [DisplayName("VatNumber")]
        public string VatNumber
        {
            get { return GetValue(() => VatNumber); }
            set { SetValue(() => VatNumber, value); }
        }
    }
}
