﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Sdl.Community.DsiViewer.Converters
{
	public class MultiBooleanToVisibilityConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Length != 2
			 || values[0] is not bool textBoxVisible
			 || values[1] is not bool isChecked)
			{
				return Visibility.Collapsed;
			}

            var visibility = (textBoxVisible && isChecked) ? Visibility.Visible : Visibility.Collapsed;

            return visibility;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
			=> throw new NotSupportedException();
	}
}