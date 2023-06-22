using Data.Context;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CertificatesWebApp.Infrastructure
{
    public interface IRepository<T> where T : class, IBaseEntity
    {
        IEnumerable<T> ReadAll();
        T Read(Guid id);
        T Create(T entity);
        T Update(T entity);
        T Delete(Guid id);
    }

    public class Repository<T> : IRepository<T> where T : class, IBaseEntity
    {
        protected CertificatesWebAppContext _context;
        protected DbSet<T> _entities;

        public Repository(CertificatesWebAppContext context)
        {
            _context = context;
            _entities = context.Set<T>();
        }

        public virtual IEnumerable<T> ReadAll()
        {
            return _entities.ToList();
        }

        public virtual T Read(Guid id)
        {
            return _entities.FirstOrDefault(e => e.Id == id);
        }

        public virtual T Create(T entity)
        {
            _entities.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public virtual T Update(T entity)
        {
            var entityToUpdate = Read(entity.Id);
            if (entityToUpdate != null)
            {
                _context.Entry(entityToUpdate).CurrentValues.SetValues(entity);
                _context.SaveChanges();
            }

            return entityToUpdate;
        }

        public virtual T Delete(Guid id)
        {
            var entityToDelete = Read(id);
            if (entityToDelete != null)
            {
                _context.Remove(entityToDelete);
                _context.SaveChanges();
            }

            return entityToDelete;
        }
    }

}
