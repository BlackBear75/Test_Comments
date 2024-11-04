using Test_Comments.Base;
using Test_Comments.Base.Repository;
using Test_Comments.Configuration;
using Test_Comments.Entities.CommentGroup.Repository;

namespace Test_Comments.Entities.RecordGroup.Repository;

public class RecordRepository<TDocument> : BaseRepository<TDocument>, IRecordRepository<TDocument> where TDocument : Document
{
    public RecordRepository(AppDbContext  databaseConfiguration) : base(databaseConfiguration)
    {
    }
    
}