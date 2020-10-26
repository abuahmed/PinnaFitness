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
    public class MemberAttendanceService : IMemberAttendanceService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<MemberAttendanceDTO> _facilitySubscriptionRepository;
        private IRepository<ServiceDTO> _serviceRepository;
        private readonly bool _disposeWhenDone;
        private IDbContext _iDbContext;
        #endregion

        #region Constructor
        public MemberAttendanceService()
        {
            InitializeDbContext();
        }
        public MemberAttendanceService(IDbContext iDbContext)
        {
            InitializeDbContext(iDbContext);
        }
        public MemberAttendanceService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext(IDbContext iDbContext = null)
        {
            _iDbContext = iDbContext;
            if (iDbContext == null)
                _iDbContext = DbContextUtil.GetDbContextInstance();
            _facilitySubscriptionRepository = new Repository<MemberAttendanceDTO>(_iDbContext);
            _serviceRepository = new Repository<ServiceDTO>(_iDbContext);
            _unitOfWork = new UnitOfWork(_iDbContext);
        }

        #endregion

        #region Common Methods
        public IRepositoryQuery<MemberAttendanceDTO> Get()
        {
            var piList = _facilitySubscriptionRepository
                .Query()
                .Include(a => a.MemberSubscription, a => a.MemberSubscription.Member, a => a.MemberSubscription.Member.Address,
                         a => a.MemberSubscription.FacilitySubscription, a => a.Services, a => a.MemberSubscription.Member.Assessments,
                         a => a.MemberSubscription.FacilitySubscription.Facility, a => a.MemberSubscription.FacilitySubscription.Facility.Services,
                         a => a.MemberSubscription.FacilitySubscription.Subscription, a => a.MemberSubscription.Attendances)
                .OrderBy(q => q.OrderByDescending(c => c.CheckedInTime));
            return piList;
        }

        public IEnumerable<MemberAttendanceDTO> GetAll(SearchCriteria<MemberAttendanceDTO> criteria = null)
        {
            int totalCount = 0;
            return this.GetAll(criteria, out totalCount);
        }

        public IEnumerable<MemberAttendanceDTO> GetAll(SearchCriteria<MemberAttendanceDTO> criteria, out int totalCount)//, 
        {
            totalCount = 0;
            IEnumerable<MemberAttendanceDTO> piList = new List<MemberAttendanceDTO>();
            try
            {
                if (criteria != null)
                {
                    var pdto = Get();
                    if (criteria.IncludePhoto)
                        pdto = pdto.Include(a => a.MemberSubscription.Member.Photo)
                            .Include(a => a.CreatedByUser, a => a.ModifiedByUser);

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }
                    #region By Duration

                    if (criteria.BeginingDate != null)
                    {
                        var beginDate = new DateTime(criteria.BeginingDate.Value.Year, criteria.BeginingDate.Value.Month,
                            criteria.BeginingDate.Value.Day, 0, 0, 0);
                        if(criteria.Shift==ShiftTypes.Afternoon)
                            beginDate = new DateTime(criteria.BeginingDate.Value.Year, criteria.BeginingDate.Value.Month,
                            criteria.BeginingDate.Value.Day, 14, 0, 0);
                        pdto.FilterList(p => p.CheckedInTime >= beginDate);
                    }

                    if (criteria.EndingDate != null)
                    {
                        var endDate = new DateTime(criteria.EndingDate.Value.Year, criteria.EndingDate.Value.Month,
                            criteria.EndingDate.Value.Day, 23, 59, 59);
                        if (criteria.Shift == ShiftTypes.Morning)
                            endDate = new DateTime(criteria.EndingDate.Value.Year, criteria.EndingDate.Value.Month,
                            criteria.EndingDate.Value.Day, 13, 59, 59);
                        pdto.FilterList(p => p.CheckedInTime <= endDate);
                    }

                    #endregion

                    IList<MemberAttendanceDTO> pdtoList;
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
                    piList = Get().Get().OrderBy(i => i.Id).ToList();

                _serviceRepository.Query().Include(t => t.TimeTable,t=>t.Trainers,t=>t.Attendances,t=>t.Facilities).Get().ToList();

            }
            finally
            {
                Dispose(_disposeWhenDone);
            }
            return piList;
        }

        public IEnumerable<ServiceDTO> GetAllServices()
        {
            return _iDbContext.Set<ServiceDTO>().Include("TimeTable").Include("Facilities").Include("Trainers").Include("Attendances").ToList();
        }

        public MemberAttendanceDTO Find(string facilitySubscriptionId)
        {
            var bpId = Convert.ToInt32(facilitySubscriptionId);
            var bpDto = Get().Filter(b => b.Id == bpId).Get().FirstOrDefault();
            if (_disposeWhenDone)
                Dispose();
            return bpDto;
        }

        public MemberAttendanceDTO GetByName(string displayName)
        {
            var bp = Get()
                //.Filter(b => b.PlateNumber == displayName)
                .Get()
                .FirstOrDefault();
            return bp;
        }

        public string InsertOrUpdate(MemberAttendanceDTO facilitySubscription)
        {
            try
            {
                var validate = Validate(facilitySubscription);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(facilitySubscription))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists + Environment.NewLine +
                           "With the same Name Exists";

                _facilitySubscriptionRepository.InsertUpdate(facilitySubscription);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(MemberAttendanceDTO facilitySubscription)
        {
            if (facilitySubscription == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            try
            {
                _facilitySubscriptionRepository.Update(facilitySubscription);
                _unitOfWork.Commit();
                stat = string.Empty;
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            return stat;
        }

        public int Delete(string facilitySubscriptionId)
        {
            try
            {
                _facilitySubscriptionRepository.Delete(facilitySubscriptionId);
                _unitOfWork.Commit();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public bool ObjectExists(MemberAttendanceDTO memberAttendance)
        {
            var objectExists = false;
            //var iDbContext = DbContextUtil.GetDbContextInstance();
            //try
            //{
            //    var catRepository = new Repository<MemberAttendanceDTO>(iDbContext);
            //    var catExists = catRepository.Query()
            //        .Filter(bp => (bp.MemberSubscriptionId == memberAttendance.MemberSubscriptionId &&
            //            bp.CheckedInTime.Date == memberAttendance.CheckedInTime.Date)
            //            && bp.Id != memberAttendance.Id)
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

        public string Validate(MemberAttendanceDTO facilitySubscription)
        {
            if (null == facilitySubscription)
                return GenericMessages.ObjectIsNull;

            //if (String.IsNullOrEmpty(facilitySubscription.PlateNumber))
            //    return facilitySubscription.PlateNumber + " " + GenericMessages.StringIsNullOrEmpty;

            return string.Empty;
        }

        #endregion

        #region Private Methods
        public string GetMemberAttendanceCode()
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