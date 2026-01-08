using MicroservicesEcosystem.Models;
using MicroservicesEcosystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace MicroservicesEcosystem.Repositories
{
    public abstract class Repository<TEntity> : IRepository<TEntity>
 where TEntity : class, IEntity
    {
        public EcosystemBaseDbContext context { get; set; }
        protected IServiceProvider serviceProvider;
        protected IHttpContextAccessor httpContextAccessor;
        public Repository(IServiceProvider serviceProvider, IHttpContextAccessor httpContextAccessor)
        {
            this.serviceProvider = serviceProvider;
            this.httpContextAccessor = httpContextAccessor;
            this.context = (EcosystemBaseDbContext)this.serviceProvider.GetService(typeof(EcosystemBaseDbContext));
        }
        public async Task<TEntity> Add(TEntity entity)
        {
            context.Set<TEntity>().Add(entity);    
            await context.SaveChangesAsync();
            return entity;
        }

        public async Task<TEntity> Delete(Guid id)
        {
            var entity = await context.Set<TEntity>().FindAsync(id);
            if (entity == null)
            {
                return entity;
            }

            context.Set<TEntity>().Remove(entity);
            await context.SaveChangesAsync();

            return entity;
        }

        


        public async Task<TEntity> Get(Guid id)
        {
            return await context.Set<TEntity>().FindAsync(id);
        }

        public async Task<List<TEntity>> GetAll()
        {
            return await context.Set<TEntity>().ToListAsync();
        }

        public async Task<TEntity> Update(TEntity entity)
        {
            context.Entry(entity).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateDetached(TEntity entity)
        {
            context.Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
        }


        public async Task<IDisposable> BeginTransactionAsync()
        {
            return context.Database.CurrentTransaction
                ?? await context.Database.BeginTransactionAsync();
        }


        // Método para confirmar una transacción
        public async Task CommitTransactionAsync()
        {
            if (context.Database.CurrentTransaction != null)
            {
                await context.Database.CurrentTransaction.CommitAsync();
            }
        }


        // Método para revertir una transacción
        public async Task RollbackTransactionAsync()
        {
            if (context.Database.CurrentTransaction != null)
            {
                await context.Database.CurrentTransaction.RollbackAsync();
            }
        }


        // Método para guardar cambios explícitamente
        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }

    }
}
