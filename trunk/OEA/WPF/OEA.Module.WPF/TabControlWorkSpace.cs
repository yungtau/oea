﻿/*******************************************************
 * 
 * 作者：胡庆访
 * 创建时间：20110401
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 运行环境：.NET 4.0
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 胡庆访 20100401
 * 
*******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using OEA.Module.WPF.Controls;

namespace OEA.Module.WPF
{
    /// <summary>
    /// 使用 TabControl 来实现 IWorkspace。
    /// </summary>
    public class TabControlWorkSpace : IWorkspace
    {
        private TabControl _tabControl;

        public TabControlWorkSpace(TabControl tc)
        {
            if (tc == null) throw new ArgumentNullException("tc");

            this._tabControl = tc;
            this._tabControl.SelectionChanged += new SelectionChangedEventHandler(On_tabControl_SelectionChanged);
        }

        public IEnumerable<FrameworkElement> Windows
        {
            get
            {
                return this._tabControl.Items.Cast<TabItem>()
                    .Select(t => GetWindow(t));
            }
        }

        public FrameworkElement ActiveWindow
        {
            get
            {
                var tabItem = this._tabControl.SelectedItem as TabItem;
                return GetWindow(tabItem);
            }
        }

        public void Add(FrameworkElement window)
        {
            var ti = new CloseableTabItem()
            {
                Header = WorkspaceWindow.GetTitle(window),
                Content = window
            };
            ti.AddHandler(TabItem.PreviewMouseLeftButtonDownEvent,
                new MouseButtonEventHandler(this.On_tabItem_PreviewMouseButtonDown)
                );

            this._tabControl.Items.Add(ti);
        }

        public bool TryRemove(FrameworkElement window)
        {
            try
            {
                this._suppressEvent = true;

                var title = WorkspaceWindow.GetTitle(window);
                TabItem dc = GetTabItem(title);
                if (dc != null)
                {
                    var args = new WorkspaceWindowClosingEventArgs(window);
                    this.OnWindowClosing(args);
                    if (!args.Cancel)
                    {
                        this._tabControl.Items.Remove(dc);

                        //处理内存泄漏：当没有页签时，清除SelectedItem属性，
                        //否则仍旧保留了最后一个页面的值在DependencyObject的_effectiveValues中
                        if (this._tabControl.Items.Count == 0)
                        {
                            this._tabControl.SelectedItem = null;
                        }

                        this.OnWindowClosed(new WorkspaceWindowChangedEventArgs(window, null));

                        return true;
                    }
                }

                return false;
            }
            finally
            {
                this._suppressEvent = false;
            }
        }

        public bool TryActive(FrameworkElement window)
        {
            try
            {
                this._suppressEvent = true;

                var title = WorkspaceWindow.GetTitle(window);
                TabItem newItem = GetTabItem(title);
                if (newItem != null)
                {
                    var oldItem = this._tabControl.SelectedItem as TabItem;
                    if (oldItem == newItem) { return true; }

                    var args = new WorkspaceWindowActivingEventArgs(GetWindow(oldItem), window);
                    this.OnWindowActiving(args);

                    if (!args.Cancel)
                    {
                        this._tabControl.SelectedItem = newItem;

                        this.OnWindowActived(args);

                        return true;
                    }
                }

                return false;
            }
            finally
            {
                this._suppressEvent = false;
            }
        }

        #region 适配控件事件

        private bool _suppressEvent = false;

        /// <summary>
        /// 发生 WindowActiving 事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_tabItem_PreviewMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this._suppressEvent) return;

            //如果发生事件的源头在 _tabControl 中的 TabItem
            var newItem = e.Source as TabItem;
            if (newItem != null && newItem.Parent == this._tabControl)
            {
                var oldItem = this._tabControl.SelectedItem as TabItem;
                if (oldItem != e.Source)
                {
                    var deactiveWindow = GetWindow(oldItem);
                    var activeWindow = GetWindow(newItem); ;
                    var args = new WorkspaceWindowActivingEventArgs(deactiveWindow, activeWindow);

                    this.OnWindowActiving(args);

                    if (args.Cancel)
                    {
                        e.Handled = true;
                    }
                    else
                    {
                        //由于在 Activing 事件中很可能会让 newItem 失去焦点（例如弹出窗口），所以需要重新让该页签获取焦点，
                        //否则页签由于没有焦点，就不会发生 SelectedItemChanged 事件。
                        newItem.Focus();
                    }
                }
            }
        }

        /// <summary>
        /// 发生 OnWindowActived 事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //TabControl 内部的小 TabControl 的事件会冒泡到这里，需要进行过滤。
            if (e.Source != sender || this._suppressEvent) return;

            if (e.AddedItems.Count > 0)
            {
                var newTabitem = e.AddedItems[0] as TabItem;
                if (newTabitem != null)
                {
                    var newWindow = GetWindow(newTabitem);

                    FrameworkElement oldWindow = null;
                    if (e.RemovedItems.Count > 0)
                    {
                        var oldTabItem = e.RemovedItems[0] as TabItem;
                        oldWindow = GetWindow(oldTabItem);
                    }

                    this.OnWindowActived(new WorkspaceWindowActivedEventArgs(oldWindow, newWindow));
                }
            }
        }

        #endregion

        #region 事件

        public event EventHandler<WorkspaceWindowActivedEventArgs> WindowActived;

        private void OnWindowActived(WorkspaceWindowActivedEventArgs e)
        {
            var handler = this.WindowActived;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<WorkspaceWindowActivingEventArgs> WindowActiving;

        private void OnWindowActiving(WorkspaceWindowActivingEventArgs e)
        {
            var handler = this.WindowActiving;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<WorkspaceWindowClosingEventArgs> WindowClosing;

        private void OnWindowClosing(WorkspaceWindowClosingEventArgs e)
        {
            var handler = this.WindowClosing;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<WorkspaceWindowChangedEventArgs> WindowClosed;

        private void OnWindowClosed(WorkspaceWindowChangedEventArgs e)
        {
            var handler = this.WindowClosed;
            if (handler != null) handler(this, e);
        }

        #endregion

        #region 帮助方法

        public FrameworkElement GetWindow(string title)
        {
            TabItem item = GetTabItem(title);
            return GetWindow(item);
        }

        private TabItem GetTabItem(string title)
        {
            return this._tabControl.Items.Cast<TabItem>()
                .FirstOrDefault(item => item.Header.ToString() == title);
        }

        private static FrameworkElement GetWindow(TabItem item)
        {
            return item != null ? item.Content as FrameworkElement : null;
        }

        #endregion
    }
}