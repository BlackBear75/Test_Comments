using Test_Comments.Base;
using Test_Comments.Base.Repository;

namespace Test_Comments.Entities.CommentGroup.Repository;

public interface ICommentRepository<TDocument> : IBaseRepository<TDocument> where TDocument : Document
{
   
}