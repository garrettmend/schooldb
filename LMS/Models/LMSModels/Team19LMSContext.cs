using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LMS.Models.LMSModels
{
    public partial class Team19LMSContext : DbContext
    {
        public Team19LMSContext()
        {
        }

        public Team19LMSContext(DbContextOptions<Team19LMSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Administrators> Administrators { get; set; }
        public virtual DbSet<AssignmentCategories> AssignmentCategories { get; set; }
        public virtual DbSet<Assignments> Assignments { get; set; }
        public virtual DbSet<Classes> Classes { get; set; }
        public virtual DbSet<Courses> Courses { get; set; }
        public virtual DbSet<Departments> Departments { get; set; }
        public virtual DbSet<Enrolled> Enrolled { get; set; }
        public virtual DbSet<Professors> Professors { get; set; }
        public virtual DbSet<Students> Students { get; set; }
        public virtual DbSet<Submissions> Submissions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("Server=atr.eng.utah.edu;User Id=...;Password=...;Database=Team19LMS");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrators>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("char(8)");

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("date");

                entity.Property(e => e.FName)
                    .IsRequired()
                    .HasColumnName("fName")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.LName)
                    .IsRequired()
                    .HasColumnName("lName")
                    .HasColumnType("varchar(100)");
            });

            modelBuilder.Entity<AssignmentCategories>(entity =>
            {
                entity.HasKey(e => e.CategoryId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.InClass)
                    .HasName("AssignmentCategories_ibfk_1");

                entity.HasIndex(e => new { e.Name, e.InClass })
                    .HasName("Name")
                    .IsUnique();

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.HasOne(d => d.InClassNavigation)
                    .WithMany(p => p.AssignmentCategories)
                    .HasForeignKey(d => d.InClass)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AssignmentCategories_ibfk_1");
            });

            modelBuilder.Entity<Assignments>(entity =>
            {
                entity.HasKey(e => e.AssignmentId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Category)
                    .HasName("Assignments_ibfk_1");

                entity.HasIndex(e => new { e.Name, e.Category })
                    .HasName("name_unique")
                    .IsUnique();

                entity.Property(e => e.AssignmentId).HasColumnName("AssignmentID");

                entity.Property(e => e.Contents)
                    .IsRequired()
                    .HasColumnType("varchar(8192)");

                entity.Property(e => e.Due).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.HasOne(d => d.CategoryNavigation)
                    .WithMany(p => p.Assignments)
                    .HasForeignKey(d => d.Category)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Assignments_ibfk_1");
            });

            modelBuilder.Entity<Classes>(entity =>
            {
                entity.HasKey(e => e.ClassId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Listing)
                    .HasName("Classes_ibfk_1");

                entity.HasIndex(e => e.TaughtBy)
                    .HasName("Taught");

                entity.HasIndex(e => new { e.Season, e.Year, e.Listing })
                    .HasName("Season")
                    .IsUnique();

                entity.Property(e => e.ClassId).HasColumnName("ClassID");

                entity.Property(e => e.EndTime).HasColumnType("time");

                entity.Property(e => e.Location)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.Season)
                    .IsRequired()
                    .HasColumnType("varchar(6)");

                entity.Property(e => e.StartTime).HasColumnType("time");

                entity.Property(e => e.TaughtBy).HasColumnType("char(8)");

                entity.HasOne(d => d.ListingNavigation)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.Listing)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Classes_ibfk_1");

                entity.HasOne(d => d.TaughtByNavigation)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.TaughtBy)
                    .HasConstraintName("Taught");
            });

            modelBuilder.Entity<Courses>(entity =>
            {
                entity.HasKey(e => e.CatalogId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Department)
                    .HasName("Courses_ibfk_1");

                entity.HasIndex(e => new { e.Number, e.Department })
                    .HasName("Number")
                    .IsUnique();

                entity.Property(e => e.CatalogId).HasColumnName("CatalogID");

                entity.Property(e => e.Department)
                    .IsRequired()
                    .HasColumnType("varchar(4)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.HasOne(d => d.DepartmentNavigation)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.Department)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Courses_ibfk_1");
            });

            modelBuilder.Entity<Departments>(entity =>
            {
                entity.HasKey(e => e.Subject)
                    .HasName("PRIMARY");

                entity.Property(e => e.Subject).HasColumnType("varchar(4)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)");
            });

            modelBuilder.Entity<Enrolled>(entity =>
            {
                entity.HasKey(e => new { e.Student, e.Class })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Class)
                    .HasName("Enrolled_ibfk_2");

                entity.Property(e => e.Student).HasColumnType("char(8)");

                entity.Property(e => e.Grade)
                    .IsRequired()
                    .HasColumnType("varchar(2)");

                entity.HasOne(d => d.ClassNavigation)
                    .WithMany(p => p.Enrolled)
                    .HasForeignKey(d => d.Class)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Enrolled_ibfk_2");

                entity.HasOne(d => d.StudentNavigation)
                    .WithMany(p => p.Enrolled)
                    .HasForeignKey(d => d.Student)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Enrolled_ibfk_1");
            });

            modelBuilder.Entity<Professors>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.WorksIn)
                    .HasName("Professors_ibfk_1");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("char(8)");

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("date");

                entity.Property(e => e.FName)
                    .IsRequired()
                    .HasColumnName("fName")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.LName)
                    .IsRequired()
                    .HasColumnName("lName")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.WorksIn)
                    .IsRequired()
                    .HasColumnType("varchar(4)");

                entity.HasOne(d => d.WorksInNavigation)
                    .WithMany(p => p.Professors)
                    .HasForeignKey(d => d.WorksIn)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Professors_ibfk_1");
            });

            modelBuilder.Entity<Students>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Major)
                    .HasName("Students_ibfk_1");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("char(8)");

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("date");

                entity.Property(e => e.FName)
                    .IsRequired()
                    .HasColumnName("fName")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.LName)
                    .IsRequired()
                    .HasColumnName("lName")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.Major)
                    .IsRequired()
                    .HasColumnType("varchar(4)");

                entity.HasOne(d => d.MajorNavigation)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.Major)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Students_ibfk_1");
            });

            modelBuilder.Entity<Submissions>(entity =>
            {
                entity.HasKey(e => new { e.Assignment, e.Student })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Student)
                    .HasName("Submissions_ibfk_2");

                entity.Property(e => e.Student).HasColumnType("char(8)");

                entity.Property(e => e.SubmissionContents).HasColumnType("varchar(8192)");

                entity.Property(e => e.Time).HasColumnType("datetime");

                entity.HasOne(d => d.AssignmentNavigation)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.Assignment)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Submissions_ibfk_1");

                entity.HasOne(d => d.StudentNavigation)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.Student)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Submissions_ibfk_2");
            });
        }
    }
}
