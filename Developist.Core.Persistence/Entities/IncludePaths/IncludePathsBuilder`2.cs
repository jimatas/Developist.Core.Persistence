using Developist.Core.Persistence.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Developist.Core.Persistence.Entities.IncludePaths
{
    public class IncludePathsBuilder<TEntity, TProperty> : IncludePathsBuilder<TEntity>, IIncludePathsBuilder<TEntity, TProperty>
        where TEntity : IEntity
    {
        internal IncludePathsBuilder(IList<string> paths) 
            : base(paths) { }

        public void ThenInclude(string pathSegment)
        {
            if (!Paths.Any())
            {
                throw new InvalidOperationException($"{nameof(ThenInclude)} cannot be called before at least one path has been included.");
            }

            ArgumentExceptionHelper.ThrowIfNullOrWhiteSpace(() => pathSegment);
            Paths[^1] += $".{pathSegment}";
        }
    }
}
