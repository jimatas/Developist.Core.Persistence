using Developist.Core.Persistence.Tests.Fixture;

namespace Developist.Core.Persistence.EntityFrameworkCore.Tests;

[TestClass]
public class ExtendableQueryRepositoryTests
{
    [TestMethod]
    public void Initialize_WithNullUnitOfWork_ThrowsArgumentNullException()
    {
        // Arrange
        IUnitOfWork unitOfWork = null!;
        var queryExtender = new Mock<IQueryExtender<Person>>().Object;

        // Act
        void action() => _ = new ExtendableQueryRepository<Person>(unitOfWork, queryExtender);

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual(nameof(unitOfWork), exception.ParamName);
    }

    [TestMethod]
    public void Initialize_WithNullQueryExtender_ThrowsArgumentNullException()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>().Object;
        IQueryExtender<Person> queryExtender = null!;

        // Act
        void action() => _ = new ExtendableQueryRepository<Person>(unitOfWork, queryExtender);

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual(nameof(queryExtender), exception.ParamName);
    }
}
