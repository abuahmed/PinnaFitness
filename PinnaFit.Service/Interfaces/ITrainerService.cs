using System;
using System.Collections.Generic;
using PinnaFit.Core;
using PinnaFit.Core.Models;

namespace PinnaFit.Service.Interfaces
{
    public interface ITrainerService : IDisposable
    {
        IEnumerable<TrainerDTO> GetAll(SearchCriteria<TrainerDTO> criteria = null);
        IEnumerable<TrainerDTO> GetAll(SearchCriteria<TrainerDTO> criteria, out int totalCount);
        TrainerDTO Find(string trainerId);
        TrainerDTO GetByName(string displayName);
        string InsertOrUpdate(TrainerDTO trainer);
        string Disable(TrainerDTO trainer);
        int Delete(string trainerId);
        string GetTrainerNumber(int trainerId);
    }
}