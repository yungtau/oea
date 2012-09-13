/*******************************************************
 * 
 * ���ߣ������
 * ����ʱ�䣺20120330
 * ˵�������ļ�ֻ����һ���࣬�������ݼ�����ע�͡�
 * ���л�����.NET 4.0
 * �汾�ţ�1.0.0
 * 
 * ��ʷ��¼��
 * ���䵽 Entity���й������ϡ� ����� 20120330
 * 
*******************************************************/

using System;
using System.Runtime.Serialization;
using OEA.ManagedProperty;
using OEA;
using OEA.Serialization.Mobile;

namespace OEA.Library.Validation
{
    /// <summary>
    /// Stores details about a specific broken business rule.
    /// </summary>
    public class BrokenRule
    {
        internal BrokenRule(IRuleMethod rule)
        {
            RuleLabel = rule.RuleLabel;
            Description = rule.RuleArgs.BrokenDescription;
            Property = rule.RuleArgs.Property;
            Level = rule.RuleArgs.Level;
        }

        //internal BrokenRule(string source, BrokenRule rule)
        //{
        //    RuleLabel = string.Format("rule://{0}.{1}", source, rule.RuleLabel.Replace("rule://", string.Empty));
        //    Description = rule.Description;
        //    Property = rule.Property;
        //    Level = rule.Level;
        //}

        /// <summary>
        /// Provides access to the name of the broken rule.
        /// </summary>
        /// <value>The name of the rule.</value>
        public string RuleLabel { get; private set; }

        /// <summary>
        /// Provides access to the description of the broken rule.
        /// </summary>
        /// <value>The description of the rule.</value>
        public string Description { get; private set; }

        /// <summary>
        /// �������ĳ�����Թ����Ĺ����������������Ա�ʾ�������й�����
        /// </summary>
        public IManagedProperty Property { get; private set; }

        /// <summary>
        /// Gets the severity of the broken rule.
        /// </summary>
        public RuleLevel Level { get; private set; }
    }
}