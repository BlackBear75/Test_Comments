namespace Test_Comments.Models.AuthModels;

using System.ComponentModel.DataAnnotations;


    public class RegisterRequest
    {
        [Required(ErrorMessage = "Ім'я користувача є обов'язковим")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Ім'я користувача має бути від 3 до 20 символів")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email є обов'язковим")]
        [EmailAddress(ErrorMessage = "Неправильний формат електронної пошти")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пароль є обов'язковим")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Пароль має містити принаймні 8 символів")]
        public string Password { get; set; }
    }
