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
    public class MemberService : IMemberService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<MemberDTO> _memberRepository;
        private IRepository<MemberSubscriptionDTO> _memberSubscriptionRepository;
        private readonly bool _disposeWhenDone;
        private IDbContext _iDbContext;
        #endregion

        #region Constructor
        public MemberService()
        {
            InitializeDbContext();
        }

        public MemberService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            _iDbContext = DbContextUtil.GetDbContextInstance();
            _memberRepository = new Repository<MemberDTO>(_iDbContext);
            _memberSubscriptionRepository = new Repository<MemberSubscriptionDTO>(_iDbContext);
            _unitOfWork = new UnitOfWork(_iDbContext);
        }

        #endregion

        #region Common Methods
        public IRepositoryQuery<MemberDTO> GetShort()
        {
            var piList = _memberRepository
                .Query()
                .Include(a => a.Address)//, a => a.LastSubscription)
                .Filter(a => !string.IsNullOrEmpty(a.DisplayName))
                .OrderBy(a => a.OrderByDescending(m => m.Id));
            return piList;
        }

        public IRepositoryQuery<MemberDTO> Get()
        {
            var piList = _memberRepository
                .Query()
                .Include(a => a.Address, a => a.ContactPerson, a => a.ContactPerson.Address,
                            a => a.LastSubscription, a => a.LastSubscription.FacilitySubscription,
                            a => a.LastSubscription.FacilitySubscription.Subscription,
                            a => a.LastSubscription.FacilitySubscription.Facility,
                            a => a.LastSubscription.Attendances, a => a.Subscriptions)
                .Include(a => a.CreatedByUser, a => a.ModifiedByUser)
                .Include(a => a.Assessments, a => a.MemberAssessmentMore)
                .Filter(a => !string.IsNullOrEmpty(a.DisplayName))
                .OrderBy(a=>a.OrderByDescending(m=>m.Id));
            return piList;
        }

        public IEnumerable<MemberDTO> GetAll(SearchCriteria<MemberDTO> criteria = null)
        {
            int totalCount;
            return GetAll(criteria, out totalCount);
        }

        public IEnumerable<MemberDTO> GetAll(SearchCriteria<MemberDTO> criteria, out int totalCount)//, 
        {
            totalCount = 0;
            IEnumerable<MemberDTO> piList = new List<MemberDTO>();
            try
            {
                if (criteria != null)
                {

                    var pdto = GetShort();
                    if (!criteria.ShortForm)
                        pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }


                    IList<MemberDTO> pdtoList;
                    if (criteria.Page != 0 && criteria.PageSize != 0)
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
                    piList = Get().Get().ToList();//.OrderBy(i => i.Id)

                #region For Eager Loading Childs
                ////foreach (var transactionHeaderDTO in piList)
                ////{
                //   var subscriptioneDtos = _memberSubscriptionRepository
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

        public MemberDTO Find(string memberId)
        {
            var bpId = Convert.ToInt32(memberId);
            var bpDto = Get().Filter(b => b.Id == bpId).Get().FirstOrDefault();
            if (_disposeWhenDone)
                Dispose();
            return bpDto;
        }

        public MemberDTO GetByName(string displayName)
        {
            var bp = Get()
                .Filter(b => b.DisplayName == displayName)
                .Get()
                .FirstOrDefault();
            return bp;
        }

        public string InsertOrUpdate(MemberDTO member)
        {
            try
            {
                var validate = Validate(member);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(member))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists + Environment.NewLine +
                           "With the same name exists";

                _memberRepository.InsertUpdate(member);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(MemberDTO member)
        {
            //if (_unitOfWork.Repository<DeliveryHeaderDTO>().Query().Get().Any(i => i.MemberId == Member.Id) ||
            //    _unitOfWork.Repository<DocumentDTO>().Query().Get().Any(i => i.MemberId == Member.Id))
            //{
            //    return "Can't delete the item, it is already in use...";
            //}

            if (member == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            try
            {
                _memberRepository.Update(member);
                _unitOfWork.Commit();
                stat = string.Empty;
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            return stat;
        }

        public int Delete(string memberId)
        {
            try
            {
                _memberRepository.Delete(memberId);
                _unitOfWork.Commit();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public bool ObjectExists(MemberDTO member)
        {
            var objectExists = false;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                var memRepository = new Repository<MemberDTO>(iDbContext);
                var catExists = memRepository
                    .Query()
                    .Include(a=>a.Address)
                    .Filter(bp => (bp.Number == member.Number || bp.Address.Mobile==member.Address.Mobile) && bp.Id != member.Id)
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

        public string Validate(MemberDTO member)
        {
            if (null == member)
                return GenericMessages.ObjectIsNull;

            if (member.Address == null)
                return "Address " + GenericMessages.ObjectIsNull;

            if (String.IsNullOrEmpty(member.DisplayName))
                return member.DisplayName + " " + GenericMessages.StringIsNullOrEmpty;

            if (member.DisplayName.Length > 255)
                return member.DisplayName + " can not be more than 255 characters ";

            if (!string.IsNullOrEmpty(member.Number) && member.Number.Length > 50)
                return member.Number + " can not be more than 50 characters ";


            return string.Empty;
        }

        #endregion

        public IRepositoryQuery<MemberDTO> GetQuerable()
        {
            //var mem = _memberRepository.SqlQuery(
            //        "select m.Id as memberId,s.Id,Age,Sex,StartDate,EndDate,AmountPaid,Amount,f.DisplayName,f.Id as facilityId,sb.DisplayName,sb.Id as subscriptionid"+ 
            //        "from Members as m inner join MemberSubscriptions as s on m.LastSubscriptionId=s.Id"+
            //        "inner join FacilitySubscriptions as fs on s.FacilitySubscriptionId=fs.Id"+
            //        "inner join Facilities as f on f.Id=fs.FacilityId"+ 
            //        "inner join Subscriptions as sb on sb.Id=fs.SubscriptionId");
            return null;
        }

        #region Private Methods
        public string GetMemberNumber(int memberId)
        {
            var pref = memberId.ToString();
            if (memberId < 1000)
            {
                var id = memberId + 10000;
                pref = id.ToString();
                pref = pref.Substring(1);
            }
            var amhCalender = ReportUtility.GetEthCalendar(DateTime.Now, false);
            return "DF/" + pref + "/" + amhCalender.Substring(6);
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