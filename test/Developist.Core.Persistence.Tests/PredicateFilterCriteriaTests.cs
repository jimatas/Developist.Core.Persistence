using Developist.Core.Persistence.Tests.Fixture;

namespace Developist.Core.Persistence.Tests;

[TestClass]
public class PredicateFilterCriteriaTests
{
    [TestMethod]
    public void Initialize_GivenNullPredicate_ThrowsArgumentNullException()
    {
        // Arrange
        Expression<Func<Person, bool>> predicate = null!;

        // Act
        void action() => _ = new PredicateFilterCriteria<Person>(predicate);

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual(nameof(predicate), exception.ParamName);
    }

    [TestMethod]
    public void Apply_ByDefault_ReturnsFilteredQuery()
    {
        // Arrange
        Expression<Func<Person, bool>>? predicate = p => p.FamilyName.Equals("Connor");
        var criteria = new PredicateFilterCriteria<Person>(predicate);
        var originalQuery = People.AsQueryable();

        // Act
        var filteredQuery = criteria.Apply(originalQuery);

        // Assert
        var expectedResult = filteredQuery.ToList();

        Assert.AreEqual(2, expectedResult.Count);
        Assert.IsTrue(expectedResult.Any(p => p.GivenName.Equals("Phillipa")));
        Assert.IsTrue(expectedResult.Any(p => p.GivenName.Equals("Peter")));
    }
}
