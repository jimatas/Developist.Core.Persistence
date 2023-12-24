using Developist.Core.Persistence.Filtering;
using Developist.Core.Persistence.Tests.Fixture;

namespace Developist.Core.Persistence.Tests;

[TestClass]
public class PredicateExpressionExtensionsTests
{
    [TestMethod]
    public void AndAlso_GivenExpression_ReturnsExpectedResult()
    {
        // Arrange
        Expression<Func<Person, bool>> familyNameIsConnor = p => p.FamilyName.Equals("Connor");
        Expression<Func<Person, bool>> favoriteBookIsTreasureIsland = p => p.FavoriteBook!.Title.Equals("Treasure Island");

        // Act
        var result = PeopleWithBooks().Where(familyNameIsConnor.AndAlso(favoriteBookIsTreasureIsland));

        // Assert
        Assert.AreEqual(1, result.Count());
        Assert.AreEqual("Peter", result.Single().GivenName);
    }

    [TestMethod]
    public void OrElse_GivenExpression_ReturnsExpectedResult()
    {
        // Arrange
        Expression<Func<Person, bool>> familyNameIsConnor = p => p.FamilyName.Equals("Connor");
        Expression<Func<Person, bool>> favoriteBookIsTreasureIsland = p => p.FavoriteBook != null && p.FavoriteBook.Title.Equals("Treasure Island");

        // Act
        var result = PeopleWithBooks().Where(familyNameIsConnor.OrElse(favoriteBookIsTreasureIsland)).ToList();

        // Assert
        Assert.AreEqual(3, result.Count);
        Assert.AreEqual("Glenn Hensley", result[0].ToString());
        Assert.AreEqual("Phillipa Connor", result[1].ToString());
        Assert.AreEqual("Peter Connor", result[2].ToString());
    }

    private static IQueryable<Person> PeopleWithBooks()
    {
        var people = People.AsEnumerable().ToList();
        var books = Books.AsEnumerable().ToList();

        // Randall Bloom - A Princess of Mars
        people[3].FavoriteBook = books[2];

        // Glenn Hensley - Treasure Island
        people[4].FavoriteBook = books[4];

        // Phillipa Connor - To Kill a Mockingbird
        people[5].FavoriteBook = books[1];

        // Peter Connor - Treasure Island
        people[6].FavoriteBook = books[4];

        return people.AsQueryable();
    }
}
