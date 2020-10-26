#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PinnaFit.Core;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Models;
using PinnaFit.DAL;
using PinnaFit.DAL.Interfaces;
using PinnaFit.Repository;
using PinnaFit.Repository.Interfaces;
using PinnaFit.Service.Interfaces;

#endregion

namespace PinnaFit.Service
{
    public class TransactionService : ITransactionService
    {
        #region Fields

        private IDbContext _iDbContext;
        private IUnitOfWork _unitOfWork;
        private IRepository<TransactionHeaderDTO> _transactionRepository;
        private IRepository<TransactionLineDTO> _transactionLineRepository;
        private IRepository<ItemQuantityDTO> _itemsQuantityRepository;
        private readonly bool _disposeWhenDone;

        #endregion

        #region Constructor

        public TransactionService()
        {
            InitializeDbContext();
        }

        public TransactionService(IDbContext dbContext)
        {
            _iDbContext = dbContext;
            _transactionRepository = new Repository<TransactionHeaderDTO>(_iDbContext);
            _transactionLineRepository = new Repository<TransactionLineDTO>(_iDbContext);
            _itemsQuantityRepository = new Repository<ItemQuantityDTO>(_iDbContext);
            _unitOfWork = new UnitOfWork(_iDbContext);
        }

        public TransactionService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            _iDbContext = DbContextUtil.GetDbContextInstance();
            _transactionRepository = new Repository<TransactionHeaderDTO>(_iDbContext);
            _transactionLineRepository = new Repository<TransactionLineDTO>(_iDbContext);
            _itemsQuantityRepository = new Repository<ItemQuantityDTO>(_iDbContext);
            _unitOfWork = new UnitOfWork(_iDbContext);
        }

        #endregion

        #region Common Methods

        public IRepositoryQuery<TransactionHeaderDTO> Get()
        {
            var piList = _transactionRepository
                .Query()
                .Include(a => a.Warehouse, p => p.Warehouse.Address, p => p.TransactionLines)
                //.Filter(a => !string.IsNullOrEmpty(a.TransactionNumber))
                .OrderBy(q => q.OrderByDescending(c => c.Id));
            return piList;
        }

        public IEnumerable<TransactionHeaderDTO> GetAll(SearchCriteria<TransactionHeaderDTO> criteria = null)
        {
            int totalCount;
            return GetAll(criteria, out totalCount);
        }

        public IEnumerable<TransactionHeaderDTO> GetAll(SearchCriteria<TransactionHeaderDTO> criteria,
            out int totalCount)
        {
            totalCount = 0;
            IEnumerable<TransactionHeaderDTO> piList = new List<TransactionHeaderDTO>();
            try
            {
                if (criteria != null && criteria.CurrentUserId != -1)
                {
                    var warehouseList = new WarehouseService(true)
                        .GetWarehousesPrevilegedToUser(criteria.CurrentUserId).ToList();

                    if (criteria.TransactionType == (int) TransactionTypes.TransferStock)
                        warehouseList = new WarehouseService(true).GetAll().ToList();

                    if (criteria.SelectedWarehouseId != null)
                        warehouseList = warehouseList.Where(w => w.Id == criteria.SelectedWarehouseId).ToList();


                    foreach (var warehouse in warehouseList.Where(w => w.Id != -1))
                    {
                        var pdto = Get();

                        foreach (var cri in criteria.FiList)
                        {
                            pdto.FilterList(cri);
                        }

                        pdto.FilterList(p => p.TransactionType == (TransactionTypes) criteria.TransactionType);

                        #region By Warehouse

                        var warehouse1 = warehouse;
                        pdto.FilterList(p => p.WarehouseId == warehouse1.Id);

                        #endregion

                        #region By Duration

                        if (criteria.BeginingDate != null)
                        {
                            var beginDate = new DateTime(criteria.BeginingDate.Value.Year, criteria.BeginingDate.Value.Month,
                                criteria.BeginingDate.Value.Day, 0, 0, 0);
                            if (criteria.Shift == ShiftTypes.Afternoon)
                                beginDate = new DateTime(criteria.BeginingDate.Value.Year, criteria.BeginingDate.Value.Month,
                                criteria.BeginingDate.Value.Day, 14, 0, 0);
                            pdto.FilterList(p => p.TransactionDate >= beginDate);
                        }

                        if (criteria.EndingDate != null)
                        {
                            var endDate = new DateTime(criteria.EndingDate.Value.Year, criteria.EndingDate.Value.Month,
                                criteria.EndingDate.Value.Day, 23, 59, 59);
                            if (criteria.Shift == ShiftTypes.Morning)
                                endDate = new DateTime(criteria.EndingDate.Value.Year, criteria.EndingDate.Value.Month,
                                criteria.EndingDate.Value.Day, 13, 59, 59);
                            pdto.FilterList(p => p.TransactionDate <= endDate);
                        }

                        #endregion

                        IList<TransactionHeaderDTO> pdtoList;
                        if (criteria.Page != 0 && criteria.PageSize != 0 && criteria.PaymentListType == -1)
                        {
                            int totalCount2;
                            pdtoList = pdto.GetPage(criteria.Page, criteria.PageSize, out totalCount2).ToList();
                            totalCount = totalCount2;
                        }
                        else
                        {
                            pdtoList = pdto.GetList().ToList();
                            totalCount = pdtoList.Count;
                        }


                        piList = piList.Concat(pdtoList).ToList();
                    }
                }
                else
                    piList = Get().Get().ToList();

                #region For Eager Loading Childs

                foreach (var transactionHeaderDto in piList)
                {
                    var transactionLineDtos =
                        (ICollection<TransactionLineDTO>) GetChilds(transactionHeaderDto.Id, false);
                }

                #endregion
            }
            finally
            {
                Dispose(_disposeWhenDone);
            }

            return piList;
        }

        public TransactionHeaderDTO Find(string transactionId)
        {
            return _transactionRepository.FindById(Convert.ToInt32(transactionId));
        }

        public string InsertOrUpdate(TransactionHeaderDTO transaction)
        {
            try
            {
                //InsertOrUpdateChild(transaction.TransactionLines.FirstOrDefault());
                //return "";
                var validate = Validate(transaction);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(transaction))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists;

                _transactionRepository.InsertUpdate(transaction);
                _unitOfWork.Commit();

                //Generate New TransactionNumber
                if (string.IsNullOrEmpty(transaction.TransactionNumber))
                    return GetNewTransactionNumber(transaction);

                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(TransactionHeaderDTO transaction2)
        {
            if (transaction2 == null)
                return GenericMessages.ObjectIsNull;

            var iDbContextTemp = DbContextUtil.GetDbContextInstance();
            var unitOfWorkTemp = new UnitOfWork(iDbContextTemp);
            string stat;
            try
            {
                var transaction = unitOfWorkTemp.Repository<TransactionHeaderDTO>().Query()
                    .Include(t => t.TransactionLines)
                    .Filter(t => t.Id == transaction2.Id)
                    .Get()
                    .FirstOrDefault();
                var transactionRepository2 = unitOfWorkTemp.Repository<TransactionHeaderDTO>();
                var transactionLineRepository2 = unitOfWorkTemp.Repository<TransactionLineDTO>();

                if (transaction != null)
                {
                    foreach (var transactionLine in transaction.TransactionLines.Where(t => t.Enabled))
                    {
                        transactionLine.Enabled = false;
                        transactionLineRepository2.Update(transactionLine);
                    }
                    transaction.Enabled = false;
                    transactionRepository2.Update(transaction);
                }
                unitOfWorkTemp.Commit();
                stat = string.Empty;
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            finally
            {
                iDbContextTemp.Dispose();
            }
            return stat;
        }

        public int Delete(string transactionId)
        {
            try
            {
                _transactionRepository.Delete(transactionId);
                _unitOfWork.Commit();
                return 0;
            }
            catch (Exception exception)
            {
                return -1;
            }
        }

        public bool ObjectExists(TransactionHeaderDTO transaction)
        {
            var objectExists = false;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                var catRepository = new Repository<TransactionHeaderDTO>(iDbContext);
                var catExists = catRepository
                    .Query()
                    .Filter(bp => bp.TransactionNumber == transaction.TransactionNumber && bp.Id != transaction.Id)
                    .Get()
                    .FirstOrDefault();
                if (catExists != null)
                    objectExists = true;
            }
            finally
            {
                iDbContext.Dispose();
            }

            return objectExists;
        }

        public string Validate(TransactionHeaderDTO transaction)
        {
            if (null == transaction)
                return GenericMessages.ObjectIsNull;

            if (transaction.WarehouseId == 0)
                return "Warehouse " + GenericMessages.ObjectIsNull;

            //if (String.IsNullOrEmpty(transaction.TransactionNumber))
            //    return transaction.TransactionNumber + " " + GenericMessages.StringIsNullOrEmpty;

            //if (transaction.TransactionNumber.Length > 50)
            //    return transaction.TransactionNumber + " can not be more than 50 characters ";

            return string.Empty;
        }

        #endregion

        #region Private Methods

        public string Post(TransactionHeaderDTO transaction)
        {
            try
            {
                if (transaction.TransactionLines.Count == 0)
                    return "No Items To Post, Add Item First....";

                var validate = Validate(transaction);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (transaction.TransactionType == TransactionTypes.RecieveStock ||
                    transaction.TransactionType == TransactionTypes.SellStock ||
                    transaction.TransactionType == TransactionTypes.UseStock)
                {
                    var iqList = new ItemQuantityService(_iDbContext).UpdateInventoryByTransaction(transaction);
                    foreach (var itemQuantityDto in iqList)
                    {
                        _itemsQuantityRepository.InsertUpdate(itemQuantityDto);
                    }

                    //Update transaction
                    transaction.Status = TransactionStatus.Received;
                    _transactionRepository.InsertUpdate(transaction);

                    _unitOfWork.Commit();
                }
                else if (transaction.TransactionType == TransactionTypes.TransferStock)
                {
                    var stat = TransactionStatus.New;
                    if (transaction.Status == TransactionStatus.New)
                    {
                        stat = TransactionStatus.Requested;
                        transaction.RequestedBy = Singleton.User.FullName;
                        transaction.RequestedDate = DateTime.Now;
                    }
                    else if (transaction.Status == TransactionStatus.Requested)
                    {
                        stat = TransactionStatus.Sent;
                        transaction.SentBy = Singleton.User.FullName;
                        transaction.SentDate = DateTime.Now;
                        var iqList = new ItemQuantityService(_iDbContext).UpdateInventoryByTransaction(transaction);
                        foreach (var itemQuantityDto in iqList)
                        {
                            _itemsQuantityRepository.InsertUpdate(itemQuantityDto);
                        }
                    }
                    else if (transaction.Status == TransactionStatus.Sent)
                    {
                        stat = TransactionStatus.Received;
                        transaction.ReceivedBy = Singleton.User.FullName;
                        transaction.ReceivedDate = DateTime.Now;
                        var iqList = new ItemQuantityService(_iDbContext).UpdateInventoryByTransaction(transaction);
                        foreach (var itemQuantityDto in iqList)
                        {
                            _itemsQuantityRepository.InsertUpdate(itemQuantityDto);
                        }
                    }

                    transaction.Status = stat;
                    _transactionRepository.InsertUpdate(transaction);

                    _unitOfWork.Commit();
                }
                //transaction.Status = TransactionStatus.Posted;
                //_transactionRepository.Update(transaction);

                //_unitOfWork.Commit();

                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string UnPost(TransactionHeaderDTO transaction2)
        {
            if (transaction2 == null)
                return GenericMessages.ObjectIsNull;

            var iDbContextTemp = DbContextUtil.GetDbContextInstance();
            var unitOfWorkTemp = new UnitOfWork(iDbContextTemp);
            var stat = "";
            try
            {
                var transaction = unitOfWorkTemp.Repository<TransactionHeaderDTO>().Query()
                    .Include(t => t.TransactionLines)
                    .Filter(t => t.Id == transaction2.Id)
                    .Get()
                    .FirstOrDefault();

                var itemQtyRepository = unitOfWorkTemp.Repository<ItemQuantityDTO>();
                var transactionRepository2 = unitOfWorkTemp.Repository<TransactionHeaderDTO>();

                if (transaction != null)
                {
                    foreach (var transactionLine in transaction.TransactionLines.Where(t => t.Enabled))
                    {
                        var itemQty = UnPostUpdateInventory(transactionLine, iDbContextTemp);

                        if (itemQty == null)
                        {
                            stat = "Got problem while updating inventory data, please try agan later...";
                            stat = stat + Environment.NewLine + " may be, The Store has less quantity for the item '" +
                                   transactionLine.Item.DisplayName + "'";
                            break;
                        }
                        itemQtyRepository.InsertUpdate(itemQty);

                        if (transactionLine.Transaction.TransactionType == TransactionTypes.TransferStock &&
                            transactionLine.Transaction.Status == TransactionStatus.Received)
                        {
                            if (transactionLine.Transaction.ToWarehouseId != null)
                            {
                                var toWareId = (int)transactionLine.Transaction.ToWarehouseId;
                                var itemQtyTo = new ItemQuantityService(iDbContextTemp).GetByCriteria(toWareId,
                                    transactionLine.ItemId);

                                if (itemQtyTo != null)
                                {
                                    if (itemQtyTo.QuantityOnHand < transactionLine.Unit)
                                        return null;
                                    itemQtyTo.QuantityOnHand = itemQtyTo.QuantityOnHand - transactionLine.Unit;
                                }
                                else
                                {
                                    stat = "Got problem while updating inventory data, please try agan later...";
                                    stat = stat + Environment.NewLine +
                                           " may be, The Store has less quantity for the item '" +
                                           transactionLine.Item.DisplayName + "'";
                                    break;
                                }
                                itemQtyRepository.InsertUpdate(itemQtyTo);
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(stat))
                    {
                        if (transaction.TransactionType == TransactionTypes.SellStock)
                        {
                            transaction.Status = TransactionStatus.New;
                            transaction.Enabled = false;
                            transactionRepository2.Update(transaction);
                        }
                        else
                        {
                            transaction.Status = TransactionStatus.New;
                            transaction.RequestedBy = "";
                            transaction.RequestedDate = null;
                            transaction.SentBy = "";
                            transaction.SentDate = null;
                            transaction.ReceivedBy = "";
                            transaction.ReceivedDate = null;
                            transactionRepository2.Update(transaction);
                        }
                        unitOfWorkTemp.Commit();
                    }
                }

                stat = string.Empty;
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            finally
            {
                iDbContextTemp.Dispose();
            }
            return stat;
        }

        public ItemQuantityDTO UnPostUpdateInventory(TransactionLineDTO line, IDbContext idbDbContext)
        {
            var itemQty = new ItemQuantityService(idbDbContext).GetByCriteria(line.Transaction.WarehouseId, line.ItemId);
            if (itemQty == null)
            {
                MessageBox.Show("Can't get item to do the Un-post, Please try again!!");
                return null;
            }

            switch (line.Transaction.TransactionType)
            {
                case TransactionTypes.SellStock:
                    itemQty.QuantityOnHand = itemQty.QuantityOnHand + line.Unit;
                    break;
                case TransactionTypes.RecieveStock:
                    if (itemQty.QuantityOnHand < line.Unit)
                        return null;
                    itemQty.QuantityOnHand = itemQty.QuantityOnHand - line.Unit;
                    break;
                case TransactionTypes.TransferStock:
                        itemQty.QuantityOnHand = itemQty.QuantityOnHand + line.Unit;
                    break;
            }
            return itemQty;
        }

        public string GetNewTransactionNumber(TransactionHeaderDTO transaction)
        {
            var tranNumber = "";
            try
            {
                //like
            }
            catch (Exception exception)
            {
                tranNumber = "Problem getting transaction number, try again..." + Environment.NewLine +
                             exception.Message;
            }
            return tranNumber;
        }

        #endregion

        #region Child Methods

        public IEnumerable<TransactionLineDTO> GetChilds(int parentId, bool disposeWhenDone)
        {
            IEnumerable<TransactionLineDTO> piList;
            try
            {
                piList = _transactionLineRepository
                    .Query()
                    .Include(a => a.Transaction, a => a.Item)
                    .Get()
                    .OrderBy(i => i.Id)
                    .ToList();

                if (parentId != 0)
                    piList = piList.Where(l => l.TransactionId == parentId).ToList();
            }
            finally
            {
                Dispose(disposeWhenDone);
            }

            return piList;
        }

        public IRepositoryQuery<TransactionLineDTO> GetChildsQuery()
        {
            var piList = _transactionLineRepository
                .Query()
                .Include(i => i.Item, s => s.Transaction, s => s.Transaction.BusinessPartner,
                    s => s.Transaction.Warehouse)
                .OrderBy(q => q.OrderByDescending(c => c.Id));
            return piList;
        }

        public IEnumerable<TransactionLineDTO> GetAllChilds(SearchCriteria<TransactionLineDTO> criteria,
            out int totalCount)
        {
            totalCount = 0;
            IEnumerable<TransactionLineDTO> piList = new List<TransactionLineDTO>();
            try
            {
                if (criteria != null && criteria.CurrentUserId != -1)
                {
                    var warehouseList = new WarehouseService(true)
                        .GetWarehousesPrevilegedToUser(criteria.CurrentUserId).ToList();

                    if (criteria.SelectedWarehouseId != null)
                        warehouseList = warehouseList.Where(w => w.Id == criteria.SelectedWarehouseId).ToList();

                    foreach (var warehouse in warehouseList.Where(w => w.Id != -1))
                    {
                        var pdto = GetChildsQuery();

                        foreach (var cri in criteria.FiList)
                        {
                            pdto.FilterList(cri);
                        }

                        #region By Warehouse

                        var warehouse1 = warehouse;
                        pdto.FilterList(p => p.Transaction.WarehouseId == warehouse1.Id);

                        #endregion

                        #region By Duration

                        if (criteria.BeginingDate != null)
                        {
                            var beginDate = new DateTime(criteria.BeginingDate.Value.Year,
                                criteria.BeginingDate.Value.Month,
                                criteria.BeginingDate.Value.Day, 0, 0, 0);
                            pdto.FilterList(p => p.Transaction.TransactionDate >= beginDate);
                        }

                        if (criteria.EndingDate != null)
                        {
                            var endDate = new DateTime(criteria.EndingDate.Value.Year, criteria.EndingDate.Value.Month,
                                criteria.EndingDate.Value.Day, 23, 59, 59);
                            pdto.FilterList(p => p.Transaction.TransactionDate <= endDate);
                        }

                        #endregion

                        IList<TransactionLineDTO> pdtoList;
                        if (criteria.Page != 0 && criteria.PageSize != 0)
                        {
                            int totalCount2;
                            pdtoList = pdto.GetPage(criteria.Page, criteria.PageSize, out totalCount2).ToList();
                            totalCount = totalCount2;
                        }
                        else
                            pdtoList = pdto.GetList().ToList();

                        piList = piList.Concat(pdtoList).ToList();
                    }
                }
                else
                    piList = GetChildsQuery().Get().ToList();
            }
            finally
            {
                Dispose(_disposeWhenDone);
            }

            return piList;
        }

        public string InsertOrUpdateChild(TransactionLineDTO transactionLine)
        {
            try
            {
                var validate = ValidateChild(transactionLine);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExistsChild(transactionLine))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists;

                _transactionLineRepository.InsertUpdate(transactionLine);


                _unitOfWork.Commit();


                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string DisableChild(TransactionLineDTO transactionLine)
        {
            if (transactionLine == null || transactionLine.Id == 0 || transactionLine.Transaction == null)
            {
                return "First choose Item to delete...";
            }

            string stat;
            try
            {
                _transactionLineRepository.Update(transactionLine);


                _unitOfWork.Commit();
                stat = string.Empty;
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            return stat;
        }

        public bool ObjectExistsChild(TransactionLineDTO transactionLine)
        {
            var objectExists = false;
            //var iDbContext = DbContextUtil.GetDbContextInstance();
            //try
            //{
            //    var catRepository = new Repository<TransactionLineDTO>(iDbContext);
            //    var catExists = catRepository.GetAll()
            //        .FirstOrDefault(bp => bp.TransactionId == transactionLine.TransactionId && bp.ItemId == transactionLine.ItemId && bp.Id != transactionLine.Id);
            //    if (catExists != null)
            //        objectExists = true;
            //}
            //finally
            //{
            //    iDbContext.Dispose();
            //}

            return objectExists;
        }

        public string ValidateChild(TransactionLineDTO transactionLine)
        {
            if (null == transactionLine)
                return GenericMessages.ObjectIsNull;

            if (transactionLine.Transaction == null)
                return "Transaction " + GenericMessages.ObjectIsNull;

            if (transactionLine.ItemId == 0)
                return "No item is found in the physical inventory line";

            if (transactionLine.Unit <= 0)
                return transactionLine.Unit + " can not be less than 0 ";

            return string.Empty;
        }

        #endregion

        #region Disposing

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unitOfWork.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}