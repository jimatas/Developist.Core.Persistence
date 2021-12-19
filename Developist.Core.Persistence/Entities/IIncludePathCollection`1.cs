// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System.Collections.Generic;

namespace Developist.Core.Persistence.Entities
{
    public interface IIncludePathCollection<TEntity> : IEnumerable<string>
        where TEntity : IEntity
    {
        void Add(string path);
        bool Remove(string path);
    }
}
