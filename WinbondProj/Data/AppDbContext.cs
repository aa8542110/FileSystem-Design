using Microsoft.EntityFrameworkCore;
using WinbondProj.Models;
using Directory = WinbondProj.Models.Directory;
using File = WinbondProj.Models.File;

namespace WinbondProj.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<FileSystemItem> FileSystemItems { get; set; }
    public DbSet<Directory> Directories { get; set; }
    public DbSet<File> Files { get; set; }
    public DbSet<WordFile> WordFiles { get; set; }
    public DbSet<ImageFile> ImageFiles { get; set; }
    public DbSet<TextFile> TextFiles { get; set; }
    public DbSet<Tag> Tags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 配置 TPH (Table Per Hierarchy) 繼承策略
        modelBuilder.Entity<FileSystemItem>()
            .HasDiscriminator<string>("ItemType")
            .HasValue<Directory>("Directory")
            .HasValue<WordFile>("WordFile")
            .HasValue<ImageFile>("ImageFile")
            .HasValue<TextFile>("TextFile");

        // 配置自參照關係（Parent-Child）
        modelBuilder.Entity<Directory>()
            .HasMany(d => d.Items)
            .WithOne(i => i.Parent)
            .HasForeignKey(i => i.ParentId)
            .OnDelete(DeleteBehavior.Restrict); // 防止循環刪除

        // 多對多：FileSystemItem <-> Tag
        modelBuilder.Entity<FileSystemItem>()
            .HasMany(i => i.Tags)
            .WithMany()
            .UsingEntity("FileSystemItemTags");

        // 索引優化
        modelBuilder.Entity<FileSystemItem>()
            .HasIndex(f => f.ParentId);

        modelBuilder.Entity<FileSystemItem>()
            .HasIndex(f => f.Name);
    }
}
