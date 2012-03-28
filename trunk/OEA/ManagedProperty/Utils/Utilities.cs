using System;
using System.ComponentModel;
using System.Reflection;
using System.Xml;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace OEA
{
    /// <summary>
    /// Contains utility methods used by the
    /// CSLA .NET framework.
    /// </summary>
    public static class Utilities
    {
        #region Replacements for VB runtime functionality

        /// <summary>
        /// Determines whether the specified
        /// value can be converted to a valid number.
        /// </summary>
        public static bool IsNumeric(object value)
        {
            double dbl;
            return double.TryParse(value.ToString(), System.Globalization.NumberStyles.Any,
              System.Globalization.NumberFormatInfo.InvariantInfo, out dbl);
        }

        /// <summary>
        /// Allows late bound invocation of
        /// properties and methods.
        /// </summary>
        /// <param name="target">Object implementing the property or method.</param>
        /// <param name="methodName">Name of the property or method.</param>
        /// <param name="callType">Specifies how to invoke the property or method.</param>
        /// <param name="args">List of arguments to pass to the method.</param>
        /// <returns>The result of the property or method invocation.</returns>
        public static object CallByName(
          object target, string methodName, CallType callType,
          params object[] args)
        {
            switch (callType)
            {
                case CallType.Get:
                    {
                        PropertyInfo p = target.GetType().GetProperty(methodName);
                        return p.GetValue(target, args);
                    }
                case CallType.Let:
                case CallType.Set:
                    {
                        PropertyInfo p = target.GetType().GetProperty(methodName);
                        p.SetValue(target, args[0], null);
                        return null;
                    }
                case CallType.Method:
                    {
                        MethodInfo m = target.GetType().GetMethod(methodName);
                        return m.Invoke(target, args);
                    }
            }
            return null;
        }

        #endregion
    }

    /// <summary>
    /// Valid options for calling a property or method
    /// via the <see cref="OEA.Utilities.CallByName"/> method.
    /// </summary>
    public enum CallType
    {
        /// <summary>
        /// Gets a value from a property.
        /// </summary>
        Get,
        /// <summary>
        /// Sets a value into a property.
        /// </summary>
        Let,
        /// <summary>
        /// Invokes a method.
        /// </summary>
        Method,
        /// <summary>
        /// Sets a value into a property.
        /// </summary>
        Set
    }
}
