/*******************************************************
 * 
 * ���ߣ������
 * ����ʱ�䣺20111123
 * ˵�������ļ�ֻ����һ���࣬�������ݼ�����ע�͡�
 * ���л�����.NET 4.0
 * �汾�ţ�1.0.0
 * 
 * ��ʷ��¼��
 * �����ļ� ����� 20111123
 * 
*******************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Collections;
using System.Windows.Automation.Peers;
using System.Collections.Specialized;

namespace OEA.Module.WPF.Controls
{
    /// <summary>
    /// һ������� TreeView�����е�ÿһ���һ������С�
    /// </summary>
    public class GridTreeView : TreeView
    {
        static GridTreeView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridTreeView), new FrameworkPropertyMetadata(typeof(GridTreeView)));
        }

        public GridTreeView()
        {
            this.Columns = new GridTreeViewColumnCollection();
        }

        public MultiTypesTreeGrid TreeGrid { get; internal set; }

        /// <summary>
        /// ���û��ȡ�ڱ��ģʽ�£�<see cref="OnlyGridMode"/> = true���Ƿ��Ѿ����� UI ���⻯��
        /// </summary>
        internal bool IsGridVirtualizing
        {
            get { return VirtualizingStackPanel.GetIsVirtualizing(this); }
            set
            {
                /*********************** �������� *********************************
                 * ���ø����Իᴥ�� Style �е� Trigger ���滻 ItemsPanel��
                 * ���÷����� TreeView Ĭ��ģ��ģ���еķ���һ�£�
                 * 
                 * ���� UIV���μ���http://www.cnblogs.com/zgynhqf/archive/2011/12/12/2284335.html
                **********************************************************************/

                VirtualizingStackPanel.SetIsVirtualizing(this, value);
            }
        }

        #region ColumnsProperty

        private static readonly DependencyPropertyKey ColumnsPropertyKey = DependencyProperty.RegisterReadOnly(
            "Columns", typeof(GridTreeViewColumnCollection), typeof(GridTreeView), new PropertyMetadata((d, e) => (d as GridTreeView).OnColumnsChanged(e))
            );

        public static readonly DependencyProperty ColumnsProperty = ColumnsPropertyKey.DependencyProperty;

        /// <summary> 
        /// ������ʾ���е��б�
        /// </summary>
        public GridTreeViewColumnCollection Columns
        {
            get { return (GridTreeViewColumnCollection)this.GetValue(ColumnsProperty); }
            internal set { this.SetValue(ColumnsPropertyKey, value); }
        }

        private void OnColumnsChanged(DependencyPropertyChangedEventArgs e)
        {
            var oldColumns = e.OldValue as GridTreeViewColumnCollection;
            if (oldColumns != null) { oldColumns.Owner = null; }

            var newColumns = e.NewValue as GridTreeViewColumnCollection;
            if (newColumns != null) { newColumns.Owner = this; }
        }

        #endregion

        #region DataProperty

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(IList), typeof(GridTreeView));

        /// <summary>
        /// GridTreeView����ʾ��ItemScource��
        /// 
        /// ��������Ե�ԭ����MultiTypesTreeGrid������ϣ���ʹ��GridTreeView��ItemSource���ԣ�����ֱ�ӿ���Items��
        /// ��������ʹ��������Ա���ItemSource�����ֵ��
        /// 
        /// ��Ҫ����Xaml�󶨡�
        /// </summary>
        public IList Data
        {
            get { return (IList)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        #endregion

        #region HasRowNoProperty

        public static readonly DependencyProperty HasRowNoProperty = DependencyProperty.Register(
            "HasRowNo", typeof(bool), typeof(GridTreeView),
            new PropertyMetadata(true)
            );

        /// <summary>
        /// �Ƿ���Ҫ��ʾ�кš�
        /// </summary>
        public bool HasRowNo
        {
            get { return (bool)this.GetValue(HasRowNoProperty); }
            set { this.SetValue(HasRowNoProperty, value); }
        }

        #endregion

        #region OnlyGridModeProperty

        public static readonly DependencyProperty OnlyGridModeProperty = DependencyProperty.Register(
            "OnlyGridMode", typeof(bool), typeof(GridTreeView)
            );

        /// <summary>
        /// ��ʾ�Ƿ��� ���򵥱�� ģʽ�¡�
        /// ��ģʽ�£�û�в㼶�ڵ㡣
        /// </summary>
        public bool OnlyGridMode
        {
            get { return (bool)this.GetValue(OnlyGridModeProperty); }
            set { this.SetValue(OnlyGridModeProperty, value); }
        }

        #endregion

        protected override DependencyObject GetContainerForItemOverride()
        {
            throw new NotSupportedException("Container �� MultiTypesTreeGrid �ؼ��ڲ����ɣ��˷�����Ч��");
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is GridTreeViewRow;
        }

        #region ֧���Զ���

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new GridTreeViewAutomationPeer(this);
        }

        #endregion
    }

    public class GridTreeViewAutomationPeer : TreeViewAutomationPeer
    {
        private GridTreeView _owner;

        public GridTreeViewAutomationPeer(GridTreeView owner)
            : base(owner)
        {
            this._owner = owner;
        }

        protected override string GetNameCore()
        {
            return this._owner.TreeGrid.RootEntityViewMeta.Label;
        }
    }
}
