/*******************************************************
 * 
 * 作者：胡庆访
 * 创建时间：20110414
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 运行环境：.NET 4.0
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 胡庆访 20110414
 * 
*******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OEA.MetaModel.Attributes;
using OEA.ORM;
using OEA;
using OEA.Library;
using System.Diagnostics;
using OEA.MetaModel;
using OEA.MetaModel.View;
using OEA.ManagedProperty;
using System.Runtime.Serialization;

namespace OEA.RBAC
{
    [Serializable]
    [RootEntity]
    public class UserLoginLog : Entity
    {
        public static readonly RefProperty<User> UserRefProperty =
            P<UserLoginLog>.RegisterRef(e => e.User, ReferenceType.Normal);
        public int UserId
        {
            get { return this.GetRefId(UserRefProperty); }
            set { this.SetRefId(UserRefProperty, value); }
        }
        public User User
        {
            get { return this.GetRefEntity(UserRefProperty); }
            set { this.SetRefEntity(UserRefProperty, value); }
        }

        public static readonly Property<string> UserNameProperty = P<UserLoginLog>.RegisterReadOnly(e => e.UserName, e => (e as UserLoginLog).GetUserName(), null);
        public string UserName
        {
            get { return this.GetProperty(UserNameProperty); }
        }
        private string GetUserName()
        {
            return this.User.Name;
        }

        /// <summary>
        /// 是否表示登入操作。（true：登录；false：登出。）
        /// </summary>
        public static readonly Property<bool> IsInProperty = P<UserLoginLog>.Register(e => e.IsIn);
        public bool IsIn
        {
            get { return this.GetProperty(IsInProperty); }
            set { this.SetProperty(IsInProperty, value); }
        }

        public static readonly Property<string> IsInTextProperty = P<UserLoginLog>.RegisterReadOnly(e => e.IsInText, e => (e as UserLoginLog).GetIsInText(), null);
        public string IsInText
        {
            get { return this.GetProperty(IsInTextProperty); }
        }
        private string GetIsInText()
        {
            return this.IsIn ? "登录" : "退出";
        }

        /// <summary>
        /// 记录时间
        /// </summary>
        public static readonly Property<DateTime> LogTimeProperty = P<UserLoginLog>.Register(e => e.LogTime);
        public DateTime LogTime
        {
            get { return this.GetProperty(LogTimeProperty); }
            set { this.SetProperty(LogTimeProperty, value); }
        }
    }

    [Serializable]
    public partial class UserLoginLogList : EntityList { }

    public class UserLoginLogRepository : EntityRepository
    {
        protected UserLoginLogRepository() { }
    }

    internal class UserLoginLogConfig : EntityConfig<UserLoginLog>
    {
        protected override void ConfigMeta()
        {
            base.ConfigMeta();

            this.Meta.MapTable().HasColumns(
                UserLoginLog.UserRefProperty,
                UserLoginLog.IsInProperty,
                UserLoginLog.LogTimeProperty
                );
        }

        protected override void ConfigView()
        {
            base.ConfigView();

            View.DisableEditing(true)
                .RemoveWPFCommands(WPFCommandNames.Add, WPFCommandNames.Cancel, WPFCommandNames.Delete, WPFCommandNames.SaveList)
                .DomainName("用户登录记录");

            View.Property(UserLoginLog.UserNameProperty).HasLabel("用户").ShowIn(ShowInWhere.List);
            View.Property(UserLoginLog.IsInTextProperty).HasLabel("类型").ShowIn(ShowInWhere.List);
            View.Property(UserLoginLog.LogTimeProperty).HasLabel("时间");
        }
    }

    /// <summary>
    /// 为UserLoginLog提供了一些便利的服务方法。
    /// </summary>
    public static class UserLoginLogService
    {
        /// <summary>
        /// 登录之后的用户。
        /// 
        /// 由于当前系统只让一个用户登录。
        /// 登录成功后，使用这个静态字段保存它。
        /// </summary>
        private static User _user;

        /// <summary>
        /// 外部系统使用本服务来记录登录操作
        /// </summary>
        /// <param name="user"></param>
        public static void NotifyLogin(User user)
        {
            Debug.Assert(user != null, "user != null");

            _user = user;

            Log(user, true);
        }

        /// <summary>
        /// 外部系统使用本服务来记录出操作
        /// </summary>
        public static void NotifyLogout()
        {
            if (_user != null) { Log(_user, false); }
        }

        /// <summary>
        /// 记录操作
        /// </summary>
        /// <param name="user"></param>
        /// <param name="isIn">
        /// 登入还是登出
        /// </param>
        private static void Log(User user, bool isIn)
        {
            RF.Save(new UserLoginLog()
            {
                User = user,
                IsIn = isIn,
                LogTime = DateTime.Now
            });
        }
    }
}