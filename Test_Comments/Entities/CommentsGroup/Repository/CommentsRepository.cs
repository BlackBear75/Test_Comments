using Test_Comments.Base;
using Test_Comments.Base.Repository;
using Test_Comments.Configuration;

namespace Test_Comments.Entities.CommentsGroup.Repository;

public class CommentsRepository<TDocument> : BaseRepository<TDocument>, ICommentsRepository<TDocument> where TDocument : Document
{
    public CommentsRepository(AppDbContext  databaseConfiguration) : base(databaseConfiguration)
    {
    }
    
}