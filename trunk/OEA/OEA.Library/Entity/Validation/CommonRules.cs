/*******************************************************
 * 
 * ���ߣ������
 * ����ʱ�䣺20120331
 * ˵�������ļ�ֻ����һ���࣬�������ݼ�����ע�͡�
 * ���л�����.NET 4.0
 * �汾�ţ�1.0.0
 * 
 * ��ʷ��¼��
 * �����ļ� ����� 20120331
 * 
*******************************************************/

using System;
using System.Text.RegularExpressions;
using System.Reflection;
using OEA.ManagedProperty;
using OEA;

namespace OEA.Library.Validation
{
    /// <summary>
    /// Implements common business rules.
    /// </summary>
    public static class CommonRules
    {
        /// <summary>
        /// ��Ϊ��(null)��
        /// </summary>
        /// <param name="target"></param>
        /// <param name="e"></param>
        public static void Required(Entity target, RuleArgs e)
        {
            var property = e.Property;

            bool isNull = false;

            if (property is IRefProperty)
            {
                var lazyRef = target.GetLazyRef(property as IRefProperty);
                isNull = lazyRef.NullableId == null;
            }
            else
            {
                var value = target.GetProperty(property);
                isNull = value == null;
            }

            if (isNull)
            {
                e.BrokenDescription = string.Format("{0} ��û����д��", e.GetPropertyDisplay());
            }
        }

        /// <summary>
        /// �ַ�����Ϊ�ա�
        /// </summary>
        /// <param name="target"></param>
        /// <param name="e"></param>
        public static void StringRequired(Entity target, RuleArgs e)
        {
            var value = target.GetProperty(e.Property) as string;
            if (string.IsNullOrEmpty(value))
            {
                e.BrokenDescription = string.Format("{0} ��û����д��", e.GetPropertyDisplay());
            }
        }

        /// <summary>
        /// �����ַ����ȡ�
        /// 
        /// ע�⣬����֤��Ҫ������
        /// int MaxLength
        /// </summary>
        /// <param name="target"></param>
        /// <param name="e"></param>
        public static void StringMaxLength(Entity target, RuleArgs e)
        {
            var max = e.TryGetCustomParams<int>("MaxLength");
            var value = target.GetProperty(e.Property) as string;

            if (!string.IsNullOrEmpty(value) && value.Length > max)
            {
                e.BrokenDescription = string.Format("{0} ���ܳ��� {1} ���ַ���", e.GetPropertyDisplay(), max);
            }
        }

        /// <summary>
        /// ��̵��ַ����ȡ�
        /// 
        /// ע�⣬����֤��Ҫ������
        /// int MinLength
        /// </remarks>
        public static void StringMinLength(Entity target, RuleArgs e)
        {
            var min = e.TryGetCustomParams<int>("MaxLength");
            var value = target.GetProperty(e.Property) as string;

            if (!string.IsNullOrEmpty(value) && value.Length < min)
            {
                e.BrokenDescription = string.Format("{0} ���ܵ��� {1} ���ַ���", e.GetPropertyDisplay(), min);
            }
        }

        /// <summary>
        /// �����������ƹ���
        /// 
        /// ע�⣬����֤��Ҫ������
        /// int MaxValue
        /// </summary>
        /// <param name="target"></param>
        /// <param name="e"></param>
        public static void IntegerMaxValue(Entity target, RuleArgs e)
        {
            var max = e.TryGetCustomParams<int>("MaxValue");
            var value = Convert.ToInt32(target.GetProperty(e.Property));

            if (value > max)
            {
                e.BrokenDescription = string.Format("{0} ���ܳ��� {1}��", e.GetPropertyDisplay(), max);
            }
        }

        /// <summary>
        /// ��С���������ƹ���
        /// 
        /// ע�⣬����֤��Ҫ������
        /// int MinValue
        /// </summary>
        /// <param name="target"></param>
        /// <param name="e"></param>
        public static void IntegerMinValue(Entity target, RuleArgs e)
        {
            var min = e.TryGetCustomParams<int>("MinValue");
            var value = Convert.ToInt32(target.GetProperty(e.Property));

            if (value < min)
            {
                e.BrokenDescription = string.Format("{0} ���ܵ��� {1}��", e.GetPropertyDisplay(), min);
            }
        }

        /// <summary>
        /// ��С���������ƹ���
        /// 
        /// ע�⣬����֤��Ҫ������
        /// Regex Regex
        /// string RegexLabel
        /// </summary>
        /// <param name="target"></param>
        /// <param name="e"></param>
        public static void RegexMatch(Entity target, RuleArgs e)
        {
            Regex re = e.TryGetCustomParams<Regex>("Regex");
            var value = (string)target.GetProperty(e.Property) ?? string.Empty;
            if (!re.IsMatch(value))
            {
                var regexLabel = e.TryGetCustomParams<string>("RegexLabel");
                e.BrokenDescription = string.Format("{0} ������ {1}��", e.GetPropertyDisplay(), regexLabel);
            }
        }
    }
}