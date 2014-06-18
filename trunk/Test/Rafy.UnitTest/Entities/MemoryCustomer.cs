﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using Rafy;
using Rafy.Domain.ORM;
using Rafy.Domain;
using Rafy.Domain.Validation;
using Rafy.MetaModel;
using Rafy.MetaModel.Attributes;
using Rafy.MetaModel.View;
using Rafy.ManagedProperty;

namespace UT
{
    /// <summary>
    /// 序列化到 Xml 的用户
    /// </summary>
    [RootEntity, Serializable]
    public partial class MemoryCustomer : IntEntity
    {
        #region 构造函数

        public MemoryCustomer() { }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected MemoryCustomer(SerializationInfo info, StreamingContext context) : base(info, context) { }

        #endregion

        #region 引用属性

        #endregion

        #region 子属性

        #endregion

        #region 一般属性

        public static readonly Property<string> NameProperty = P<MemoryCustomer>.Register(e => e.Name);
        public string Name
        {
            get { return this.GetProperty(NameProperty); }
            set { this.SetProperty(NameProperty, value); }
        }

        public static readonly Property<int> AgeProperty = P<MemoryCustomer>.Register(e => e.Age);
        public int Age
        {
            get { return this.GetProperty(AgeProperty); }
            set { this.SetProperty(AgeProperty, value); }
        }

        public static readonly Property<int> VersionProperty = P<MemoryCustomer>.Register(e => e.Version);
        public int Version
        {
            get { return this.GetProperty(VersionProperty); }
            set { this.SetProperty(VersionProperty, value); }
        }

        #endregion

        #region 只读属性

        #endregion
    }

    [Serializable]
    public partial class MemoryCustomerList : EntityList { }

    public partial class MemoryCustomerRepository : MemoryEntityRepository
    {
        protected override string GetRealKey(Entity entity)
        {
            return (entity as MemoryCustomer).Name;
        }

        protected MemoryCustomerRepository() { }

        [DataProviderFor(typeof(MemoryCustomerRepository))]
        private class MemoryCustomerRepositoryDataProvider : MemoryRepositoryDataProvider
        {
            protected override IEnumerable<Entity> LoadAll()
            {
                return Enumerable.Empty<Entity>();
            }

            protected override void Submit(SubmitArgs e)
            {
                if (e.Action != SubmitAction.Delete)
                {
                    (e.Entity as MemoryCustomer).Version++;
                }

                base.Submit(e);

                if (e.Action == SubmitAction.Delete)
                {
                    var item = e.Entity as MemoryCustomer;
                    item.LoadProperty(MemoryCustomer.VersionProperty, item.Version + 1);
                }
            }

            protected override void Insert(Entity entity)
            {
                (entity as MemoryCustomer).Version++;

                base.Insert(entity);
            }

            protected override void Update(Entity entity)
            {
                base.Update(entity);

                var item = entity as MemoryCustomer;
                item.LoadProperty(MemoryCustomer.VersionProperty, item.Version + 1);
            }

            protected override void Delete(Entity entity)
            {
                (entity as MemoryCustomer).Version++;

                base.Delete(entity);
            }
        }
    }

    internal class MemoryCustomerConfig : EntityConfig<MemoryCustomer> { }
}