using System.ComponentModel.DataAnnotations;
using PinnaFit.Core.Enumerations;

namespace PinnaFit.Core.Models
{
    public class SubscriptionDTO : CommonFieldsB
    {
        public SubscriptionTypes Type
        {
            get { return GetValue(() => Type); }
            set { SetValue(() => Type, value); }
        }

        [Required]
        public int Value
        {
            get { return GetValue(() => Value); }
            set { SetValue(() => Value, value); }
        }
    }
}