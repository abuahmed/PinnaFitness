using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PinnaFit.Core;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Models;

namespace PinnaFit.Service.Interfaces
{
    public interface IMemberAttendanceService : IDisposable
    {
        IEnumerable<MemberAttendanceDTO> GetAll(SearchCriteria<MemberAttendanceDTO> criteria=null);
        IEnumerable<MemberAttendanceDTO> GetAll(SearchCriteria<MemberAttendanceDTO> criteria, out int totalCount);
        IEnumerable<ServiceDTO> GetAllServices();
        MemberAttendanceDTO Find(string memberAttendanceId);
        MemberAttendanceDTO GetByName(string displayName);
        string InsertOrUpdate(MemberAttendanceDTO memberAttendance);
        string Disable(MemberAttendanceDTO memberAttendance);
        int Delete(string memberAttendanceId);
        string GetMemberAttendanceCode();
    }
}