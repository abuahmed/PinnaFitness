using System;
using System.Collections.Generic;
using PinnaFit.Core;
using PinnaFit.Core.Models;

namespace PinnaFit.Service.Interfaces
{
    public interface IEquipmentService : IDisposable
    {
        IEnumerable<EquipmentDTO> GetAll(SearchCriteria<EquipmentDTO> criteria = null);
        IEnumerable<EquipmentDTO> GetAll(SearchCriteria<EquipmentDTO> criteria, out int totalCount);
        EquipmentDTO Find(string equipmentId);
        EquipmentDTO GetByName(string displayName);
        string InsertOrUpdate(EquipmentDTO equipment);
        string Disable(EquipmentDTO equipment);
        int Delete(string equipmentId);
        string GetEquipmentNumber(int equipmentId);
    }
}