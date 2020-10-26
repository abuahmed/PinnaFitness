using System.ComponentModel.DataAnnotations;
using PinnaFit.Core.Common;

namespace PinnaFit.Core.Models
{
    public class AttachmentDTO : CommonFieldsA
    {
        [MaxLength]
        public byte[] AttachedFile
        {
            get { return GetValue(() => AttachedFile); }
            set { SetValue(() => AttachedFile, value); }
        }

        public string AttachmentUrl
        {
            get { return GetValue(() => AttachmentUrl); }
            set { SetValue(() => AttachmentUrl, value); }
        }

        public string Comments
        {
            get { return GetValue(() => Comments); }
            set { SetValue(() => Comments, value); }
        }
    }
}