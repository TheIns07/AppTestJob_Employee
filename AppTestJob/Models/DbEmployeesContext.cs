using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AppTestJob.Models;

public partial class DbEmployeesContext : DbContext
{
    public DbEmployeesContext()
    {
    }

    public DbEmployeesContext(DbContextOptions<DbEmployeesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Employee> Employees { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { }
   
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC0700874170");

            entity.ToTable("Employee");

            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.Dob)
                .HasColumnType("date")
                .HasColumnName("DOB");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Salary).HasColumnType("decimal(18, 2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
