using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Text;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Services.Common.Domain.Repository;

public interface IBaseDapperRepository<T>
{
    Task<IEnumerable> GetAllAsync();
    Task DeleteRowAsync(Guid id);
    Task<T> GetAsync(Guid id);
    Task<int> SaveRangeAsync(IEnumerable list);
    Task UpdateAsync(T t);
    Task InsertAsync(T t);
}



