using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using SystemCore.Infrastructure.Interfaces;
using SystemCore.Infrastructure.SharedKernel;

namespace SystemCore.Data.EF
{
    public class EFRepository<T, K> : IRepository<T, K>, IDisposable where T : DomainEntity<K>
    {

        private readonly AppDbContex _contex;

        public EFRepository(AppDbContex contex)
        {
            _contex = contex;
        }


        public void Add(T entity)
        {
            _contex.Add(entity);
        }

        public void Dispose()
        {
            if(_contex != null)
            {
                _contex.Dispose();
            }
        }

        public IQueryable<T> FindAll(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> items = _contex.Set<T>();

            if(includeProperties != null)
            {
                foreach(var includeProperty in includeProperties)
                {
                    items = items.Include(includeProperty);
                }
            }

            return items;
        }

        public IQueryable<T> FindAll(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> items = _contex.Set<T>();

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    items = items.Include(includeProperty);
                }
            }

            return items.Where(predicate);
        }

        public T FindById(K id, params Expression<Func<T, object>>[] includeProperties)
        {
            return FindAll(includeProperties).SingleOrDefault(x => x.Id.Equals(id));
        }

        public T FindBySingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            return FindAll(includeProperties).SingleOrDefault(predicate);
        }

        public void Remove(T entity)
        {
            _contex.Set<T>().Remove(entity);
        }

        public void Remove(K id)
        {
            Remove(FindById(id));
        }

        public void RemoveMulti(List<T> entities)
        {
            _contex.Set<T>().RemoveRange(entities);
        }

        public void Update(T entity)
        {
            _contex.Update(entity);
        }
    }
}
