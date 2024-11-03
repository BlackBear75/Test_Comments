using Test_Comments.Base;
using Test_Comments.Base.Repository;
using Test_Comments.Configuration;

namespace Test_Comments.Entities.CommentGroup.Repository;

public class CommentRepository<TDocument> : BaseRepository<TDocument>, ICommentRepository<TDocument> where TDocument : Document
{
    public CommentRepository(AppDbContext  databaseConfiguration) : base(databaseConfiguration)
    {
    }
    
}