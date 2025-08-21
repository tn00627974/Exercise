using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Todo.Models;

public partial class TodoContext : DbContext
{
    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Division> Division { get; set; }

    public virtual DbSet<Employee> Employee { get; set; }

    public virtual DbSet<JobTitle> JobTitle { get; set; }

    public virtual DbSet<TodoList> TodoList { get; set; }

    public virtual DbSet<UploadFile> UploadFile { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Division>(entity =>
        {
            entity.HasKey(e => e.DivisionId).HasName("PK__Division__20EFC6A8CB77CC7A");

            entity.Property(e => e.DivisionId).ValueGeneratedNever();
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04F11473B94AD");

            entity.Property(e => e.EmployeeId).ValueGeneratedNever();
        });

        modelBuilder.Entity<JobTitle>(entity =>
        {
            entity.HasKey(e => e.JobTitleId).HasName("PK__JobTitle__35382FE96E025BCE");

            entity.Property(e => e.JobTitleId).ValueGeneratedNever();
        });

        modelBuilder.Entity<TodoList>(entity =>
        {
            entity.ToTable("TodoList", "dbo");

            entity.HasKey(e => e.TodoId).HasName("PK__TodoList__958625526CEF186C");

            //entity.Property(e => e.TodoId).ValueGeneratedNever(); // 手動新增Guid
            entity.Property(e => e.TodoId).ValueGeneratedOnAdd(); // // 自動新增Guid
            entity.Property(e => e.InsertTime).HasColumnType("datetime");
            entity.Property(e => e.UpdateTime).HasColumnType("datetime");
        });

        modelBuilder.Entity<UploadFile>(entity =>
        {
            entity.HasKey(e => e.UploadFileId).HasName("PK__UploadFi__6819F4EE651D848B");

            entity.Property(e => e.UploadFileId).ValueGeneratedNever();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
