using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaFit.Core.Models;

namespace PinnaFit.Core
{
    public class CommonFieldsD : CommonFieldsC
    {
        [ForeignKey("ResponsibleStaff")]
        public int? ResponsibleStaffId { get; set; }
        public TrainerDTO ResponsibleStaff
        {
            get { return GetValue(() => ResponsibleStaff); }
            set { SetValue(() => ResponsibleStaff, value); }
        }

        [MaxLength(255, ErrorMessage = "Contact Title Exceeded 255 letters")]
        [DisplayName("Contact Title")]
        public string ContactTitle
        {
            get { return GetValue(() => ContactTitle); }
            set { SetValue(() => ContactTitle, value); }
        }

        [MaxLength(255, ErrorMessage = "Contact Name Exceeded 255 letters")]
        [DisplayName("Contact Name")]
        public string ContactName
        {
            get { return GetValue(() => ContactName); }
            set { SetValue(() => ContactName, value); }
        }
    }
    public class CommonFieldsD2 : CommonFieldsC
    {
        [MaxLength(255, ErrorMessage = "Contact Title Exceeded 255 letters")]
        [DisplayName("Contact Title")]
        public string ContactTitle
        {
            get { return GetValue(() => ContactTitle); }
            set { SetValue(() => ContactTitle, value); }
        }

        [MaxLength(255, ErrorMessage = "Contact Name Exceeded 255 letters")]
        [DisplayName("Contact Name")]
        public string ContactName
        {
            get { return GetValue(() => ContactName); }
            set { SetValue(() => ContactName, value); }
        }
    }
}