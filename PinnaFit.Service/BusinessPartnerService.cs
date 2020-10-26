using System;
using System.Collections.Generic;
using System.Linq;
using PinnaFit.Core;
using PinnaFit.Core.Models;
using PinnaFit.DAL;
using PinnaFit.DAL.Interfaces;
using PinnaFit.Repository;
using PinnaFit.Repository.Interfaces;
using PinnaFit.Service.Interfaces;

namespace PinnaFit.Service
{
    public class BusinessPartnerService : IBusinessPartnerService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<BusinessPartnerDTO> _businessPartnerRepository;
        private readonly bool _disposeWhenDone;
        private IDbContext _iDbContext;
        #endregion

        #region Constructor
        public BusinessPartnerService()
        {
            InitializeDbContext();
        }

        public BusinessPartnerService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            _iDbContext = DbContextUtil.GetDbContextInstance();
            _businessPartnerRepository = new Repository<BusinessPartnerDTO>(_iDbContext);
            //_businessPartnerSubscriptionRepository = new Repository<BusinessPartnerSubscriptionDTO>(_iDbContext);
            _unitOfWork = new UnitOfWork(_iDbContext);
        }

        #endregion

        #region Common Methods
        public IRepositoryQuery<BusinessPartnerDTO> Get()
        {
            var piList = _businessPartnerRepository
                .Query()
                .Include(a => a.Address)
                .Include(a => a.CreatedByUser, a => a.ModifiedByUser)
                .Filter(a => !string.IsNullOrEmpty(a.DisplayName))
                .OrderBy(q => q.OrderBy(c => c.DisplayName).ThenBy(c => c.Number));
            return piList;
        }

        public IEnumerable<BusinessPartnerDTO> GetAll(SearchCriteria<BusinessPartnerDTO> criteria = null)
        {
            int totalCount = 0;
            return this.GetAll(criteria, out totalCount);
        }

        public IEnumerable<BusinessPartnerDTO> GetAll(SearchCriteria<BusinessPartnerDTO> criteria, out int totalCount)//, 
        {
            totalCount = 0;
            IEnumerable<BusinessPartnerDTO> piList = new List<BusinessPartnerDTO>();
            try
            {
                if (criteria != null)
                {
                    var pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }


                    IList<BusinessPartnerDTO> pdtoList;
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

                #region For Eager Loading Childs
                ////foreach (var transactionHeaderDTO in piList)
                ////{
                //   var subscriptioneDtos = _businessPartnerSubscriptionRepository
                //        .Query()
                //        .Include(a => a.FacilitySubscription, a => a.FacilitySubscription.Subscription,
                //            a => a.FacilitySubscription.Facility)
                //        .Get()
                //        .OrderBy(i => i.Id)
                //        .ToList();
                //    //var transactionLineDtos =
                //    //(ICollection<TransactionLineDTO>)GetChilds(transactionHeaderDTO.Id, false);
                ////}
                #endregion

            }
            finally
            {
                Dispose(_disposeWhenDone);
            }
            return piList;
        }

        public BusinessPartnerDTO Find(string businessPartnerId)
        {
            var bpId = Convert.ToInt32(businessPartnerId);
            var bpDto = Get().Filter(b => b.Id == bpId).Get().FirstOrDefault();
            if (_disposeWhenDone)
                Dispose();
            return bpDto;
        }

        public BusinessPartnerDTO GetByName(string displayName)
        {
            var bp = Get()
                .Filter(b => b.DisplayName == displayName)
                .Get()
                .FirstOrDefault();
            return bp;
        }

        public string InsertOrUpdate(BusinessPartnerDTO businessPartner)
        {
            try
            {
                var validate = Validate(businessPartner);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(businessPartner))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists + Environment.NewLine +
                           "With the same name exists";

                _businessPartnerRepository.InsertUpdate(businessPartner);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(BusinessPartnerDTO businessPartner)
        {
            //if (_unitOfWork.Repository<DeliveryHeaderDTO>().Query().Get().Any(i => i.BusinessPartnerId == BusinessPartner.Id) ||
            //    _unitOfWork.Repository<DocumentDTO>().Query().Get().Any(i => i.BusinessPartnerId == BusinessPartner.Id))
            //{
            //    return "Can't delete the item, it is already in use...";
            //}

            if (businessPartner == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            try
            {
                _businessPartnerRepository.Update(businessPartner);
                _unitOfWork.Commit();
                stat = string.Empty;
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            return stat;
        }

        public int Delete(string businessPartnerId)
        {
            try
            {
                _businessPartnerRepository.Delete(businessPartnerId);
                _unitOfWork.Commit();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public bool ObjectExists(BusinessPartnerDTO BusinessPartner)
        {
            var objectExists = false;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                var catRepository = new Repository<BusinessPartnerDTO>(iDbContext);
                var catExists = catRepository.Query()
                    .Filter(bp => (bp.DisplayName == BusinessPartner.DisplayName) &&
                                  bp.Id != BusinessPartner.Id)
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

        public string Validate(BusinessPartnerDTO BusinessPartner)
        {
            if (null == BusinessPartner)
                return GenericMessages.ObjectIsNull;

            if (BusinessPartner.Address == null)
                return "Address " + GenericMessages.ObjectIsNull;

            if (String.IsNullOrEmpty(BusinessPartner.DisplayName))
                return BusinessPartner.DisplayName + " " + GenericMessages.StringIsNullOrEmpty;

            if (BusinessPartner.DisplayName.Length > 255)
                return BusinessPartner.DisplayName + " can not be more than 255 characters ";

            if (!string.IsNullOrEmpty(BusinessPartner.Number) && BusinessPartner.Number.Length > 50)
                return BusinessPartner.Number + " can not be more than 50 characters ";


            return string.Empty;
        }

        #endregion

        #region Private Methods
        public string GetBusinessPartnerNumber(int businessPartnerId)
        {
            var pref = businessPartnerId.ToString();
            if (businessPartnerId < 1000)
            {
                var id = businessPartnerId + 10000;
                pref = id.ToString();
                pref = pref.Substring(1);
            }
            var amhCalender = ReportUtility.GetEthCalendar(DateTime.Now, false);
            return "DFT/" + pref + "/" + amhCalender.Substring(6);
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