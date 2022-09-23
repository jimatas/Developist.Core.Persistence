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
                var paramName = argument.GetMemberName();
                throw new ArgumentException($"Parameter '{paramName}' cannot be empty.", paramName);
            }
            return value;
        }

        [return: NotNull]
        public static string ThrowIfNullOrWhiteSpace(Expression<Func<string?>> argument)
        {
            var value = ThrowIfNullOrEmpty(argument);
            if (value.All(char.IsWhiteSpace))
            {
                var paramName = argument.GetMemberName();
                throw new ArgumentException($"Parameter '{paramName}' cannot be all white space.", paramName);
            }
            return value;
        }
    }
}
