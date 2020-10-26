using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PinnaFit.Core;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Models;

namespace PinnaFit.Service.Interfaces
{
    public interface IPervisitSubscriptionService : IDisposable
    {
        IEnumerable<PervisitSubscriptionDTO> GetAll(SearchCriteria<PervisitSubscriptionDTO> criteria=null);
        IEnumerable<PervisitSubscriptionDTO> GetAll(SearchCriteria<PervisitSubscriptionDTO> criteria, out int totalCount);
        PervisitSubscriptionDTO Find(string memberSubscriptionId);
        PervisitSubscriptionDTO GetByName(string displayName);
        string InsertOrUpdate(PervisitSubscriptionDTO memberSubscription);
        string Disable(PervisitSubscriptionDTO memberSubscription);
        int Delete(string memberSubscriptionId);
        string GetPervisitSubscriptionCode();
    }
}