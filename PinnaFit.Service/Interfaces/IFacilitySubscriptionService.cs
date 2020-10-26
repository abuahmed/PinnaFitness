using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PinnaFit.Core;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Models;

namespace PinnaFit.Service.Interfaces
{
    public interface IFacilitySubscriptionService : IDisposable
    {
        IEnumerable<FacilitySubscriptionDTO> GetAll(SearchCriteria<FacilitySubscriptionDTO> criteria=null);
        IEnumerable<FacilitySubscriptionDTO> GetAll(SearchCriteria<FacilitySubscriptionDTO> criteria, out int totalCount);
        FacilitySubscriptionDTO Find(string facilitySubscriptionId);
        FacilitySubscriptionDTO GetByName(string displayName);
        string InsertOrUpdate(FacilitySubscriptionDTO facilitySubscription);
        string Disable(FacilitySubscriptionDTO facilitySubscription);
        int Delete(string facilitySubscriptionId);
        string GetFacilitySubscriptionCode();
    }
}