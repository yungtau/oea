﻿/*******************************************************
 * 
 * 作者：胡庆访
 * 创建时间：20100925
 * 说明：实体约束类，从Library中往下层移动到MetaModel中。
 * 运行环境：.NET 4.0
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 胡庆访 20100925
 * 约定项添加RepositoryType 胡庆访 20101101
 * 
*******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using System.Diagnostics;
using Rafy.Threading;
using Rafy.Utils;
using Rafy.MetaModel;
using Rafy.ManagedProperty;

namespace Rafy
{
    /// <summary>
    /// 实体类的约定
    /// </summary>
    public static class EntityConvention
    {
        /// <summary>
        /// 实体仓库数据层查询方法的约定名称。
        /// </summary>
        public const string QueryMethod = "FetchBy";

        ///// <summary>
        ///// 目前实体使用的主键类型。Int。
        ///// </summary>
        //public static readonly Type IdType = typeof(int);

        /// <summary>
        /// 目前实体使用的主键属性的名称。Id。
        /// </summary>
        internal static IManagedProperty Property_Id;

        /// <summary>
        /// 自关联属性名
        /// </summary>
        internal static IManagedProperty Property_TreePId;

        /// <summary>
        /// 树型实体的编码
        /// </summary>
        internal static IManagedProperty Property_TreeIndex;

        ///// <summary>
        ///// 目前实体使用的主键属性的名称。Id。
        ///// </summary>
        //public const string Property_Id = "Id";

        ///// <summary>
        ///// 自关联属性名
        ///// </summary>
        //public const string Property_TreePId = "TreePId";

        ///// <summary>
        ///// 树型实体的编码
        ///// </summary>
        //public const string Property_TreeCode = "TreeCode";
    }
}
