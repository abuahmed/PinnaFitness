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
    public class TrainerService : ITrainerService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<TrainerDTO> _trainerRepository;
        //private IRepository<TrainerSubscriptionDTO> _trainerSubscriptionRepository;
        private readonly bool _disposeWhenDone;
        private IDbContext _iDbContext;
        #endregion

        #region Constructor
        public TrainerService()
        {
            InitializeDbContext();
        }

        public TrainerService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            _iDbContext = DbContextUtil.GetDbContextInstance();
            _trainerRepository = new Repository<TrainerDTO>(_iDbContext);
            //_trainerSubscriptionRepository = new Repository<TrainerSubscriptionDTO>(_iDbContext);
            _unitOfWork = new UnitOfWork(_iDbContext);
        }

        #endregion

        #region Common Methods
        public IRepositoryQuery<TrainerDTO> Get()
        {
            var piList = _trainerRepository
                .Query()
                .Include(a => a.Address, a => a.ContactPerson, a => a.ContactPerson.Address)
                .Include(a => a.CreatedByUser, a => a.ModifiedByUser)
                .Filter(a => !string.IsNullOrEmpty(a.DisplayName))
                .OrderBy(q => q.OrderBy(c => c.DisplayName).ThenBy(c => c.Number));
            return piList;
        }

        public IEnumerable<TrainerDTO> GetAll(SearchCriteria<TrainerDTO> criteria = null)
        {
            int totalCount = 0;
            return this.GetAll(criteria, out totalCount);
        }

        public IEnumerable<TrainerDTO> GetAll(SearchCriteria<TrainerDTO> criteria, out int totalCount)//, 
        {
            totalCount = 0;
            IEnumerable<TrainerDTO> piList = new List<TrainerDTO>();
            try
            {
                if (criteria != null)
                {
                    var pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }


                    IList<TrainerDTO> pdtoList;
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
                //   var subscriptioneDtos = _trainerSubscriptionRepository
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

        public TrainerDTO Find(string trainerId)
        {
            var bpId = Convert.ToInt32(trainerId);
            var bpDto = Get().Filter(b => b.Id == bpId).Get().FirstOrDefault();
            if (_disposeWhenDone)
                Dispose();
            return bpDto;
        }

        public TrainerDTO GetByName(string displayName)
        {
            var bp = Get()
                .Filter(b => b.DisplayName == displayName)
                .Get()
                .FirstOrDefault();
            return bp;
        }

        public string InsertOrUpdate(TrainerDTO trainer)
        {
            try
            {
                var validate = Validate(trainer);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(trainer))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists + Environment.NewLine +
                           "With the same name exists";

                _trainerRepository.InsertUpdate(trainer);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(TrainerDTO trainer)
        {
            //if (_unitOfWork.Repository<DeliveryHeaderDTO>().Query().Get().Any(i => i.TrainerId == Trainer.Id) ||
            //    _unitOfWork.Repository<DocumentDTO>().Query().Get().Any(i => i.TrainerId == Trainer.Id))
            //{
            //    return "Can't delete the item, it is already in use...";
            //}

            if (trainer == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            try
            {
                _trainerRepository.Update(trainer);
                _unitOfWork.Commit();
                stat = string.Empty;
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            return stat;
        }

        public int Delete(string trainerId)
        {
            try
            {
                _trainerRepository.Delete(trainerId);
                _unitOfWork.Commit();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public bool ObjectExists(TrainerDTO Trainer)
        {
            var objectExists = false;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                var catRepository = new Repository<TrainerDTO>(iDbContext);
                var catExists = catRepository.Query()
                    .Filter(bp => (bp.DisplayName == Trainer.DisplayName) &&
                                  bp.Id != Trainer.Id)
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

        public string Validate(TrainerDTO Trainer)
        {
            if (null == Trainer)
                return GenericMessages.ObjectIsNull;

            if (Trainer.Address == null)
                return "Address " + GenericMessages.ObjectIsNull;

            if (String.IsNullOrEmpty(Trainer.DisplayName))
                return Trainer.DisplayName + " " + GenericMessages.StringIsNullOrEmpty;

            if (Trainer.DisplayName.Length > 255)
                return Trainer.DisplayName + " can not be more than 255 characters ";

            if (!string.IsNullOrEmpty(Trainer.Number) && Trainer.Number.Length > 50)
                return Trainer.Number + " can not be more than 50 characters ";


            return string.Empty;
        }

        #endregion

        #region Private Methods
        public string GetTrainerNumber(int trainerId)
        {
            var pref = trainerId.ToString();
            if (trainerId < 1000)
            {
                var id = trainerId + 10000;
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