﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Developist.Core.Persistence.Utilities
{
    internal static class ArgumentNullExceptionHelper
    {
        [return: NotNull]
        public static T ThrowIfNull<T>(Expression<Func<T>> argument)
        {
            var value = argument.Compile().Invoke();
            if (value is null)
            {
                var paramName = argument.GetMemberName();
                throw new ArgumentNullException(paramName, $"Parameter '{paramName}' cannot be null.");
            }
            return value;
        }
    }
}
