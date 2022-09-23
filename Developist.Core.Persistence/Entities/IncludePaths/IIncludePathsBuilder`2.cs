namespace Developist.Core.Persistence.Entities.IncludePaths
{
    public interface IIncludePathsBuilder<TEntity, out TProperty> : IIncludePathsBuilder<TEntity>
        where TEntity : IEntity
    {
        void ThenInclude(string pathSegment);
    }
}
