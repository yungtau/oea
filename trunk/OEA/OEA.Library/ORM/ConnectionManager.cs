/*******************************************************
 * 
 * ���ߣ�CSLA
 * ����ʱ�䣺2009
 * ˵�������ļ�ֻ����һ���࣬�������ݼ�����ע�͡�
 * ���л�����.NET 4.0
 * �汾�ţ�1.0.0
 * 
 * ��ʷ��¼��
 * �����ļ� ����� 2009
 * 
*******************************************************/

using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using hxy.Common.Data;

namespace OEA.Library
{
    /// <summary>
    /// �����Լ��洢�� ApplicationContext.LocalContext �У�
    /// ���ṩһ���򵥵ķ�ʽ����һ�����������Ļ��������õ������ӡ�
    /// </remarks>
    public class ConnectionManager : IDisposable
    {
        private static object _lock = new object();

        private IDbConnection _connection;

        private DbSetting _dbSetting;

        /// <summary>
        /// �������ݿ����û�ȡһ�����ӹ�����
        /// </summary>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        public static ConnectionManager GetManager(DbSetting dbSetting)
        {
            lock (_lock)
            {
                var ctxName = GetContextName(dbSetting);
                ConnectionManager mgr = null;
                if (ApplicationContext.LocalContext.Contains(ctxName))
                {
                    mgr = ApplicationContext.LocalContext[ctxName] as ConnectionManager;
                }
                else
                {
                    mgr = new ConnectionManager(dbSetting);
                    ApplicationContext.LocalContext[ctxName] = mgr;

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
        /// ��Ӧ�����ݿ�������Ϣ
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

                //��������Ϊ 0 ʱ��Ӧ���ͷ����ӡ�
                if (_refCount == 0)
                {
                    _connection.Dispose();
                    var name = GetContextName(this._dbSetting);
                    ApplicationContext.LocalContext.Remove(name);
                }
            }
        }

        #region  IDisposable

        /// <summary>
        /// Dispose ʱ����������
        /// </summary>
        public void Dispose()
        {
            DeRef();
        }

        #endregion
    }
}