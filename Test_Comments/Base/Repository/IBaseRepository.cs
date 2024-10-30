using System.Linq.Expressions;

namespace Test_Comments.Base.Repository;

public interface IBaseRepository<TDocument> where TDocument : Document
{
    Task<IEnumerable<TDocument>> GetAllAsync();
    Task<TDocument> FindByIdAsync(Guid id);
    Task InsertOneAsync(TDocument document);
    Task UpdateOneAsync(TDocument document);
    Task DeleteOneAsync(Guid id);
    Task<IEnumerable<TDocument>> FilterByAsync(Expression<Func<TDocument, bool>> filterExpression);
    Task<int> CountAsync(Expression<Func<TDocument, bool>> filterExpression);
}