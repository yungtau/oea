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

namespace OEA.Library.Validation
{
    /// <summary>
    /// Exception class indicating that there was a validation
    /// problem with a business object.
    /// </summary>
    [Serializable]
    public class ValidationException : Exception
    {
        /// <summary>
        /// Creates an instance of the object.
        /// </summary>
        public ValidationException() { }

        /// <summary>
        /// Creates an instance of the object.
        /// </summary>
        /// <param name="message">Message describing the exception.</param>
        public ValidationException(string message) : base(message) { }

        /// <summary>
        /// Creates an instance of the object.
        /// </summary>
        /// <param name="message">Message describing the exception.</param>
        /// <param name="innerException">Inner exception object.</param>
        public ValidationException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Creates an instance of the object for serialization.
        /// </summary>
        /// <param name="context">Serialization context.</param>
        /// <param name="info">Serialization info.</param>
        protected ValidationException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}