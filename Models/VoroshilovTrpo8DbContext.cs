using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace trpo8.Models;

public partial class VoroshilovTrpo8DbContext : DbContext
{
    public VoroshilovTrpo8DbContext()
    {
    }

    public VoroshilovTrpo8DbContext(DbContextOptions<VoroshilovTrpo8DbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=sql.ects;Database=voroshilov_trpo8_DB;User Id=student_00;Password=student_00;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Task>(entity =>
        {
            entity.ToTable("Task");

            entity.Property(e => e.OwnerLogin).HasMaxLength(50);

            entity.HasOne(d => d.OwnerLoginNavigation).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.OwnerLogin)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Task_User");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Login).HasName("PK_User_1");

            entity.ToTable("User");

            entity.HasIndex(e => e.Id, "IX_User").IsUnique();

            entity.Property(e => e.Login).HasMaxLength(50);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
