using System.Linq.Expressions;

namespace Test_Comments.Base.Repository;

public interface IBaseRepository<TDocument> where TDocument : Document
{
    Task<IEnumerable<TDocument>> GetAllAsync();
    Task<TDocument> FindByIdAsync(Guid id);
    Task InsertOneAsync(TDocument document);
    Task UpdateOneAsync(TDocument document);
    Task DeleteOneAsync(Guid id);
    
    Task<IEnumerable<TDocument>> GetWithSkipAsync(int skip, int take); 
    Task<IEnumerable<TDocument>> FilterByAsync(Expression<Func<TDocument, bool>> filterExpression);

    Task<IEnumerable<TDocument>> FilterBySkipAsync(Expression<Func<TDocument, bool>> filterExpression, int skip,
        int take);
    Task<int> CountAsync(Expression<Func<TDocument, bool>> filterExpression);

    Task<TDocument> FindOneAsync(Expression<Func<TDocument, bool>> filterExpression);
    Task<bool> ExistsAsync(Expression<Func<TDocument, bool>> filterExpression);
    Task UpdateManyAsync(Expression<Func<TDocument, bool>> filterExpression, Action<TDocument> updateAction);
    Task DeleteManyAsync(Expression<Func<TDocument, bool>> filterExpression);
}