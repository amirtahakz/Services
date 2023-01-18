using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Domain.Repository
{
    public interface IMongoRepository<TEntity> where TEntity : BaseEntity
    {
        Task Delete(Guid id);
        Task<TEntity?> GetById(Guid id);
        Task Insert(TEntity entity);
        Task Update(TEntity entity);

    }
}
