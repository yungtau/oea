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
using OEA.ManagedProperty;
using OEA;

namespace OEA
{
    /// <summary>
    /// �� C/S��B/S �ķ������
    /// </summary>
    [Serializable]
    public abstract class Service : IService
    {
        /// <summary>
        /// ������д�˷���ʵ�־����ҵ���߼�
        /// </summary>
        internal void ExecuteInternal()
        {
            this.Execute();
        }

        protected abstract void Execute();

        public IService Invoke()
        {
            return DataPortal.Update(this) as IService;
        }

        public void Invoke<T>(out T svcReturn)
            where T : IService
        {
            svcReturn = (T)this.Invoke();
        }
    }
}