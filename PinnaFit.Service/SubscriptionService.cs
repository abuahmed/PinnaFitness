using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using PinnaFit.Core;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Models;
using PinnaFit.DAL;
using PinnaFit.DAL.Interfaces;
using PinnaFit.Repository;
using PinnaFit.Repository.Interfaces;
using PinnaFit.Service.Interfaces;

namespace PinnaFit.Service
{
    public class SubscriptionService : ISubscriptionService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<SubscriptionDTO> _subscriptionRepository;
        private readonly bool _disposeWhenDone;
        private IDbContext _iDbContext;
        #endregion

        #region Constructor
        public SubscriptionService()
        {
            InitializeDbContext();
        }

        public SubscriptionService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            _iDbContext = DbContextUtil.GetDbContextInstance();
            _subscriptionRepository = new Repository<SubscriptionDTO>(_iDbContext);
            _unitOfWork = new UnitOfWork(_iDbContext);
        }

        #endregion

        #region Common Methods
        public IRepositoryQuery<SubscriptionDTO> Get()
        {
            var piList = _subscriptionRepository
                .Query()
                .Include(a => a.CreatedByUser, a => a.ModifiedByUser)
                //.Include(a => a.AssignedDriver)
                //.Filter(a => !string.IsNullOrEmpty(a.PlateNumber))
                .OrderBy(q => q.OrderBy(c => c.Id));
            return piList;
        }

        public IEnumerable<SubscriptionDTO> GetAll(SearchCriteria<SubscriptionDTO> criteria = null)
        {
            int totalCount = 0;
            return this.GetAll(criteria, out totalCount);
        }

        public IEnumerable<SubscriptionDTO> GetAll(SearchCriteria<SubscriptionDTO> criteria, out int totalCount)//, 
        {
            totalCount = 0;
            IEnumerable<SubscriptionDTO> piList = new List<SubscriptionDTO>();
            try
            {
                if (criteria != null)
                {
                    var pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }


                    IList<SubscriptionDTO> pdtoList;
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

                    piList = pdtoList.ToList();
                }
                else
                    piList = Get().Get().OrderBy(i => i.Id).ToList();
                
            }
            finally
            {
                Dispose(_disposeWhenDone);
            }
            return piList;
        }

        public SubscriptionDTO Find(string subscriptionId)
        {
            var bpId = Convert.ToInt32(subscriptionId);
            var bpDto = Get().Filter(b => b.Id == bpId).Get().FirstOrDefault();
            if (_disposeWhenDone)
                Dispose();
            return bpDto;
        }

        public SubscriptionDTO GetByName(string displayName)
        {
            var bp = Get()
                //.Filter(b => b.PlateNumber == displayName)
                .Get()
                .FirstOrDefault();
            return bp;
        }

        public string InsertOrUpdate(SubscriptionDTO subscription)
        {
            try
            {
                var validate = Validate(subscription);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(subscription))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists + Environment.NewLine +
                           "With the same Name Exists";

                _subscriptionRepository.InsertUpdate(subscription);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(SubscriptionDTO subscription)
        {
            if (subscription == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            try
            {
                _subscriptionRepository.Update(subscription);
                _unitOfWork.Commit();
                stat = string.Empty;
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            return stat;
        }

        public int Delete(string subscriptionId)
        {
            try
            {
                _subscriptionRepository.Delete(subscriptionId);
                _unitOfWork.Commit();
                return 0;
            }
            catch 
            {
                return -1;
            }
        }

        public bool ObjectExists(SubscriptionDTO subscription)
        {
            var objectExists = false;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                var catRepository = new Repository<SubscriptionDTO>(iDbContext);
                var catExists = catRepository.Query()
                    .Filter(bp => (bp.DisplayName == subscription.DisplayName) && bp.Id != subscription.Id)
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

        public string Validate(SubscriptionDTO subscription)
        {
            if (null == subscription)
                return GenericMessages.ObjectIsNull;
            
            //if (String.IsNullOrEmpty(subscription.PlateNumber))
            //    return subscription.PlateNumber + " " + GenericMessages.StringIsNullOrEmpty;
            
            return string.Empty;
        }

        #endregion
        
        #region Private Methods
        public string GetSubscriptionCode()
        {
            const string prefix = "V"; 
            var bpCode = prefix + "0001";

            try
            {
                var bpDto = Get().Get(1)
               .OrderByDescending(d => d.Id)
               .FirstOrDefault();

                if (bpDto != null)
                {
                    var code = 10000 + bpDto
                        .Id + 1;
                    bpCode = prefix + code.ToString(CultureInfo.InvariantCulture).Substring(1);
                }
            }
            catch
            {
                return "";
            }

            return bpCode;
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