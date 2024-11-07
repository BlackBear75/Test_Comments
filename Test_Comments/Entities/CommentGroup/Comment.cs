using System.ComponentModel.DataAnnotations;
using Test_Comments.Base;

namespace Test_Comments.Entities.CommentGroup
{
    public class Comment : Document
    {
            public string UserName { get; set; }
            public string Email { get; set; }

            [Required]
            [MaxLength(500)]
            public string Text { get; set; }

            public Guid RecordId { get; set; } 
            public Guid? ParentCommentId { get; set; } 

            public List<Comment> Replies { get; set; } = new List<Comment>(); 
        
    }
}
