using System;
using System.Collections.Generic;
using PinnaFit.Core;
using PinnaFit.Core.Models;

namespace PinnaFit.Service.Interfaces
{
    public interface IAttachmentService : IDisposable
    {
        IEnumerable<AttachmentDTO> GetAll(SearchCriteria<AttachmentDTO> criteria = null);
        AttachmentDTO Find(string attachmentId);
        AttachmentDTO GetByName(string displayName);
        string InsertOrUpdate(AttachmentDTO attachment);
        string Disable(AttachmentDTO attachment);
        int Delete(string attachmentId);
    }
}