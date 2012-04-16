﻿/*******************************************************
 * 
 * 作者：胡庆访
 * 创建时间：20110315
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 运行环境：.NET 4.0
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 胡庆访 20100315
 * 
*******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OEA.MetaModel.Attributes;

namespace OEA.MetaModel.View
{
    /// <summary>
    /// 属性的视图模型
    /// </summary>
    public class EntityPropertyViewMeta : ViewMeta
    {
        private EntityPropertyMeta _PropertyMeta;
        /// <summary>
        /// 对应的属性信息
        /// </summary>
        public EntityPropertyMeta PropertyMeta
        {
            get { return this._PropertyMeta; }
            set { this.SetValue(ref this._PropertyMeta, value); }
        }

        /// <summary>
        /// 属性名
        /// </summary>
        public override string Name
        {
            get { return this.PropertyMeta.Name; }
            set { throw new NotSupportedException(); }
        }

        private bool _IsReadonly;
        public bool IsReadonly
        {
            get { return this._IsReadonly; }
            set { this.SetValue(ref this._IsReadonly, value); }
        }

        /// <summary>
        /// 是否为“引用属性”
        /// </summary>
        public bool IsReference
        {
            get { return this._refViewInfo != null; }
        }

        private ReferenceViewInfo _refViewInfo;
        /// <summary>
        /// 标记的LookupAttribute
        /// </summary>
        public ReferenceViewInfo ReferenceViewInfo
        {
            get { return this._refViewInfo; }
            set { this._refViewInfo = value; }
        }

        private EntityViewMeta _Owner;
        /// <summary>
        /// 属性所在的实体类型的视图信息
        /// </summary>
        public EntityViewMeta Owner
        {
            get { return this._Owner; }
            set { this.SetValue(ref this._Owner, value); }
        }

        #region Web

        private int? _WidthFlex;
        /// <summary>
        /// 用于初始化表格控件的宽度属性
        /// </summary>
        public int? WidthFlex
        {
            get { return this._WidthFlex; }
            set { this.SetValue(ref this._WidthFlex, value); }
        }

        #endregion

        #region WPF

        private PropertyVisibilityIndicator _VisibilityIndicator = new PropertyVisibilityIndicator();
        /// <summary>
        /// 用于检测是否可见的属性
        /// </summary>
        public PropertyVisibilityIndicator VisibilityIndicator
        {
            get { return this._VisibilityIndicator; }
            set { this.SetValue(ref this._VisibilityIndicator, value); }
        }

        private string _StringFormat;
        /// <summary>
        /// 格式化String Column Format
        /// </summary>
        public string StringFormat
        {
            get { return this._StringFormat; }
            set { this.SetValue(ref this._StringFormat, value); }
        }

        private string _EditorName;
        /// <summary>
        /// 编辑器名
        /// EditorAttribute中标记
        /// </summary>
        public string EditorName
        {
            get { return this._EditorName; }
            set { this.SetValue(ref this._EditorName, value); }
        }

        private double? _GridWidth;
        /// <summary>
        /// 用于初始化表格控件的宽度属性
        /// </summary>
        public double? GridWidth
        {
            get { return this._GridWidth; }
            set { this.SetValue(ref this._GridWidth, value); }
        }

        private int? _DetailColumnsSpan;
        /// <summary>
        /// 表单中该属性所占的列数
        /// 不指定，则使用系统默认宽度。
        /// </summary>
        public int? DetailColumnsSpan
        {
            get { return this._DetailColumnsSpan; }
            set { this.SetValue(ref this._DetailColumnsSpan, value); }
        }

        private double? _DetailWidth;
        /// <summary>
        /// 表单中该属性所占的格子宽度。
        /// 
        /// 如果值在 0 - 1 之间，表示百分比。否则表示绝对值。
        /// 
        /// 不指定，则使用系统默认值。
        /// </summary>
        public double? DetailWidth
        {
            get { return this._DetailWidth; }
            set { this.SetValue(ref this._DetailWidth, value); }
        }

        private int? _DetailHeight;
        /// <summary>
        /// 表单中该属性所占的总高度
        /// 不指定，则使用系统默认宽度。
        /// </summary>
        public int? DetailHeight
        {
            get { return this._DetailHeight; }
            set { this.SetValue(ref this._DetailHeight, value); }
        }

        private int? _DetailLabelWidth;
        /// <summary>
        /// 在 DetailPanel 中显示的 Label 的宽度。
        /// 不指定，则使用系统默认宽度。
        /// </summary>
        public int? DetailLabelWidth
        {
            get { return this._DetailLabelWidth; }
            set { this.SetValue(ref this._DetailLabelWidth, value); }
        }

        #endregion

        private double _OrderNo;
        /// <summary>
        /// 排序此属性使用的属性。
        /// </summary>
        public double OrderNo
        {
            get { return this._OrderNo; }
            set { this.SetValue(ref this._OrderNo, value); }
        }

        private ShowInWhere _ShowInWhere;
        /// <summary>
        /// 在哪里显示
        /// </summary>
        public ShowInWhere ShowInWhere
        {
            get { return this._ShowInWhere; }
            set
            {
                if (this.IsChildReference()) throw new InvalidOperationException("聚合子属性，请设置他的 IsVisible 属性");

                this.SetValue(ref this._ShowInWhere, value);
            }
        }

        private NavigationPropertyMeta _NavigationPropertyMeta;
        /// <summary>
        /// 如果当前属性是一个导航触发属性，则这个属性不为 null。
        /// </summary>
        public NavigationPropertyMeta NavigationMeta
        {
            get { return this._NavigationPropertyMeta; }
            set { this.SetValue(ref this._NavigationPropertyMeta, value); }
        }

        public override bool IsVisible
        {
            get
            {
                if (this.IsChildReference()) return base.IsVisible;

                return this.ShowInWhere != ShowInWhere.Hide;
            }
            set
            {
                if (this.IsChildReference())
                {
                    base.IsVisible = value;
                    return;
                }

                throw new InvalidOperationException("请设置他的 ShowInWhere 属性");
            }
        }

        private bool IsChildReference()
        {
            return this.IsReference && this._refViewInfo.ReferenceInfo.Type == ReferenceType.Child;
        }

        #region 查询方法

        /// <summary>
        /// 判断是否可以显示在某处
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public bool CanShowIn(ShowInWhere where)
        {
            return (this.ShowInWhere & where) == where;
        }

        /// <summary>
        /// 用于绑定显示的属性名
        /// </summary>
        /// <returns></returns>
        public string BindingPath()
        {
            if (this.IsReference)
            {
                var reference = this.ReferenceViewInfo;

                //该引用属性没有对应的引用实体属性
                var title = reference.RefTypeDefaultView.TitleProperty;
                if (string.IsNullOrEmpty(reference.RefEntityProperty) || title == null) { return this.Name; }

                return reference.RefEntityProperty + "." + title.Name;
            }
            else
            {
                return this.Name;
            }
        }

        #endregion
    }

    /// <summary>
    /// 导航属性的元数据
    /// </summary>
    public class NavigationPropertyMeta : Freezable
    {
        private string _IdPropertyName;

        /// <summary>
        /// 如果本导航属性是一个子实体集合时，IdPropertyName 表示集合的主键应该赋值给我这个导航对象的哪个属性。
        /// </summary>
        public string IdPropertyAfterSelection
        {
            get { return this._IdPropertyName; }
            set { this.SetValue(ref this._IdPropertyName, value); }
        }
    }

    [Flags]
    public enum ShowInWhere
    {
        /// <summary>
        /// 默认值：不显示。
        /// </summary>
        Hide = 0,

        /// <summary>
        /// 是否在下拉框中显示
        /// </summary>
        DropDown = 1,

        /// <summary>
        /// 是否在列表中显示
        /// </summary>
        List = 2,

        /// <summary>
        /// 是否在表单中显示
        /// </summary>
        Detail = 4,

        /// <summary>
        /// 显示在列表和表单中
        /// </summary>
        ListDetail = List | Detail,

        /// <summary>
        /// 显示在列表和下拉框中
        /// </summary>
        ListDropDown = List | DropDown,

        [Label("全显示")]
        All = DropDown | List | Detail
    }
}