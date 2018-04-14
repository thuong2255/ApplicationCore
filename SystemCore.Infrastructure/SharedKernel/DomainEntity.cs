using System;
using System.Collections.Generic;
using System.Text;

namespace SystemCore.Infrastructure.SharedKernel
{
    public abstract class DomainEntity<T>
    {
        public T Id { get; set; }

        /// <summary>
        /// True If domain entity has an identity
        /// </summary>
        /// <returns></returns>
        public bool Transient()
        {
            return Id.Equals(default(T));
        }
    }
}
