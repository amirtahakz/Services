using System.Data;
using System.Reflection;
using Amazon.Runtime.Internal.Util;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver.Core.Configuration;

//public abstract class DapperBaseRepository<TEntity, TKeyType, TContext> where TEntity : class
//{
//    private static readonly IConfiguration _configuration;
//    private readonly string _connectionString = _configuration.GetConnectionString("DefaultConnection");
//    public abstract string TableName { get; }
//    public virtual string KeyFieldName => "Id";
//    private readonly ILogger _logger;
//    private bool IsGuidKey => typeof(TKeyType) == typeof(Guid);
//    protected readonly TContext _context;
//    public DapperBaseRepository(IConfiguration configuration, TContext context, ILogger logger)
//    {
//        _configuration = configuration;
//        _context = context;
//        _logger = logger;
//    }

//    public virtual async Task<TKeyType> Add(TEntity entity)
//    {
//        var columns = GetColumns(IsGuidKey);
//        var stringOfColumns = string.Join(", ", columns);
//        var stringOfParameters = string.Join(", ", columns.Select(e => "@" + e));

//        if (IsGuidKey)
//        {
//            using (var connection = new SqlConnection(_connectionString))
//            {
//                var query = $"insert into [{TableName}] ({stringOfColumns}) values ({stringOfParameters})";
//                connection.Open();
//                await connection.ExecuteAsync(query, entity);
//            }
//            await InsertAuditLog(entity.Id.ToString(), entity, AuditActionType.Insert);
//            return entity.Id;
//        }
//        else
//        {
//            using (var connection = _context.CreateConnection())
//            {
//                var query = $"insert into [{TableName}] ({stringOfColumns}) OUTPUT INSERTED.{KeyFieldName} values ({stringOfParameters})";
//                connection.Open();
//                var resultId = await connection.QuerySingleAsync<TKeyType>(query, entity);
//                await InsertAuditLog(resultId.ToString(), entity, AuditActionType.Insert);
//                return resultId;
//            }
//        }
//    }

//    public virtual async Task Delete(TEntity entity)
//    {
//        var query = $"delete from [{TableName}] where {KeyFieldName} = @{KeyFieldName}";

//        using (var connection = _context.CreateConnection())
//        {
//            connection.Open();
//            await connection.ExecuteAsync(query, entity);
//        }

//        await InsertAuditLog(entity.Id.ToString(), entity, AuditActionType.Delete);
//    }

//    public virtual async Task Update(TEntity entity)
//    {
//        var columns = GetColumns(false);
//        var stringOfColumns = string.Join(", ", columns.Select(e => $"{e} = @{e}"));
//        var query = $"update [{TableName}] set {stringOfColumns} where {KeyFieldName} = @{KeyFieldName}";

//        using (var connection = _context.CreateConnection())
//        {
//            connection.Open();
//            await connection.ExecuteAsync(query, entity);
//        }

//        await InsertAuditLog(entity.Id.ToString(), entity, AuditActionType.Update);
//    }


//    protected async Task SyncCheckpointAsync(long? offsetValue, SnapshotTypeEnum SnapshotType)
//    {
//        if (offsetValue.HasValue)
//        {
//            try
//            {
//                var lastCheckpoint = await GetLastCheckpointAsync(SnapshotType);

//                if (lastCheckpoint == null || lastCheckpoint.InsertDate < DateTime.Today)
//                {
//                    var checkpointEntity = new CheckpointEntity();
//                    checkpointEntity.SnapshotType = SnapshotType;
//                    checkpointEntity.Position = offsetValue.Value;
//                    checkpointEntity.LastDayPosition = lastCheckpoint != null ? lastCheckpoint.Position : 0;
//                    checkpointEntity.InsertDate = DateTime.Now;
//                    checkpointEntity.LastUpdateDateTime = DateTime.Now;

//                    var columns = typeof(CheckpointEntity)
//                                    .GetProperties()
//                                    .Where(e => (e.Name != "Id") && !e.PropertyType.GetTypeInfo().IsGenericType)
//                                .Select(e => e.Name);
//                    var stringOfColumns = string.Join(", ", columns);
//                    var stringOfParameters = string.Join(", ", columns.Select(e => "@" + e));
//                    using (var connection = _context.CreateConnection())
//                    {
//                        var query = $"insert into [Checkpoint] ({stringOfColumns}) OUTPUT INSERTED.{KeyFieldName} values ({stringOfParameters})";
//                        connection.Open();
//                        var resultId = await connection.QuerySingleAsync<TKeyType>(query, checkpointEntity);
//                    }
//                }
//                else if (offsetValue.Value > lastCheckpoint.Position)
//                {
//                    lastCheckpoint.Position = offsetValue.Value;
//                    lastCheckpoint.LastUpdateDateTime = DateTime.Now;
//                    var columns = typeof(CheckpointEntity)
//                                    .GetProperties()
//                                    .Where(e => (e.Name != "Id") && !e.PropertyType.GetTypeInfo().IsGenericType)
//                                .Select(e => e.Name);
//                    var stringOfColumns = string.Join(", ", columns.Select(e => $"{e} = @{e}"));
//                    var query = $"update [Checkpoint] set {stringOfColumns} where {KeyFieldName} = @{KeyFieldName}";
//                    using (var connection = _context.CreateConnection())
//                    {
//                        connection.Open();
//                        await connection.ExecuteAsync(query, lastCheckpoint);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
//    }
//    private async Task<CheckpointEntity> GetLastCheckpointAsync(SnapshotTypeEnum SnapshotType)
//    {
//        var sql = $"SELECT TOP 1 * FROM [Checkpoint] WHERE [{nameof(CheckpointEntity.SnapshotType)}]={(int)SnapshotType} ORDER BY {nameof(CheckpointEntity.InsertDate)} DESC";
//        using (var connection = _context.CreateConnection())
//        {
//            connection.Open();
//            return await connection.QueryFirstOrDefaultAsync<CheckpointEntity>(sql);
//        }
//    }
//    private IEnumerable<string> GetColumns(bool includeKey)
//    {
//        var result = typeof(TEntity)
//                .GetProperties()
//                .Where(e => (includeKey || e.Name != "Id") && !e.PropertyType.GetTypeInfo().IsGenericType)
//                .Select(e => e.Name);

//        return result;
//    }
//}

//public class DapperContext
//{
//    private readonly IConfiguration _configuration;
//    private readonly string _connectionString;
//    public DapperContext(IConfiguration configuration)
//    {
//        _configuration = configuration;
//        _connectionString = _configuration.GetConnectionString("DefaultConnection");
//    }
//    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
//}

public class DapperBaseRepository<TEntity> : IDapperBaseRepository<TEntity> where TEntity : class
{
    public string TableName { get; }
    private readonly IConfiguration _configuration;

    public DapperBaseRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        string connectionString = _configuration.GetConnectionString("DefaultConnection");
        var columns = GetColumns();
        var stringOfColumns = string.Join(", ", columns);
        var stringOfParameters = string.Join(", ", columns.Select(e => "@" + e));
        using (var connection = new SqlConnection(connectionString))
        {
            var query = $"INSERT INTO [dbo].[ShippingMethods] ({stringOfColumns}) VALUES ({stringOfParameters})";
            connection.Open();
            await connection.ExecuteAsync(query, entity);
        }
        return entity;

    }

    private IEnumerable<string> GetColumns()
    {
        var result = typeof(TEntity)
            .GetProperties()
            .Where(e => (e.Name != "Id") && !e.PropertyType.GetTypeInfo().IsGenericType)
            .Select(e => e.Name);

        return result;
    }

}

public interface IDapperBaseRepository<TEntity> where TEntity : class
{
    Task<TEntity> AddAsync(TEntity entity);
}


