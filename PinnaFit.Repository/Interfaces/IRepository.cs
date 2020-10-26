using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using PinnaFit.Core.Models;

namespace PinnaFit.Repository.Interfaces
{
    public interface IRepository<TEntity> where TEntity : EntityBase
    {
        Guid InstanceId { get; }
        
        TEntity FindById(object id);
        Task<TEntity> FindAsync(params object[] keyValues);
        Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues);
        TEntity FindByRowGuid(Guid rowGuid);
        IQueryable<TEntity> SqlQuery(string query, params object[] parameters);
    
        void Insert(TEntity entity);
        void InsertRange(IEnumerable<TEntity> entities);
        void InsertGraph(TEntity entity);
        void InsertGraphRange(IEnumerable<TEntity> entities);
        void SimpleUpdate(TEntity entity);
        void Update(TEntity entity);
        void InsertUpdate(TEntity entity);
        void CrudByRowGuid(TEntity entity);
        void Delete(object id);
        void Delete(TEntity entity);
        
        IRepositoryQuery<TEntity> Query();
    }
}
