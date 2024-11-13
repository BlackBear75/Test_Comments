using System.ComponentModel.DataAnnotations;
using Test_Comments.Base;

namespace Test_Comments.Entities.UserGroup;

public class User : Document
{
    [Required]
    [MaxLength(100)]
    [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "User Name может содержать только буквы и цифры латинского алфавита")]
    public string UserName { get; set; }

    [Required]
    [EmailAddress(ErrorMessage = "Некорректный формат E-mail")]
    public string Email { get; set; }

    public string Captcha { get; set; }


    [Required]
    public string PasswordHash { get; set; }
}
