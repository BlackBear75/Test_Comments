using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Test_Comments.Base;

public class Record : Document
{
    public string UserName { get; set; }
    public string Email { get; set; }

    [Required]
    [MaxLength(500)]
    public string Text { get; set; }

    public Guid? ParentRecordId { get; set; } 

    [NotMapped]
    public List<Record> Comments { get; set; } = new List<Record>();
}