// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

namespace Developist.Core.Persistence.Tests
{
    public static class UnitOfWorkExtensions
    {
        public static IRepository<Person> People(this IUnitOfWork uow) => uow.Repository<Person>();
        public static IRepository<Message> Messages(this IUnitOfWork uow) => uow.Repository<Message>();
    }
}
