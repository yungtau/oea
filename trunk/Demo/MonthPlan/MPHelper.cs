﻿/*******************************************************
 * 
 * 作者：胡庆访
 * 创建时间：20121106 21:13
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 运行环境：.NET 4.0
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 胡庆访 20121106 21:13
 * 
*******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Windows;
using Rafy.WPF;
using Rafy.WPF.Controls;

namespace MP
{
    public static class MPHelper
    {
        public static void ModifyRowStyle(ListLogicalView taskView, string styleName)
        {
            var grid = taskView.Control as TreeGrid;
            grid.RowStyle = RafyResources.FindStyle(styleName);
        }
    }
}
