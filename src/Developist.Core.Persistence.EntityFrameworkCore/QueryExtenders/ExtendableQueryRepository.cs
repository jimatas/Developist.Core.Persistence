using Developist.Core.Persistence.Utilities;

namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <summary>
/// A subclass of <see cref="Repository{T}"/> that supports extendable queries through an <see cref="IQueryExtender{T}"/>.
/// </summary>
/// <typeparam name="T">The type of entity being managed by the repository.</typeparam>
public class ExtendableQueryRepository<T> : Repository<T> where T : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendableQueryRepository{T}"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work associated with this repository.</param>
    /// <param name="queryExtender">The query extender to apply to the repository.</param>
    public ExtendableQueryRepository(IUnitOfWork unitOfWork, IQueryExtender<T> queryExtender)
        : base(unitOfWork)
    {
        QueryExtender = Ensure.NotNull(queryExtender);
    }

    /// <summary>
    /// Gets the query extender applied to this repository.
    /// </summary>
    public IQueryExtender<T> QueryExtender { get; }

    /// <inheritdoc/>
    protected override IQueryable<T> CreateQuery()
    {
        return QueryExtender.Extend(base.CreateQuery());
    }
}
