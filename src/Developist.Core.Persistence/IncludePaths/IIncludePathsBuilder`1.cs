using System.Collections.Generic;

namespace Developist.Core.Persistence.IncludePaths
{
    /// <summary>
    /// Represents a builder for include paths of entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    public interface IIncludePathsBuilder<T>
    {
        /// <summary>
        /// Includes a path for related entities in the query result.
        /// </summary>
        /// <param name="path">The path to include.</param>
        void Include(string path);

        /// <summary>
        /// Returns a list of the include paths built so far.
        /// </summary>
        /// <returns>A list of the include paths built so far.</returns>
        IList<string> AsList();
    }
}
