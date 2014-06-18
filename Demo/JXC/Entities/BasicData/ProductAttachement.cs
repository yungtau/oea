﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using Rafy;
using Rafy.Domain;
using Rafy.Domain.Validation;
using Rafy.MetaModel;
using Rafy.MetaModel.Attributes;
using Rafy.MetaModel.View;
using Rafy.ManagedProperty;

namespace JXC
{
    /// <summary>
    /// 产品附件
    /// </summary>
    [ChildEntity, Serializable]
    public partial class ProductAttachement : FileAttachement
    {
        #region 构造函数

        public ProductAttachement() { }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected ProductAttachement(SerializationInfo info, StreamingContext context) : base(info, context) { }

        #endregion

        public static readonly IRefIdProperty ProductIdProperty =
            P<ProductAttachement>.RegisterRefId(e => e.ProductId, ReferenceType.Parent);
        public int ProductId
        {
            get { return (int)this.GetRefId(ProductIdProperty); }
            set { this.SetRefId(ProductIdProperty, value); }
        }
        public static readonly RefEntityProperty<Product> ProductProperty =
            P<ProductAttachement>.RegisterRef(e => e.Product, ProductIdProperty);
        public Product Product
        {
            get { return this.GetRefEntity(ProductProperty); }
            set { this.SetRefEntity(ProductProperty, value); }
        }
    }

    [Serializable]
    public partial class ProductAttachementList : FileAttachementList { }

    public partial class ProductAttachementRepository : FileAttachementRepository
    {
        protected ProductAttachementRepository() { }
    }

    internal class ProductAttachementConfig : EntityConfig<ProductAttachement>
    {
        protected override void ConfigMeta()
        {
            Meta.EnableClientCache();
        }

        //    protected override void ConfigView()
        //    {
        //        View.DomainName("实体标签").HasDelegate(ProductAttachement.NameProperty);

        //        using (View.OrderProperties())
        //        {
        //            View.Property(ProductAttachement.NameProperty).HasLabel("名称").ShowIn(ShowInWhere.All);
        //        }
        //    }
    }
}