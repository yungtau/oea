﻿/*******************************************************
 * 
 * 作者：胡庆访
 * 创建时间：20100326
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 运行环境：.NET 4.0
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 胡庆访 20120326
 * 
*******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OEA;
using OEA.Library;
using OEA.MetaModel;
using OEA.MetaModel.Attributes;

using OEA.ManagedProperty;

using OEA.MetaModel.View;
using OEA.RBAC;

namespace OEA.Library
{
    /// <summary>
    /// （Module Access Control）
    /// 模块元数据的显示模型
    /// </summary>
    [RootEntity]
    public class ModuleAC : Entity
    {
        #region 支持树型实体

        public static readonly Property<string> TreeCodeProperty = P<ModuleAC>.Register(e => e.TreeCode);
        public override string TreeCode
        {
            get { return GetProperty(TreeCodeProperty); }
            set { SetProperty(TreeCodeProperty, value); }
        }

        public static readonly Property<int?> TreePIdProperty = P<ModuleAC>.Register(e => e.TreePId);
        public override int? TreePId
        {
            get { return this.GetProperty(TreePIdProperty); }
            set { this.SetProperty(TreePIdProperty, value); }
        }

        public override bool SupportTree { get { return true; } }

        #endregion

        public ModuleMeta Core { get; set; }

        public static readonly Property<string> KeyNameProperty = P<ModuleAC>.RegisterReadOnly(e => e.KeyName, e => (e as ModuleAC).GetKeyName(), null);
        public string KeyName
        {
            get { return this.GetProperty(KeyNameProperty); }
        }
        private string GetKeyName()
        {
            if (this.Core == null) return string.Empty;
            return this.Core.Label;
        }

        public static readonly Property<OperationACList> OperationACListProperty = P<ModuleAC>.Register(e => e.OperationACList);
        [Association]
        public OperationACList OperationACList
        {
            get { return this.GetLazyChildren(OperationACListProperty); }
        }
    }

    public class ModuleACList : EntityList { }

    public class ModuleACRepository : MemoryEntityRepository
    {
        protected ModuleACRepository() { }

        protected override string GetRealKey(Entity entity)
        {
            return (entity as ModuleAC).KeyName;
        }

        /// <summary>
        /// 重写此方法，直接把模块元数据读取到界面上
        /// </summary>
        /// <returns></returns>
        protected override EntityList GetAllCore()
        {
            var list = new ModuleACList();

            var roots = UIModel.Modules.GetRoots();
            foreach (var root in UIModel.Modules.GetRoots())
            {
                this.AddItemRecur(list, root);
            }

            this.NotifyLoaded(list);

            return list;
        }

        private ModuleAC AddItemRecur(EntityList list, ModuleMeta module)
        {
            var item = new ModuleAC();
            item.Core = module;

            //这句会生成 Id。
            this.NotifyLoaded(item);

            list.Add(item);

            foreach (var child in module.Children)
            {
                var childModule = this.AddItemRecur(list, child);
                childModule.TreeParent = item;
            }

            item.Status = PersistenceStatus.Unchanged;

            return item;
        }
    }

    internal class ModuleACConfig : EntityConfig<ModuleAC>
    {
        protected override void ConfigView()
        {
            base.ConfigView();

            View.HasTitle(ModuleAC.KeyNameProperty).HasLabel("界面模块");

            View.Property(ModuleAC.KeyNameProperty).HasLabel("模块").ShowIn(ShowInWhere.List | ShowInWhere.Lookup);
        }
    }
}