/*******************************************************
 * 
 * ���ߣ������
 * ����ʱ�䣺20120404
 * ˵�������ļ�ֻ����һ���࣬�������ݼ�����ע�͡�
 * ���л�����.NET 4.0
 * �汾�ţ�1.0.0
 * 
 * ��ʷ��¼��
 * �����ļ� ����� 20120404
 * 
*******************************************************/

using System;
using System.IO;
using OEA.Serialization;

namespace OEA.Utils
{
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