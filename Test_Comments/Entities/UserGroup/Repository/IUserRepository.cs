using Test_Comments.Base;
using Test_Comments.Base.Repository;

namespace Test_Comments.Entities.UserGroup.Repository;

public interface IUserRepository<TDocument> : IBaseRepository<TDocument> where TDocument : Document
{
   
}