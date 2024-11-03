using System.ComponentModel.DataAnnotations;
using Test_Comments.Base;

namespace Test_Comments.Entities.CommentsGroup
{
    public class Comment : Document
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "User Name может содержать только буквы и цифры латинского алфавита")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Некорректный формат E-mail")]
        public string Email { get; set; }

        [Url(ErrorMessage = "Некорректный формат URL для Home Page")]
        public string? HomePage { get; set; } 

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "CAPTCHA может содержать только буквы и цифры латинского алфавита")]
        public string Captcha { get; set; }

        [Required]
        [MaxLength(500, ErrorMessage = "Текст сообщения не должен превышать 500 символов")]
        public string Text { get; set; } 

        public DateTime PostedAt { get; set; } = DateTime.UtcNow; 
    }
}
