// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;

namespace Developist.Core.Persistence.Entities
{
    public abstract class EntityBase<TIdentifier> : IEntity<TIdentifier>
        where TIdentifier : IEquatable<TIdentifier>
    {
        private int? hashCode;

        protected EntityBase() { }
        protected EntityBase(TIdentifier id) => Id = id;

        public virtual TIdentifier Id { get; protected set; }
        public virtual bool IsTransient => Id == null || Id.Equals(default);

        public override string ToString() => $"{GetType().Name} with {nameof(Id)} {(IsTransient ? "[None]" : Id.ToString())}";

        public override bool Equals(object obj)
        {
            if (!(obj is EntityBase<TIdentifier> that) || this.GetType() != that.GetType())
            {
                return false;
            }

            if (ReferenceEquals(this, that))
            {
                return true;
            }

            if (this.IsTransient || that.IsTransient)
            {
                return false;
            }

            return this.Id.Equals(that.Id);
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
                    hashCode = (GetType(), Id).GetHashCode();
                }
            }
            return (int)hashCode;
        }

        public static bool operator ==(EntityBase<TIdentifier> x, EntityBase<TIdentifier> y)
            => x is null || y is null ? ReferenceEquals(x, y) : x.Equals(y);

        public static bool operator !=(EntityBase<TIdentifier> x, EntityBase<TIdentifier> y) => !(x == y);
    }
}
