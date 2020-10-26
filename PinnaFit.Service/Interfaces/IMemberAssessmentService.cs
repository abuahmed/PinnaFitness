using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PinnaFit.Core;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Models;

namespace PinnaFit.Service.Interfaces
{
    public interface IMemberAssessmentService : IDisposable
    {
        IEnumerable<MemberAssessmentDTO> GetAll(SearchCriteria<MemberAssessmentDTO> criteria=null);
        IEnumerable<MemberAssessmentDTO> GetAll(SearchCriteria<MemberAssessmentDTO> criteria, out int totalCount);
        MemberAssessmentDTO Find(string memberAssessmentId);
        MemberAssessmentDTO GetByName(string displayName);
        string InsertOrUpdate(MemberAssessmentDTO memberAssessment);
        string Disable(MemberAssessmentDTO memberAssessment);
        int Delete(string memberAssessmentId);
        string GetMemberAssessmentCode();
    }
}