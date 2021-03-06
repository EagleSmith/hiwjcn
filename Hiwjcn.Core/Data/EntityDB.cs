﻿#define use_mysql

using Hiwjcn.Core.Model.Sys;
using Lib.core;
using Lib.data;
using Lib.helper;
using Model;
using Model.Category;
using Model.Sys;
using Model.User;
using MySql.Data.Entity;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using WebLogic.Model.Page;
using WebLogic.Model.Sys;
using WebLogic.Model.Tag;
using WebLogic.Model.User;
using Hiwjcn.Core.Domain.Auth;
using System.Data.Entity.SqlServer;
using System.Configuration;
using System.Reflection;
using System;

namespace Hiwjcn.Dal
{
    /// <summary>
    /// mysql数据库
    /// </summary>
    public class MySqlConfiguration : DbConfiguration
    {
        public MySqlConfiguration()
        {
            this.SetExecutionStrategy(MySqlProviderInvariantName.ProviderName, () => new MySqlExecutionStrategy());
            this.SetDefaultConnectionFactory(new MySqlConnectionFactory());
        }
    }

    /// <summary>
    /// sqlserver数据库
    /// </summary>
    public class SqlServerConfiguration : DbConfiguration
    {
        public SqlServerConfiguration()
        {
            this.SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy());
            this.SetDefaultConnectionFactory(new LocalDbConnectionFactory("mssqllocaldb"));
        }
    }

    /// <summary>
    /// EF
    /// </summary>
    public abstract class BaseEFContext : DbContext
    {
        public BaseEFContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ValidateOnSaveEnabled = false;

            this.Database.CommandTimeout = 3;
            this.Database.Log = (log) =>
            {
                //if (!ValidateHelper.IsPlumpString(log)) { return; }
                //LogHelper.Error(typeof(EntityDB), "EF日志：\n" + log);
            };
        }

        /// <summary>
        /// 注册mapping
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //不在表名后加s或者es
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //注册映射
            var ass = this.FindRegisterTableFluentMappingAssembly();
            if (ValidateHelper.IsPlumpList(ass))
            {
                foreach (var a in ass)
                {
                    //手动注册
                    //modelBuilder.Configurations.Add(new UserModelMapping());
                    //通过反射自动注册
                    modelBuilder.RegisterTableFluentMapping(a);
                }
            }
        }

        protected virtual Assembly[] FindRegisterTableFluentMappingAssembly()
        {
            return new Assembly[] { };
        }

        /// <summary>
        /// Attach an entity to the context or return an already attached entity (if it was already attached)
        /// 附加一个实体，如果已经存在就直接返回
        /// </summary>
        /// <typeparam name="TEntity">TEntity</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Attached entity</returns>
        //protected virtual TEntity AttachEntityToContext<TEntity>(TEntity entity) where TEntity : ModelBase, new()
        //{
        //    //little hack here until Entity Framework really supports stored procedures
        //    //otherwise, navigation properties of loaded entities are not loaded until an entity is attached to the context
        //    var alreadyAttached = Set<TEntity>().Local.FirstOrDefault(x => x.Id == entity.Id);
        //    if (alreadyAttached == null)
        //    {
        //        //attach new entity
        //        Set<TEntity>().Attach(entity);
        //        return entity;
        //    }

        //    //entity is already loaded
        //    return alreadyAttached;
        //}

        /// <summary>
        /// 实体集合
        /// new的用法搜索 override new
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public new IDbSet<T> Set<T>() where T : BaseEntity
        {
            return base.Set<T>();
        }

    }

    /// <summary>
    /// EF容器上下文
    /// EF做映射的时候也可以数据库是int，代码里是string，
    /// 用attribute mapping测试成功，用fluent测试失败
    /// </summary>
#if use_mysql_
    [DbConfigurationType(typeof(MySqlConfiguration))]
#else
    [DbConfigurationType(typeof(SqlServerConfiguration))]
#endif
    public class EntityDB : BaseEFContext
    {
        public EntityDB() : base(ConfigurationManager.ConnectionStrings["db"]?.ConnectionString
            ?? throw new Exception("请配置db数据库链接字符串"))
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ValidateOnSaveEnabled = false;

            this.Database.CommandTimeout = 3;
            this.Database.Log = (log) =>
            {
                //if (!ValidateHelper.IsPlumpString(log)) { return; }
                //LogHelper.Error(typeof(EntityDB), "EF日志：\n" + log);
            };
        }

        #region model对应的实体

        public virtual DbSet<AuthClient> AuthClient { get; set; }
        public virtual DbSet<AuthScope> AuthScope { get; set; }
        public virtual DbSet<AuthToken> AuthToken { get; set; }
        public virtual DbSet<AuthTokenScope> AuthTokenScope { get; set; }
        public virtual DbSet<AuthCode> AuthCode { get; set; }

        public virtual DbSet<AuthClientCheckLog> AuthClientCheckLog { get; set; }
        public virtual DbSet<AuthClientUseage> AuthClientUseage { get; set; }

        public virtual DbSet<UserAvatar> UserAvatar { get; set; }
        public virtual DbSet<UserModel> UserModel { get; set; }

        public virtual DbSet<LoginErrorLogModel> LoginErrorLogModel { get; set; }
        public virtual DbSet<RoleModel> RoleModel { get; set; }
        public virtual DbSet<RolePermissionModel> RolePermissionModel { get; set; }
        public virtual DbSet<PermissionModel> PermissionModel { get; set; }
        public virtual DbSet<UserRoleModel> UserRoleModel { get; set; }
        public virtual DbSet<UserOneTimeCode> UserOneTimeCode { get; set; }

        public virtual DbSet<CategoryModel> CategoryModel { get; set; }
        public virtual DbSet<SectionModel> SectionModel { get; set; }
        public virtual DbSet<AreaModel> AreaModel { get; set; }
        public virtual DbSet<CommentModel> CommentModel { get; set; }
        public virtual DbSet<LinkModel> LinkModel { get; set; }
        public virtual DbSet<MessageModel> MessageModel { get; set; }
        public virtual DbSet<OptionModel> OptionModel { get; set; }
        public virtual DbSet<ReqLogModel> ReqLogModel { get; set; }
        public virtual DbSet<CacheHitLog> CacheHitLog { get; set; }
        public virtual DbSet<UpFileModel> UpFileModel { get; set; }
        public virtual DbSet<TagMapModel> TagMapModel { get; set; }
        public virtual DbSet<TagModel> TagModel { get; set; }
        #endregion
    }
}
