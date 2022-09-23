using System;

namespace Developist.Core.Persistence.Entities
{
    public abstract class EntityBase<TIdentifier> : IEntity<TIdentifier>
        where TIdentifier : IEquatable<TIdentifier>
    {
        private int? hashCode;

        protected EntityBase() : this(default!) { }
        protected EntityBase(TIdentifier id) => Id = id;

        public virtual TIdentifier Id { get; protected set; }
        public virtual bool IsTransient => Id is null || Id.Equals(default!);

        public override string ToString() => $"{GetType().Name} with {nameof(Id)} {(IsTransient ? "[None]" : Id.ToString())}";

        public override bool Equals(object obj)
        {
            if (!(obj is EntityBase<TIdentifier> other) || GetType() != other.GetType())
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (IsTransient || other.IsTransient)
            {
                return false;
            }

            return Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            if (hashCode is null)
            {
                if (IsTransient)
                {
                    hashCode = base.GetHashCode();
                }
                else
                {
                    hashCode = HashCode.Combine(GetType(), Id);
                }
            }
            return (int)hashCode;
        }

        public static bool operator ==(EntityBase<TIdentifier> one, EntityBase<TIdentifier> other)
        {
            return one is null || other is null ? ReferenceEquals(one, other) : one.Equals(other);
        }

        public static bool operator !=(EntityBase<TIdentifier> one, EntityBase<TIdentifier> other) => !(one == other);
    }
}
