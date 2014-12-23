﻿/*******************************************************
 * 
 * 作者：CSLA
 * 创建时间：2009
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 运行环境：.NET 4.0
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 胡庆访 2009
 * 
*******************************************************/

using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using Rafy;
using Rafy.Data;

namespace Rafy.Domain.ORM
{
    /// <summary>
    /// 它把自己存储在 ServerContext.Items 中，
    /// 以提供一个简单的方式来在一个数据上下文环境中重用单个连接。
    /// </summary>
    internal class ConnectionManager : IDisposable
    {
        private static object _lock = new object();

        private IDbConnection _connection;

        private DbSetting _dbSetting;

        /// <summary>
        /// 根据数据库配置获取一个连接管理器
        /// </summary>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static ConnectionManager GetManager(DbSetting dbSetting)
        {
            var ctxName = GetContextName(dbSetting);

            lock (_lock)
            {
                var items = AppContext.Items;

                object value = null;
                items.TryGetValue(ctxName, out value);
                var mgr = value as ConnectionManager;
                if (mgr == null)
                {
                    mgr = new ConnectionManager(dbSetting);
                    items.Add(ctxName, mgr);

                    mgr._connection.Open();
                }

                mgr.AddRef();
                return mgr;
            }
        }

        private ConnectionManager(DbSetting dbSetting)
        {
            this._dbSetting = dbSetting;
            DbProviderFactory factory = DbProviderFactories.GetFactory(dbSetting.ProviderName);

            // open connection
            _connection = factory.CreateConnection();
            _connection.ConnectionString = dbSetting.ConnectionString;
        }

        private static string GetContextName(DbSetting dbSetting)
        {
            return "__db:" + dbSetting.Name;
        }

        /// <summary>
        /// Dispose object, dereferencing or
        /// disposing the connection it is
        /// managing.
        /// </summary>
        public IDbConnection Connection
        {
            get { return this._connection; }
        }

        /// <summary>
        /// 对应的数据库配置信息
        /// </summary>
        public DbSetting DbSetting
        {
            get { return this._dbSetting; }
        }

        private int _refCount;

        private void AddRef()
        {
            _refCount += 1;
        }

        private void DeRef()
        {
            lock (_lock)
            {
                _refCount -= 1;

                //当引用数为 0 时，应该释放连接。
                if (_refCount == 0)
                {
                    _connection.Dispose();
                    var name = GetContextName(this._dbSetting);
                    AppContext.Items.Remove(name);
                }
            }
        }

        #region  IDisposable

        /// <summary>
        /// Dispose 时减少引用数
        /// </summary>
        public void Dispose()
        {
            DeRef();
        }

        #endregion
    }
}