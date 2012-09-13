/*******************************************************
 * 
 * ���ߣ������
 * ����ʱ�䣺2012
 * ˵�������ļ�ֻ����һ���࣬�������ݼ�����ע�͡�
 * ���л�����.NET 4.0
 * �汾�ţ�1.0.0
 * 
 * ��ʷ��¼��
 * �����ļ� ����� 2012
 * 
*******************************************************/

using System;
using System.ComponentModel;
using OEA;
using OEA.Library;
using OEA.Reflection;
using OEA.Server;
using OEA.DataPortalClient;
using System.Security.Principal;

namespace OEA
{
    /// <summary>
    /// This is the client-side DataPortal as described in
    /// Chapter 4.
    /// </summary>
    public static class DataPortal
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="criteria"></param>
        /// <param name="runAtLocal">���һ�����ݲ㷽����Ҫ�ڱ���ִ�У�Ӧ���ڰѱ�����ָ��Ϊ true��</param>
        /// <returns></returns>
        public static object Fetch(Type objectType, object criteria, DataPortalLocation loc = DataPortalLocation.Remote)
        {
            var proxy = GetDataPortalProxy(loc);

            var dpContext = new Server.DataPortalContext(GetPrincipal(), proxy.IsServerRemote);

            Server.DataPortalResult result = null;

            try
            {
                result = proxy.Fetch(objectType, criteria, dpContext);
            }
            finally
            {
                if (proxy.IsServerRemote && result != null) { ApplicationContext.SetGlobalContext(result.GlobalContext); }
            }

            return result.ReturnObject;
        }

        /// <summary>
        /// Called by the business object's Save() method to
        /// insert, update or delete an object in the database.
        /// </summary>
        /// <remarks>
        /// Note that this method returns a reference to the updated business object.
        /// If the server-side DataPortal is running remotely, this will be a new and
        /// different object from the original, and all object references MUST be updated
        /// to use this new object.
        /// </remarks>
        /// <param name="obj">A reference to the business object to be updated.</param>
        /// <returns>A reference to the updated business object.</returns>
        public static object Update(object obj, DataPortalLocation loc = DataPortalLocation.Remote)
        {
            var proxy = GetDataPortalProxy(loc);

            var dpContext = new Server.DataPortalContext(GetPrincipal(), proxy.IsServerRemote);

            var result = proxy.Update(obj, dpContext);

            if (proxy.IsServerRemote) ApplicationContext.SetGlobalContext(result.GlobalContext);

            return result.ReturnObject;
        }

        #region Helpers

        private static Type _proxyType;

        private static IDataPortalProxy GetDataPortalProxy(DataPortalLocation loc)
        {
            if (loc == DataPortalLocation.Local) return new LocalProxy();

            if (_proxyType == null)
            {
                string proxyTypeName = ApplicationContext.DataPortalProxy;
                if (proxyTypeName == "Local") return new LocalProxy();

                _proxyType = Type.GetType(proxyTypeName, true, true);
            }
            return Activator.CreateInstance(_proxyType) as IDataPortalProxy;
        }

        private static IPrincipal GetPrincipal()
        {
            if (ApplicationContext.AuthenticationType == "Windows")
            {
                // Windows integrated security
                return null;
            }
            else
            {
                // we assume using the CSLA framework security
                return ApplicationContext.User;
            }
        }

        #endregion
    }

    /// <summary>
    /// ���ݷ��ʲ�ִ�еĵص�
    /// </summary>
    public enum DataPortalLocation
    {
        /// <summary>
        /// ��Զ�̷����ִ��
        /// </summary>
        Remote,
        /// <summary>
        /// �ڱ���ִ�У������ǿͻ���Ҳ�����Ƿ���ˣ���
        /// </summary>
        Local,
    }
}