﻿using Microsoft.EntityFrameworkCore;
using Test_Comments.Entities;
using Test_Comments.Entities.RecordGroup;
using Test_Comments.Entities.UserGroup;

namespace Test_Comments.Configuration;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

   
    public DbSet<User> Users { get; set; }
    public DbSet<Record> Records { get; set; }
    
}