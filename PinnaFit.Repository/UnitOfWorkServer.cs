using System;
using PinnaFit.DAL.Interfaces;

namespace PinnaFit.Repository
{
    public class UnitOfWorkServer : UnitOfWorkCommon
    {
        public UnitOfWorkServer(IDbContext dbContext)
        {
            if (dbContext == null)
                throw new ArgumentNullException("context");

            Context = dbContext;
            _instanceId = Guid.NewGuid();
        }
    }
}