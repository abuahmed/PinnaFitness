using System;
using System.Collections.Generic;
using PinnaFit.Core;
using PinnaFit.Core.Models;

namespace PinnaFit.Service.Interfaces
{
    public interface IItemService : IDisposable
    {
        IEnumerable<ItemDTO> GetAll(SearchCriteria<ItemDTO> criteria = null);
        ItemDTO Find(string itemId);
        ItemDTO GetByName(string displayName);
        string InsertOrUpdate(ItemDTO item);
        string Disable(ItemDTO item);
        int Delete(string itemId);
        string GetItemCode();
        string GetItemNumber(int itemId);
    }
}