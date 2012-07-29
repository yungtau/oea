/*******************************************************
 * 
 * ���ߣ������
 * ����ʱ�䣺20120330
 * ˵�������ļ�ֻ����һ���࣬�������ݼ�����ע�͡�
 * ���л�����.NET 4.0
 * �汾�ţ�1.0.0
 * 
 * ��ʷ��¼��
 * �����ļ� ����� 20120330
 * 
*******************************************************/

using System;
using OEA.ManagedProperty;
using System.Dynamic;
using OEA.MetaModel.View;
using OEA.MetaModel;
using System.Collections.Generic;
using OEA.Reflection;

namespace OEA.Library.Validation
{
    /// <summary>
    /// Ϊҵ�������֤�����ṩһЩ��Ҫ�Ĳ�����
    /// 
    /// ����̳��Զ�̬���ͣ���ζ�Ŷ���ʱ�ɶ�̬�������ԡ�
    /// </summary>
    public sealed class RuleArgs : ICustomParamsHolder
    {
        /// <summary>
        /// �������ĳ�����Թ����Ĺ����������������Ա�ʾ�������й�����
        /// </summary>
        public IManagedProperty Property { get; internal set; }

        /// <summary>
        /// Gets or sets the severity of the broken rule.
        /// </summary>
        /// <value>The severity of the broken rule.</value>
        /// <remarks>
        /// Setting this property only has an effect if
        /// the rule method returns <see langword="false" />.
        /// </remarks>
        public RuleLevel Level { get; internal set; }

        /// <summary>
        /// �����Ƿ�ִ�д����˹���
        /// </summary>
        public bool IsBroken
        {
            get { return !string.IsNullOrEmpty(this.BrokenDescription); }
        }

        /// <summary>
        /// �ڹ����麯����������ǰ�Ĵ�����Ϣ��
        /// </remarks>
        public string BrokenDescription { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this
        /// broken rule should stop the processing of subsequent
        /// rules for this property.
        /// </summary>
        /// <value><see langword="true" /> if no further
        /// rules should be process for this property.</value>
        /// <remarks>
        /// Setting this property only has an effect if
        /// the rule method returns <see langword="false" />.
        /// </remarks>
        public bool StopProcessing { get; set; }

        /// <summary>
        /// Returns a string representation of the object.
        /// </summary>
        public override string ToString()
        {
            var p = this.Property;
            if (p != null) return this.Property.Name;

            return "TypeRules";
        }

        #region GetPropertyDisplay

        private static EntityViewMeta _lastViewMeta;

        /// <summary>
        /// ��ȡ��ǰ���Ե���ʾ���ơ�
        /// 
        /// ��������һ���̰߳�ȫ�ķ�ʽȥ�������һ��ʹ�õ� EVM��
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public string GetPropertyDisplay()
        {
            EntityViewMeta safeView = _lastViewMeta;

            var ownerType = this.Property.OwnerType;
            if (safeView == null || safeView.EntityType != ownerType)
            {
                safeView = UIModel.Views.CreateDefaultView(ownerType);
                _lastViewMeta = safeView;
            }

            string res = null;

            var pvm = safeView.Property(this.Property);
            if (pvm != null) res = pvm.Label;

            return res ?? this.Property.Name;
        }

        #endregion

        #region ICustomParamsHolder Members

        private Dictionary<string, object> _customParams = new Dictionary<string, object>();

        /// <summary>
        /// ��ȡָ��������ֵ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public T TryGetCustomParams<T>(string paramName)
        {
            object result;

            if (_customParams.TryGetValue(paramName, out result))
            {
                return (T)TypeHelper.CoerceValue(typeof(T), result);
            }

            return default(T);
        }

        /// <summary>
        /// �����Զ������
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        public void SetCustomParams(string paramName, object value)
        {
            this._customParams[paramName] = value;
        }

        public IEnumerable<KeyValuePair<string, object>> GetAllCustomParams()
        {
            return this._customParams;
        }

        #endregion
    }
}