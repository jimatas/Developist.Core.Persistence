namespace Developist.Core.Persistence.Entities
{
    public interface IEntity
    {
        bool IsTransient { get; }
    }
}
