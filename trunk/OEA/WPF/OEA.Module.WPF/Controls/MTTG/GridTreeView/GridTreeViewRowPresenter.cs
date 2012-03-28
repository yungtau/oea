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
using System.Diagnostics;
using System.Reflection;
using System.ComponentModel;
using System.Windows.Input;
using OEA.Module.WPF;
using System.Windows.Media;
using System.Windows.Data;
using System.Collections.Specialized;
using System.Windows.Controls.Primitives;

namespace OEA.Module.WPF.Controls
{
    /// <summary>
    /// GridTreeViewRow ������ʾ����
    /// ��ʹ�� GridView ��������ʾ����
    /// 
    /// ÿһ�еĵ�һ��Ӧ���ǣ�RowHeader + Expander + ColumnUI
    /// </summary>
    public class GridTreeViewRowPresenter : GridViewRowPresenter
    {
        #region �ֶ�

        /// <summary>
        /// ���� RowHeader ���ܲ��� UIElement��������������Ҫ����һ������
        /// </summary>
        private ContentControl _rowHeaderContainer = new ContentControl();

        /// <summary>
        /// ��Ҫ����һ�� UIElementCollection Ԫ�أ����������� Add �����������ʵ�������ӿؼ��� LogicalParent �� VisualParent��
        /// </summary>
        private UIElementCollection _childs;

        #endregion

        public GridTreeViewRowPresenter()
        {
            this._childs = new UIElementCollection(this, this);
            this.SetAncestorBinding(ColumnsProperty, GridTreeView.ColumnsProperty, typeof(GridTreeView));
            this.SetAncestorBinding(HasRowHeaderProperty, GridTreeView.HasRowNoProperty, typeof(GridTreeView));
            this.SetAncestorBinding(HideExpanderProperty, GridTreeView.OnlyGridModeProperty, typeof(GridTreeView));

            //����ʹ�� Loaded �¼�������ʹ�� LayoutUpdated �¼���
            //������ʱ�ڶ������壨GIX4 �������ԣ�����޷��ҵ� Columns ����ԭ��δ�顣
            this.LayoutUpdated += new EventHandler(OnLayoutUpdated);
        }

        #region RowHeaderWidthProperty

        public static readonly DependencyProperty RowHeaderWidthProperty = DependencyProperty.Register(
            "RowHeaderWidth", typeof(int), typeof(GridTreeViewRowPresenter), new PropertyMetadata(21)
            );

        /// <summary>
        /// �кŵ���������
        /// </summary>
        public int RowHeaderWidth
        {
            get { return (int)this.GetValue(RowHeaderWidthProperty); }
            set { this.SetValue(RowHeaderWidthProperty, value); }
        }

        #endregion

        #region RowHeaderProperty

        public static DependencyProperty RowHeaderProperty = DependencyProperty.Register(
            "RowHeader", typeof(object), typeof(GridTreeViewRowPresenter),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, (d, e) => (d as GridTreeViewRowPresenter).OnRowHeaderChanged(e))
            );

        /// <summary>
        /// ���������Ҫ���ڰ� GridTreeViewRow �е� RowHeader
        /// </summary>
        public object RowHeader
        {
            get { return this.GetValue(RowHeaderProperty); }
            set { this.SetValue(RowHeaderProperty, value); }
        }

        private void OnRowHeaderChanged(DependencyPropertyChangedEventArgs e)
        {
            this.OnVisualChildChanged(e);

            this._rowHeaderContainer.Content = e.NewValue;
        }

        #endregion

        #region FirstColumnIndentProperty

        public static DependencyProperty FirstColumnIndentProperty = DependencyProperty.Register(
            "FirstColumnIndent", typeof(double), typeof(GridTreeViewRowPresenter),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender)
            );

        /// <summary>
        /// ��ǰ�еĵ�һ�е���������
        /// 
        /// ���ֵ��Ҫ�󶨵� GridTreeViewRow �� Level �ϡ�
        /// </summary>
        public double FirstColumnIndent
        {
            get { return (double)this.GetValue(FirstColumnIndentProperty); }
            set { this.SetValue(FirstColumnIndentProperty, value); }
        }

        #endregion

        #region ExpanderProperty

        public static DependencyProperty ExpanderProperty = DependencyProperty.Register(
            "Expander", typeof(UIElement), typeof(GridTreeViewRowPresenter),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, (d, e) => (d as GridTreeViewRowPresenter).OnVisualChildChanged(e))
            );

        public UIElement Expander
        {
            get { return (UIElement)this.GetValue(ExpanderProperty); }
            set { this.SetValue(ExpanderProperty, value); }
        }

        private void OnVisualChildChanged(DependencyPropertyChangedEventArgs e)
        {
            var oldElement = e.OldValue as UIElement;
            if (oldElement != null) { this._childs.Remove(oldElement); }

            var newElement = e.NewValue as UIElement;
            if (newElement != null) { this._childs.Add(newElement); }
        }

        #endregion

        #region HasRowHeaderProperty

        private static readonly DependencyProperty HasRowHeaderProperty = DependencyProperty.Register(
            "HasRowHeader", typeof(bool), typeof(GridTreeViewRowPresenter),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender)
            );

        /// <summary>
        /// �Ƿ���� RowHeader �ؼ���
        /// 
        /// ���ֵ��ֱ�Ӱ󶨵� <see cref="GridTreeView.HasRowNoProperty"/> �����ϵġ�
        /// </summary>
        private bool HasRowHeader
        {
            get { return (bool)this.GetValue(HasRowHeaderProperty); }
        }

        #endregion

        #region HideExpanderProperty

        private static readonly DependencyProperty HideExpanderProperty = DependencyProperty.Register(
            "HideExpander", typeof(bool), typeof(GridTreeViewRowPresenter),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender)
            );

        /// <summary>
        /// �Ƿ���Ҫ������ Expander��
        /// 
        /// ע�⣺
        /// ������Ҫ��ʾ�㼶�����������㵱ǰ��û�����У����� Expander Ҳ��Ҫռλ���� Hidden ������ Collapsed��
        /// ���ֻ�е���ȫ����ʾ�㼶����ʱ�����ֵ�Ż��� true��
        /// 
        /// ���ֵ��ֱ�Ӱ󶨵� <see cref="GridTreeView.OnlyGridModeProperty"/> �����ϵġ�
        /// </summary>
        private bool HideExpander
        {
            get { return (bool)this.GetValue(HideExpanderProperty); }
        }

        #endregion

        #region ========= ���� == ��ͼ ===========

        /**********************************************************************
         * 
         * ˵����
         * ÿһ�еĵ�һ��Ӧ���ǣ�RowHeader + Expander + ColumnUI
         * λ�ö������ڵ�һ�У�
         * �������������Ԫ����Ϊ�� Visual �б�����������Index ���� RowHeader ��ǰ��Expander �ں�
         * ��GridTreeViewRow ��Ҳ��ʹ�õ�������򡣣�
         * 
        **********************************************************************/

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            //��һ���еĲ��֣�RowHeaderWidth + FirstColumnIndent(Level) + ExpanderWidth + ColumnUI

            var size = base.ArrangeOverride(arrangeSize);

            var columns = this.Columns;
            if (columns == null || columns.Count == 0) return size;

            double columnX = 0;
            double rowWidth = arrangeSize.Width;
            for (int i = 0; i < columns.Count; i++)
            {
                var column = columns[i] as GridTreeViewColumn;

                var actualIndex = column.ActualIndexReflected;
                var uiColumn = (UIElement)base.GetVisualChild(actualIndex);

                var columnAvailableWidth = column.StateReflected != ColumnMeasureState.SpecificWidth ?
                    column.DesiredWidthReflected : column.Width;
                columnAvailableWidth = Math.Min(columnAvailableWidth, rowWidth);

                if (i == 0)
                {
                    //����������Ҫ�Ŀ�ȣ�
                    var columnWidth = columnAvailableWidth;

                    //���� RowHeader
                    if (this.HasRowHeader)
                    {
                        var rowHeaderWidth = Math.Min(columnWidth, this.RowHeaderWidth);

                        this._rowHeaderContainer.Arrange(new Rect(0, 0, rowHeaderWidth, double.MaxValue));

                        columnX += rowHeaderWidth;
                        columnWidth -= rowHeaderWidth;
                    }

                    //���� FirstColumnIndent
                    var firstColumnIndent = Math.Min(columnWidth, this.FirstColumnIndent);
                    columnX += firstColumnIndent;
                    columnWidth -= firstColumnIndent;

                    //���� Expander
                    if (!this.HideExpander)
                    {
                        var expander = this.Expander;
                        double expanderWidth = Math.Min(columnWidth, expander.DesiredSize.Width);

                        expander.Arrange(new Rect(columnX, 0, expanderWidth, expander.DesiredSize.Height));

                        columnX += expanderWidth;
                        columnWidth -= expanderWidth;
                    }

                    //this._firstColumnDesiredWidth = columnX + uiColumn.DesiredSize.Width;

                    //���� ColumnUI
                    uiColumn.Arrange(new Rect(columnX, 0, columnWidth, arrangeSize.Height));

                    columnX += columnWidth;
                }
                else
                {
                    uiColumn.Arrange(new Rect(columnX, 0, columnAvailableWidth, arrangeSize.Height));

                    columnX += columnAvailableWidth;
                }

                rowWidth -= columnAvailableWidth;
                if (rowWidth == 0) break;
            }

            return size;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var columns = base.Columns;
            if (columns == null) { return default(Size); }

            double desiredHeight = 0.0;
            double x = 0.0;
            double maxHeight = constraint.Height;
            bool flag = false;
            for (int i = 0, c = columns.Count; i < c; i++)
            {
                var current = columns[i] as GridTreeViewColumn;
                var actualIndex = current.ActualIndexReflected;
                UIElement uIElement = base.GetVisualChild(actualIndex) as UIElement;
                if (uIElement != null)
                {
                    double availableWidth = Math.Max(0.0, constraint.Width - x);
                    double columnWidth = availableWidth;

                    var state = current.StateReflected;
                    if (state == ColumnMeasureState.Init || state == ColumnMeasureState.Headered)
                    {
                        if (!flag)
                        {
                            GridViewInternal.EnsureDesiredWidthListMethod.Invoke(this, null);
                            base.LayoutUpdated += new EventHandler(this.OnMeasureLayoutUpdated);
                            flag = true;
                        }

                        if (i == 0)
                        {
                            if (this.HasRowHeader)
                            {
                                x += this.RowHeaderWidth;
                                columnWidth -= this.RowHeaderWidth;
                                //this._rowHeaderContainer.Measure(new Size(columnWidth, maxHeight));
                                //x += this._rowHeaderContainer.DesiredSize.Width;
                                //columnWidth -= this._rowHeaderContainer.DesiredSize.Width;
                            }

                            var firstColumnIndent = Math.Min(this.FirstColumnIndent, columnWidth);
                            x += firstColumnIndent;
                            columnWidth -= firstColumnIndent;

                            if (!this.HideExpander)
                            {
                                var expander = this.Expander;
                                expander.Measure(new Size(columnWidth, maxHeight));
                                x += expander.DesiredSize.Width;
                                columnWidth -= expander.DesiredSize.Width;
                            }

                            uIElement.Measure(new Size(columnWidth, maxHeight));
                            var desiredWidth = current.EnsureWidth(uIElement.DesiredSize.Width + x);
                            this.DesiredWidthList[actualIndex] = desiredWidth;
                            x = desiredWidth;
                        }
                        else
                        {

                            uIElement.Measure(new Size(columnWidth, maxHeight));
                            var desiredWidth = current.EnsureWidth(uIElement.DesiredSize.Width);
                            this.DesiredWidthList[actualIndex] = desiredWidth;
                            x += desiredWidth;
                        }
                    }
                    else
                    {
                        if (state == ColumnMeasureState.Data)
                        {
                            columnWidth = Math.Min(columnWidth, current.DesiredWidthReflected);
                            uIElement.Measure(new Size(columnWidth, maxHeight));
                            x += current.DesiredWidthReflected;
                        }
                        else
                        {
                            columnWidth = Math.Min(columnWidth, current.Width);
                            uIElement.Measure(new Size(columnWidth, maxHeight));
                            x += current.Width;
                        }
                    }
                    desiredHeight = Math.Max(desiredHeight, uIElement.DesiredSize.Height);
                }
            }
            x += 2.0;

            return new Size(x, desiredHeight);
        }

        protected override Visual GetVisualChild(int index)
        {
            var rawCount = base.VisualChildrenCount;

            if (index < rawCount) return base.GetVisualChild(index);

            var rowHeader = this.RowHeader;

            //����һ��Ԫ�أ�������ʹ�� RowHeader
            if (index == rawCount)
            {
                if (this.HasRowHeader) { return this._rowHeaderContainer; }
                return this.Expander;
            }

            //��������Ԫ�أ���ڶ���Ԫ��Ӧ���� Expander
            if (index == rawCount + 1) { return this.Expander; }

            throw new NotSupportedException();
        }

        protected override int VisualChildrenCount
        {
            get
            {
                var count = base.VisualChildrenCount;

                if (this.HasRowHeader) count++;

                if (!this.HideExpander) count++;

                return count;
            }
        }

        #region ���ƻ��� MeasureOverride �����Ĵ��룬�������µ� GridTreeViewColumn.EnsureWidth ����

        private List<double> _desiredWidthList;

        public List<double> DesiredWidthList
        {
            get
            {
                if (this._desiredWidthList == null)
                {
                    this._desiredWidthList = GridViewInternal.DesiredWidthListProperty
                            .GetValue(this, null) as List<double>;
                }

                return this._desiredWidthList;
            }
        }

        private void OnMeasureLayoutUpdated(object sender, EventArgs e)
        {
            bool flag = false;
            foreach (GridTreeViewColumn current in base.Columns)
            {
                if (current.StateReflected != ColumnMeasureState.SpecificWidth)
                {
                    current.StateReflected = ColumnMeasureState.Data;

                    var actualIndex = current.ActualIndexReflected;

                    if (this.DesiredWidthList == null || actualIndex >= this.DesiredWidthList.Count)
                    {
                        flag = true;
                        break;
                    }

                    var desiredWidth = current.DesiredWidthReflected;
                    if (!GridViewInternal.AreClose(desiredWidth, this.DesiredWidthList[actualIndex]))
                    {
                        this.DesiredWidthList[actualIndex] = desiredWidth;
                        flag = true;
                    }
                }
            }
            if (flag)
            {
                base.InvalidateMeasure();
            }
            base.LayoutUpdated -= new EventHandler(this.OnMeasureLayoutUpdated);
        }

        #endregion

        #endregion

        #region ��������

        private void SetAncestorBinding(
            DependencyProperty property, DependencyProperty ancestorProperty, Type ancestorType
            )
        {
            this.SetBinding(property, new Binding
            {
                Path = new PropertyPath(ancestorProperty),
                Mode = BindingMode.OneWay,
                RelativeSource = new RelativeSource
                {
                    Mode = RelativeSourceMode.FindAncestor,
                    AncestorType = ancestorType
                }
            });
        }

        #endregion

        #region ClearCellContainerMargin

        private bool _marginInitialized;

        private void OnLayoutUpdated(object sender, EventArgs e)
        {
            this.InitMargin();
        }

        private void InitMargin()
        {
            if (this._marginInitialized) return;

            var columns = this.Columns;
            if (columns != null)
            {
                this._marginInitialized = true;

                this.ClearCellContainerMargin();

                columns.CollectionChanged += (o, ee) =>
                {
                    switch (ee.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                        case NotifyCollectionChangedAction.Replace:
                        case NotifyCollectionChangedAction.Reset:
                            this.ClearCellContainerMargin();
                            break;
                        default:
                            break;
                    }
                };
            }
        }

        /// <summary>
        /// ������ GridViewRowPresenter.CreateCell ������ֱ��������ÿһ�� Cell �� Margin �� 6,0��
        /// ����������Ҫ����Щ Margin ��ȥ����
        /// </summary>
        private void ClearCellContainerMargin()
        {
            var e = this.LogicalChildren;
            while (e.MoveNext())
            {
                var cellContainer = e.Current as FrameworkElement;
                if (cellContainer == null) break;

                cellContainer.ClearValue(FrameworkElement.MarginProperty);
            }
        }

        #endregion

        //protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    base.OnPropertyChanged(e);

        //    if (e.Property == WidthProperty)
        //    {
        //        this.InvalidateArrange();
        //    }
        //}
    }
}