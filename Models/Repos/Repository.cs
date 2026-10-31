using GenbaStudio.Data;
using Microsoft.EntityFrameworkCore;

namespace GenbaStudio.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<T> Set;

    public Repository(AppDbContext context)
    {
        Context = context;
        Set = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id) => await Set.FindAsync(id);

    public virtual async Task AddAsync(T entity)
    {
        Set.Add(entity);
        await Context.SaveChangesAsync();
    }

    public virtual async Task UpdateAsync(T entity)
    {
        Set.Update(entity);
        await Context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(T entity)
    {
        Set.Remove(entity);
        await Context.SaveChangesAsync();
    }
}
