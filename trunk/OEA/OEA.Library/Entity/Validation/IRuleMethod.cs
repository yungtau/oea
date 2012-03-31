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

namespace OEA.Library.Validation
{
    /// <summary>
    /// Tracks all information for a rule.
    /// </summary>
    public interface IRuleMethod
    {
        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        /// <remarks>
        /// The rule's name must be unique and is used
        /// to identify a broken rule in the BrokenRules
        /// collection.
        /// </remarks>
        string RuleLabel { get; }

        /// <summary>
        /// Returns the name of the field, property or column
        /// to which the rule applies.
        /// </summary>
        RuleArgs RuleArgs { get; }

        /// <summary>
        /// Gets the priority of the rule method.
        /// </summary>
        /// <value>The priority value.</value>
        /// <remarks>
        /// Priorities are processed in descending
        /// order, so priority 0 is processed
        /// before priority 1, etc.</remarks>
        int Priority { get; }

        /// <summary>
        /// Invokes the rule to validate the data.
        /// </summary>
        /// <returns>
        /// <see langword="true" /> if the data is valid, 
        /// <see langword="false" /> if the data is invalid.
        /// </returns>
        void Check(Entity target);
    }
}