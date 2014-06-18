﻿/*******************************************************
 * 
 * 作者：胡庆访
 * 创建日期：2012
 * 运行环境：.NET 4.0
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 胡庆访 20130523 16:36
 * 
*******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Rafy.Domain.ORM;
using Rafy;
using Rafy.Data;
using Rafy.Domain.ORM.SqlTree;
using Rafy.Domain.ORM.Query;
using Rafy.ManagedProperty;

namespace Rafy.Domain
{
    /// <summary>
    /// 所有实体查询的参数类型的基类。
    /// </summary>
    public abstract class EntityQueryArgsBase : QueryArgs, ISelectArgs
    {
        #region FetchType

        private FetchType _fetchType = FetchType.List;

        /// <summary>
        /// 当前查询数据的类型。
        /// 实体查询时，不会对应 <see cref="Rafy.Domain.FetchType.Table" /> 类型。
        /// </summary>
        public override FetchType FetchType
        {
            get { return _fetchType; }
        }

        internal void SetFetchType(FetchType value)
        {
            _fetchType = value;
        }

        bool ISelectArgs.FetchingFirst
        {
            get { return this.FetchType == FetchType.First; }
        }

        #endregion

        /// <summary>
        /// 如果是内存加载，则使用这个列表。
        /// </summary>
        internal IList<Entity> MemoryList;

        /// <summary>
        /// 加载的列表对象
        /// </summary>
        public EntityList EntityList { get; set; }

        /// <summary>
        /// 对查询出来的对象进行内存级别的过滤器，默认为 null。
        /// </summary>
        public Predicate<Entity> Filter { get; set; }

        private PagingInfo _pagingInfo = PagingInfo.Empty;

        /// <summary>
        /// 要对结果进行分页的分页信息。
        /// 默认为 PagingInfo.Empty。
        /// </summary>
        public PagingInfo PagingInfo
        {
            get { return _pagingInfo; }
            set { _pagingInfo = value; }
        }

        IList<Entity> ISelectArgs.List
        {
            get { return this.MemoryList ?? this.EntityList; }
        }

        internal List<ConcreteProperty> EagerLoadProperties;

        /// <summary>
        /// 贪婪加载某个属性
        /// </summary>
        /// <param name="property">需要贪婪加载的托管属性。可以是一个引用属性，也可以是一个组合子属性。</param>
        /// <param name="propertyOwner">这个属性的拥有者类型。</param>
        public void EagerLoad(IProperty property, Type propertyOwner = null)
        {
            propertyOwner = propertyOwner ?? property.OwnerType;

            if (this.EagerLoadProperties == null)
            {
                this.EagerLoadProperties = new List<ConcreteProperty>();
            }

            this.EagerLoadProperties.Add(new ConcreteProperty(property, propertyOwner));
        }
    }

    /// <summary>
    /// 使用 IQuery 进行查询的参数。
    /// </summary>
    public class EntityQueryArgs : EntityQueryArgsBase, IEntitySelectArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityQueryArgs"/> class.
        /// </summary>
        public EntityQueryArgs() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityQueryArgs"/> class.
        /// </summary>
        /// <param name="query">The query.</param>
        public EntityQueryArgs(IQuery query)
        {
            this.Query = query;
        }

        /// <summary>
        /// 对应的查询条件定义。
        /// </summary>
        public IQuery Query { get; set; }
    }

    ///// <summary>
    ///// 使用 Linq 进行查询的参数。
    ///// </summary>
    //public class LinqQueryArgs : EntityQueryArgs
    //{
    //    /// <summary>
    //    /// 对应的 Linq 查询条件表达式。
    //    /// 此条件在内部会被转换为 IQuery 对象来描述整个查询。
    //    /// </summary>
    //    public IQueryable Queryable { get; set; }
    //}

    /// <summary>
    /// 使用 Sql 查询的参数。
    /// </summary>
    public class SqlQueryArgs : EntityQueryArgsBase, ISqlSelectArgs
    {
        private object[] _parameters;
        internal Type EntityType;

        /// <summary>
        /// 空构造函数，配合属性使用。
        /// </summary>
        public SqlQueryArgs() { }

        /// <summary>
        /// 通过一个 FormatSql 来构造。
        /// </summary>
        /// <param name="sql"></param>
        public SqlQueryArgs(FormattedSql sql) : this(sql, sql.Parameters) { }

        /// <summary>
        /// 通过标准跨库 Sql 及参数值来构造。
        /// </summary>
        /// <param name="formattedSql"></param>
        /// <param name="parameters"></param>
        private SqlQueryArgs(string formattedSql, params object[] parameters)
        {
            this.FormattedSql = formattedSql;
            this.Parameters = parameters;
        }

        /// <summary>
        /// 格式化参数的标准 SQL。
        /// </summary>
        public string FormattedSql { get; set; }

        /// <summary>
        /// FormatSql 对应的参数值。
        /// </summary>
        public object[] Parameters
        {
            get
            {
                //这个返回值不会是 null。
                if (this._parameters == null)
                {
                    this._parameters = new object[0];
                }
                return this._parameters;
            }
            set { this._parameters = value; }
        }

        #region ISelectArgs 成员

        Type ISqlSelectArgs.EntityType
        {
            get { return this.EntityType; }
        }

        #endregion
    }
}
