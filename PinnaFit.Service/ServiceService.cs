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
    public class ServiceService : IServiceService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<ServiceDTO> _serviceRepository;
        private readonly bool _disposeWhenDone;
        private IDbContext _iDbContext;
        #endregion

        #region Constructor
        public ServiceService()
        {
            InitializeDbContext();
        }

        public ServiceService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            _iDbContext = DbContextUtil.GetDbContextInstance();
            _serviceRepository = new Repository<ServiceDTO>(_iDbContext);
            _unitOfWork = new UnitOfWork(_iDbContext);
        }

        #endregion

        #region Common Methods
        public IRepositoryQuery<ServiceDTO> Get()
        {
            var piList = _serviceRepository
                .Query()
                .Include(a => a.Trainers).Include(a => a.CreatedByUser, a => a.ModifiedByUser)
                //.Filter(a => !string.IsNullOrEmpty(a.PlateNumber))
                .OrderBy(q => q.OrderBy(c => c.Id));
            return piList;
        }

        public IEnumerable<ServiceDTO> GetAll(SearchCriteria<ServiceDTO> criteria = null)
        {
            int totalCount = 0;
            return this.GetAll(criteria, out totalCount);
        }

        public IEnumerable<ServiceDTO> GetAll(SearchCriteria<ServiceDTO> criteria, out int totalCount)//, 
        {
            totalCount = 0;
            IEnumerable<ServiceDTO> piList = new List<ServiceDTO>();
            try
            {
                if (criteria != null)
                {
                    var pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }


                    IList<ServiceDTO> pdtoList;
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
           
            return _iDbContext.Set<TrainerDTO>().Include("Courses").ToList();
        }

        public IEnumerable<TrainerCourseDTO> GetTrainerServices()
        {
            return _iDbContext.Set<TrainerCourseDTO>().Include("Trainer").Include("Service").ToList();
        }

        public ServiceDTO Find(string serviceId)
        {
            var bpId = Convert.ToInt32(serviceId);
            var bpDto = Get().Filter(b => b.Id == bpId).Get().FirstOrDefault();
            if (_disposeWhenDone)
                Dispose();
            return bpDto;
        }

        public ServiceDTO GetByName(string displayName)
        {
            var bp = Get()
                //.Filter(b => b.PlateNumber == displayName)
                .Get()
                .FirstOrDefault();
            return bp;
        }

        public string InsertOrUpdate(ServiceDTO service)
        {
            try
            {
                var validate = Validate(service);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(service))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists + Environment.NewLine +
                           "With the same Name Exists";

                _serviceRepository.InsertUpdate(service);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                var inner = "";
                try
                {
                    inner = exception.InnerException.InnerException.Message;
                }
                catch{}
                return exception.Message + Environment.NewLine + inner;
            }
        }

        public string Disable(ServiceDTO service)
        {
            if (service == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            try
            {
                _serviceRepository.Update(service);
                _unitOfWork.Commit();
                stat = string.Empty;
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            return stat;
        }

        public int Delete(string serviceId)
        {
            try
            {
                _serviceRepository.Delete(serviceId);
                _unitOfWork.Commit();
                return 0;
            }
            catch 
            {
                return -1;
            }
        }

        public bool ObjectExists(ServiceDTO service)
        {
            var objectExists = false;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                var catRepository = new Repository<ServiceDTO>(iDbContext);
                var catExists = catRepository.Query()
                    .Filter(bp => (bp.DisplayName == service.DisplayName) && bp.Id != service.Id)
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

        public string Validate(ServiceDTO service)
        {
            if (null == service)
                return GenericMessages.ObjectIsNull;
            
            //if (String.IsNullOrEmpty(service.PlateNumber))
            //    return service.PlateNumber + " " + GenericMessages.StringIsNullOrEmpty;
            
            return string.Empty;
        }

        #endregion
        
        #region Private Methods
        public string GetServiceCode()
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