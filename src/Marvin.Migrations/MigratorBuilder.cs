using System;
using System.Collections.Generic;
using Marvin.Migrations.MigrationProviders;
using Marvin.Migrations.Migrations;
using Microsoft.Extensions.Logging;

namespace Marvin.Migrations
{
    /// <summary>
    /// Builder for <see cref="IDbMigrator"/>
    /// </summary>
    public class MigratorBuilder
    {
        private readonly List<IMigrationsProvider> _migrationsProviders;
        private IDbProviderFactory _dbProviderFactory;

        private MigrationPolicy _upgradePolicy;
        private MigrationPolicy _downgradePolicy;
        private DbVersion? _targetVersion;

        private ILogger _logger;

        /// <inheritdoc />
        public MigratorBuilder()
        {
            _migrationsProviders = new List<IMigrationsProvider>();
            _upgradePolicy = MigrationPolicy.All;
            _downgradePolicy = MigrationPolicy.All;
            _targetVersion = default(DbVersion?);
        }
        
        /// <summary>
        /// Allow to add <see cref="ScriptMigration"/> migrations from sql files
        /// </summary>
        /// <returns>Provider of <see cref="ScriptMigration"/></returns>
        public ScriptMigrationsProvider UseScriptMigrations()
        {
            var scriptMigrationProvider = new ScriptMigrationsProvider();
            _migrationsProviders.Add(scriptMigrationProvider);
            return scriptMigrationProvider;
        }

        /// <summary>
        /// Allow to add <see cref="CodeMigration"/> migrations from assembly
        /// </summary>
        /// <returns>Provider of <see cref="CodeMigration"/></returns>
        public CodeMigrationsProvider UseCodeMigrations()
        {
            var codeMigrationProvider = new CodeMigrationsProvider();
            _migrationsProviders.Add(codeMigrationProvider);
            return codeMigrationProvider;
        }

        /// <summary>
        /// Allow to use custom migration provider
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public MigratorBuilder UseCustomMigrationsProvider(IMigrationsProvider provider)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));
            _migrationsProviders.Add(provider);
            return this;
        }

        /// <summary>
        /// Setup upgrade migration policy
        /// </summary>
        /// <param name="policy">Policy</param>
        /// <returns></returns>
        public MigratorBuilder UseUpgradeMigrationPolicy(MigrationPolicy policy)
        {
            _upgradePolicy = policy;
            return this;
        } 
        
        /// <summary>
        /// Setup downgrade migration policy
        /// </summary>
        /// <param name="policy">Policy</param>
        /// <returns></returns>
        public MigratorBuilder UseDowngradeMigrationPolicy(MigrationPolicy policy)
        {
            _downgradePolicy = policy;
            return this;
        }

        /// <summary>
        /// Setup logger for migrator 
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public MigratorBuilder UserLogger(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            return this;
        }

        /// <summary>
        /// Setup factory for provider to database access
        /// </summary>
        /// <param name="dbProviderFactory"></param>
        /// <returns></returns>
        public MigratorBuilder UserDbProviderFactory(IDbProviderFactory dbProviderFactory)
        {
            _dbProviderFactory = dbProviderFactory ?? throw new ArgumentNullException(nameof(dbProviderFactory));
            return this;
        }

        /// <summary>
        /// Setup target version of migration
        /// </summary>
        /// <param name="targetDbVersion">Target database version</param>
        /// <returns></returns>
        /// <remarks>
        /// If <param name="targetDbVersion"></param> is not specified, migrator will upgrade database to the most newest migration, provided by <see cref="IMigrationsProvider"/>
        /// If <param name="targetDbVersion"></param> is specified, migrator will upgrade or downgrade database depending on the current DB version and the specified
        /// </remarks>
        public MigratorBuilder SetUpTargetVersion(DbVersion targetDbVersion)
        {
            _targetVersion = targetDbVersion;
            return this;
        }

        /// <summary>
        /// Build migrator
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Throws when <see cref="IDbProvider"/> or <see cref="IMigration"/> does not specified</exception>
        public IDbMigrator Build()
        {
            if (_dbProviderFactory == null) throw new InvalidOperationException($"{typeof(IDbProvider)} not set up. Use {nameof(UserDbProviderFactory)}");
            if (_migrationsProviders.Count == 0) throw new InvalidOperationException($"{typeof(IMigrationsProvider)} not set up. Use {nameof(UseScriptMigrations)} or {nameof(UseCodeMigrations)}");
            
            var migrations = new List<IMigration>();
            var dbProvider = _dbProviderFactory.CreateDbProvider();
            foreach (var migrationsProvider in _migrationsProviders)
            {
                migrations.AddRange(migrationsProvider.GetMigrations(dbProvider));
            }
            
            return new DbMigrator(
                dbProvider, 
                migrations, 
                _upgradePolicy, 
                _downgradePolicy, 
                _targetVersion, 
                _logger);
        }

    }
}