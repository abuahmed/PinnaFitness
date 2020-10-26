using System;
using System.Threading.Tasks;
using PinnaFit.Core.Models;

namespace PinnaFit.Repository.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
        Task<int> CommitAync();
        //void Dispose();        
        void Dispose(bool disposing);
        IRepository<TEntity> Repository<TEntity>() where TEntity : EntityBase;
    }
}
