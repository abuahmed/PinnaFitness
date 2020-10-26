using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class TimeTableService : ITimeTableService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<TimeTableDTO> _facilitySubscriptionRepository;
        private readonly bool _disposeWhenDone;
        private IDbContext _iDbContext;
        #endregion

        #region Constructor
        public TimeTableService()
        {
            InitializeDbContext();
        }

        public TimeTableService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            _iDbContext = DbContextUtil.GetDbContextInstance();
            _facilitySubscriptionRepository = new Repository<TimeTableDTO>(_iDbContext);
            _unitOfWork = new UnitOfWork(_iDbContext);
        }

        #endregion

        #region Common Methods
        public IRepositoryQuery<TimeTableDTO> Get()
        {
            var piList = _facilitySubscriptionRepository
                .Query()
                .Include(a => a.Service, a => a.Trainers)
                .Include(a => a.CreatedByUser, a => a.ModifiedByUser)
                .OrderBy(q => q.OrderBy(c => c.Service.DisplayName));
            return piList;
        }

        public IEnumerable<TimeTableDTO> GetAll(SearchCriteria<TimeTableDTO> criteria = null)
        {
            int totalCount = 0;
            return this.GetAll(criteria, out totalCount);
        }

        public IEnumerable<TimeTableDTO> GetAll(SearchCriteria<TimeTableDTO> criteria, out int totalCount)//, 
        {
            totalCount = 0;
            IEnumerable<TimeTableDTO> piList = new List<TimeTableDTO>();
            try
            {
                if (criteria != null)
                {
                    var pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }
                    
                    IList<TimeTableDTO> pdtoList;
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

        public IEnumerable<TrainerDTO> GetAllTrainers()
        {
            return _iDbContext.Set<TrainerDTO>().Include("TimeTable").Include("Courses").ToList();
        }

        public IEnumerable<TrainerCourseDTO> GetTrainerServices()
        {
            return _iDbContext.Set<TrainerCourseDTO>().Include("Trainer").Include("Service").ToList();
        }

        public TimeTableDTO Find(string facilitySubscriptionId)
        {
            var bpId = Convert.ToInt32(facilitySubscriptionId);
            var bpDto = Get().Filter(b => b.Id == bpId).Get().FirstOrDefault();
            if (_disposeWhenDone)
                Dispose();
            return bpDto;
        }

        public TimeTableDTO GetByName(string displayName)
        {
            var bp = Get()
                //.Filter(b => b.PlateNumber == displayName)
                .Get()
                .FirstOrDefault();
            return bp;
        }

        public string InsertOrUpdate(TimeTableDTO facilitySubscription)
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

        public string Disable(TimeTableDTO facilitySubscription)
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

        public bool ObjectExists(TimeTableDTO facilitySubscription)
        {
            var objectExists = false;
            //var iDbContext = DbContextUtil.GetDbContextInstance();
            //try
            //{
            //    var catRepository = new Repository<TimeTableDTO>(iDbContext);
            //    var catExists = catRepository.Query()
            //        .Filter(bp => (bp.SubscriptionId == facilitySubscription.SubscriptionId &&
            //            bp.FacilityId == facilitySubscription.FacilityId)
            //            && bp.Id != facilitySubscription.Id)
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

        public string Validate(TimeTableDTO facilitySubscription)
        {
            if (null == facilitySubscription)
                return GenericMessages.ObjectIsNull;

            //if (String.IsNullOrEmpty(facilitySubscription.PlateNumber))
            //    return facilitySubscription.PlateNumber + " " + GenericMessages.StringIsNullOrEmpty;

            return string.Empty;
        }

        #endregion

        #region Private Methods
        public string GetTimeTableCode()
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