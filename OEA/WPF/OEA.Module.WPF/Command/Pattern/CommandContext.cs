﻿/*******************************************************
 * 
 * 作者：http://www.codeproject.com/Articles/25445/WPF-Command-Pattern-Applied
 * 创建时间：周金根 2009
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 运行环境：.NET 4.0
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 周金根 2009
 * 重新整理 胡庆访 20120518
 * 
*******************************************************/

using System;
using System.Windows.Input;

namespace OEA.WPF.Command
{
    /// <summary>
    /// 命令执行的上下文
    /// </summary>
    public class CommandContext : IDisposable
    {
        public CommandContext() { }

        public object BindingSource { get; set; }

        public object CommandSource { get; set; }

        public object CommandParameter { get; set; }

        void IDisposable.Dispose() { }
    }
}