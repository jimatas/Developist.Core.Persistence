﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Developist.Core.Persistence.Utilities
{
    internal static class ArgumentOutOfRangeExceptionHelper
    {
        public static T ThrowIfOutOfRange<T>(Expression<Func<IComparable<T>>> argument, IComparable<T>? minValue = null, IComparable<T>? maxValue = null)
        {
            var value = argument.Compile().Invoke();
            if (minValue != null && Comparer<T>.Default.Compare((T)value, (T)minValue) < 0 ||
                maxValue != null && Comparer<T>.Default.Compare((T)value, (T)maxValue) > 0)
            {
                throw new ArgumentOutOfRangeException(paramName: argument.GetMemberName(), actualValue: value, message: FormatMessage(minValue, maxValue));
            }
            return (T)value;
        }

        public static T ThrowIfOutOfRange<T>(IComparable<T> value, string paramName, IComparable<T>? minValue = null, IComparable<T>? maxValue = null)
        {
            if (minValue != null && Comparer<T>.Default.Compare((T)value, (T)minValue) < 0 ||
                maxValue != null && Comparer<T>.Default.Compare((T)value, (T)maxValue) > 0)
            {
                throw new ArgumentOutOfRangeException(paramName, actualValue: value, message: FormatMessage(minValue, maxValue));
            }
            return (T)value;
        }

        private static string FormatMessage(object? minValue, object? maxValue)
        {
            var message = "Value cannot be";
            if (minValue != null)
            {
                message += $" less than {minValue}";
            }

            if (maxValue != null)
            {
                if (minValue != null)
                {
                    message += " or";
                }
                message += $" greater than {maxValue}";
            }
            message += ".";
            return message;
        }
    }
}
