using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Cooking.Models
{
    public partial class CookingContext : DbContext
    {
        public CookingContext()
        {
        }

        public CookingContext(DbContextOptions<CookingContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Measure> Measure { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                optionsBuilder.UseSqlServer("Server=.\\;Database=Cooking;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CategoryCode)
                    .HasName("PK_measurementType");

                entity.ToTable("category");

                entity.Property(e => e.CategoryCode)
                    .HasColumnName("categoryCode")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.MeasureCode)
                    .HasColumnName("measureCode")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.MeasureCodeNavigation)
                    .WithMany(p => p.Category)
                    .HasForeignKey(d => d.MeasureCode)
                    .HasConstraintName("FK_category_measure");
            });

            modelBuilder.Entity<Measure>(entity =>
            {
                entity.HasKey(e => e.MeasureCode)
                    .HasName("aaaaameasure_PK")
                    .ForSqlServerIsClustered(false);

                entity.ToTable("measure");

                entity.HasIndex(e => e.CategoryCode)
                    .HasName("typemeasure");

                entity.HasIndex(e => e.MeasureCode)
                    .HasName("measureCode");

                entity.Property(e => e.MeasureCode)
                    .HasColumnName("measureCode")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.CategoryCode)
                    .IsRequired()
                    .HasColumnName("categoryCode")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RatioToBase).HasColumnName("ratioToBase");

                entity.HasOne(d => d.CategoryCodeNavigation)
                    .WithMany(p => p.Measure)
                    .HasForeignKey(d => d.CategoryCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_measure_category1");
            });
        }
    }
}
