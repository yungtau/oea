﻿/*******************************************************
 * 
 * 作者：胡庆访
 * 创建时间：20120226
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 运行环境：.NET 4.0
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 胡庆访 20120226
 * 
*******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OEA.MetaModel.View;
using OEA.MetaModel.Attributes;
using System.Reflection;
using OEA.ManagedProperty;


namespace OEA.MetaModel.View
{
    /// <summary>
    /// 从代码中读取 EntityViewMeta
    /// </summary>
    class CodeEVMReader
    {
        /// <summary>
        /// 从一个实体类型读取它所对应的视图模型。
        /// </summary>
        /// <param name="meta"></param>
        /// <returns></returns>
        internal EntityViewMeta Read(EntityMeta meta)
        {
            var evm = this.CreateEntityViewMeta(meta);

            this.SetDefaultCommands(evm);

            foreach (var config in OEAEnvironment.FindConfigurations(meta.EntityType))
            {
                config.View = evm;
                config.ConfigViewInternal();
            }

            return evm;
        }

        private void SetDefaultCommands(EntityViewMeta evm)
        {
            if (!OEAEnvironment.IsWeb)
            {
                //初始化实体视图中的命令按钮
                var em = evm.EntityMeta;
                if (em.EntityCategory == EntityCategory.ConditionQueryObject)
                {
                    evm.UseWPFCommands(WPFCommandNames.CustomizeUI, WPFCommandNames.FireQuery);
                }
                else if (em.EntityCategory == EntityCategory.NavigationQueryObject)
                {
                    evm.UseWPFCommands(WPFCommandNames.CustomizeUI);
                }
                else
                {
                    if (em.IsTreeEntity)
                    {
                        evm.UseWPFCommands(WPFCommandNames.TreeCommands);
                    }
                    else
                    {
                        evm.UseWPFCommands(WPFCommandNames.CommonCommands);
                    }

                    if (em.EntityCategory == EntityCategory.Root)
                    {
                        evm.UseWPFCommands(WPFCommandNames.RootCommands);
                    }
                }

                var commands = evm.WPFCommands;
                if (evm.NotAllowEdit)
                {
                    commands.Remove(WPFCommandNames.Edit);
                    commands.Remove(WPFCommandNames.SaveList);
                }
            }
            else
            {
                //初始化实体视图中的命令按钮
                var em = evm.EntityMeta;
                if (em.EntityCategory == EntityCategory.ConditionQueryObject
                    || em.EntityCategory == EntityCategory.NavigationQueryObject)
                {
                    evm.UseWebCommands(WebCommandNames.CustomizeUI);
                }
                else
                {
                    if (em.IsTreeEntity)
                    {
                        evm.UseWebCommands(WebCommandNames.TreeCommands);
                    }
                    else
                    {
                        evm.UseWebCommands(WebCommandNames.CommonCommands);
                    }
                }

                var commands = evm.WebCommands;
                if (evm.NotAllowEdit)
                {
                    commands.Remove(WebCommandNames.Edit);
                    commands.Remove(WebCommandNames.Save);
                }
            }
        }

        /// <summary>
        /// 创建某个实体类的视图元数据
        /// </summary>
        /// <param name="entityMeta"></param>
        /// <returns></returns>
        private EntityViewMeta CreateEntityViewMeta(EntityMeta entityMeta)
        {
            var entityType = entityMeta.EntityType;
            var vm = new EntityViewMeta
            {
                EntityMeta = entityMeta,
            };

            this.CreatePropertiesViewMeta(vm);

            return vm;
        }

        /// <summary>
        /// 加载所有属性元数据
        /// </summary>
        /// <param name="boType"></param>
        private void CreatePropertiesViewMeta(EntityViewMeta viewMeta)
        {
            foreach (var property in EntityMetaHelper.GetEntityProperties(viewMeta.EntityMeta))
            {
                this.CreateEntityPropertyViewMeta(property, viewMeta);
            }

            //加入扩展属性元数据
            foreach (var mp in EntityMetaHelper.GetEntityPropertiesExtension(viewMeta.EntityMeta))
            {
                this.CreateExtensionPropertyViewMeta(mp, viewMeta);
            }
        }

        private EntityPropertyViewMeta CreateEntityPropertyViewMeta(PropertySource propertySource, EntityViewMeta evm)
        {
            var runtimeProperty = propertySource.CLR;

            var item = new EntityPropertyViewMeta
            {
                Owner = evm
            };

            var em = evm.EntityMeta;
            item.PropertyMeta = em.EntityProperties.First(p => (p.Runtime.Core as PropertyInfo) == runtimeProperty);

            //数字的默认显示方式
            var labelAttri = runtimeProperty.GetSingleAttribute<LabelAttribute>();
            if (labelAttri != null) item.Label = labelAttri.Label;

            item.IsReadonly = !runtimeProperty.CanWrite ||
                propertySource.MP != null && propertySource.MP.IsReadOnly ||
                runtimeProperty.Name == DBConvention.FieldName_TreeCode;

            item.EditorName = FindEditorName(item.PropertyMeta);

            #region 创建 ReferenceViewInfo

            var ri = item.PropertyMeta.ReferenceInfo;

            //如果是引用实体的属性
            if (ri != null)
            {
                var rvi = new ReferenceViewInfo
                {
                    ReferenceInfo = ri
                };
                item.ReferenceViewInfo = rvi;

                var lookupAttri = runtimeProperty.GetSingleAttribute<LookupAttribute>();
                if (lookupAttri != null)
                {
                    rvi.DataSourceProperty = lookupAttri.DataSourceProperty;
                    rvi.SelectedValuePath = lookupAttri.SelectedValuePath;
                    rvi.RootPIdProperty = lookupAttri.RootPIdProperty;
                    rvi.SelectionMode = lookupAttri.SelectionMode;
                }
            }

            #endregion

            evm.EntityProperties.Add(item);

            return item;
        }

        internal EntityPropertyViewMeta CreateExtensionPropertyViewMeta(IManagedProperty mp, EntityViewMeta evm)
        {
            var epm = evm.EntityMeta.Property(mp);

            var epvm = new EntityPropertyViewMeta()
            {
                Owner = evm,
                PropertyMeta = epm,
                EditorName = FindEditorName(epm)
            };

            evm.EntityProperties.Add(epvm);

            return epvm;
        }

        /// <summary>
        /// 根据属性的类型，获取编辑器的名称
        /// </summary>
        /// <param name="epm"></param>
        /// <returns></returns>
        private string FindEditorName(EntityPropertyMeta epm)
        {
            if (epm.ReferenceInfo != null)
            {
                return WPFEditorNames.LookupDropDown;
            }

            var propertyType = epm.Runtime.PropertyType;
            if (propertyType.IsEnum) { return WPFEditorNames.Enum; }

            return propertyType.FullName;
        }
    }
}