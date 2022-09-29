using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace Developist.Core.Persistence.Utilities
{
    internal static class ArgumentExceptionHelper
    {
        [return: NotNull]
        public static string ThrowIfNullOrEmpty(Expression<Func<string?>> argument)
        {
            var value = ArgumentNullExceptionHelper.ThrowIfNull(argument);
            if (value.Length == 0)
            {
                throw new ArgumentException(message: "Value cannot be empty.", paramName: argument.GetMemberName());
            }
            return value;
        }

        [return: NotNull]
        public static string ThrowIfNullOrWhiteSpace(Expression<Func<string?>> argument)
        {
            var value = ThrowIfNullOrEmpty(argument);
            if (value.All(char.IsWhiteSpace))
            {
                throw new ArgumentException(message: "Value cannot be all white space.", paramName: argument.GetMemberName());
            }
            return value;
        }
    }
}
