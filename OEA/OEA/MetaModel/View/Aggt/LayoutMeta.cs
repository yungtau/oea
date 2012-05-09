﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OEA.MetaModel.View
{
    public class LayoutMeta
    {
        public LayoutMeta(string layoutClass)
            : this()
        {
            this.Class = layoutClass;
        }

        public LayoutMeta() { }

        /// <summary>
        /// 布局类的名称
        /// 不可为 null。
        /// 
        /// 如果是 WPF，则这个字符串表示一个 LayoutMethod 类的子类的 AssemblyQualifiedName
        /// </summary>
        public string Class { get; set; }

        #region WPF

        /// <summary>
        /// 本构造函数是 WPF 专用
        /// </summary>
        /// <param name="layoutMethodType">
        /// 该类需要继承自 LayoutMethod，一般情况下，只需要使用 TraditionalLayoutMethod 泛型类即可。
        /// </param>
        public LayoutMeta(Type layoutMethodType)
            : this()
        {
            if (OEAEnvironment.IsWPF)
            {
                this.Class = layoutMethodType.AssemblyQualifiedName;
            }
        }

        /// <summary>
        /// 父子的分布比例
        /// 此属性 可空
        /// </summary>
        public ParentChildProportion ParentChildProportion { get; set; }

        /// <summary>
        /// 是否把聚合子对象横向排列。
        /// 默认为 false。
        /// </summary>
        public bool IsLayoutChildrenHorizonal { get; set; }

        #endregion
    }

    /// <summary>
    /// 父子的分布比例
    /// </summary>
    public class ParentChildProportion
    {
        /// <summary>
        /// 通过修改这个对象中的值，可以达到修改整个应用程序默认值的效果
        /// </summary>
        public static readonly ParentChildProportion Default = new ParentChildProportion(3, 7);

        public ParentChildProportion(double parent, double children)
        {
            this.Parent = parent;
            this.Children = children;
        }

        public ParentChildProportion() { }

        public double Parent { get; set; }

        public double Children { get; set; }
    }
}
