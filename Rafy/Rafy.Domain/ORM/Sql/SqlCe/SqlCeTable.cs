﻿/*******************************************************
 * 
 * 作者：胡庆访
 * 创建时间：20130123 10:49
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 运行环境：.NET 4.0
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 胡庆访 20130123 10:49
 * 
*******************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Rafy;
using Rafy.Data;
using Rafy.Domain;
using Rafy.ManagedProperty;
using Rafy.MetaModel;
using Rafy.Domain.ORM.SqlServer;
using Rafy.Utils;

namespace Rafy.Domain.ORM.SqlCe
{
    internal class SqlCeTable : SqlTable
    {
        public SqlCeTable(IRepositoryInternal repository) : base(repository) { }

        /// <summary>
        /// 在 sqlce 下，不支持 rowNumber 方案，但是支持 not in 方案。
        /// 鉴于实现 not in 方案比较耗时，所以暂时决定使用 IDataReader 分页完成。
        /// 
        /// not in 分页，参见以下 Sql：
        /// select top 10 [AuditItem].* from 
        /// [AuditItem] where 
        /// [AuditItem].id not in
        /// (
        ///     select top 100 [AuditItem].id from [AuditItem] order by LogTime desc
        /// )
        /// order by LogTime desc
        /// </summary>
        protected override PagingLocation GetPagingLocation(PagingInfo pagingInfo)
        {
            if (!PagingInfo.IsNullOrEmpty(pagingInfo) && pagingInfo.PageNumber == 1 && !pagingInfo.IsNeedCount)
            {
                return PagingLocation.Database;
            }
            return PagingLocation.Memory;
        }
    }
}