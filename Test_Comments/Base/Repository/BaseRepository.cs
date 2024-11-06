using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Test_Comments.Configuration;

namespace Test_Comments.Base.Repository;

public class BaseRepository<TDocument> : IBaseRepository<TDocument> where TDocument : Document
{
    private readonly AppDbContext _context;
    private readonly DbSet<TDocument> _dbSet;

    public BaseRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TDocument>();
    }

    public async Task<IEnumerable<TDocument>> GetAllAsync()
    {
        return await _dbSet.Where(d => !d.Deleted).ToListAsync();
    }

    public async Task<TDocument> FindByIdAsync(Guid id)
    {
        return await _dbSet.FirstOrDefaultAsync(d => d.Id == id && !d.Deleted);
    }

    public async Task InsertOneAsync(TDocument document)
    {
        await _dbSet.AddAsync(document);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateOneAsync(TDocument document)
    {
        _dbSet.Update(document);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteOneAsync(Guid id)
    {
        var document = await FindByIdAsync(id);
        if (document == null) return;

        document.Deleted = true;
        document.DeletionDate = DateTime.UtcNow;
        await UpdateOneAsync(document);
    }

    public async Task<IEnumerable<TDocument>> FilterByAsync(Expression<Func<TDocument, bool>> filterExpression)
    {
        return await _dbSet.Where(filterExpression).Where(d => !d.Deleted).ToListAsync();
    }

    public async Task<int> CountAsync(Expression<Func<TDocument, bool>> filterExpression)
    {
        return await _dbSet.CountAsync(filterExpression);
    }

    public async Task<TDocument> FindOneAsync(Expression<Func<TDocument, bool>> filterExpression)
    {
        return await _dbSet.Where(filterExpression).Where(d => !d.Deleted).FirstOrDefaultAsync();
    }
    public async Task<IEnumerable<TDocument>> GetWithSkipAsync(int skip, int take)
    {
        return await _dbSet.Where(d => !d.Deleted) 
            .Skip(skip) 
            .Take(take) 
            .ToListAsync(); 
    }

    public async Task<bool> ExistsAsync(Expression<Func<TDocument, bool>> filterExpression)
    {
        return await _dbSet.AnyAsync(filterExpression);
    }

    public async Task UpdateManyAsync(Expression<Func<TDocument, bool>> filterExpression, Action<TDocument> updateAction)
    {
        var documents = await _dbSet.Where(filterExpression).Where(d => !d.Deleted).ToListAsync();
        foreach (var document in documents)
        {
            updateAction(document);
            _dbSet.Update(document);
        }
        await _context.SaveChangesAsync();
    }

    public async Task DeleteManyAsync(Expression<Func<TDocument, bool>> filterExpression)
    {
        var documents = await _dbSet.Where(filterExpression).Where(d => !d.Deleted).ToListAsync();
        foreach (var document in documents)
        {
            document.Deleted = true;
            document.DeletionDate = DateTime.UtcNow;
            _dbSet.Update(document);
        }
        await _context.SaveChangesAsync();
    }
}
