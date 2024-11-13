using Test_Comments.Base;
using Test_Comments.Base.Repository;
using Test_Comments.Configuration;

namespace Test_Comments.Entities.UserGroup.Repository;

public class UserRepository<TDocument> : BaseRepository<TDocument>, IUserRepository<TDocument> where TDocument : Document
{
    public UserRepository(AppDbContext  databaseConfiguration) : base(databaseConfiguration)
    {
    }
    
}