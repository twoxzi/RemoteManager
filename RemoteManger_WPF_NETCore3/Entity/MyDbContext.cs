using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace Twoxzi.RemoteManager.Entity
{
    public class MyDbContext : DbContext
    {
        protected MyDbContext()
        {

            //this.Database.Migrate();
            //String s = this.Database.GetDbConnection().ConnectionString;

        }
        protected MyDbContext(DbContextOptions options) : base(options)
        {
            //this.Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            // 默认的数据库链接
            if (!optionsBuilder.IsConfigured)
            {
                String path = ConfigurationManager.ConnectionStrings["rm"]?.ConnectionString ?? "Data Source=rm.db";
#if DEBUG
                path = "Data Source=" + System.IO.Path.Combine(System.IO.Path.GetTempPath(), "rm.db");
#endif
                optionsBuilder.UseSqlite(path);
            }
            //this.Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RemoteInfo>().HasKey(x => new { x.ID, x.ToolCode });

            //Database.Migrate();
            //this.Database.EnsureCreated();
        }


        //public static DbConnection CreateConnection(String dbName)
        //{
        //    var result = SQLiteProviderFactory.Instance.CreateConnection();
        //    result.ConnectionString = $"data source={dbName}";
        //    return result;
        //}

        /// <summary>
        /// 创建数据库
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public static MyDbContext CreateDb(String dbName = null)
        {
            MyDbContext dbo;
            if (dbName == null || dbName.Length == 0)
            {
                dbo = new MyDbContext();
            }
            else
            {
                DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder();
                String path = ConfigurationManager.ConnectionStrings["rm"]?.ConnectionString ?? "Data Source={dbName}";
#if DEBUG
                path = "Data Source="+ System.IO.Path.Combine(System.IO.Path.GetTempPath(), "rm.db");
#endif
                optionsBuilder.UseSqlite(path);
                dbo = new MyDbContext(optionsBuilder.Options);
            }
            dbo.Database.EnsureCreated();
            //var con = dbo.Database.GetDbConnection();
            if (dbo.Database.GetPendingMigrations().Any())
            {
                dbo.Database.Migrate();
            }

            return dbo;
        }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        //    modelBuilder.Configurations.AddFromAssembly(typeof(DownloadContext).Assembly);
        //    //如果不存在数据库，则创建
        //    Database.SetInitializer(new SqliteCreateDatabaseIfNotExists<DownloadContext>(modelBuilder));
        //}

        //为您要在模型中包含的每种实体类型都添加 DbSet。有关配置和使用 Code First  模型
        //的详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=390109。

        public virtual DbSet<RemoteInfo> RemoteInfo { get; set; }
    }
}
