using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PinnaFit.Core;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Models;

namespace PinnaFit.Service.Interfaces
{
    public interface IServiceService : IDisposable
    {
        IEnumerable<ServiceDTO> GetAll(SearchCriteria<ServiceDTO> criteria=null);
        IEnumerable<ServiceDTO> GetAll(SearchCriteria<ServiceDTO> criteria, out int totalCount);
        IEnumerable<TrainerDTO> GetAllTrainers();
        IEnumerable<TrainerCourseDTO> GetTrainerServices();
        ServiceDTO Find(string serviceId);
        ServiceDTO GetByName(string displayName);
        string InsertOrUpdate(ServiceDTO service);
        string Disable(ServiceDTO service);
        int Delete(string serviceId);
        string GetServiceCode();
    }
}