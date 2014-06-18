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
    [ChildEntity, Serializable]
    public partial class PurchaseOrderAttachement : FileAttachement
    {
        #region 构造函数

        public PurchaseOrderAttachement() { }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected PurchaseOrderAttachement(SerializationInfo info, StreamingContext context) : base(info, context) { }

        #endregion

        public static readonly IRefIdProperty PurchaseOrderIdProperty =
            P<PurchaseOrderAttachement>.RegisterRefId(e => e.PurchaseOrderId, ReferenceType.Parent);
        public int PurchaseOrderId
        {
            get { return (int)this.GetRefId(PurchaseOrderIdProperty); }
            set { this.SetRefId(PurchaseOrderIdProperty, value); }
        }
        public static readonly RefEntityProperty<PurchaseOrder> PurchaseOrderProperty =
            P<PurchaseOrderAttachement>.RegisterRef(e => e.PurchaseOrder, PurchaseOrderIdProperty);
        public PurchaseOrder PurchaseOrder
        {
            get { return this.GetRefEntity(PurchaseOrderProperty); }
            set { this.SetRefEntity(PurchaseOrderProperty, value); }
        }
    }

    [Serializable]
    public partial class PurchaseOrderAttachementList : FileAttachementList { }

    public partial class PurchaseOrderAttachementRepository : FileAttachementRepository
    {
        protected PurchaseOrderAttachementRepository() { }
    }

    //internal class PurchaseOrderAttachementConfig : EntityConfig<PurchaseOrderAttachement>
    //{
    //    protected override void ConfigMeta()
    //    {
    //        Meta.MapTable().MapAllPropertiesToTable();
    //    }

    //    protected override void ConfigView()
    //    {
    //        View.DomainName("实体标签").HasDelegate(PurchaseOrderAttachement.NameProperty);

    //        using (View.OrderProperties())
    //        {
    //            View.Property(PurchaseOrderAttachement.NameProperty).HasLabel("名称").ShowIn(ShowInWhere.All);
    //        }
    //    }
    //}
}