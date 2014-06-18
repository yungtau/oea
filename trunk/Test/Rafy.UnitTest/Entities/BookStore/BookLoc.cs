using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using Rafy;
using Rafy.Data;
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
    /// 书籍货架
    /// </summary>
    [RootEntity, Serializable]
    public partial class BookLoc : UnitTestEntity
    {
        #region 构造函数

        public BookLoc() { }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected BookLoc(SerializationInfo info, StreamingContext context) : base(info, context) { }

        #endregion

        #region 引用属性

        #endregion

        #region 组合子属性

        #endregion

        #region 一般属性

        public static readonly Property<string> CodeProperty = P<BookLoc>.Register(e => e.Code);
        public string Code
        {
            get { return this.GetProperty(CodeProperty); }
            set { this.SetProperty(CodeProperty, value); }
        }

        public static readonly Property<string> NameProperty = P<BookLoc>.Register(e => e.Name);
        public string Name
        {
            get { return this.GetProperty(NameProperty); }
            set { this.SetProperty(NameProperty, value); }
        }

        #endregion

        #region 只读属性

        #endregion
    }

    /// <summary>
    /// 书籍货架 列表类。
    /// </summary>
    [Serializable]
    public partial class BookLocList : UnitTestEntityList { }

    /// <summary>
    /// 书籍货架 仓库类。
    /// 负责 书籍货架 类的查询、保存。
    /// </summary>
    public partial class BookLocRepository : UnitTestEntityRepository
    {
        /// <summary>
        /// 单例模式，外界不可以直接构造本对象。
        /// </summary>
        protected BookLocRepository() { }
    }

    [DataProviderFor(typeof(BookLocRepository))]
    public partial class BookLocRepositoryDataProvider : UnitTestEntityRepositoryDataProvider
    {
        public int TestSaveListTransactionItemCount = -1;

        protected override void Submit(SubmitArgs e)
        {
            if (TestSaveListTransactionItemCount >= 0)
            {
                TestSaveListTransactionItemCount++;
                if (TestSaveListTransactionItemCount > 1)
                {
                    throw new NotSupportedException("超过一条数据，直接抛出异常。之前的数据需要回滚。");
                }
            }

            base.Submit(e);
        }
    }

    /// <summary>
    /// 书籍货架 配置类。
    /// 负责 书籍货架 类的实体元数据、界面元数据的配置。
    /// </summary>
    internal class BookLocConfig : UnitTestEntityConfig<BookLoc>
    {
        /// <summary>
        /// 配置实体的元数据
        /// </summary>
        protected override void ConfigMeta()
        {
            //配置实体的所有属性都映射到数据表中。
            Meta.MapTable().MapAllProperties();
        }
    }
}