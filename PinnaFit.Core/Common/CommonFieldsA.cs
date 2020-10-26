using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaFit.Core.Models;

namespace PinnaFit.Core
{
    public class CommonFieldsA : EntityBase
    {
        [NotMapped]
        [DisplayName("No.")]
        public int SerialNumber { get; set; }
        
    }
}
