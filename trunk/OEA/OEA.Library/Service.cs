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

            //�������Ҫ�����ã��������ݴ��䡣
            var properties = this.GetType().GetProperties();
            foreach (var property in properties)
            {
                if (property.HasMarked<ServiceInputAttribute>())
                {
                    if (property.PropertyType.IsClass)
                    {
                        try
                        {
                            property.SetValue(this, null, null);
                        }
                        catch { }
                    }
                }
            }
        }

        protected abstract void Execute();

        /// <summary>
        /// ���÷��񲢰ѷ���ֵת��Ϊָ�������͡�
        /// </summary>
        /// <returns></returns>
        public void Invoke()
        {
            var res = DataPortal.Update(this) as IService;

            //ʹ�÷���ѷ��ؽ����ֵ�޸ĵ���ǰ�����ϡ�
            var properties = this.GetType().GetProperties();
            foreach (var property in properties)
            {
                if (property.HasMarked<ServiceOutputAttribute>())
                {
                    var value = property.GetValue(res, null);

                    try
                    {
                        property.SetValue(this, value, null);
                    }
                    catch { }
                }
            }
        }
    }
}