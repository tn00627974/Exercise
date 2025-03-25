using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HelloWorld.Models;

public partial class WebApiDataContext : DbContext
{
    public WebApiDataContext(DbContextOptions<WebApiDataContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
