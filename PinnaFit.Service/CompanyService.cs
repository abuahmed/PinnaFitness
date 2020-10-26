using System;
using System.Linq;
using PinnaFit.Core.Models;
using PinnaFit.DAL;
using PinnaFit.Repository;
using PinnaFit.Repository.Interfaces;
using PinnaFit.Service.Interfaces;

namespace PinnaFit.Service
{
    public class CompanyService : ICompanyService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<CompanyDTO> _clientRepository;
        private readonly bool _disposeWhenDone;
        #endregion

        #region Constructor
        public CompanyService()
        {
            InitializeDbContext();
        }

        public CompanyService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            var iDbContext = DbContextUtil.GetDbContextInstance();
            _clientRepository = new Repository<CompanyDTO>(iDbContext);
            _unitOfWork = new UnitOfWork(iDbContext);
        }
        #endregion

        #region Common Methods

        public IRepositoryQuery<CompanyDTO> Get()
        {
            var piList = _clientRepository
                .Query()
                .Include(a => a.Address)
                .Filter(a => !string.IsNullOrEmpty(a.DisplayName))
                .OrderBy(q => q.OrderBy(c => c.DisplayName));
            return piList;
        }

        public CompanyDTO GetCompany()
        {
            return Get().Get().FirstOrDefault();
        }

        public string InsertOrUpdate(CompanyDTO client)
        {
            try
            {
                var validate = Validate(client);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(client))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists;

                _clientRepository.InsertUpdate(client);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        //public string Disable(CompanyDTO client)
        //{
        //    if (client == null)
        //        return GenericMessages.ObjectIsNull;

        //    try
        //    {
        //        _clientRepository.Update(client);
        //        _unitOfWork.Commit();
        //        return string.Empty;
        //    }
        //    catch (Exception exception)
        //    {
        //        return exception.Message;
        //    }
        //}

        //public int Delete(string clientId)
        //{
        //    try
        //    {
        //        _clientRepository.Delete(clientId);
        //        _unitOfWork.Commit();
        //        return 0;
        //    }
        //    catch (Exception exception)
        //    {
        //        return -1;
        //    }
        //}

        public bool ObjectExists(CompanyDTO client)
        {
            var objectExists = false;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                var catRepository = new Repository<CompanyDTO>(iDbContext);
                var catExists = catRepository
                    .Query()
                    .Filter(bp => bp.DisplayName == client.DisplayName && bp.Id != client.Id)
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

        public string Validate(CompanyDTO client)
        {
            if (client == null)
                return GenericMessages.ObjectIsNull;

            if (client.Address == null)
                return "Address " + GenericMessages.ObjectIsNull;

            if (String.IsNullOrEmpty(client.DisplayName))
                return client.DisplayName + " " + GenericMessages.StringIsNullOrEmpty;

            if (client.DisplayName.Length > 255)
                return client.DisplayName + " can not be more than 255 characters ";

            return string.Empty;
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