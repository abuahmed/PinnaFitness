using System.Collections.Generic;

namespace PinnaFit.Core.Models
{
    public class FacilityDTO : CommonFieldsB
    {
        public ICollection<FacilityServiceDTO> Services
        {
            get { return GetValue(() => Services); }
            set { SetValue(() => Services, value); }
        }


    }
}