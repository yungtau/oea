﻿/*******************************************************
 * 
 * 作者：胡庆访
 * 创建时间：20120220
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 运行环境：.NET 4.0
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 胡庆访 20120220
 * 
*******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.CodeDom.Compiler;
using System.Collections;

namespace OEA.Web.Json
{
    /// <summary>
    /// 轻量级的 Json Writer
    /// </summary>
    public class LiteJsonWriter
    {
        private bool ConvertPropertyToField = false;

        private IndentedTextWriter _writer;

        private void WriteJsonModel(JsonModel model)
        {
            this.WriteObjectBegin();

            model.ToJsonInternal(this);

            this.WriteObjectEnd();
        }

        private void WriteObjectBegin()
        {
            this.WriteLine("{");
            this._writer.Indent++;
        }

        private void WriteObjectEnd()
        {
            this._writer.Indent--;
            this.Write("}");
        }

        public void WritePropertyIf(string name, object value, bool isLast = false)
        {
            if (value != null) { this.WriteProperty(name, value, isLast); }
        }

        public void WriteProperty(string name, object value, bool isLast = false)
        {
            if (value is IEnumerable<JsonModel>)
            {
                this.WriteProperty(name, value as IEnumerable<JsonModel>, isLast);
                return;
            }

            if (this.ConvertPropertyToField)
            {
                name = char.ToLower(name[0]) + name.Substring(1);
            }

            this.Write("\"");
            this.Write(name);
            this.Write("\": ");

            if (value is JsonModel)
            {
                this.WriteJsonModel(value as JsonModel);
            }
            else
            {
                var jsonValue = this.JsonValue(value);

                this.Write(jsonValue);
            }

            if (!isLast) { this.Write(","); }

            this.WriteLine();
        }

        public void WriteProperty(string name, IEnumerable<JsonModel> children, bool isLast = false)
        {
            if (this.ConvertPropertyToField)
            {
                name = char.ToLower(name[0]) + name.Substring(1);
            }

            this.Write("\"");
            this.Write(name);
            this.Write("\":[");
            this._writer.Indent++;

            var childrenCache = children.ToArray();
            for (int i = 0, c = childrenCache.Length; i < c; i++)
            {
                var child = childrenCache[i];
                this.WriteJsonModel(child);
                if (i < c - 1) { this.Write(","); }
            }

            this._writer.Indent--;
            this.Write("]");

            if (!isLast) { this.Write(","); }

            this.WriteLine();
        }

        public void Write(string text)
        {
            this._writer.Write(text);
        }

        public void WriteLine(string text)
        {
            this._writer.WriteLine(text);
        }

        public void WriteLine()
        {
            this._writer.WriteLine();
        }

        private string JsonValue(object value)
        {
            if (value == null) return "null";

            if (value is bool)
            {
                return ((bool)value) ? "true" : "false";
            }

            if (value is Array)
            {
                var arrayValues = (value as IEnumerable<object>).Select(v => this.JsonValue(v));
                return "[" + string.Join(",", arrayValues) + "]";
            }

            var strValue = value.ToString();

            if (value is string || value is Enum || value is DateTime || value is Guid)
            {
                strValue = strValue.Replace("\"", "\\\"");

                return "\"" + strValue + "\"";
            }

            return strValue;
        }

        public static string Convert(JsonModel model)
        {
            var converter = new LiteJsonWriter();

            using (var jsonWriter = new StringWriter())
            {
                converter._writer = new IndentedTextWriter(jsonWriter);

                converter.WriteJsonModel(model);

                var json = jsonWriter.ToString();

                return json;
            }
        }

        //public static string CommonToJson(object model)
        //{
        //    using (var buffer = new MemoryStream())
        //    {
        //        var knownTypes = new Type[] { typeof(List<int>), typeof(byte[]), typeof(DateTimeOffset), typeof(DateTime) };
        //        var serializer = new DataContractJsonSerializer(typeof(object), knownTypes);
        //        serializer.WriteObject(buffer, model);

        //        var bytes = buffer.ToArray();
        //        var json = Encoding.UTF8.GetString(bytes);

        //        json = Regex.Replace(json, @"\\/", "/");
        //        MatchEvaluator me = match =>
        //        {
        //            string sRet = string.Empty;

        //            try
        //            {
        //                System.DateTime dt = new DateTime(1970, 1, 1);
        //                dt = dt.AddMilliseconds(long.Parse(match.Groups["ms"].Value));
        //                dt = dt.ToLocalTime();
        //                sRet = dt.ToString("yyyy-MM-dd HH:mm:ss");
        //            }
        //            catch { }

        //            return sRet;
        //        };
        //        json = Regex.Replace(json, @"/Date\((?<ms>\d+)[\+\-]\d+\)/", me);

        //        return json;
        //    }
        //}
    }
}