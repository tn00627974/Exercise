using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HelloWorld.MysqlModels;

public partial class WebContext : DbContext
{
    public WebContext(DbContextOptions<WebContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Employee> Employee { get; set; }

    public virtual DbSet<News> News { get; set; }

    public virtual DbSet<NewsFiles> NewsFiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Employee", "Web");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<News>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("News", "Web");

            entity.Property(e => e.Content).HasColumnType("text");
            entity.Property(e => e.Enable).HasColumnType("bit(1)");
            entity.Property(e => e.EndDateTime).HasColumnType("datetime");
            entity.Property(e => e.InsertDateTime).HasColumnType("datetime");
            entity.Property(e => e.StartDateTime).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(250);
            entity.Property(e => e.UpdateDateTime).HasColumnType("datetime");
        });

        modelBuilder.Entity<NewsFiles>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("NewsFiles", "Web");

            entity.Property(e => e.Extentsion).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(250);
            entity.Property(e => e.Path).HasColumnType("text");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
