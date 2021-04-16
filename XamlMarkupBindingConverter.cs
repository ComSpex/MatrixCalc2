// $Header: /ComSpex/MatrixCalc.root/MatrixCalc/XamlMarkupBindingConverter.cs 2     13/10/29 9:42 Yosuke $
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace MatrixCalc {
	[ValueConversion(typeof(double),typeof(string))]
	public class StringFormatter:IValueConverter {
		public object Convert(object value,Type targetType,object parameter,CultureInfo culture) {
			return String.Format(Window1.double_format,System.Convert.ToDouble(value));
		}
		public object ConvertBack(object value,Type targetType,object parameter,CultureInfo culture) {
			return value;
		}
	}
}
