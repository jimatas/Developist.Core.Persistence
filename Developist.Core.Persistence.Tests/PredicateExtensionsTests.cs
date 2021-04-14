// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Developist.Core.Persistence.Tests
{
    [TestClass]
    public class PredicateExtensionsTests
    {
        #region People Data
        private IEnumerable<Person> People
        {
            get
            {
                yield return new Person(id: 1)
                {
                    GivenName = "Mila-Rose",
                    FamilyName = "Colley",
                    FavoriteBook = new Book(id: 10)
                    {
                        Genre = Genre.Classics,
                        Title = "The Great Gatsby"
                    }
                };

                yield return new Person(id: 2)
                {
                    GivenName = "Noah",
                    FamilyName = "Daly",
                    FavoriteBook = new Book(id: 20)
                    {
                        Genre = Genre.Classics,
                        Title = "To Kill a Mockingbird"
                    }
                };

                yield return new Person(id: 3)
                {
                    GivenName = "Kelly",
                    FamilyName = "Jones",
                    FavoriteBook = new Book(id: 110)
                    {
                        Genre = Genre.Fiction,
                        Title = "A Princess of Mars"
                    }
                };

                yield return new Person(id: 4)
                {
                    GivenName = "Helen",
                    FamilyName = "Hope",
                    FavoriteBook = new Book(id: 120)
                    {
                        Genre = Genre.Fiction,
                        Title = "Against the Fall of Night"
                    }
                };

                yield return new Person(id: 5)
                {
                    GivenName = "Rhona",
                    FamilyName = "Adams",
                    FavoriteBook = new Book(id: 210)
                    {
                        Genre = Genre.Adventure,
                        Title = "Treasure Island"
                    }
                };

                yield return new Person(id: 6)
                {
                    GivenName = "Nicholas",
                    FamilyName = "Curran",
                    FavoriteBook = new Book(id: 220)
                    {
                        Genre = Genre.Adventure,
                        Title = "Call of the Wild"
                    }
                };
            }
        }
        #endregion
    }
}
