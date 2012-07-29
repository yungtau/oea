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
using hxy.Common;

namespace OEA
{
    /// <summary>
    /// �� C/S��B/S �ķ������
    /// 
    /// ע�⣬����÷���Ҫ��ʹ�õ� B/S �ϣ���������������Ӧ���ǻ������������͡�EntityList ���͡�
    /// </summary>
    [Serializable]
    public abstract class Service : IService
    {
        public Service()
        {
            //���ڷ����ʱ��Ĭ��ֵΪ true����ʾֱ���ڷ�������С�
            this._runAtLocal = OEAEnvironment.Location.IsOnServer();
        }

        [NonSerialized]
        private bool _runAtLocal;

        /// <summary>
        /// ��ǰ�����Ƿ���Ҫ�ڱ������С�
        /// 
        /// ���ڷ����ʱ��Ĭ��ֵΪ true����ʾֱ���ڷ�������С�
        /// </summary>
        public bool RunAtLocal
        {
            get { return this._runAtLocal; }
            set { this._runAtLocal = value; }
        }

        /// <summary>
        /// ������д�˷���ʵ�־����ҵ���߼�
        /// </summary>
        internal void ExecuteByDataPortal()
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
            this.OnInvoking();

            if (this.RunAtLocal)
            {
                //�����ڱ��أ�����û�б������ ExecuteByDataPortal ��������õ����ԡ�
                this.Execute();
            }
            else
            {
                var res = DataPortal.Update(this) as IService;

                this.ReadOutput(res);
            }

            this.OnInvoked();
        }

        /// <summary>
        /// ʹ�÷���ѷ��ؽ����ֵ�޸ĵ���ǰ�����ϡ�
        /// </summary>
        /// <param name="res"></param>
        private void ReadOutput(IService res)
        {
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

        /// <summary>
        /// �ڷ��񱻵���ǰ������
        /// </summary>
        protected virtual void OnInvoking() { }

        /// <summary>
        /// �ڷ��񱻵��ú�����
        /// </summary>
        protected virtual void OnInvoked() { }

        protected bool IsWeb { get { return OEAEnvironment.IsWeb; } }
        protected bool IsWPF { get { return OEAEnvironment.IsWPF; } }
    }

    /// <summary>
    /// һ�ֹ��̻�����Ļ���
    /// 
    /// ���̻��򵥵�ָ������һϵ�в����������Ƿ�ɹ��Լ���Ӧ����ʾ��Ϣ��
    /// </summary>
    [Serializable]
    public abstract class FlowService : Service
    {
        [ServiceOutput(OutputToWeb = false)]
        public Result Result { get; set; }

        #region Web ����Ĳ���
        //������������ ClientResult ���͵�����������һ�£�����ͻ���ʹ�á�

        [ServiceOutput]
        public bool success { get; set; }
        [ServiceOutput]
        public string msg { get; set; }

        #endregion

        protected override sealed void Execute()
        {
            var res = this.ExecuteCore();

            this.success = res.Success;
            this.msg = res.Message;

            this.Result = res;
        }

        protected abstract Result ExecuteCore();
    }
}