using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Org.Proj.Image.DataAccess.Models
{
    public partial class ImageProcdbContext : DbContext
    {
        public ImageProcdbContext(DbContextOptions<ImageProcdbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CsxFileInfo> CsxFileInfo { get; set; }
        public virtual DbSet<FileRecordInfo> FileRecordInfo { get; set; }
        public virtual DbSet<FileRecordStatusCode> FileRecordStatusCode { get; set; }
        public virtual DbSet<FileStatusCode> FileStatusCode { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CsxFileInfo>(entity =>
            {
                entity.ToTable("CsxFileInfo", "ImgProc");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.FileName).HasMaxLength(2048);

                entity.Property(e => e.FilePath).HasMaxLength(2048);

                entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

                entity.HasOne(d => d.FileStatusCodeNavigation)
                    .WithMany(p => p.CsxFileInfo)
                    .HasForeignKey(d => d.FileStatusCode)
                    .HasConstraintName("FK__CsxFileIn__FileS__5441852A");
            });

            modelBuilder.Entity<FileRecordInfo>(entity =>
            {
                entity.ToTable("FileRecordInfo", "ImgProc");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Score).HasColumnType("decimal(3, 2)");

                entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

                entity.HasOne(d => d.File)
                    .WithMany(p => p.FileRecordInfo)
                    .HasForeignKey(d => d.FileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__FileRecor__FileI__5AEE82B9");

                entity.HasOne(d => d.RecordStatusCodeNavigation)
                    .WithMany(p => p.FileRecordInfo)
                    .HasForeignKey(d => d.RecordStatusCode)
                    .HasConstraintName("FK__FileRecor__Recor__5BE2A6F2");
            });

            modelBuilder.Entity<FileRecordStatusCode>(entity =>
            {
                entity.HasKey(e => e.StatusCode)
                    .HasName("PK__FileReco__6A7B44FD696BD7CA");

                entity.ToTable("FileRecordStatusCode", "ImgProc");

                entity.Property(e => e.StatusCode).ValueGeneratedOnAdd();

                entity.Property(e => e.Description).HasMaxLength(15);
            });

            modelBuilder.Entity<FileStatusCode>(entity =>
            {
                entity.HasKey(e => e.StatusCode)
                    .HasName("PK__FileStat__6A7B44FD537C1071");

                entity.ToTable("FileStatusCode", "ImgProc");

                entity.Property(e => e.StatusCode).ValueGeneratedOnAdd();

                entity.Property(e => e.Description).HasMaxLength(15);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
