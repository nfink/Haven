using System;
using System.Linq;
using System.Linq.Expressions;
using SQLite;
using System.Collections.Generic;
using System.Configuration;

namespace Haven
{
    public class Repository : IRepository
    {
        private SQLiteConnection Connection;

        public Repository()
        {
            this.Connection = new SQLiteConnection(ConfigurationManager.AppSettings["databasePath"]);
            this.Connection.BeginTransaction();
        }

        public void Add<T>(T entity) where T : class, IEntity, new()
        {
            this.Connection.Insert(entity);
            entity.Repository = this;
        }

        public void Remove<T>(T entity) where T : class, IEntity, new()
        {
            this.Connection.Delete(entity);
        }

        public void Update<T>(T entity) where T : class, IEntity, new()
        {
            this.Connection.Update(entity);
            entity.Repository = this;
        }

        public T Get<T>(int id) where T : class, IEntity, new()
        {
            var entity = this.Connection.Get<T>(id);
            entity.Repository = this;
            return entity;
        }

        public IEnumerable<T> Find<T>(Expression<Func<T, bool>> predicate) where T : class, IEntity, new()
        {
            return this.Connection.Table<T>().Where(predicate).AsEnumerable<T>().Select(x => { x.Repository = this; return x; });
        }

        public IEnumerable<T> FindAll<T>() where T : class, IEntity, new()
        {
            return this.Connection.Table<T>().AsEnumerable<T>().Select(x => { x.Repository = this; return x; });
        }

        public void Commit()
        {
            this.Connection.Commit();
        }

        public void Dispose()
        {
            this.Connection.Rollback();
            this.Connection.Close();
        }
    }
}

//using System;
//using System.Linq;
//using System.Linq.Expressions;
//using SQLite;

//namespace Haven
//{
//    public class Repository<T> : IRepository<T> where T : class, new()
//    {
//        private SQLiteConnection Connection;

//        public Repository(SQLiteConnection connection)
//        {
//            this.Connection = connection;
//        }

//        public void Add(T entity)
//        {
//            this.Connection.Insert(entity);
//        }

//        public void Remove(T entity)
//        {
//            this.Connection.Delete(entity);
//        }

//        public void Update(T entity)
//        {
//            this.Connection.Update(entity);
//        }

//        public IQueryable<T> Find(Expression<Func<T, bool>> predicate)
//        {
//            return this.Connection.Table<T>().Where(predicate).AsQueryable<T>();
//        }

//        public IQueryable<T> FindAll()
//        {
//            return this.Connection.Table<T>().AsQueryable<T>();
//        }
//    }
//}

