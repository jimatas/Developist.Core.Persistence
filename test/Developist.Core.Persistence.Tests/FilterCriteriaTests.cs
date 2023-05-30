using Developist.Core.Persistence.Filtering;
using Developist.Core.Persistence.Tests.Fixture;
using System.Linq.Expressions;

namespace Developist.Core.Persistence.Tests;

[TestClass]
public class FilterCriteriaTests
{
    [TestMethod]
    public void Initialize_GivenNullPredicate_ThrowsArgumentException()
    {
        // Arrange
        Expression<Func<Person, bool>>? predicate = null;

        // Act
        var action = () => new PredicateFilterCriteria<Person>(predicate);

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual(nameof(predicate), exception.ParamName);
    }

    [TestMethod]
    public void Filter_ByDefault_FiltersQuery()
    {
        // Arrange
        Expression<Func<Person, bool>>? predicate = p => p.FamilyName.Equals("Connor");
        var criteria = new PredicateFilterCriteria<Person>(predicate);
        var originalQuery = People.AsQueryable();

        // Act
        var filteredQuery = criteria.Filter(originalQuery);
        var expectedResult = filteredQuery.ToList();

        // Assert
        Assert.AreEqual(2, expectedResult.Count);
        Assert.IsTrue(expectedResult.Any(p => p.GivenName == "Phillipa"));
        Assert.IsTrue(expectedResult.Any(p => p.GivenName == "Peter"));
    }
}
