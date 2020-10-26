using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.SqlServer;
using PinnaFit.Core.Enumerations;
using PinnaFit.Core.Models;
using PinnaFit.Core;
using System.Data.Common;

namespace PinnaFit.DAL
{
    public class PinnaFitDBContext : DbContextBase
    {
        public PinnaFitDBContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<PinnaFitDBContext, Configuration>());
            Configuration.ProxyCreationEnabled = false;
        }
        public new DbSet<T> Set<T>() where T : class
        {
            return base.Set<T>();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            DbContextUtil.OnModelCreating(modelBuilder);
        }
    }

    public class DbContextFactory : IDbContextFactory<PinnaFitDBContext>
    {
        public PinnaFitDBContext Create()
        {
            switch (Singleton.Edition)
            {
                case PinnaFitEdition.CompactEdition:
                    var sqlCeConString = "Data Source=" + Singleton.SqlceFileName + ";" +
                                         "Max Database Size=4091;Password=amSt0ckP@ssw0rd";
                    Singleton.ConnectionStringName = sqlCeConString;
                    Singleton.ProviderName = "System.Data.SqlServerCe.4.0";
                    var sqlce = new SqlCeConnectionFactory(Singleton.ProviderName);
                    return new PinnaFitDBContext(sqlce.CreateConnection(sqlCeConString), true);

                case PinnaFitEdition.ServerEdition:
                    const string serverName = "."; // "ServerPC";
                    var sQlServConString = "data source=" + serverName + ";initial catalog=" + Singleton.SqlceFileName +
                                              ";user id=sa;password=amihan";
                    Singleton.ConnectionStringName = sQlServConString;
                    Singleton.ProviderName = "System.Data.SqlClient";
                    var sql = new SqlConnectionFactory(sQlServConString);
                    return new PinnaFitDBContext(sql.CreateConnection(sQlServConString), true);
            }
            return null;
        }
    }

    public class Configuration : DbMigrationsConfiguration<PinnaFitDBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            //SetSqlGenerator("System.Data.SqlClient", new CustomSqlServerMigrationSqlGenerator());
        }

        
        protected override void Seed(PinnaFitDBContext context)
        {
            //if (Singleton.SeedDefaults)
            //{
            //    var setting = context.Set<SettingDTO>().Find(1);
            //    if (setting == null)
            //    {
            //        context = (PinnaFitDBContext)DbContextUtil.Seed(context);
            //    }
            //}
            //#region List Seeds
            //context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.Category, DisplayName = "No Category" });

            //context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.UnitMeasure, DisplayName = "Pcs" });
            //#endregion
            base.Seed(context);
        }
    }

    //internal class CustomSqlServerMigrationSqlGenerator : SqlServerMigrationSqlGenerator
    //{
    //    protected override void Generate(AddColumnOperation addColumnOperation)
    //    {
    //        SetCreatedUtcColumn(addColumnOperation.Column);

    //        base.Generate(addColumnOperation);
    //    }

    //    protected override void Generate(CreateTableOperation createTableOperation)
    //    {
    //        SetCreatedUtcColumn(createTableOperation.Columns);

    //        base.Generate(createTableOperation);
    //    }

    //    private static void SetCreatedUtcColumn(IEnumerable<ColumnModel> columns)
    //    {
    //        foreach (var columnModel in columns)
    //        {
    //            SetCreatedUtcColumn(columnModel);
    //        }
    //    }

    //    private static void SetCreatedUtcColumn(PropertyModel column)
    //    {
    //        if (column.Name == "CreatedUtc")
    //        {
    //            column.DefaultValueSql = "GETUTCDATE()";
    //        }
    //    }

    //    private void SetAnnotatedColumn(ColumnModel col)
    //    {
    //        AnnotationValues values;
    //        if (col.Annotations.TryGetValue("SqlDefaultValue", out values))
    //        {
    //            col.DefaultValueSql = (string)values.NewValue;
    //        }
    //    }
    //}
}
