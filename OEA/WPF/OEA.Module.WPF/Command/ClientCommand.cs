/*******************************************************
 * 
 * ���ߣ������
 * ����ʱ�䣺2011
 * ˵�������ļ�ֻ����һ���࣬�������ݼ�����ע�͡�
 * ���л�����.NET 4.0
 * �汾�ţ�1.0.0
 * 
 * ��ʷ��¼��
 * �����ļ� ����� 2011
 * 
*******************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using OEA.Editors;
using OEA.Library;
using OEA.MetaModel;
using OEA.MetaModel.View;
using OEA.Module.WPF;
using OEA.Module.WPF.Controls;
using OEA.Module;

namespace OEA.WPF.Command
{
    public abstract class ClientCommand<TParamater> : ClientCommand
        where TParamater : class
    {
        protected override sealed bool CanExecuteCore(object param) { return this.CanExecute(param as TParamater); }

        protected override sealed void ExecuteCore(object param) { this.Execute(param as TParamater); }

        public virtual bool CanExecute(TParamater view) { return true; }

        public abstract void Execute(TParamater view);

        protected override void OnExecuteFailed(CommandExecuteFailedArgs e)
        {
            base.OnExecuteFailed(e);

            var sqlex = e.Exception.GetBaseException() as SqlException;
            if (sqlex != null)
            {
                var sqlerr = SqlErrorInfo.GetSqlError(sqlex.Number);
                if (sqlerr == null) return;

                App.MessageBox.Show(sqlerr.ErrorMessage);
                e.Cancel = true;
            }
        }
    }

    public abstract class ViewCommand : ClientCommand<ObjectView> { }

    public abstract class ListViewCommand : ClientCommand<ListObjectView> { }
}