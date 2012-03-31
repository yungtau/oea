/*******************************************************
 * 
 * ���ߣ������
 * ����ʱ�䣺20120330
 * ˵�������ļ�ֻ����һ���࣬�������ݼ�����ע�͡�
 * ���л�����.NET 4.0
 * �汾�ţ�1.0.0
 * 
 * ��ʷ��¼��
 * ���䵽 Entity���й������ϡ� ����� 20120330
 * 
*******************************************************/

using System;
using System.Collections.Generic;
using OEA.ManagedProperty;

namespace OEA.Library.Validation
{
    /// <summary>
    /// �򵥵Ĺ����б�
    /// 
    /// �ṩ����ķ���
    /// </summary>
    internal class RulesContainer
    {
        private List<IRuleMethod> _list = new List<IRuleMethod>();
        private bool _sorted;

        public void Add(IRuleMethod item)
        {
            _list.Add(item);
            _sorted = false;
        }

        public List<IRuleMethod> GetList(bool applySort)
        {
            if (applySort && !_sorted)
            {
                lock (_list)
                {
                    if (applySort && !_sorted)
                    {
                        _list.Sort();
                        _sorted = true;
                    }
                }
            }
            return _list;
        }
    }
}
