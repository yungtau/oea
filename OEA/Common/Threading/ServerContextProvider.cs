﻿/*******************************************************
 * 
 * 作者：胡庆访
 * 创建时间：20120913 16:41
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 运行环境：.NET 4.0
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 胡庆访 20120913 16:41
 * 
*******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hxy.Common
{
    /// <summary>
    /// 默认实现：一个标记了 ThreadStatic 的字段。
    /// </summary>
    public class ServerContextProvider
    {
        [ThreadStatic]
        private static IDictionary<string, object> _items;

        protected internal virtual IDictionary<string, object> GetValueContainer()
        {
            return _items;
        }

        protected internal virtual void SetValueContainer(IDictionary<string, object> value)
        {
            _items = value;
        }

        //protected virtual IDictionary GetLocalContext()
        //{
        //    var slot = Thread.GetNamedDataSlot(LocalContextName);
        //    return (IDictionary)Thread.GetData(slot);
        //}

        //protected virtual void SetLocalContext(IDictionary localContext)
        //{
        //    var slot = Thread.GetNamedDataSlot(LocalContextName);
        //    Thread.SetData(slot, localContext);
        //}
    }
}
