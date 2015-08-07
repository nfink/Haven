using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Haven
{
    public class TestRepository : IRepository
    {
        private Dictionary<Type, IList> Tables;

        public TestRepository()
        {
            this.Tables = new Dictionary<Type, IList>();
        }

        public void Add<T>(T entity) where T : class, IEntity, new()
        {
            if (!this.Tables.ContainsKey(typeof(T)))
            {
                this.Tables[typeof(T)] = new List<T>();

            }

            //// to simulate autoincrementing Id
            //var prop = typeof(T).GetProperty("Id", typeof(int));
            //if (prop != null)
            //{
            //    int id = 1;
            //    if (this.Tables[typeof(T)].Count > 0)
            //    {
            //        id = this.Tables[typeof(T)].Cast<T>().Select(x => (int)prop.GetValue(x)).Max() + 1;
            //    }

            //    prop.SetValue(entity, id, null);
            //}

            var table = this.Tables[typeof(T)];
            entity.Id = table.Count < 1 ? 1 : table.Cast<T>().Select(x => x.Id).Max() + 1;
            entity.Repository = this;
            table.Add(entity);
        }

        public void Remove<T>(T entity) where T : class, IEntity, new()
        {
            if (this.Tables.ContainsKey(typeof(T)))
            {
                this.Tables[typeof(T)].Remove(entity);
            }
        }

        public void Update<T>(T entity) where T : class, IEntity, new()
        {
            // intentionally left blank
        }

        public T Get<T>(int id) where T : class, IEntity, new()
        {
            var entity = this.Find<T>(x => x.Id == id).SingleOrDefault();
            entity.Repository = this;
            return entity;
        }

        public IEnumerable<T> Find<T>(Expression<Func<T, bool>> predicate) where T : class, IEntity, new()
        {
            if (this.Tables.ContainsKey(typeof(T)))
            {
                return this.Tables[typeof(T)].Cast<T>().AsQueryable<T>().Where(predicate).AsEnumerable<T>().Select(x => { x.Repository = this; return x; });
            }
            else
            {
                return new List<T>().AsQueryable<T>();
            }
        }

        public IEnumerable<T> FindAll<T>() where T : class, IEntity, new()
        {
            if (this.Tables.ContainsKey(typeof(T)))
            {
                return this.Tables[typeof(T)].Cast<T>().Select(x => { x.Repository = this; return x; });
            }
            else
            {
                return new List<T>().AsQueryable<T>();
            }
        }

        public void Commit()
        {
            // intentionally left blank
        }
        
        public void Dispose()
        {
            this.Tables.Clear();
        }

        public void AddAll<T>(IEnumerable<T> entities) where T : class, IEntity, new()
        {
            foreach (T entity in entities)
            {
                this.Add(entity);
            }
        }
    }
}


//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;

//namespace Haven
//{
//    public class TestRepository<T> : IRepository<T> where T : class, new()
//    {
//        private List<T> Table;

//        public TestRepository()
//        {
//            this.Table = new List<T>();
//        }

//        public void Add(T entity)
//        {
//            this.Table.Add(entity);
//        }

//        public void Remove(T entity)
//        {
//            this.Table.Remove(entity);
//        }

//        public void Update(T entity)
//        {
//            // intentionally left blank
//        }

//        public IQueryable<T> Find(Expression<Func<T, bool>> predicate)
//        {
//            return this.Table.AsQueryable<T>().Where(predicate);
//        }

//        public IQueryable<T> FindAll()
//        {
//            return this.Table.AsQueryable<T>();
//        }
//    }
//}
