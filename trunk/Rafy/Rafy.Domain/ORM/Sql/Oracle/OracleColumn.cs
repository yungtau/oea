﻿/*******************************************************
 * 
 * 作者：胡庆访
 * 创建时间：20120429
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 运行环境：.NET 4.0
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 胡庆访 20120429
 * 
*******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rafy.MetaModel;

namespace Rafy.Domain.ORM.Oracle
{
    class OracleColumn : DbColumn
    {
        internal OracleColumn(DbTable table, string name, PropertyMeta property) : base(table, name, property) { }

        internal override void Write(Entity entity, object value)
        {
            var type = this.DataType;
            if (type == typeof(bool))
            {
                value = value.ToString() == "1" ? true : false;
            }
            else if (value == null && type == typeof(string))//null 转换为空字符串
            {
                value = string.Empty;
            }

            base.Write(entity, value);
        }
    }
}