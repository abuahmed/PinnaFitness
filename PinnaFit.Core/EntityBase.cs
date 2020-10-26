using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaFit.Core.Models.Interfaces;

namespace PinnaFit.Core.Models
{
    public abstract class EntityBase: PropertyChangedNotification, IObjectState
    {
        protected EntityBase()
        {
            RowGuid = Guid.NewGuid();
            Enabled = true;
            CreatedByUserId = Singleton.User != null ? Singleton.User.UserId : 1;
            DateRecordCreated = DateTime.Now;
            ModifiedByUserId = Singleton.User != null ? Singleton.User.UserId : 1;
            DateLastModified = DateTime.Now;
        }
        //protected EntityBase(int userId)
        //{
        //    RowGuid = Guid.NewGuid();
        //    Enabled = true;
        //    CreatedByUserId = userId;
        //    DateRecordCreated = DateTime.Now;
        //    ModifiedByUserId = userId;
        //    DateLastModified = DateTime.Now;
        //}
        [NotMapped]
        public ObjectState ObjectState { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(Order = 0)]
        public int Id { get; set; }

        public Guid RowGuid { get; set; }
        
        public bool Enabled { get; set; }

        [ForeignKey("CreatedByUser")]
        public int? CreatedByUserId { get; set; }
        public UserDTO CreatedByUser
        {
            get { return GetValue(() => CreatedByUser); }
            set { SetValue(() => CreatedByUser, value); }
        }

        public DateTime? DateRecordCreated { get; set; }

        [ForeignKey("ModifiedByUser")]
        public int? ModifiedByUserId { get; set; }
        public UserDTO ModifiedByUser
        {
            get { return GetValue(() => ModifiedByUser); }
            set { SetValue(() => ModifiedByUser, value); }
        }

        public DateTime? DateLastModified { get; set; }

        [NotMapped]
        public string DateRecordCreatedStringAndAmharic
        {
            get
            {
                if (DateRecordCreated != null)
                    return DateRecordCreated.Value.ToString("dd-MM-yyyy hh:mm:ss") + "(" + ReportUtility.GetEthCalendarFormated(DateRecordCreated.Value, "/") + ")";
                return "";
            }
            set { SetValue(() => DateRecordCreatedStringAndAmharic, value); }
        }

        [NotMapped]
        public string DateLastModifiedStringAndAmharic
        {
            get {
                if (DateLastModified != null)
                    return DateLastModified.Value.ToString("dd-MM-yyyy hh:mm:ss") + "(" + ReportUtility.GetEthCalendarFormated(DateLastModified.Value, "/") + ")";
                return "";
            }
            set { SetValue(() => DateLastModifiedStringAndAmharic, value); }
        }
    }

    public abstract class UserEntityBase : PropertyChangedNotification, IObjectState
    {
        protected UserEntityBase()
        {
            RowGuid = Guid.NewGuid();
            Enabled = true;
            CreatedByUserId = Singleton.User != null ? Singleton.User.UserId : 1;
            DateRecordCreated = DateTime.Now;
            ModifiedByUserId = Singleton.User != null ? Singleton.User.UserId : 1;
            DateLastModified = DateTime.Now;
        }

        [NotMapped]
        public ObjectState ObjectState { get; set; }

        [NotMapped]
        [DisplayName("No.")]
        public int SerialNumber { get; set; }

        public Guid? RowGuid { get; set; }

        public bool? Enabled { get; set; }

        public int? CreatedByUserId { get; set; }

        public DateTime? DateRecordCreated { get; set; }

        public int? ModifiedByUserId { get; set; }

        public DateTime? DateLastModified { get; set; }
    }
}
