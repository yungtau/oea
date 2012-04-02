﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OEA.Module.WPF.Layout
{
    public partial class DetailLayout : TraditionalLayout
    {
        public DetailLayout()
        {
            InitializeComponent();
        }

        public override void TryArrangeMain(ControlResult control)
        {
            if (control != null)
            {
                result.Content = control.Control;
            }
            else
            {
                result.RemoveFromParent();
            }
        }

        public override void TryArrangeCommandsContainer(ControlResult toolBar)
        {
            if (toolBar != null)
            {
                toolBarContainer.Content = toolBar.Control;
            }
            else
            {
                toolBarContainer.RemoveFromParent();
            }
        }

        protected override void OnArrangedCore()
        {
            ResizingPanelExt.SetStarGridLength(detail, 3);
            ResizingPanelExt.SetStarGridLength(childrenTab, 7);

            if (this.AggtBlocks.Layout.IsLayoutChildrenHorizonal)
            {
                container.Orientation = Orientation.Horizontal;
            }
            else
            {
                container.Orientation = Orientation.Vertical;
            }
        }

        protected override TabControl ChildrenTab
        {
            get { return childrenTab; }
        }
    }
}