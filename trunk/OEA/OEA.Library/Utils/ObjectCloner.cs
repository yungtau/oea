using System;
using System.IO;
using OEA.Serialization;

namespace OEA.Utils
{
    /// <summary>
    /// This class provides an implementation of a deep
    /// clone of a complete object graph. Objects are
    /// copied at the field level.
    /// </summary>
    public static class ObjectCloner
    {
        /// <summary>
        /// ʹ�ö��������л������л��ķ�������һ������
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T Clone<T>(T obj)
        {
            using (MemoryStream buffer = new MemoryStream())
            {
                var formatter = SerializationFormatterFactory.GetFormatter();
                formatter.Serialize(buffer, obj);
                buffer.Position = 0;
                object temp = formatter.Deserialize(buffer);
                return (T)temp;
            }
        }
    }
}