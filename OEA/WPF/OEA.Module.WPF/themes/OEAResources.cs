﻿/*******************************************************
 * 
 * 作者：胡庆访
 * 创建时间：20110421
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 运行环境：.NET 4.0
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 胡庆访 20110421
 * 
*******************************************************/

using System.Windows;
using System;

namespace OEA.Module.WPF
{
    /// <summary>
    /// OEA 框架在皮肤中用到的所有约定样式
    /// </summary>
    public static class OEAResources
    {
        public static Style GroupContainerStyle
        {
            get { return FindStyle("OEA_GroupContainerStyle"); }
        }

        public static Style CommandsContainer
        {
            get { return FindStyle("OEA_CommandsContainer"); }
        }

        public static Style TabControlHeaderHide
        {
            get { return FindStyle("OEA_TabControlHeaderHide"); }
        }

        public static Style StringPropertyEditor_TextBox
        {
            get { return FindStyle("OEA_StringPropertyEditor_TextBox"); }
        }

        public static Style TreeColumn_TextBlock
        {
            get { return FindStyle("OEA_TreeColumn_TextBlock"); }
        }

        public static Style TreeColumn_TextBlock_Number
        {
            get { return FindStyle("OEA_TreeColumn_TextBlock_Number"); }
        }

        public static DataTemplate OEA_MTTG_SelectionColumnTemplate
        {
            get { return FindResource("OEA_MTTG_SelectionColumnTemplate") as DataTemplate; }
        }

        public static Style FindStyle(object key)
        {
            return FindResource(key) as Style;
        }

        public static object FindResource(object key)
        {
            var app = Application.Current;
            return app != null ? app.TryFindResource(key) : null;
        }

        /// <summary>
        /// 把指定的Resouce加入到应用程序中
        /// 
        /// 使用方法：
        /// AddResource("OEA.Module.WPF;component/Resources/ComboListControl.xaml");
        /// </summary>
        /// <param name="packUri"></param>
        public static void AddResource(string packUri)
        {
            var uri = new Uri(packUri, UriKind.RelativeOrAbsolute);
            var resouceDic = Application.LoadComponent(uri) as ResourceDictionary;
            var app = Application.Current;
            if (app != null) app.Resources.MergedDictionaries.Add(resouceDic);
        }
    }
}