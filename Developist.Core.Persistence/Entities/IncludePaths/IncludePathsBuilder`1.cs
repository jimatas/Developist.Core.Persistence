using Developist.Core.Persistence.Utilities;

using System.Collections.Generic;
using System.Linq;

namespace Developist.Core.Persistence.Entities.IncludePaths
{
    public class IncludePathsBuilder<TEntity> : IIncludePathsBuilder<TEntity>
        where TEntity : IEntity
    {
        public IncludePathsBuilder()
            : this(new List<string>()) { }

        protected IncludePathsBuilder(IList<string> paths) => Paths = paths;

        protected IList<string> Paths { get; }

        public void Include(string path)
        {
            ArgumentExceptionHelper.ThrowIfNullOrWhiteSpace(() => path);
            Paths.Add(path);
        }

        public string[] ToArray() => Paths.ToArray();

        IList<string> IIncludePathsBuilder<TEntity>.AsList() => Paths;
    }
}
