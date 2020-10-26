using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PinnaFit.Core;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Models;

namespace PinnaFit.Service.Interfaces
{
    public interface IMemberService : IDisposable
    {
        IEnumerable<MemberDTO> GetAll(SearchCriteria<MemberDTO> criteria=null);
        IEnumerable<MemberDTO> GetAll(SearchCriteria<MemberDTO> criteria, out int totalCount);
        MemberDTO Find(string memberId);
        MemberDTO GetByName(string displayName);
        string InsertOrUpdate(MemberDTO member);
        string Disable(MemberDTO member);
        int Delete(string memberId);
        string GetMemberNumber(int memberId);
    }
}