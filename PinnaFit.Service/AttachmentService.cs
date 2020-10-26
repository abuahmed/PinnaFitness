using System;
using System.Collections.Generic;
using System.Linq;
using PinnaFit.Core;
using PinnaFit.Core.Models;
using PinnaFit.DAL;
using PinnaFit.Repository;
using PinnaFit.Repository.Interfaces;
using PinnaFit.Service.Interfaces;

namespace PinnaFit.Service
{
    public class AttachmentService : IAttachmentService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<AttachmentDTO> _attachmentRepository;
        private readonly bool _disposeWhenDone;
        #endregion

        #region Constructor
        public AttachmentService()
        {
            InitializeDbContext();
        }
        public AttachmentService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            var iDbContext = DbContextUtil.GetDbContextInstance();
            _attachmentRepository = new Repository<AttachmentDTO>(iDbContext);
            _unitOfWork = new UnitOfWork(iDbContext);
        }
        #endregion

        #region Common Methods
        public IRepositoryQuery<AttachmentDTO> Get()
        {
            var piAttachment = _attachmentRepository
                .Query().Include(a => a.CreatedByUser, a => a.ModifiedByUser);
            return piAttachment;
        }

        public IEnumerable<AttachmentDTO> GetAll(SearchCriteria<AttachmentDTO> criteria = null)
        {
            IEnumerable<AttachmentDTO> catAttachment;
            try
            {
                if (criteria != null)
                {
                    var pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }

                    IList<AttachmentDTO> pdtoAttachment;
                    if (criteria.Page != 0 && criteria.PageSize != 0)
                    {
                        int totalCount;
                        pdtoAttachment = pdto.GetPage(criteria.Page, criteria.PageSize, out totalCount).ToList();
                    }
                    else
                        pdtoAttachment = pdto.GetList().ToList();

                    catAttachment = pdtoAttachment.ToList();
                }
                else
                {
                    catAttachment = Get().Get().ToList();
                }

            }
            finally
            {
                Dispose(_disposeWhenDone);
            }

            return catAttachment;
        }

        public AttachmentDTO Find(string attachmentId)
        {
            AttachmentDTO attDTO = null;
            try
            {
                int attId;
                if (int.TryParse(attachmentId, out attId))
                    if(attId!=0)
                    attDTO = _attachmentRepository.FindById(attId);
            }
            finally
            {
                Dispose(_disposeWhenDone);
            }

            return attDTO;
        }

        public AttachmentDTO GetByName(string displayName)
        {
            var cat = _attachmentRepository
                .Query()
                .Get()
                .FirstOrDefault();
            return cat;
        }

        public string InsertOrUpdate(AttachmentDTO attachment)
        {
            try
            {
                var validate = Validate(attachment);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(attachment))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists;

                _attachmentRepository.InsertUpdate(attachment);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(AttachmentDTO attachment)
        {
            if (attachment == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                _attachmentRepository.Update(attachment);
                _unitOfWork.Commit();
                stat = string.Empty;
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            finally
            {
                iDbContext.Dispose();
            }
            return stat;
        }

        public int Delete(string attachmentId)
        {
            try
            {
                _attachmentRepository.Delete(attachmentId);
                _unitOfWork.Commit();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public bool ObjectExists(AttachmentDTO attachment)
        {
            //var objectExists = false;
            //var iDbContext = DbContextUtil.GetDbContextInstance();
            //try
            //{
            //    var catRepository = new Repository<AttachmentDTO>(iDbContext);
            //    var catExists = catRepository.Query()
            //        .Filter(bp => bp.FirstName == attachment.FirstName && bp.Id != attachment.Id && bp.Type == attachment.Type)
            //        .Get()
            //        .FirstOrDefault();
            //    if (catExists != null)
            //        objectExists = true;
            //}
            //finally
            //{
            //    iDbContext.Dispose();
            //}

            //return objectExists;
            return false;
        }

        public string Validate(AttachmentDTO attachment)
        {
            if (null == attachment)
                return GenericMessages.ObjectIsNull;



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