using System;
using System.IO;
using System.Linq;
using Marvin.Migrations.MigrationProviders;
using Marvin.Migrations.Migrations;
using Moq;
using Xunit;

namespace Marvin.Migrations.UnitTests
{
    public class ScriptMigrationProviderTests
    {
        [Fact]
        public void GetMigrations_FromDirectory_Ok()
        {
            var dbProvider = Mock.Of<IDbProvider>();

            var migrationsProvider = new ScriptMigrationsProvider();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Scripts");
            migrationsProvider.FromDirectory(path);

            var migrations = migrationsProvider.GetMigrations(dbProvider).ToList();
            
            Assert.Equal(4, migrations.Count);
            
            Assert.True(migrations[0] is ScriptMigration);
            Assert.Equal(new DbVersion(1,0), migrations[0].Version);
            Assert.Equal("comment", migrations[0].Comment);
            Assert.Equal("up", ((ScriptMigration)migrations[0]).UpScript);
            Assert.Equal("down", ((ScriptMigration)migrations[0]).DownScript);
            
            
            Assert.True(migrations[1] is ScriptMigration);
            Assert.Equal(new DbVersion(1,1), migrations[1].Version);
            Assert.True(String.IsNullOrEmpty(migrations[1].Comment));
            Assert.Equal("up", ((ScriptMigration)migrations[1]).UpScript);
            Assert.Equal("down", ((ScriptMigration)migrations[1]).DownScript);
            
            
            Assert.True(migrations[2] is ScriptMigration);
            Assert.Equal(new DbVersion(1,2), migrations[2].Version);
            Assert.Equal("comment", migrations[2].Comment);
            Assert.Equal("up", ((ScriptMigration)migrations[2]).UpScript);
            Assert.True(String.IsNullOrEmpty(((ScriptMigration)migrations[2]).DownScript));
            
            Assert.True(migrations[3] is ScriptMigration);
            Assert.Equal(new DbVersion(1,3), migrations[3].Version);
            Assert.True(String.IsNullOrEmpty(migrations[3].Comment));
            Assert.Equal("up", ((ScriptMigration)migrations[3]).UpScript);
            Assert.True(String.IsNullOrEmpty(((ScriptMigration)migrations[3]).DownScript));
        }
    }
}