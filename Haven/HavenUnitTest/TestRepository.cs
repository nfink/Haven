using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Haven
{
    public class TestRepository : IRepository
    {
        private Dictionary<Type, IEnumerable> Tables;

        public TestRepository()
        {
            this.Tables = new Dictionary<Type, IEnumerable>();
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

            var table = this.Tables[typeof(T)].Cast<T>();
            entity.Id = table.Count() < 1 ? 1 : table.Cast<T>().Select(x => x.Id).Max() + 1;
            entity.Repository = this;
            this.Tables[typeof(T)] = table.Concat(new T[] { entity });
        }

        public void Remove<T>(T entity) where T : class, IEntity, new()
        {
            if (this.Tables.ContainsKey(typeof(T)))
            {
                this.Tables[typeof(T)] = this.Tables[typeof(T)].Cast<T>().Where(x => x.Id != entity.Id);
            }
        }

        public void Update<T>(T entity) where T : class, IEntity, new()
        {
            if (this.Tables.ContainsKey(typeof(T)))
            {
                if (this.Tables[typeof(T)].Cast<T>().Select(x => x.Id).Contains(entity.Id))
                {
                    this.Remove(entity);
                    this.Tables[typeof(T)] = this.Tables[typeof(T)].Cast<T>().Concat(new T[] { entity });
                }
            }
        }

        public T Get<T>(int id) where T : class, IEntity, new()
        {
            var entity = this.Find<T>(x => x.Id == id).SingleOrDefault();
            
            MethodInfo dynMethod = entity.GetType().GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);
            var copiedEntity = (T)dynMethod.Invoke(entity, new object[] { });
            copiedEntity.Repository = this;
            return copiedEntity;
        }

        public IEnumerable<T> Find<T>(Expression<Func<T, bool>> predicate) where T : class, IEntity, new()
        {
            if (this.Tables.ContainsKey(typeof(T)))
            {
                return this.Tables[typeof(T)].Cast<T>().AsQueryable<T>().Where(predicate).AsEnumerable<T>().Select(x =>
                    {
                        MethodInfo dynMethod = x.GetType().GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);
                        var copiedEntity = (T)dynMethod.Invoke(x, new object[] { });
                        copiedEntity.Repository = this;
                        return copiedEntity;
                    });
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
                return this.Tables[typeof(T)].Cast<T>().Select(x =>
                {
                    MethodInfo dynMethod = x.GetType().GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);
                    var copiedEntity = (T)dynMethod.Invoke(x, new object[] { });
                    copiedEntity.Repository = this;
                    return copiedEntity;
                });
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