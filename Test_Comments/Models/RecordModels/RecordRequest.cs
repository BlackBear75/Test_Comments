using System.ComponentModel.DataAnnotations;

namespace Test_Comments.Models.RecordModels;

public class RecordRequest
{
    [Required]
    public string UserName { get; set; }

    [Required]
    [EmailAddress(ErrorMessage = "Некорректный формат E-mail")]
    public string Email { get; set; }

    [Required]
    public string Captcha { get; set; }

    [Required]
    [MaxLength(500, ErrorMessage = "Текст сообщения не должен превышать 500 символов")]
    public string Text { get; set; }
}