using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinnaFit.Core.Models
{
    public class FacilitySubscriptionDTO : CommonFieldsA
    {
        [Key]
        [ForeignKey("Subscription")]
        [Column(Order = 1)]
        public int SubscriptionId { get; set; }
        public SubscriptionDTO Subscription
        {
            get { return GetValue(() => Subscription); }
            set { SetValue(() => Subscription, value); }
        }

        [Key]
        [ForeignKey("Facility")]
        [Column(Order = 2)]
        public int FacilityId { get; set; }
        public FacilityDTO Facility
        {
            get { return GetValue(() => Facility); }
            set { SetValue(() => Facility, value); }
        }

        public decimal Amount
        {
            get { return GetValue(() => Amount); }
            set { SetValue(() => Amount, value); }
        }

        [NotMapped]
        public string PackageName
        {
            get
            {
                var pkg = "";
                if (Facility != null)
                    pkg = Facility.DisplayName;
                if (Subscription != null)
                    pkg = pkg + " (" + Subscription.DisplayName + ")";

                return pkg;
            }
            set { SetValue(() => PackageName, value); }
        }
    }
}