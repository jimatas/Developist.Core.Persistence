// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Utilities;

using System.Collections;
using System.Collections.Generic;

namespace Developist.Core.Persistence.Entities
{
    public class IncludePathCollection<TEntity> : IIncludePathCollection<TEntity>
        where TEntity : IEntity
    {
        private readonly List<string> paths;

        public IncludePathCollection() : this(new List<string>()) { }
        internal IncludePathCollection(List<string> paths) => this.paths = paths;
        
        public IEnumerator<string> GetEnumerator() => paths.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(string path)
        {
            Ensure.Argument.NotNullOrWhiteSpace(path, nameof(path));

            paths.Add(path);
        }

        public bool Remove(string path)
        {
            Ensure.Argument.NotNullOrWhiteSpace(path, nameof(path));

            var i = paths.LastIndexOf(path);
            if (i >= 0)
            {
                paths.RemoveAt(i);
                return true;
            }
            return false;
        }
    }
}
