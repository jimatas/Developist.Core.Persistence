namespace Developist.Core.Persistence
{
    /// <summary>
    /// An interface for a factory that creates repositories for various types of entities.
    /// </summary>
    public interface IRepositoryFactory
    {
        /// <summary>
        /// Creates a new repository instance for entities of type <typeparamref name="T"/>, associated with the specified unit of work.
        /// </summary>
        /// <typeparam name="T">The type of entity being managed by the repository.</typeparam>
        /// <param name="unitOfWork">The unit of work associated with the repository.</param>
        /// <returns>A repository instance for entities of type <typeparamref name="T"/>.</returns>
        IRepository<T> Create<T>(IUnitOfWork unitOfWork) where T : class;
    }
}
