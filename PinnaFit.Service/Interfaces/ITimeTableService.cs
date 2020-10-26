using System;
using System.Collections.Generic;
using PinnaFit.Core;
using PinnaFit.Core.Models;

namespace PinnaFit.Service.Interfaces
{
    public interface ITimeTableService : IDisposable
    {
        IEnumerable<TimeTableDTO> GetAll(SearchCriteria<TimeTableDTO> criteria = null);
        IEnumerable<TimeTableDTO> GetAll(SearchCriteria<TimeTableDTO> criteria, out int totalCount);
        IEnumerable<TrainerDTO> GetAllTrainers();
        IEnumerable<TrainerCourseDTO> GetTrainerServices();
        TimeTableDTO Find(string timeTableId);
        TimeTableDTO GetByName(string displayName);
        string InsertOrUpdate(TimeTableDTO timeTable);
        string Disable(TimeTableDTO timeTable);
        int Delete(string timeTableId);
        string GetTimeTableCode();
    }
}