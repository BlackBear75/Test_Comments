using Test_Comments.Base;
using Test_Comments.Base.Repository;

namespace Test_Comments.Entities.RecordGroup.Repository;

public interface IRecordRepository<TDocument> : IBaseRepository<TDocument> where TDocument : Document
{
   
}