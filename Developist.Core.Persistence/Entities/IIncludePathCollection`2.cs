// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

namespace Developist.Core.Persistence.Entities
{
    public interface IIncludePathCollection<TEntity, out TProperty> : IIncludePathCollection<TEntity> 
        where TEntity : IEntity
    {
        new void Add(string path);
    }
}
