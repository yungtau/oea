﻿/*******************************************************
 * 
 * 作者：胡庆访
 * 创建日期：20131210
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 运行环境：.NET 4.0
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 胡庆访 20131210 10:49
 * 
*******************************************************/

using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Rafy;
using Rafy.Data;
using Rafy.Domain.ORM.SqlTree;

namespace Rafy.Domain.ORM
{
    /// <summary>
    /// 为 SqlNode 语法树生成相应 Sql 的生成器。
    /// </summary>
    abstract class SqlGenerator : SqlNodeVisitor
    {
        private FormattedSql _sql;

        public SqlGenerator()
        {
            _sql = new FormattedSql();
            _sql.InnerWriter = new IndentedTextWriter(_sql.InnerWriter);
            this.AutoQuota = true;
        }

        /// <summary>
        /// 当前需要的缩进量。
        /// </summary>
        protected int Indent
        {
            get { return (_sql.InnerWriter as IndentedTextWriter).Indent; }
            set { (_sql.InnerWriter as IndentedTextWriter).Indent = value; }
        }

        /// <summary>
        /// 是否自动添加标识符的括号
        /// </summary>
        public bool AutoQuota { get; set; }

        /// <summary>
        /// 生成完毕后的 Sql 语句及参数。
        /// </summary>
        public FormattedSql Sql
        {
            get { return _sql; }
        }

        #region 分页支持

        /// <summary>
        /// 为指定的原始查询生成指定分页效果的新查询。
        /// </summary>
        /// <param name="raw">原始查询</param>
        /// <param name="pkColumn">需要指定主键列</param>
        /// <param name="pagingInfo">分页信息。</param>
        /// <returns></returns>
        public virtual SqlSelect ModifyToPagingTree(SqlSelect raw, SqlColumn pkColumn, PagingInfo pagingInfo)
        {
            if (PagingInfo.IsNullOrEmpty(pagingInfo)) { throw new ArgumentNullException("pagingInfo"); }
            if (!raw.HasOrdered()) { throw new InvalidProgramException("必须排序后才能使用分页功能。"); }

            //如果是第一页，则只需要使用 TOP 语句即可。
            if (pagingInfo.PageNumber == 1)
            {
                raw.Top = pagingInfo.PageSize;
                return raw;
            }

            /*********************** 代码块解释 *********************************
             * 
             * 转换方案：
             * 
             * SELECT * 
             * FROM ASN
             * WHERE ASN.Id > 0
             * ORDER BY ASN.AsnCode ASC
             * 
             * SELECT TOP 10 * 
             * FROM ASN
             * WHERE ASN.Id > 0 AND ASN.Id NOT IN(
             *     SELECT TOP 20 Id
             *     FROM ASN
             *     WHERE ASN.Id > 0 
             *     ORDER BY ASN.AsnCode ASC
             * )
             * ORDER BY ASN.AsnCode ASC
             * 
            **********************************************************************/

            var excludeSelect = new SqlSelect
            {
                Top = (pagingInfo.PageNumber - 1) * pagingInfo.PageSize,
                Selection = pkColumn,
                From = raw.From,
                Where = raw.Where,
                OrderBy = raw.OrderBy,
            };

            var res = new SqlSelect
            {
                Top = pagingInfo.PageSize,
                Selection = raw.Selection,
                From = raw.From,
                OrderBy = raw.OrderBy,
            };

            var newWhere = new SqlColumnConstraint
            {
                Column = pkColumn,
                Operator = SqlColumnConstraintOperator.NotIn,
                Value = excludeSelect
            };
            if (raw.Where != null)
            {
                res.Where = new SqlBinaryConstraint
                {
                    Left = raw.Where,
                    Opeartor = SqlBinaryConstraintType.And,
                    Right = newWhere
                };
            }
            else
            {
                res.Where = newWhere;
            }

            return res;
        }

        #endregion

        /// <summary>
        /// 访问 sql 语法树中的每一个结点，并生成相应的 Sql 语句。
        /// </summary>
        /// <param name="tree"></param>
        public void Generate(SqlNode tree)
        {
            base.Visit(tree);
        }

        protected override SqlLiteral VisitSqlLiteral(SqlLiteral sqlLiteral)
        {
            if (sqlLiteral.Parameters != null && sqlLiteral.Parameters.Length > 0)
            {
                sqlLiteral.FormattedSql = Regex.Replace(sqlLiteral.FormattedSql, @"\{(?<index>\d+)\}", m =>
                {
                    var index = Convert.ToInt32(m.Groups["index"].Value);
                    var value = sqlLiteral.Parameters[index];
                    index = _sql.Parameters.Add(value);
                    return "{" + index + "}";
                });
            }

            _sql.Append(sqlLiteral.FormattedSql);
            return sqlLiteral;
        }

        protected override SqlBinaryConstraint VisitSqlBinaryConstraint(SqlBinaryConstraint node)
        {
            var leftBinary = node.Left as SqlBinaryConstraint;
            var rightBinary = node.Right as SqlBinaryConstraint;
            var isLeftOr = leftBinary != null && leftBinary.Opeartor == SqlBinaryConstraintType.Or;
            var isRightOr = rightBinary != null && rightBinary.Opeartor == SqlBinaryConstraintType.Or;

            switch (node.Opeartor)
            {
                case SqlBinaryConstraintType.And:
                    if (isLeftOr) _sql.Append("(");
                    this.Visit(node.Left);
                    if (isLeftOr) _sql.Append(")");
                    _sql.AppendAnd();
                    if (isRightOr) _sql.Append("(");
                    this.Visit(node.Right);
                    if (isRightOr) _sql.Append(")");
                    break;
                case SqlBinaryConstraintType.Or:
                    this.Visit(node.Left);
                    _sql.AppendOr();
                    this.Visit(node.Right);
                    break;
                default:
                    throw new NotSupportedException();
            }

            return node;
        }

        protected override SqlSelect VisitSqlSelect(SqlSelect sqlSelect)
        {
            _sql.Append("SELECT ");

            if (sqlSelect.IsCounting)
            {
                _sql.Append("COUNT(0)");
            }
            else
            {
                if (sqlSelect.IsDistinct)
                {
                    _sql.Append("DISTINCT ");
                }
                else if (sqlSelect.Top.HasValue)
                {
                    _sql.Append("TOP ");
                    _sql.Append(sqlSelect.Top.Value);
                    _sql.Append(" ");
                }

                //选择的列
                if (sqlSelect.Selection == null)
                {
                    _sql.Append("*");
                }
                else
                {
                    this.Visit(sqlSelect.Selection);
                }
            }

            //FROM
            _sql.AppendLine();
            _sql.Append("FROM ");
            this.Visit(sqlSelect.From);

            //WHERE
            if (sqlSelect.Where != null)
            {
                _sql.AppendLine();
                _sql.Append("WHERE ");
                this.Visit(sqlSelect.Where);
            }

            //ORDER BY
            if (!sqlSelect.IsCounting && sqlSelect.OrderBy != null && sqlSelect.OrderBy.Count > 0)
            {
                _sql.AppendLine();
                _sql.Append("ORDER BY ");

                for (int i = 0, c = sqlSelect.OrderBy.Count; i < c; i++)
                {
                    var item = sqlSelect.OrderBy[i] as SqlOrderBy;
                    if (i > 0)
                    {
                        _sql.Append(", ");
                    }
                    this.AppendColumnUsage(item.Column);
                    _sql.Append(" ");
                    _sql.Append(item.Direction == OrderDirection.Ascending ? "ASC" : "DESC");
                }
            }

            return sqlSelect;
        }

        protected override SqlColumn VisitSqlColumn(SqlColumn sqlColumn)
        {
            this.AppendColumn(sqlColumn);

            return sqlColumn;
        }

        protected override SqlTable VisitSqlTable(SqlTable sqlTable)
        {
            this.QuoteAppend(sqlTable.TableName);
            if (!string.IsNullOrEmpty(sqlTable.Alias))
            {
                _sql.Append(" AS ");
                this.QuoteAppend(sqlTable.Alias);
            }

            return sqlTable;
        }

        protected override SqlJoin VisitSqlJoin(SqlJoin sqlJoin)
        {
            this.Visit(sqlJoin.Left);

            switch (sqlJoin.JoinType)
            {
                //case SqlJoinType.Cross:
                //    _sql.Append(", ");
                //    break;
                case SqlJoinType.Inner:
                    _sql.AppendLine();
                    this.Indent++;
                    _sql.Append("INNER JOIN ");
                    this.Indent--;
                    break;
                case SqlJoinType.LeftOuter:
                    _sql.AppendLine();
                    this.Indent++;
                    _sql.Append("LEFT OUTER JOIN ");
                    this.Indent--;
                    break;
                default:
                    throw new NotSupportedException();
            }

            this.Visit(sqlJoin.Right);

            _sql.Append(" ON ");

            this.Visit(sqlJoin.Condition);

            return sqlJoin;
        }

        protected override SqlArray VisitSqlArray(SqlArray sqlArray)
        {
            for (int i = 0, c = sqlArray.Items.Count; i < c; i++)
            {
                var item = sqlArray.Items[i] as SqlNode;
                if (i > 0)
                {
                    _sql.Append(", ");
                }
                this.Visit(item);
            }

            return sqlArray;
        }

        protected override SqlColumnConstraint VisitSqlColumnConstraint(SqlColumnConstraint node)
        {
            var op = node.Operator;
            var value = node.Value;

            #region 处理一些特殊的值

            switch (op)
            {
                case SqlColumnConstraintOperator.Like:
                case SqlColumnConstraintOperator.Contains:
                case SqlColumnConstraintOperator.StartWith:
                case SqlColumnConstraintOperator.EndWith:
                    //如果是空字符串的模糊对比操作，直接认为是真。
                    var strValue = value as string;
                    if (string.IsNullOrEmpty(strValue))
                    {
                        _sql.Append("1 = 1");
                        return node;
                    }
                    break;
                case SqlColumnConstraintOperator.In:
                case SqlColumnConstraintOperator.NotIn:
                    //对于 In、NotIn 操作，如果传入的是空列表时，需要特殊处理：
                    //In(Empty) 表示 false，NotIn(Empty) 表示 true。
                    if (value is IEnumerable)
                    {
                        bool hasValue = false;
                        foreach (var item in value as IEnumerable)
                        {
                            hasValue = true;
                            break;
                        }
                        if (!hasValue)
                        {
                            if (op == SqlColumnConstraintOperator.In)
                            {
                                _sql.Append("0 = 1");
                            }
                            else
                            {
                                _sql.Append("1 = 1");
                            }

                            return node;
                        }
                    }
                    break;
                default:
                    break;
            }

            #endregion

            this.AppendColumnUsage(node.Column);

            //根据不同的操作符，来生成不同的_sql。
            switch (op)
            {
                case SqlColumnConstraintOperator.Equal:
                    if (value == null || value == DBNull.Value)
                    {
                        _sql.Append(" IS NULL");
                    }
                    else
                    {
                        _sql.Append(" = ");
                        _sql.AppendParameter(value);
                    }
                    break;
                case SqlColumnConstraintOperator.NotEqual:
                    if (value == null || value == DBNull.Value)
                    {
                        _sql.Append(" IS NOT NULL");
                    }
                    else
                    {
                        _sql.Append(" != ");
                        _sql.AppendParameter(value);
                    }
                    break;
                case SqlColumnConstraintOperator.Greater:
                    _sql.Append(" > ");
                    _sql.AppendParameter(value);
                    break;
                case SqlColumnConstraintOperator.GreaterEqual:
                    _sql.Append(" >= ");
                    _sql.AppendParameter(value);
                    break;
                case SqlColumnConstraintOperator.Less:
                    _sql.Append(" < ");
                    _sql.AppendParameter(value);
                    break;
                case SqlColumnConstraintOperator.LessEqual:
                    _sql.Append(" <= ");
                    _sql.AppendParameter(value);
                    break;
                case SqlColumnConstraintOperator.Like:
                    _sql.Append(" LIKE ");
                    _sql.AppendParameter(value);
                    break;
                case SqlColumnConstraintOperator.Contains:
                    _sql.Append(" LIKE ");
                    _sql.AppendParameter("%" + value + "%");
                    break;
                case SqlColumnConstraintOperator.StartWith:
                    _sql.Append(" LIKE ");
                    _sql.AppendParameter(value + "%");
                    break;
                case SqlColumnConstraintOperator.EndWith:
                    _sql.Append(" LIKE ");
                    _sql.AppendParameter("%" + value);
                    break;
                case SqlColumnConstraintOperator.In:
                case SqlColumnConstraintOperator.NotIn:
                    var opSql = op == SqlColumnConstraintOperator.In ? "IN" : "NOT IN";
                    _sql.Append(" ").Append(opSql).Append(" (");
                    if (value is IEnumerable)
                    {
                        bool first = true;
                        foreach (var item in value as IEnumerable)
                        {
                            if (!first) _sql.Append(',');
                            _sql.AppendParameter(item);
                            first = false;
                        }
                    }
                    else if (value is SqlNode)
                    {
                        _sql.AppendLine();
                        this.Indent++;
                        this.Visit(value as SqlNode);
                        this.Indent--;
                        _sql.AppendLine();
                    }
                    _sql.Append(')');
                    break;
                default:
                    throw new NotSupportedException();
            }

            return node;
        }

        protected override SqlSelectAll VisitSqlSelectAll(SqlSelectAll sqlSelectStar)
        {
            if (sqlSelectStar.Table != null)
            {
                this.QuoteAppend(sqlSelectStar.Table.GetName());
                _sql.Append(".*");
            }
            else
            {
                _sql.Append("*");
            }

            return sqlSelectStar;
        }

        protected override SqlColumnsComparisonConstraint VisitSqlColumnsComparisonConstraint(SqlColumnsComparisonConstraint node)
        {
            this.AppendColumnUsage(node.LeftColumn);
            switch (node.Operator)
            {
                case SqlColumnConstraintOperator.Equal:
                    _sql.Append(" = ");
                    break;
                case SqlColumnConstraintOperator.NotEqual:
                    _sql.Append(" != ");
                    break;
                case SqlColumnConstraintOperator.Greater:
                    _sql.Append(" > ");
                    break;
                case SqlColumnConstraintOperator.GreaterEqual:
                    _sql.Append(" >= ");
                    break;
                case SqlColumnConstraintOperator.Less:
                    _sql.Append(" < ");
                    break;
                case SqlColumnConstraintOperator.LessEqual:
                    _sql.Append(" <= ");
                    break;
                default:
                    throw new NotSupportedException("两个属性之间的对比，只能使用 6 类基本对比。");
            }
            this.AppendColumnUsage(node.RightColumn);

            return node;
        }

        protected override SqlExistsConstraint VisitSqlExistsConstraint(SqlExistsConstraint sqlExistsConstraint)
        {
            _sql.Append("EXISTS (");
            _sql.AppendLine();

            this.Indent++;
            this.Visit(sqlExistsConstraint.Select);
            this.Indent--;

            _sql.AppendLine();
            _sql.Append(")");

            return sqlExistsConstraint;
        }

        protected override SqlNotConstraint VisitSqlNotConstraint(SqlNotConstraint sqlNotConstraint)
        {
            _sql.Append("NOT (");
            this.Visit(sqlNotConstraint.Constraint);
            _sql.Append(")");

            return sqlNotConstraint;
        }

        protected override SqlSubSelect VisitSqlSubSelect(SqlSubSelect sqlSelectRef)
        {
            _sql.Append("(");
            _sql.AppendLine();
            this.Indent++;
            this.Visit(sqlSelectRef.Select);
            this.Indent--;
            _sql.AppendLine();
            _sql.Append(") AS ");
            _sql.Append(sqlSelectRef.Alias);

            return sqlSelectRef;
        }

        /// <summary>
        /// 把标识符添加到 Sql 语句中。
        /// 子类可重写此方法来为每一个标识符添加引用符。
        /// SqlServer 生成 [identifier]
        /// Oracle 生成 "IDENTIFIER"
        /// </summary>
        /// <param name="identifier"></param>
        protected virtual void QuoteAppend(string identifier)
        {
            identifier = this.PrepareIdentifier(identifier);
            _sql.Append(identifier);
        }

        /// <summary>
        /// 准备所有标识符。
        /// Oracle 可重写此方法，使得标识符都是大写的。
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        protected virtual string PrepareIdentifier(string identifier)
        {
            return identifier;
        }

        private void AppendColumn(SqlColumn sqlColumn)
        {
            this.QuoteAppend(sqlColumn.Table.GetName());

            _sql.Append(".");
            this.QuoteAppend(sqlColumn.ColumnName);

            if (!string.IsNullOrEmpty(sqlColumn.Alias))
            {
                _sql.Append(" AS ");
                this.QuoteAppend(sqlColumn.Alias);
            }
        }

        private void AppendColumnUsage(SqlColumn sqlColumn)
        {
            this.QuoteAppend(sqlColumn.Table.GetName());
            _sql.Append(".");
            this.QuoteAppend(sqlColumn.ColumnName);
        }
    }
}
