﻿/*******************************************************
 * 
 * 作者：胡庆访
 * 创建时间：20120906 20:52
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 运行环境：.NET 4.0
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 胡庆访 20120906 20:52
 * 
*******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rafy.MetaModel.Attributes;

namespace UT
{
    public enum RoleType
    {
        [Label("一般")]
        Normal,
        [Label("管理员")]
        Administrator
    }
}
