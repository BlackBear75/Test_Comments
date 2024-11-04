namespace Test_Comments.Models.AuthModels;

public class AuthResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    
    public Guid? UserId { get; set; } 
  
}