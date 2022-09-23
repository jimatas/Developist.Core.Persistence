using System;

namespace Developist.Core.Persistence.Entities
{
    public interface IEntity<TIdentifier> : IEntity
        where TIdentifier : IEquatable<TIdentifier>
    {
        TIdentifier Id { get; }
    }
}
