// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Utilities;

using System.Collections.Generic;
using System.Linq;

namespace Developist.Core.Persistence.Entities
{
    internal class IncludePathCollection<TEntity, TProperty> : IncludePathCollection<TEntity>, IIncludePathCollection<TEntity, TProperty>
        where TEntity : IEntity
    {
        public IncludePathCollection() : base() { }
        public IncludePathCollection(List<string> paths) : base(paths) { }

        void IIncludePathCollection<TEntity, TProperty>.Add(string path)
        {
            Ensure.Argument.NotNullOrWhiteSpace(path, nameof(path));

            var previousPath = this.LastOrDefault();
            if (previousPath != null && Remove(previousPath))
            {
                path = $"{previousPath}.{path}";
            }
            Add(path);
        }
    }
}
