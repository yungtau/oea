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
using Rafy.Web.Json;

namespace Rafy.Web.ClientMetaModel
{
    /// <summary>
    /// 表单的配置
    /// </summary>
    public class FormConfig : JsonModel
    {
        public FormConfig()
        {
            this.tbar = new List<ToolbarItem>();
            this.items = new List<FieldConfig>();
        }

        /// <summary>
        /// 表单所用的工具条配置
        /// </summary>
        public IList<ToolbarItem> tbar { get; private set; }

        /// <summary>
        /// 表单中所有字段的配置
        /// </summary>
        public IList<FieldConfig> items { get; private set; }

        protected override void ToJson(LiteJsonWriter json)
        {
            if (tbar.Count > 0) { json.WriteProperty("tbar", tbar); }
            json.WriteProperty("items", items);
        }
    }
}
