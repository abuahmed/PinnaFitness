using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
    public class MemberSubscriptionService : IMemberSubscriptionService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<MemberSubscriptionDTO> _memberSubscriptionRepository;
        private IRepository<MemberDTO> _memberRepository;
        private readonly bool _disposeWhenDone;
        private IDbContext _iDbContext;
        #endregion

        #region Constructor
        public MemberSubscriptionService()
        {
            InitializeDbContext();
        }

        public MemberSubscriptionService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }
        public MemberSubscriptionService(IDbContext iDbContext)
        {
            InitializeDbContext(iDbContext);
        }
        public void InitializeDbContext(IDbContext iDbContext = null)
        {
            _iDbContext = iDbContext;
            if (iDbContext == null)
                _iDbContext = DbContextUtil.GetDbContextInstance();
            _memberSubscriptionRepository = new Repository<MemberSubscriptionDTO>(_iDbContext);
            _memberRepository = new Repository<MemberDTO>(_iDbContext);
            _unitOfWork = new UnitOfWork(_iDbContext);
        }
        #endregion

        #region Common Methods
        public IRepositoryQuery<MemberSubscriptionDTO> Get()
        {
            var piList = _memberSubscriptionRepository
                .Query()
                .Include(a => a.FacilitySubscription,
                                a => a.FacilitySubscription.Subscription,
                                a => a.FacilitySubscription.Facility, a => a.FacilitySubscription.Facility.Services,
                                a => a.Member, a => a.Member.Address, a => a.Attendances)
                .Include(a => a.CreatedByUser, a => a.ModifiedByUser)
                .OrderBy(q => q.OrderBy(c => c.Member.DisplayName))
                .OrderBy(q => q.OrderByDescending(c => c.SubscribedDate));
            return piList;
        }

        public IEnumerable<MemberSubscriptionDTO> GetAll(SearchCriteria<MemberSubscriptionDTO> criteria = null)
        {
            int totalCount = 0;
            return this.GetAll(criteria, out totalCount);
        }

        public IEnumerable<MemberSubscriptionDTO> GetAll(SearchCriteria<MemberSubscriptionDTO> criteria, out int totalCount)//, 
        {
            totalCount = 0;
            IEnumerable<MemberSubscriptionDTO> piList = new List<MemberSubscriptionDTO>();
            try
            {
                if (criteria != null)
                {
                    var pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }
                    #region By Duration

                    if (criteria.BeginingDate != null)
                    {
                        var beginDate = new DateTime(criteria.BeginingDate.Value.Year, criteria.BeginingDate.Value.Month,
                            criteria.BeginingDate.Value.Day, 0, 0, 0);
                        if (criteria.Shift == ShiftTypes.Afternoon)
                            beginDate = new DateTime(criteria.BeginingDate.Value.Year, criteria.BeginingDate.Value.Month,
                            criteria.BeginingDate.Value.Day, 14, 0, 0);
                        pdto.FilterList(p => p.SubscribedDate >= beginDate);
                    }

                    if (criteria.EndingDate != null)
                    {
                        var endDate = new DateTime(criteria.EndingDate.Value.Year, criteria.EndingDate.Value.Month,
                            criteria.EndingDate.Value.Day, 23, 59, 59);
                        if (criteria.Shift == ShiftTypes.Morning)
                            endDate = new DateTime(criteria.EndingDate.Value.Year, criteria.EndingDate.Value.Month,
                            criteria.EndingDate.Value.Day, 13, 59, 59);
                        pdto.FilterList(p => p.SubscribedDate <= endDate);
                    }

                    #endregion
                    IList<MemberSubscriptionDTO> pdtoList;
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

        public MemberSubscriptionDTO Find(string memberSubscriptionId)
        {
            var bpId = Convert.ToInt32(memberSubscriptionId);
            var bpDto = Get().Filter(b => b.Id == bpId).Get().FirstOrDefault();
            if (_disposeWhenDone)
                Dispose();
            return bpDto;
        }

        public MemberSubscriptionDTO GetByName(string displayName)
        {
            var bp = Get()
                //.Filter(b => b.PlateNumber == displayName)
                .Get()
                .FirstOrDefault();
            return bp;
        }

        public string InsertOrUpdate(MemberSubscriptionDTO memberSubscription)
        {
            try
            {
                var validate = Validate(memberSubscription);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(memberSubscription))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists + Environment.NewLine +
                           " with the same name exists";

                //if (memberSubscription.Id == 0)//ONLY THE LAST SUBSCRIPTION CAN BE ADDED/UPDATED
                //{
                var memDto = _memberRepository.FindById(memberSubscription.MemberId);
                memDto.LastSubscription = memberSubscription;
                _memberRepository.InsertUpdate(memDto);
                //}

                //if (memberSubscription.PreviousSuscrptionId != null)
                //{
                //     var prevSub = _memberSubscriptionRepository.FindById(memberSubscription.PreviousSuscrptionId);
                //   //prevSub
                //    _memberSubscriptionRepository.InsertUpdate(prevSub);
                //}
                 

                _memberSubscriptionRepository.InsertUpdate(memberSubscription);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(MemberSubscriptionDTO memberSubscription)
        {
            if (memberSubscription == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            try
            {
                _memberSubscriptionRepository.Update(memberSubscription);
                _unitOfWork.Commit();
                stat = string.Empty;
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            return stat;
        }

        public int Delete(string memberSubscriptionId)
        {
            try
            {
                _memberSubscriptionRepository.Delete(Convert.ToInt32(memberSubscriptionId));
                _unitOfWork.Commit();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public bool ObjectExists(MemberSubscriptionDTO memberSubscription)
        {
            var objectExists = false;
            //var iDbContext = DbContextUtil.GetDbContextInstance();
            //try
            //{
            //    var catRepository = new Repository<MemberSubscriptionDTO>(iDbContext);
            //    var catExists = catRepository.Query()
            //        .Filter(bp => (bp.Id == memberSubscription.Id &&
            //            bp.MemberId == memberSubscription.MemberId)
            //            && bp.Id != memberSubscription.Id)
            //        .Get()
            //        .FirstOrDefault();

            //    if (catExists != null)
            //        objectExists = true;
            //}
            //finally
            //{
            //    iDbContext.Dispose();
            //}

            return objectExists;
        }

        public string Validate(MemberSubscriptionDTO memberSubscription)
        {
            if (null == memberSubscription)
                return GenericMessages.ObjectIsNull;

            //if (String.IsNullOrEmpty(memberSubscription.PlateNumber))
            //    return memberSubscription.PlateNumber + " " + GenericMessages.StringIsNullOrEmpty;

            return string.Empty;
        }

        #endregion

        #region Private Methods
        public string GetMemberSubscriptionCode()
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