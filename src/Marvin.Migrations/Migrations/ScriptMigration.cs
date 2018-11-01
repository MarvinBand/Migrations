using System;
using System.Threading.Tasks;

namespace Marvin.Migrations.Migrations
{
    /// <summary>
    /// Migration that uses raw sql 
    /// </summary>
    public class ScriptMigration : IMigration
    {
        /// <inheritdoc />
        public DbVersion Version { get; }
        
        /// <inheritdoc />
        public string Comment { get; }
        
        /// <summary>
        /// SQL script to apply migration
        /// </summary>
        public string UpScript { get; }
        
        /// <summary>
        /// SQL script to undo migration
        /// </summary>
        public string DownScript { get; }

        private readonly IDbProvider _dbProvider;

        /// <inheritdoc />
        public ScriptMigration(
            IDbProvider dbProvider, 
            DbVersion version, 
            string upScript, 
            string downScript, 
            string comment)
        {
            _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
            if (String.IsNullOrWhiteSpace(upScript)) throw new ArgumentException(nameof(upScript));
            
            Version = version;
            UpScript = upScript;
            DownScript = downScript;
            Comment = comment;
        }

        /// <inheritdoc />
        public Task UpgradeAsync()
        {
            return _dbProvider.ExecuteScriptAsync(UpScript);
        }

        /// <inheritdoc />
        public Task DowngradeAsync()
        {
            if (String.IsNullOrWhiteSpace(DownScript)) return Task.CompletedTask;
            
            return _dbProvider.ExecuteScriptAsync(DownScript);
        }
    }
}