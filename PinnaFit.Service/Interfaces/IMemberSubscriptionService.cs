using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PinnaFit.Core;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Models;

namespace PinnaFit.Service.Interfaces
{
    public interface IMemberSubscriptionService : IDisposable
    {
        IEnumerable<MemberSubscriptionDTO> GetAll(SearchCriteria<MemberSubscriptionDTO> criteria=null);
        IEnumerable<MemberSubscriptionDTO> GetAll(SearchCriteria<MemberSubscriptionDTO> criteria, out int totalCount);
        MemberSubscriptionDTO Find(string memberSubscriptionId);
        MemberSubscriptionDTO GetByName(string displayName);
        string InsertOrUpdate(MemberSubscriptionDTO memberSubscription);
        string Disable(MemberSubscriptionDTO memberSubscription);
        int Delete(string memberSubscriptionId);
        string GetMemberSubscriptionCode();
    }
}