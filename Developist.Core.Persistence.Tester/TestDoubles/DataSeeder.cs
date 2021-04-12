// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System.Collections.Generic;

namespace Developist.Core.Persistence.Tester
{
    public class DataSeeder
    {
        public void Seed(IRepository<Person> repository)
        {
            foreach (var person in People)
            {
                repository.Add(person);
            }
        }

        public IEnumerable<Person> People
        {
            get
            {
                yield return new Person(PersonIdGenerator.Default.GenerateId())
                {
                    GivenName = "Dwayne",
                    FamilyName = "Welsh",
                    Contact = new ContactInformation
                    {
                        Email = "test--dstuart@dev.developist.net",
                        HomeAddress = new Address
                        {
                            Street = "1 Leamington Dell",
                            City = "Vineland",
                            PostalCode = "08360",
                            State = "NJ",
                            Country = "USA"
                        }
                    }
                };

                yield return new Person(PersonIdGenerator.Default.GenerateId())
                {
                    GivenName = "Ed",
                    FamilyName = "Stuart",
                    Contact = new ContactInformation
                    {
                        Email = "test--estuart@dev.developist.net",
                        HomeAddress = new Address
                        {
                            Street = "2 Norris Promenade",
                            City = "Deerfield",
                            PostalCode = "60015",
                            State = "IL",
                            Country = "USA"
                        }
                    }
                };

                yield return new Person(PersonIdGenerator.Default.GenerateId())
                {
                    GivenName = "Hollie",
                    FamilyName = "Marin",
                    Contact = new ContactInformation
                    {
                        Email = "test--hmarin@dev.developist.net",
                        HomeAddress = new Address
                        {
                            Street = "10 Cranwell Spur",
                            City = "Windsor Mill",
                            PostalCode = "21244",
                            State = "MD",
                            Country = "USA"
                        }
                    }
                };

                yield return new Person(PersonIdGenerator.Default.GenerateId())
                {
                    GivenName = "Randall",
                    FamilyName = "Bloom",
                    Contact = new ContactInformation
                    {
                        Email = "test--rbloom@dev.developist.net",
                        HomeAddress = new Address
                        {
                            Street = "16 Norris Promenade",
                            City = "Deerfield",
                            PostalCode = "60015",
                            State = "IL",
                            Country = "USA"
                        }
                    }
                };

                yield return new Person(PersonIdGenerator.Default.GenerateId())
                {
                    GivenName = "Glenn",
                    FamilyName = "Hensley",
                    Contact = new ContactInformation
                    {
                        Email = "test--ghensley@dev.developist.net",
                        HomeAddress = new Address
                        {
                            Street = "22 Holmeswalk Drive",
                            City = "Lynchburg",
                            PostalCode = "24502",
                            State = "VA",
                            Country = "USA"
                        }
                    }
                };
            }
        }
    }
}
