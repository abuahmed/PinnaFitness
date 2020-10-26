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
    public class EquipmentService : IEquipmentService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<EquipmentDTO> _equipmentRepository;
        private readonly bool _disposeWhenDone;
        private IDbContext _iDbContext;
        #endregion

        #region Constructor
        public EquipmentService()
        {
            InitializeDbContext();
        }

        public EquipmentService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            _iDbContext = DbContextUtil.GetDbContextInstance();
            _equipmentRepository = new Repository<EquipmentDTO>(_iDbContext);
            _unitOfWork = new UnitOfWork(_iDbContext);
        }

        #endregion

        #region Common Methods
        public IRepositoryQuery<EquipmentDTO> Get()
        {
            var piList = _equipmentRepository
                .Query()
                .Include(a => a.Category)
                .Include(a => a.CreatedByUser, a => a.ModifiedByUser)
                //.Filter(a => !string.IsNullOrEmpty(a.PlateNumber))
                .OrderBy(q => q.OrderBy(c => c.Id));
            return piList;
        }

        public IEnumerable<EquipmentDTO> GetAll(SearchCriteria<EquipmentDTO> criteria = null)
        {
            int totalCount = 0;
            return this.GetAll(criteria, out totalCount);
        }

        public IEnumerable<EquipmentDTO> GetAll(SearchCriteria<EquipmentDTO> criteria, out int totalCount)//, 
        {
            totalCount = 0;
            IEnumerable<EquipmentDTO> piList = new List<EquipmentDTO>();
            try
            {
                if (criteria != null)
                {
                    var pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }


                    IList<EquipmentDTO> pdtoList;
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

        public EquipmentDTO Find(string equipmentId)
        {
            var bpId = Convert.ToInt32(equipmentId);
            var bpDto = Get().Filter(b => b.Id == bpId).Get().FirstOrDefault();
            if (_disposeWhenDone)
                Dispose();
            return bpDto;
        }

        public EquipmentDTO GetByName(string displayName)
        {
            var bp = Get()
                //.Filter(b => b.PlateNumber == displayName)
                .Get()
                .FirstOrDefault();
            return bp;
        }

        public string InsertOrUpdate(EquipmentDTO equipment)
        {
            try
            {
                var validate = Validate(equipment);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(equipment))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists + Environment.NewLine +
                           "With the same Name Exists";

                _equipmentRepository.InsertUpdate(equipment);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(EquipmentDTO equipment)
        {
            if (equipment == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            try
            {
                _equipmentRepository.Update(equipment);
                _unitOfWork.Commit();
                stat = string.Empty;
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            return stat;
        }

        public int Delete(string equipmentId)
        {
            try
            {
                _equipmentRepository.Delete(equipmentId);
                _unitOfWork.Commit();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public bool ObjectExists(EquipmentDTO equipment)
        {
            var objectExists = false;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                var catRepository = new Repository<EquipmentDTO>(iDbContext);
                var catExists = catRepository.Query()
                    .Filter(bp => (bp.Number == equipment.Number) && bp.Id != equipment.Id)
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

        public string Validate(EquipmentDTO equipment)
        {
            if (null == equipment)
                return GenericMessages.ObjectIsNull;

            //if (String.IsNullOrEmpty(equipment.PlateNumber))
            //    return equipment.PlateNumber + " " + GenericMessages.StringIsNullOrEmpty;

            return string.Empty;
        }

        #endregion

        #region Private Methods
        public string GetEquipmentNumber(int equipmentId)
        {
            var pref = equipmentId.ToString();
            if (equipmentId < 1000)
            {
                var id = equipmentId + 110000;
                pref = id.ToString();
                //pref = pref.Substring(1);
            }
            var amhCalender = ReportUtility.GetEthCalendar(DateTime.Now, false);
            return pref + amhCalender.Substring(6);
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