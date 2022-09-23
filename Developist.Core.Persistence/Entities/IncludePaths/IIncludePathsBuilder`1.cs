using System.Collections.Generic;

namespace Developist.Core.Persistence.Entities.IncludePaths
{
    public interface IIncludePathsBuilder<TEntity>
        where TEntity : IEntity
    {
        void Include(string path);
        string[] ToArray();
        internal IList<string> AsList();
    }
}
