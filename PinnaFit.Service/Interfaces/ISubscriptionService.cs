using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PinnaFit.Core;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Models;

namespace PinnaFit.Service.Interfaces
{
    public interface ISubscriptionService : IDisposable
    {
        IEnumerable<SubscriptionDTO> GetAll(SearchCriteria<SubscriptionDTO> criteria=null);
        IEnumerable<SubscriptionDTO> GetAll(SearchCriteria<SubscriptionDTO> criteria, out int totalCount);
        SubscriptionDTO Find(string subscriptionId);
        SubscriptionDTO GetByName(string displayName);
        string InsertOrUpdate(SubscriptionDTO subscription);
        string Disable(SubscriptionDTO subscription);
        int Delete(string subscriptionId);
        string GetSubscriptionCode();
    }
}