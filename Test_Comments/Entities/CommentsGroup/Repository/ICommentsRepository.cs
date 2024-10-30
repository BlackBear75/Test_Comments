using Test_Comments.Base;
using Test_Comments.Base.Repository;

namespace Test_Comments.Entities.CommentsGroup.Repository;

public interface ICommentsRepository<TDocument> : IBaseRepository<TDocument> where TDocument : Document
{
   
}