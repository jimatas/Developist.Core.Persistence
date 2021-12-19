// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System.Linq;

namespace Developist.Core.Persistence
{
    public interface IQueryablePaginator<T>
    {
        IQueryable<T> Paginate(IQueryable<T> sequence);
    }
}
