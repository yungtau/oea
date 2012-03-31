/*******************************************************
 * 
 * ���ߣ������
 * ����ʱ�䣺20120327
 * ˵�������ļ�ֻ����һ���࣬�������ݼ�����ע�͡�
 * ���л�����.NET 4.0
 * �汾�ţ�1.0.0
 * 
 * ��ʷ��¼��
 * ���䵽 Entity���й������ϡ� ����� 20120327
 * 
*******************************************************/

using System;
using System.Collections.Generic;
using OEA.ManagedProperty;

namespace OEA.Library.Validation
{
    /// <summary>
    /// Maintains rule methods for a business object
    /// or business object type.
    /// </summary>
    public class ValidationRulesManager
    {
        private RulesContainer _typeRules = new RulesContainer();

        private Dictionary<IManagedProperty, RulesContainer> _propertyRulesList;

        /// <summary>
        /// Key: IManagedProperty
        /// Value: Rules
        /// </summary>
        internal Dictionary<IManagedProperty, RulesContainer> PropertyRules
        {
            get
            {
                if (_propertyRulesList == null) _propertyRulesList = new Dictionary<IManagedProperty, RulesContainer>();
                return _propertyRulesList;
            }
        }

        /// <summary>
        /// ��Щ������ĳ�����Թ�������ֱ������������ʵ���ϵġ�
        /// </summary>
        internal RulesContainer TypeRules
        {
            get { return this._typeRules; }
        }

        internal RulesContainer GetRulesForProperty(IManagedProperty property, bool createList)
        {
            // get the list (if any) from the dictionary
            RulesContainer list = null;
            PropertyRules.TryGetValue(property, out list);

            if (createList && list == null)
            {
                // there is no list for this name - create one
                list = new RulesContainer();
                PropertyRules.Add(property, list);
            }

            return list;
        }

        public void AddRule(RuleHandler handler, RuleArgs args, int priority)
        {
            if (args.Property != null)
            {
                // we have the list, add our new rule
                GetRulesForProperty(args.Property, true)
                    .Add(new RuleMethod(handler, args, priority));
            }
            else
            {
                this._typeRules
                    .Add(new RuleMethod(handler, args, priority));
            }
        }
    }
}