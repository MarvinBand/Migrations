using System;
using System.Threading.Tasks;

namespace Marvin.Migrations
{
    /// <summary>
    /// Provide access to underlying database
    /// </summary>
    public interface IDbProvider : IDisposable
    {
        /// <summary>
        /// Name of connected database
        /// </summary>
        string DbName { get; }

        /// <summary>
        /// Open connection to database
        /// </summary>
        /// <returns></returns>
        Task OpenConnectionAsync();
        
        /// <summary>
        /// Returns actual DB state
        /// </summary>
        /// <param name="desiredVersion">Version of the newest migration</param>
        /// <returns></returns>
        Task<DbState> GetDbStateSafeAsync(DbVersion desiredVersion);
        
        /// <summary>
        /// Create database with default schema if not exist
        /// </summary>
        /// <exception cref="MigrationException"></exception>
        Task CreateDatabaseIfNotExistsAsync();

        /// <summary>
        /// Check if database already exists
        /// </summary>
        /// <param name="databaseName">Database name</param>
        /// <returns><see langword="true"/> if exists, <see langword="false"/> if not</returns>
        /// <exception cref="MigrationException"></exception>
        Task<bool> CheckIfDatabaseExistsAsync(string databaseName);

        /// <summary>
        /// Create table for storing migration history info no exist
        /// </summary>
        /// <exception cref="MigrationException"></exception>
        Task CreateHistoryTableIfNotExistsAsync();

        /// <summary>
        /// Check if table already exists
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <returns><see langword="true"/> if exists, <see langword="false"/> if not</returns>
        /// <exception cref="MigrationException"></exception>
        Task<bool> CheckIfTableExistsAsync(string tableName);

        /// <summary>
        /// Returns actual database version from migration history table
        /// </summary>
        /// <exception cref="MigrationException"></exception>
        Task<DbVersion?> GetDbVersionSafeAsync();

        /// <summary>
        /// Update actual database version in migration history table
        /// </summary>
        /// <exception cref="MigrationException"></exception>
        Task UpdateCurrentDbVersionAsync(DbVersion version);
        
        /// <summary>
        /// Execute sql script without return value
        /// </summary>
        /// <param name="script">SQL script with DDL or DML commands</param>
        /// <exception cref="MigrationException"></exception>
        Task ExecuteScriptAsync(string script);
        
        /// <summary>
        /// Execute sql script and return scalar value
        /// </summary>
        /// <param name="script">SQL script with DDL or DML commands</param>
        /// <exception cref="MigrationException"></exception>
        Task<object> ExecuteScalarScriptAsync(string script);
        
        /// <exception cref="MigrationException"></exception>
        Task ExecuteScriptWithoutInitialCatalogAsync(string script);
        
        /// <exception cref="MigrationException"></exception>
        Task<object> ExecuteScalarScriptWithoutInitialCatalogAsync(string script);

        /// <summary>
        /// Close connection to database
        /// </summary>
        /// <returns></returns>
        Task CloseConnectionAsync();
    }
}