using System.ComponentModel.DataAnnotations;

namespace Test_Comments.Models.RecordModels;

public class RecordRequest
{
    public string Captcha { get; set; }
    public string Text { get; set; }
}