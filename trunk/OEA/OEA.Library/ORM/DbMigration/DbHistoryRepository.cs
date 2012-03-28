﻿/*******************************************************
 * 
 * 作者：胡庆访
 * 创建时间：20110104
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 运行环境：.NET 4.0
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 胡庆访 20110104
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
    public class DbHistoryRepository : HistoryRepository
    {
        private DbMigrationHistoryRepository _historyRepo;

        public DbHistoryRepository()
        {
            this._historyRepo = RF.Concreate<DbMigrationHistoryRepository>();
        }

        protected override IList<HistoryItem> GetHistoriesCore(string database)
        {
            var list = this._historyRepo.GetByDb(database);

            var items = new List<HistoryItem>(list.Count);

            for (int i = 0, c = list.Count; i < c; i++)
            {
                var dbItem = list[i] as DbMigrationHistory;

                items.Add(new HistoryItem
                {
                    TimeId = dbItem.TimeId,
                    Description = dbItem.Description,
                    IsGenerated = dbItem.IsGenerated,
                    MigrationClass = dbItem.MigrationClass,
                    MigrationContent = dbItem.MigrationContent,
                    DataObject = dbItem,
                });
            }

            return items;
        }

        protected override Result AddHistoryCore(string database, HistoryItem history)
        {
            var item = this._historyRepo.New().CastTo<DbMigrationHistory>();

            item.Database = database;

            item.TimeId = history.TimeId;
            item.Description = history.Description;
            item.IsGenerated = history.IsGenerated;
            item.MigrationClass = history.MigrationClass;
            item.MigrationContent = history.MigrationContent;

            try
            {
                this._historyRepo.Save(item);
                return true;
            }
            catch (Exception ex)
            {
                return "添加数据库更新日志 时出错：" + ex.Message;
            }
        }

        protected override Result RemoveHistoryCore(string database, HistoryItem history)
        {
            var item = history.DataObject.CastTo<DbMigrationHistory>();

            item.MarkDeleted();

            try
            {
                this._historyRepo.Save(item);
                return true;
            }
            catch (Exception ex)
            {
                return "添加数据库更新日志 时出错：" + ex.Message;
            }
        }
    }
}