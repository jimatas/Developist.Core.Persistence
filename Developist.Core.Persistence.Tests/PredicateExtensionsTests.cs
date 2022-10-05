using Developist.Core.Persistence.Tests.Fixture;

using System.Linq.Expressions;

namespace Developist.Core.Persistence.Tests
{
    [TestClass]
    public class PredicateExtensionsTests
    {
        [TestMethod]
        public void OrElse_GivenTwoFuncs_CombinesThem()
        {
            Func<Person, bool> familyNameStartsWithB = p => p.FamilyName!.StartsWith("B");
            Func<Person, bool> underFifty = p => p.Age < 50;

            var result = People.AsQueryable().Where(familyNameStartsWithB.OrElse(underFifty));

            Assert.AreEqual(8, result.Count());
            Assert.IsTrue(result.Select(p => $"{p.GivenName} {p.FamilyName}").SequenceEqual(
                new[] { "Dwayne Welsh", "Ed Stuart", "Hollie Marin", "Randall Bloom", "Glenn Hensley", "Peter Connor", "Ana Bryan", "Edgar Bernard" }));
        }

        [TestMethod]
        public void AndAlso_GivenTwoFuncs_CombinesThem()
        {
            Func<Person, bool> familyNameStartsWithB = p => p.FamilyName!.StartsWith("B");
            Func<Person, bool> overFifty = p => p.Age > 50;

            var result = People.AsQueryable().Where(familyNameStartsWithB.AndAlso(overFifty));

            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Select(p => $"{p.GivenName} {p.FamilyName}").SequenceEqual(
                new[] { "Randall Bloom", "Edgar Bernard" }));
        }

        [TestMethod]
        public void OrElse_GivenTwoExpressions_CombinesThem()
        {
            Expression<Func<Person, bool>> namedConnor = p => p.FamilyName == "Connor";
            Expression<Func<Person, bool>> overThirty = p => p.Age > 30;

            var result = People.AsQueryable().Where(namedConnor.OrElse(overThirty));

            Assert.AreEqual(6, result.Count());
            Assert.IsTrue(result.Select(p => $"{p.GivenName} {p.FamilyName}").SequenceEqual(
                new[] { "Hollie Marin", "Randall Bloom", "Phillipa Connor", "Peter Connor", "Ana Bryan", "Edgar Bernard" }));
        }

        [TestMethod]
        public void AndAlso_GivenTwoExpressions_CombinesThem()
        {
            Expression<Func<Person, bool>> namedConnor = p => p.FamilyName == "Connor";
            Expression<Func<Person, bool>> underThirty = p => p.Age < 30;

            var result = People.AsQueryable().Where(namedConnor.AndAlso(underThirty));

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Peter Connor", result.Select(p => $"{p.GivenName} {p.FamilyName}").Single());
        }

        [TestMethod]
        public void AndAlso_GivenThreeExpressions_CombinesThem()
        {
            Expression<Func<Person, bool>> namedConnor = p => p.FamilyName == "Connor";
            Expression<Func<Person, bool>> givenNameStartsWithP = p => p.GivenName!.StartsWith("P");
            Expression<Func<Person, bool>> ageIsUnknown = p => p.Age == null;

            // ((p.FamilyName == "Connor") AndAlso p.GivenName.StartsWith("P")) AndAlso (p.Age == null)
            var result = People.AsQueryable().Where(namedConnor.AndAlso(givenNameStartsWithP).AndAlso(ageIsUnknown));

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Phillipa Connor", result.Select(p => $"{p.GivenName} {p.FamilyName}").Single());
        }

        [TestMethod]
        public void AndAlso_GivenThreeExpressionsCombinedAlternatively_YieldsSameResult()
        {
            Expression<Func<Person, bool>> namedConnor = p => p.FamilyName == "Connor";
            Expression<Func<Person, bool>> givenNameStartsWithP = p => p.GivenName!.StartsWith("P");
            Expression<Func<Person, bool>> ageIsUnknown = p => p.Age == null;

            // (p.FamilyName == "Connor") AndAlso (p.GivenName.StartsWith("P") AndAlso (p.Age == null))
            var result = People.AsQueryable().Where(namedConnor.AndAlso(givenNameStartsWithP.AndAlso(ageIsUnknown)));

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Phillipa Connor", result.Select(p => $"{p.GivenName} {p.FamilyName}").Single());
        }
    }
}
