﻿using System;
using System.Data;
using System.Collections.Generic;
using hxy.Common.Data;

namespace OEA.ORM.sqlserver
{
    public class SqlConstraint : IConstraint
    {
        private string column;
        private string oper;
        private object val;
        private SqlQuery query;
        protected int index;

        public SqlConstraint(string column, string op, object val)
        {
            this.column = column.ToLower();
            this.oper = op;
            this.val = val;
        }

        public SqlConstraint(IQuery query)
        {
            this.query = (SqlQuery)query;
        }

        public string Column
        {
            get { return column; }
        }

        public string Operator
        {
            get { return oper; }
        }

        public object Value
        {
            get { return val; }
            set { val = value; }
        }

        public bool HasQuery
        {
            get { return query != null; }
        }

        public IQuery Query
        {
            get { return query; }
        }

        public virtual string GetSql(SqlTable table, ref int offset)
        {
            if (HasQuery)
            {
                string sql = query.GetSql(table, ref offset);
                return string.Format("({0})", sql);
            }
            else
            {
                index = offset;
                offset = offset + 1;
                string name = table.Translate(column);
                if (name == null || name.Length == 0)
                    //name = column;
                    throw new LightException(string.Format("column {0} not found in table {1}", column, table.Name));
                return string.Format("[{0}]{1}@{2}", name, oper, index);
            }
        }

        public virtual void SetParameters(IParameterFactory pf, List<IDbDataParameter> paramaters)
        {
            if (HasQuery)
            {
                query.SetParameters(pf, paramaters);
            }
            else
            {
                IDbDataParameter p = pf.CreateParameter();
                p.ParameterName = string.Format("@{0}", index);
                SqlUtils.PrepParam(p, val);
                paramaters.Add(p);
            }
        }
    }
}
