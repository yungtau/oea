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
    /// Values for validation rule severities.
    /// </summary>
    public enum RuleLevel
    {
        /// <summary>
        /// Represents a serious
        /// business rule violation that
        /// should cause an object to
        /// be considered invalid.
        /// </summary>
        Error = 0,

        /// <summary>
        /// Represents a business rule
        /// violation that should be
        /// displayed to the user, but which
        /// should not make an object be
        /// invalid.
        /// </summary>
        Warning = 1,

        /// <summary>
        /// Represents a business rule
        /// result that should be displayed
        /// to the user, but which is less
        /// severe than a warning.
        /// </summary>
        Information = 2
    }
}
