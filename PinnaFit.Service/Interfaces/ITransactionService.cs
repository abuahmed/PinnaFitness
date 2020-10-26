using System;
using System.Collections.Generic;
using PinnaFit.Core;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Models;

namespace PinnaFit.Service.Interfaces
{
    public interface ITransactionService : IDisposable
    {
        IEnumerable<TransactionHeaderDTO> GetAll(SearchCriteria<TransactionHeaderDTO> criteria = null);
        IEnumerable<TransactionHeaderDTO> GetAll(SearchCriteria<TransactionHeaderDTO> criteria, out int totalCount);
        TransactionHeaderDTO Find(string transactionHeaderId);
        
        string InsertOrUpdate(TransactionHeaderDTO transactionHeader);
        string Post(TransactionHeaderDTO transactionHeader);
        string UnPost(TransactionHeaderDTO transaction);
        string Disable(TransactionHeaderDTO transactionHeader);
        int Delete(string transactionHeaderId);
        
        IEnumerable<TransactionLineDTO> GetChilds(int parentId, bool disposeWhenDone);
        IEnumerable<TransactionLineDTO> GetAllChilds(SearchCriteria<TransactionLineDTO> criteria, out int totalCount);
        string InsertOrUpdateChild(TransactionLineDTO transactionLine);
        string DisableChild(TransactionLineDTO transactionLine);
    }
}