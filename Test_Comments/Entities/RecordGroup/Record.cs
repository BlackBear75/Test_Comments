﻿using System.ComponentModel.DataAnnotations;
using Test_Comments.Base;

namespace Test_Comments.Entities.RecordGroup;

public class Record : Document
{
    public string UserName { get; set; }
    public string Email { get; set; }
    
    
    [Required]
    [MaxLength(500)]
    public string Text { get; set; }
    
    public List<Record> Comments { get; set; } = new List<Record>();
}