using System;
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
                throw new ArgumentNullException(paramName: argument.GetMemberName());
            }
            return value;
        }
    }
}
