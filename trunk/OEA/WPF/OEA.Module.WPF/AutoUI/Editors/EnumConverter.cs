/*******************************************************
 * 
 * ���ߣ�Glodon
 * ����ʱ�䣺2009
 * ˵�������ļ�ֻ����һ���࣬�������ݼ�����ע�͡�
 * ���л�����.NET 4.0
 * �汾�ţ�1.0.0
 * 
 * ��ʷ��¼��
 * �����ļ� Glodon 2009
 * 
*******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using OEA.Module.WPF.Editors;

using System.Globalization;
using OEA.MetaModel;
using OEA.MetaModel.View;
using OEA.Utils;

namespace OEA.Module.WPF.Converter
{
    public class EnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) { return string.Empty; }

            return new EnumViewModel((Enum)value).Label;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var strValue = value.ToString();

            if (string.IsNullOrEmpty(strValue)) return null;

            return EnumViewModel.LabelToEnum(strValue, targetType);
        }
    }
}