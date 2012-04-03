/*******************************************************
 * 
 * ���ߣ������
 * ����ʱ�䣺20120220
 * ˵�������ļ�ֻ����һ���࣬�������ݼ�����ע�͡�
 * ���л�����.NET 4.0
 * �汾�ţ�1.0.0
 * 
 * ��ʷ��¼��
 * �����ļ� ����� 20120220
 * 
*******************************************************/

using System;
using System.ComponentModel;
using SimpleCsla.Server;

using SimpleCsla.Core;


using SimpleCsla.DataPortalClient;
using OEA.ManagedProperty;
using SimpleCsla;

namespace OEA
{
    /// <summary>
    /// �� C/S��B/S �ķ������
    /// </summary>
    [Serializable]
    public abstract class Service
    {
        /// <summary>
        /// ������д�˷���ʵ�־����ҵ���߼�
        /// </summary>
        internal protected abstract void Execute();

        public Service Invoke()
        {
            return DataPortal.Update(this) as Service;
        }

        public void Invoke<T>(out T svcReturn)
            where T : Service
        {
            svcReturn = this.Invoke() as T;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ServiceInputAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ServiceOutputAttribute : Attribute { }
}