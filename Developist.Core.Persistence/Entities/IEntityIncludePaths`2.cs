// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

namespace Developist.Core.Persistence
{
    public interface IEntityIncludePaths<TEntity, TProperty> : IEntityIncludePaths<TEntity> where TEntity : IEntity
    {
        new void Add(string path);
    }
}
