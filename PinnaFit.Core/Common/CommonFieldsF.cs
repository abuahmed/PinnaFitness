using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using PinnaFit.Core.CustomValidationAttributes;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Extensions;
using PinnaFit.Core.Models;

namespace PinnaFit.Core
{
    public class CommonFieldsF : CommonFieldsA
    {
        [MaxLength(255, ErrorMessage = "Full Name Exceeded 255 letters")]
        [DisplayName("Full Name")]
        [Required]
        public string DisplayName
        {
            get { return GetValue(() => DisplayName); }
            set { SetValue(() => DisplayName, value); }
        }

        [Index(IsUnique = true)]
        [MaxLength(50, ErrorMessage = "Number exceeded 50 letters")]
        [DisplayName("Registration Number")]
        public string Number
        {
            get { return GetValue(() => Number); }
            set { SetValue(() => Number, value); }
        }

        [DisplayName("Description")]
        public string Description
        {
            get { return GetValue(() => Description); }
            set { SetValue(() => Description, value); }
        }

        [Required]
        public Sex Sex
        {
            get { return GetValue(() => Sex); }
            set { SetValue(() => Sex, value); }
        }
        [NotMapped]
        public string SexAmharic
        {
            get
            {
                return EnumUtil.GetEnumDesc(Sex);
            }
            set { SetValue(() => SexAmharic, value); }
        }

        [Required]
        public DateTime DateOfBirth
        {
            get { return GetValue(() => DateOfBirth); }
            set
            {
                SetValue(() => DateOfBirth, value);
                //SetValue<int?>(() => AgeFromBirthDate, DateTime.Now.Year - value.Year);
            }
        }
        
        [ForeignKey("Photo")]
        public int? PhotoId { get; set; }
        public AttachmentDTO Photo
        {
            get { return GetValue(() => Photo); }
            set { SetValue(() => Photo, value); }
        }

        [ForeignKey("Address")]
        public int? AddressId { get; set; }
        public AddressDTO Address
        {
            get { return GetValue(() => Address); }
            set { SetValue(() => Address, value); }
        }

        /*********************/
        public MaritalStatusTypes MaritalStatus
        {
            get { return GetValue(() => MaritalStatus); }
            set { SetValue(() => MaritalStatus, value); }
        }

        [MaxLength(50, ErrorMessage = "exceeded 50 letters")]
        [ExcludeChar("/.,!@#$%", ErrorMessage = "contains invalid letters")]
        public string Occupation
        {
            get { return GetValue(() => Occupation); }
            set { SetValue(() => Occupation, value); }
        }

        public int? Age
        {
            get { return GetValue(() => Age); }
            set { SetValue(() => Age, value); }
        }

        [NotMapped]
        public int AgeFromBirthDate
        {
            get
            {
                int age = DateTime.Now.Subtract(DateOfBirth).Days;
                age = (int)(age / 365.25);
                return age;
            }
            set { SetValue(() => AgeFromBirthDate, value); }
        }

        [NotMapped]
        public string BirthDateEc
        {
            get
            {
                var ec = ReportUtility.GetEthCalendarFormated(DateOfBirth, "-") +
                                    "(" + DateOfBirth.ToString("dd-MM-yyyy") + ")";
                return ec;
            }
            set { SetValue(() => BirthDateEc, value); }
        }

        [MaxLength(50, ErrorMessage = "exceeded 50 letters")]
        [ExcludeChar("/.,!@#$%", ErrorMessage = "contains invalid letters")]
        public string NickName
        {
            get { return GetValue(() => NickName); }
            set { SetValue(() => NickName, value); }
        }

        public bool IsActive
        {
            get { return GetValue(() => IsActive); }
            set { SetValue(() => IsActive, value); }
        }

        [ForeignKey("ContactPerson")]
        public int? ContactPersonId { get; set; }
        public ContactPersonDTO ContactPerson
        {
            get { return GetValue(() => ContactPerson); }
            set { SetValue(() => ContactPerson, value); }
        }

        [ForeignKey("MemberAssessmentMore")]
        public int? MemberAssessmentMoreId { get; set; }
        public MemberAssessmentMoreDTO MemberAssessmentMore
        {
            get { return GetValue(() => MemberAssessmentMore); }
            set { SetValue(() => MemberAssessmentMore, value); }
        }

        [MaxLength(10, ErrorMessage = "Name Exceeded 10 letters")]
        [DisplayName("TinNumber")]
        public string TinNumber
        {
            get { return GetValue(() => TinNumber); }
            set { SetValue(() => TinNumber, value); }
        }

        [NotMapped]
        public string MemberDetail
        {
            get
            {
                var clDet = DisplayName + " - " + Number;
                if (Address != null)
                    clDet = clDet + " - " + Address.Mobile;
                return clDet;
            }
            set { SetValue(() => MemberDetail, value); }
        }
    }
}