﻿/*******************************************************
 * 
 * 作者：胡庆访
 * 创建时间：20120416
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 运行环境：.NET 4.0
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 胡庆访 20120416
 * 
*******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using Rafy;
using Rafy.Reflection;
using Rafy.MetaModel;
using Rafy.MetaModel.Attributes;
using Rafy.MetaModel.View;

namespace JXC.Web.Templates
{
    /// <summary>
    /// 通用的条件查询单据模块模板
    /// </summary>
    public class ConditionQueryBlocksTemplate : BlocksTemplate
    {
        /// <summary>
        /// 生成条件查询和主体模块
        /// </summary>
        /// <returns></returns>
        protected override AggtBlocks DefineBlocks()
        {
            var entityType = this.EntityType;

            AggtBlocks result = new AggtBlocks
            {
                MainBlock = new Block(entityType),
            };

            var conAttri = entityType.GetSingleAttribute<ConditionQueryTypeAttribute>();
            if (conAttri != null)
            {
                result.Surrounders.Add(new AggtBlocks
                {
                    MainBlock = new ConditionBlock()
                    {
                        EntityType = conAttri.QueryType,
                    },
                });
            }

            return result;
        }
    }
}