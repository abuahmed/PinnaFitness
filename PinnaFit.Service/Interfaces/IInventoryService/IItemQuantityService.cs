using System;
using System.Collections.Generic;
using PinnaFit.Core;
using PinnaFit.Core.Models;

namespace PinnaFit.Service.Interfaces
{
    public interface IItemQuantityService : IDisposable
    {
        IEnumerable<ItemQuantityDTO> GetAll(SearchCriteria<ItemQuantityDTO> criteria = null);
        IEnumerable<ItemQuantityDTO> GetAll(SearchCriteria<ItemQuantityDTO> criteria, out int totalCount);
        ItemQuantityDTO Find(string itemQuantityId);
        ItemQuantityDTO GetByName(string displayName);
        ItemQuantityDTO GetByCriteria(int warehouseId, int itemId);
        string InsertOrUpdate(ItemQuantityDTO itemQuantity, bool insertPi);
        ItemQuantityDTO InsertOrUpdate(ItemQuantityDTO itemQuantity);
        string Disable(ItemQuantityDTO itemQuantity);
        int Delete(string itemQuantityId);
    }
}