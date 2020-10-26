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
    public class FacilityService : IFacilityService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<FacilityDTO> _facilityRepository;
        private readonly bool _disposeWhenDone;
        private IDbContext _iDbContext;
        #endregion

        #region Constructor
        public FacilityService()
        {
            InitializeDbContext();
        }

        public FacilityService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            _iDbContext = DbContextUtil.GetDbContextInstance();
            _facilityRepository = new Repository<FacilityDTO>(_iDbContext);
            _unitOfWork = new UnitOfWork(_iDbContext);
        }

        #endregion

        #region Common Methods
        public IRepositoryQuery<FacilityDTO> Get()
        {
            var piList = _facilityRepository
                .Query()
                .Include(a => a.Services)
                .Include(a => a.CreatedByUser, a => a.ModifiedByUser)
                //.Filter(a => !string.IsNullOrEmpty(a.PlateNumber))
                .OrderBy(q => q.OrderBy(c => c.Id));
            return piList;
        }

        public IEnumerable<FacilityDTO> GetAll(SearchCriteria<FacilityDTO> criteria = null)
        {
            int totalCount = 0;
            return this.GetAll(criteria, out totalCount);
        }

        public IEnumerable<FacilityDTO> GetAll(SearchCriteria<FacilityDTO> criteria, out int totalCount)//, 
        {
            totalCount = 0;
            IEnumerable<FacilityDTO> piList = new List<FacilityDTO>();
            try
            {
                if (criteria != null)
                {
                    var pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }


                    IList<FacilityDTO> pdtoList;
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
        public IEnumerable<ServiceDTO> GetAllServices()
        {
            return _iDbContext.Set<ServiceDTO>().Include("Facilities").ToList();
        }
        public FacilityDTO Find(string facilityId)
        {
            var bpId = Convert.ToInt32(facilityId);
            var bpDto = Get().Filter(b => b.Id == bpId).Get().FirstOrDefault();
            if (_disposeWhenDone)
                Dispose();
            return bpDto;
        }

        public FacilityDTO GetByName(string displayName)
        {
            var bp = Get()
                //.Filter(b => b.PlateNumber == displayName)
                .Get()
                .FirstOrDefault();
            return bp;
        }

        public string InsertOrUpdate(FacilityDTO facility)
        {
            try
            {
                var validate = Validate(facility);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(facility))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists + Environment.NewLine +
                           "With the same Name Exists";

                _facilityRepository.InsertUpdate(facility);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(FacilityDTO facility)
        {
            if (facility == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            try
            {
                _facilityRepository.Update(facility);
                _unitOfWork.Commit();
                stat = string.Empty;
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            return stat;
        }

        public int Delete(string facilityId)
        {
            try
            {
                _facilityRepository.Delete(facilityId);
                _unitOfWork.Commit();
                return 0;
            }
            catch 
            {
                return -1;
            }
        }

        public bool ObjectExists(FacilityDTO facility)
        {
            var objectExists = false;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                var catRepository = new Repository<FacilityDTO>(iDbContext);
                var catExists = catRepository.Query()
                    .Filter(bp => (bp.DisplayName == facility.DisplayName) && bp.Id != facility.Id)
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

        public string Validate(FacilityDTO facility)
        {
            if (null == facility)
                return GenericMessages.ObjectIsNull;
            
            //if (String.IsNullOrEmpty(facility.PlateNumber))
            //    return facility.PlateNumber + " " + GenericMessages.StringIsNullOrEmpty;
            
            return string.Empty;
        }

        #endregion
        
        #region Private Methods
   
        public string GetFacilityCode()
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