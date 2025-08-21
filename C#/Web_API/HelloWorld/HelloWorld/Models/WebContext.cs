using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HelloWorld.Models;

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
            entity.HasNoKey();

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<News>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.EndDateTime).HasColumnType("datetime");
            entity.Property(e => e.InsertDateTime).HasColumnType("datetime");
            entity.Property(e => e.NewsId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.StartDateTime).HasColumnType("datetime");
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(250);
            entity.Property(e => e.UpdateDateTime).HasColumnType("datetime");
        });

        modelBuilder.Entity<NewsFiles>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.Extentsion).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(250);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
