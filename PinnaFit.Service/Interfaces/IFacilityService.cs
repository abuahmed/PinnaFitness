using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PinnaFit.Core;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Models;

namespace PinnaFit.Service.Interfaces
{
    public interface IFacilityService : IDisposable
    {
        IEnumerable<FacilityDTO> GetAll(SearchCriteria<FacilityDTO> criteria=null);
        IEnumerable<FacilityDTO> GetAll(SearchCriteria<FacilityDTO> criteria, out int totalCount);
        IEnumerable<ServiceDTO> GetAllServices();
        FacilityDTO Find(string facilityId);
        FacilityDTO GetByName(string displayName);
        string InsertOrUpdate(FacilityDTO facility);
        string Disable(FacilityDTO facility);
        int Delete(string facilityId);
        string GetFacilityCode();
    }
}