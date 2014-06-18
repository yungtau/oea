﻿/*******************************************************
 * 
 * 作者：胡庆访
 * 创建时间：20120220
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 运行环境：.NET 4.0
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 胡庆访 20120220
 * 
*******************************************************/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Web;
using Rafy;
using Rafy.MetaModel;
using Rafy.MetaModel.View;
using Rafy.Web.ClientMetaModel;

namespace Rafy.Web
{
    /// <summary>
    /// WebApp 启动环境
    /// </summary>
    public class WebApp : AppImplementationBase
    {
        public void Startup()
        {
            try
            {
                this.StartupApplication();
            }
            catch (Exception ex)
            {
                Logger.LogError("Web 项目启动时发生异常", ex);
                throw;
            }
        }

        public void NotifyExit()
        {
            try
            {
                this.OnExit();
            }
            catch (Exception ex)
            {
                Logger.LogError("Web 项目退出时发生异常", ex);
                throw;
            }
        }

        protected override void PrepareToStartup()
        {
            base.PrepareToStartup();

            JsonServiceRepository.Clear();
        }

        protected override void InitEnvironment()
        {
            //如果在初始化时发生异常，则会引发再次启动。这时应该保证之前的所有的初始化工作归零。
            var isWcfServer = ConfigurationHelper.GetAppSettingOrDefault("Rafy_IsWCFServer", false);
            RafyEnvironment.Location.IsWebUI = !isWcfServer;
            RafyEnvironment.Location.IsWPFUI = false;
            RafyEnvironment.Location.DataPortalMode = DataPortalMode.ConnectDirectly;

            base.InitEnvironment();

            if (RafyEnvironment.Location.IsWebUI)
            {
                JsonServiceRepository.LoadAllServices();
            }
        }

        protected override void CompileMeta()
        {
            //虽然是 WebApp，但是可能只是作为 WPFClient 的服务端。
            if (RafyEnvironment.Location.IsWebUI)
            {
                //webcommand 需要在实体前初始化好
                this.InitCommandMetas();
            }

            base.CompileMeta();
        }

        private void InitCommandMetas()
        {
            UIModel.WebCommands.AddByAssembly(typeof(WebApp).Assembly);

            UIModel.InitCommandMetas();

            //UIModel.Commands.SortByName(CommandNames.CommonCommands);
            //UIModel.Commands.SortByLabel("添加", "编辑", "删除", "保存", "刷新");

            this.OnCommandMetasIntialized();
        }

        protected override void OnAppModelCompleted()
        {
            UIModel.Freeze();

            base.OnAppModelCompleted();
        }

        public override void Shutdown()
        {
            throw new InvalidOperationException("Web 服务器不支持关闭操作。");
        }
    }
}