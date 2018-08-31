using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace ToOut3.Models
{
    public partial class StudentSelection2Context : DbContext
    {
        public StudentSelection2Context()
        {
        }

        public StudentSelection2Context(DbContextOptions<StudentSelection2Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<FinalDistribution> FinalDistribution { get; set; }
        public virtual DbSet<InfoTable> InfoTable { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<StudnetGpa> StudnetGpa { get; set; }
        public virtual DbSet<StuSelection> StuSelection { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                //                optionsBuilder.UseSqlServer("Server=.\\SQLExpress;Database=StudentSelection2;Trusted_Connection=True;");
                var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
                var configuration = builder.Build();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.Did);

                entity.Property(e => e.Did)
                    .HasColumnName("DID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Dname)
                    .HasColumnName("DName")
                    .HasMaxLength(15)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<FinalDistribution>(entity =>
            {
                entity.HasKey(e => new { e.StuId, e.DepId });

                entity.Property(e => e.StuId)
                    .HasColumnName("stu_ID")
                    .HasMaxLength(20);

                entity.Property(e => e.DepId).HasColumnName("Dep_ID");
            });

            modelBuilder.Entity<InfoTable>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(20)
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Pass)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.RoleId).HasColumnName("RoleID");
            });

            modelBuilder.Entity<Roles>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .HasMaxLength(15)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<StudnetGpa>(entity =>
            {
                entity.HasKey(e => e.StuId);

                entity.ToTable("StudnetGPA");

                entity.Property(e => e.StuId)
                    .HasColumnName("StuID")
                    .HasMaxLength(20)
                    .ValueGeneratedNever();

                entity.Property(e => e.StuGpa).HasColumnName("StuGPA");
            });

            modelBuilder.Entity<StuSelection>(entity =>
            {
                entity.HasKey(e => e.StuId);

                entity.Property(e => e.StuId)
                    .HasColumnName("StuID")
                    .HasMaxLength(20)
                    .ValueGeneratedNever();
            });
        }
    }
}
