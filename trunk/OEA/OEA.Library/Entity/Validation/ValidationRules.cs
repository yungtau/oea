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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using OEA;
using OEA.ManagedProperty;
using OEA.Serialization.Mobile;
using System.ComponentModel;

namespace OEA.Library.Validation
{
    /// <summary>
    /// Tracks the business rules broken within a business object.
    /// </summary>
    [Serializable]
    public class ValidationRules : MobileObject
    {
        /// <summary>
        /// For serialization
        /// </summary>
        private ValidationRules() { }

        internal ValidationRules(Entity entity)
        {
            SetTarget(entity);
        }

        #region Target ����

        /// <summary>
        /// reference to current business object
        /// </summary>
        [NonSerialized]
        private Entity _target;

        internal void SetTarget(Entity entity)
        {
            this._target = entity;
        }

        internal Entity Target
        {
            get { return this._target; }
        }

        #endregion

        #region �����ں�

        /// <summary>
        /// reference to per-instance rules manager for this object
        /// </summary>
        [NonSerialized]
        private ValidationRulesManager _instanceRules;
        /// <summary>
        /// reference to the active set of rules for this object
        /// </summary>
        [NonSerialized]
        private ValidationRulesManager _rulesToCheck;

        private ValidationRulesManager GetInstanceRules(bool createObject)
        {
            if (_instanceRules == null && createObject)
            {
                _instanceRules = new ValidationRulesManager();

                this._target.AddInstanceValidations();
            }

            return _instanceRules;
        }

        private ValidationRulesManager GetTypeRules()
        {
            //���͵Ĺ��򣬶����� Repository �ϡ�
            return this._target.FindRepository().CastTo<ITypeValidationsHost>().Rules;
        }

        /// <summary>
        /// ������ InstanceRules �� TypeRules �������ϵĹ����б�
        /// </summary>
        private ValidationRulesManager RulesToCheck
        {
            get
            {
                if (_rulesToCheck == null)
                {
                    var instanceRules = GetInstanceRules(false);
                    var typeRules = GetTypeRules();

                    if (instanceRules == null)
                    {
                        _rulesToCheck = typeRules;
                    }
                    else if (typeRules == null)
                    {
                        _rulesToCheck = instanceRules;
                    }
                    else
                    {
                        // both have values - consolidate into instance rules

                        foreach (var pr in typeRules.PropertyRules)
                        {
                            var rules = instanceRules.GetRulesForProperty(pr.Key, true);
                            rules.GetList(false).AddRange(pr.Value.GetList(false));
                        }

                        instanceRules.TypeRules.GetList(false)
                            .AddRange(typeRules.TypeRules.GetList(false));

                        _rulesToCheck = instanceRules;
                    }
                }
                return _rulesToCheck;
            }
        }

        #endregion

        #region ������

        /// <summary>
        /// ���ĳ�������Ƿ��������
        /// </summary>
        /// <param name="property">�й�����</param>
        public BrokenRulesCollection CheckRules(IManagedProperty property)
        {
            var res = new BrokenRulesCollection();

            if (!_suppressRuleChecking)
            {
                // get the rules dictionary
                ValidationRulesManager rules = RulesToCheck;
                if (rules != null)
                {
                    // get the rules list for this property
                    RulesContainer rulesList = rules.GetRulesForProperty(property, false);
                    if (rulesList != null)
                    {
                        // get the actual list of rules (sorted by priority)
                        CheckRules(rulesList, res);
                    }

                    CheckRules(rules.TypeRules, res);
                }
            }

            return res;
        }

        /// <summary>
        /// �������ʵ������Ƿ��������
        /// </summary>
        public BrokenRulesCollection CheckRules()
        {
            var res = new BrokenRulesCollection();

            if (!_suppressRuleChecking)
            {
                var rules = this.RulesToCheck;
                if (rules != null)
                {
                    foreach (var de in rules.PropertyRules) { this.CheckRules(de.Value, res); }

                    CheckRules(rules.TypeRules, res);
                }
            }

            return res;
        }

        private void CheckRules(ValidationRulesManager rules, IManagedProperty property, BrokenRulesCollection brokenRulesList)
        {
            // get the rules list for this property
            RulesContainer rulesList = rules.GetRulesForProperty(property, false);
            if (rulesList != null)
            {
                // get the actual list of rules (sorted by priority)
                this.CheckRules(rulesList, brokenRulesList);
            }
        }

        /// <summary>
        /// Given a list
        /// containing IRuleMethod objects, this
        /// method executes all those rule methods.
        /// </summary>
        private void CheckRules(RulesContainer rules, BrokenRulesCollection brokenRulesList)
        {
            var list = rules.GetList(true);

            bool previousRuleBroken = false;

            // Lock the rules here to ensure that all rules are run before allowing
            // async rules to notify that they have completed.

            for (int index = 0; index < list.Count; index++)
            {
                IRuleMethod rule = list[index];
                // see if short-circuiting should kick in
                if (previousRuleBroken && rule.Priority > _processThroughPriority) continue;

                var args = rule.RuleArgs;

                // we're not short-circuited, so check rule
                try
                {
                    args.BrokenDescription = null;
                    rule.Check(this._target);
                }
                catch (Exception ex)
                {
                    throw new ValidationException("Properties.Resources.ValidationRulesException" + args.Property.Name + rule.RuleLabel, ex);
                }

                if (args.IsBroken)
                {
                    // the rule is broken
                    brokenRulesList.Add(rule);
                    if (args.Level == RuleLevel.Error)
                    {
                        previousRuleBroken = true;
                    }
                }

                if (rule.RuleArgs.StopProcessing)
                {
                    // reset the value for next time
                    rule.RuleArgs.StopProcessing = false;
                }
            }
        }

        #endregion

        #region /*Adding Instance Rules ��ʱ��֧��*/

        //��ʱ��֧�� InstanceRules���� TypeRules ������ʱ����ӡ�

        ///// <summary>
        ///// Adds a rule to the list of rules to be enforced.
        ///// </summary>
        ///// <remarks>
        ///// <para>
        ///// A rule is implemented by a method which conforms to the 
        ///// method signature defined by the RuleHandler delegate.
        ///// </para><para>
        ///// The propertyName may be used by the method that implements the rule
        ///// in order to retrieve the value to be validated. If the rule
        ///// implementation is inside the target object then it probably has
        ///// direct access to all data. However, if the rule implementation
        ///// is outside the target object then it will need to use reflection
        ///// or CallByName to dynamically invoke this property to retrieve
        ///// the value to be validated.
        ///// </para>
        ///// </remarks>
        ///// <param name="handler">The method that implements the rule.</param>
        ///// <param name="propertyName">
        ///// The property name on the target object where the rule implementation can retrieve
        ///// the value to be validated.
        ///// </param>
        //public void AddInstanceRule(RuleHandler handler, string propertyName)
        //{
        //    GetInstanceRules(true).AddRule(handler, new RuleArgs(propertyName), 0);
        //}

        ///// <summary>
        ///// Adds a rule to the list of rules to be enforced.
        ///// </summary>
        ///// <remarks>
        ///// <para>
        ///// A rule is implemented by a method which conforms to the 
        ///// method signature defined by the RuleHandler delegate.
        ///// </para><para>
        ///// The propertyName may be used by the method that implements the rule
        ///// in order to retrieve the value to be validated. If the rule
        ///// implementation is inside the target object then it probably has
        ///// direct access to all data. However, if the rule implementation
        ///// is outside the target object then it will need to use reflection
        ///// or CallByName to dynamically invoke this property to retrieve
        ///// the value to be validated.
        ///// </para>
        ///// </remarks>
        ///// <param name="handler">The method that implements the rule.</param>
        ///// <param name="propertyName">
        ///// The property name on the target object where the rule implementation can retrieve
        ///// the value to be validated.
        ///// </param>
        ///// <param name="priority">
        ///// The priority of the rule, where lower numbers are processed first.
        ///// </param>
        //public void AddInstanceRule(RuleHandler handler, string propertyName, int priority)
        //{
        //    GetInstanceRules(true).AddRule(handler, new RuleArgs(propertyName), priority);
        //}

        ///// <summary>
        ///// Adds a rule to the list of rules to be enforced.
        ///// </summary>
        ///// <remarks>
        ///// A rule is implemented by a method which conforms to the 
        ///// method signature defined by the RuleHandler delegate.
        ///// </remarks>
        ///// <param name="handler">The method that implements the rule.</param>
        ///// <param name="args">
        ///// A RuleArgs object specifying the property name and other arguments
        ///// passed to the rule method
        ///// </param>
        //public void AddInstanceRule(RuleHandler handler, RuleArgs args)
        //{
        //    GetInstanceRules(true).AddRule(handler, args, 0);
        //}

        ///// <summary>
        ///// Adds a rule to the list of rules to be enforced.
        ///// </summary>
        ///// <remarks>
        ///// A rule is implemented by a method which conforms to the 
        ///// method signature defined by the RuleHandler delegate.
        ///// </remarks>
        ///// <param name="handler">The method that implements the rule.</param>
        ///// <param name="args">
        ///// A RuleArgs object specifying the property name and other arguments
        ///// passed to the rule method
        ///// </param>
        ///// <param name="priority">
        ///// The priority of the rule, where lower numbers are processed first.
        ///// </param>
        //public void AddInstanceRule(RuleHandler handler, RuleArgs args, int priority)
        //{
        //    GetInstanceRules(true).AddRule(handler, args, priority);
        //}

        #endregion

        #region Adding Shared Rules

        /// <summary>
        /// Ϊ����ʵ�����һ��ҵ����֤����
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="level"></param>
        /// <param name="priority"></param>
        public void AddRule(RuleHandler handler,
            object parameters = null, RuleLevel level = RuleLevel.Error, int priority = 0
            )
        {
            ValidateHandler(handler);

            var args = new RuleArgs { Level = level };

            if (parameters != null)
            {
                var properties = TypeDescriptor.GetProperties(parameters);
                foreach (PropertyDescriptor propertyDescriptor in properties)
                {
                    object value = propertyDescriptor.GetValue(parameters);
                    args.SetCustomParams(propertyDescriptor.Name, value);
                }
            }

            GetTypeRules().AddRule(handler, args, priority);
        }

        /// <summary>
        /// Ϊĳ���������һ��ҵ����֤����
        /// </summary>
        /// <param name="property"></param>
        /// <param name="handler"></param>
        /// <param name="level"></param>
        /// <param name="priority"></param>
        public void AddRule(IManagedProperty property, RuleHandler handler,
            object parameters = null, RuleLevel level = RuleLevel.Error, int priority = 0
            )
        {
            ValidateHandler(handler);

            var args = new RuleArgs
            {
                Property = property,
                Level = level
            };

            if (parameters != null)
            {
                var properties = TypeDescriptor.GetProperties(parameters);
                foreach (PropertyDescriptor propertyDescriptor in properties)
                {
                    object value = propertyDescriptor.GetValue(parameters);
                    args.SetCustomParams(propertyDescriptor.Name, value);
                }
            }

            GetTypeRules().AddRule(handler, args, priority);
        }

        private void ValidateHandler(RuleHandler handler)
        {
            if (handler == null) throw new ArgumentNullException("handler");

            var method = handler.Method;
            if (!method.IsStatic && method.DeclaringType.IsInstanceOfType(_target))
                throw new InvalidOperationException(string.Format("{0}: {1}", "Properties.Resources.InvalidRuleMethodException", method.Name));
        }

        #endregion

        #region ��������������

        /// <summary>
        /// threshold for short-circuiting to kick in
        /// </summary>
        private int _processThroughPriority;
        /// <summary>
        /// Gets or sets the priority through which
        /// CheckRules should process before short-circuiting
        /// processing on broken rules.
        /// </summary>
        /// <value>Defaults to 0.</value>
        /// <remarks>
        /// All rules for each property are processed by CheckRules
        /// though this priority. Rules with lower priorities are
        /// only processed if no previous rule has been marked as
        /// broken.
        /// </remarks>
        public int ProcessThroughPriority
        {
            get { return _processThroughPriority; }
            set { _processThroughPriority = value; }
        }

        private bool _suppressRuleChecking;
        /// <summary>
        /// Gets or sets a value indicating whether calling
        /// CheckRules should result in rule
        /// methods being invoked.
        /// </summary>
        /// <value>True to suppress all rule method invocation.</value>
        public bool SuppressRuleChecking
        {
            get { return _suppressRuleChecking; }
            set { _suppressRuleChecking = value; }
        }

        /// <summary>
        /// Returns an array containing the text descriptions of all
        /// validation rules associated with this object.
        /// </summary>
        /// <returns>String array.</returns>
        /// <remarks></remarks>
        public string[] GetRuleDescriptions()
        {
            List<string> result = new List<string>();
            ValidationRulesManager rules = RulesToCheck;
            if (rules != null)
            {
                foreach (var de in rules.PropertyRules)
                {
                    var list = de.Value.GetList(false);
                    for (int i = 0; i < list.Count; i++)
                    {
                        result.Add(list[i].ToString());
                    }
                }
            }
            return result.ToArray();
        }

        #endregion
    }
}