

using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Text;
using AngleSharp;
using Common.Domain.Repository;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Common.Infrastructure.Repository;

public abstract class BaseDapperRepository<T> : IBaseDapperRepository<T> where T : class
{
    private readonly string _tableName;

    public IConfiguration _configuration { get; }
    protected BaseDapperRepository(IConfiguration configuration)
    {
        _tableName = typeof(T).Name;
        _configuration = configuration;
    }

    private IDbConnection CreateConnection()
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        var conn = new SqlConnection(connectionString);
        conn.Open();
        return conn;
    }
    private IEnumerable<PropertyInfo> GetProperties => typeof(T).GetProperties();

    private static List<string> GenerateListOfProperties(IEnumerable<PropertyInfo> listOfProperties)
    {
        return (from prop in listOfProperties
                let attributes = prop.GetCustomAttributes(typeof(DescriptionAttribute), false)
                where attributes.Length <= 0 || (attributes[0] as DescriptionAttribute)?.Description != "ignore"
                select prop.Name).ToList();
    }

    private string GenerateUpdateQuery()
    {
        var updateQuery = new StringBuilder($"UPDATE {_tableName} SET ");
        var properties = GenerateListOfProperties(GetProperties);

        properties.ForEach(property =>
        {
            if (!property.Equals("Id"))
            {
                updateQuery.Append($"{property}=@{property},");
            }
        });

        updateQuery.Remove(updateQuery.Length - 1, 1); //remove last comma
        updateQuery.Append(" WHERE Id=@Id");

        return updateQuery.ToString();
    }

    private string GenerateInsertQuery()
    {
        var insertQuery = new StringBuilder($"INSERT INTO {_tableName} ");

        insertQuery.Append("(");

        var properties = GenerateListOfProperties(GetProperties);
        properties.ForEach(prop => { insertQuery.Append($"[{prop}],"); });

        insertQuery
            .Remove(insertQuery.Length - 1, 1)
            .Append(") VALUES (");

        properties.ForEach(prop => { insertQuery.Append($"@{prop},"); });

        insertQuery
            .Remove(insertQuery.Length - 1, 1)
            .Append(")");

        return insertQuery.ToString();
    }


    #region CRUD

    public async Task<IEnumerable> GetAllAsync()
    {
        using (var connection = CreateConnection())
        {
            return await connection.QueryAsync($"SELECT * FROM {_tableName}");
        }
    }
    public async Task<T> GetAsync(Guid id)
    {
        using (var connection = CreateConnection())
        {
            var result = await connection.QuerySingleOrDefaultAsync($"SELECT * FROM {_tableName} WHERE Id=@Id", new { Id = id });
            if (result == null)
                throw new KeyNotFoundException($"{_tableName} with id [{id}] could not be found.");

            return result;
        }
    }
    public async Task<int> SaveRangeAsync(IEnumerable list)
    {
        var inserted = 0;
        var query = GenerateInsertQuery();
        using (var connection = CreateConnection())
        {
            inserted += await connection.ExecuteAsync(query, list);
        }

        return inserted;

    }

    public async Task InsertAsync(T t)
    {
        var insertQuery = GenerateInsertQuery();

        using (var connection = CreateConnection())
        {
            await connection.ExecuteAsync(insertQuery, t);
        }
    }

    public async Task UpdateAsync(T t)
    {
        var updateQuery = GenerateUpdateQuery();

        using (var connection = CreateConnection())
        {
            await connection.ExecuteAsync(updateQuery, t);
        }
    }
    public async Task DeleteRowAsync(Guid id)
    {
        using (var connection = CreateConnection())
        {
            await connection.ExecuteAsync($"DELETE FROM {_tableName} WHERE Id=@Id", new { Id = id });
        }
    }

    #endregion

}