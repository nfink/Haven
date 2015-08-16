using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Haven
{
    public interface IRepository : IDisposable
    {
        void Add<T>(T entity) where T : class, IEntity, new();
        void Remove<T>(T entity) where T : class, IEntity, new();
        void Update<T>(T entity) where T : class, IEntity, new();
        T Get<T>(int id) where T : class, IEntity, new();
        IEnumerable<T> Find<T>(Expression<Func<T, bool>> predicate) where T : class, IEntity, new();
        IEnumerable<T> FindAll<T>() where T : class, IEntity, new();
        void Commit();
    }
}