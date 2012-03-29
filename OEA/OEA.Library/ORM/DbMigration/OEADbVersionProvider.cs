﻿/*******************************************************
 * 
 * 作者：胡庆访
 * 创建时间：20120107
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 运行环境：.NET 4.0
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 胡庆访 20120107
 * 
*******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DbMigration.History;
using OEA.Library.ORM.DbMigration.Presistence;
using hxy.Common;

namespace OEA.Library.ORM.DbMigration
{
    public class OEADbVersionProvider : DbVersionProvider
    {
        private DbVersionRepository _versionRepo;

        public OEADbVersionProvider()
        {
            this._versionRepo = RF.Concreate<DbVersionRepository>();
        }

        protected override DateTime GetDbVersionCore(string database)
        {
            var item = this._versionRepo.GetByDb(database);
            if (item != null) { return item.Version; }

            return DefaultMinTime;
        }

        protected override Result SetDbVersionCore(string database, DateTime version)
        {
            var item = this._versionRepo.GetByDb(database);

            if (item == null)
            {
                item = this._versionRepo.New().CastTo<DbVersion>();
                item.Database = database;
            }

            item.Version = version;

            try
            {
                this._versionRepo.Save(item);
                return true;
            }
            catch (Exception ex)
            {
                return "设置数据库版本号 时出错：" + ex.Message;
            }
        }

        protected override bool IsEmbaded()
        {
            return false;
        }
    }
}