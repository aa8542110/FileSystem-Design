using Microsoft.EntityFrameworkCore;
using WinbondProj.Data;
using WinbondProj.DTOs;
using WinbondProj.Models;
using Directory = WinbondProj.Models.Directory;

namespace WinbondProj.Services;

public class FileSystemService : IFileSystemService
{
    private readonly AppDbContext _context;
    private readonly FileFactory _fileFactory;

    public FileSystemService(AppDbContext context, FileFactory fileFactory)
    {
        _context = context;
        _fileFactory = fileFactory;
    }

    public async Task<FileSystemItemDto?> GetTreeAsync()
    {
        var root = await LoadSubtreeAsync(null);
        if (root == null) return null;
        return MapToDto(root);
    }

    /// <summary>
    /// 一次查詢載入子樹：避免 N+1 問題
    /// 傳入 null 表示從根目錄開始；傳入 id 表示載入該節點及其所有後代
    /// </summary>
    private async Task<FileSystemItem?> LoadSubtreeAsync(Guid? id)
    {
        // 1. 一次撈出所有項目（含 Tags），避免逐節點遞迴查詢
        var allItems = await _context.FileSystemItems
            .Include(i => i.Tags)
            .AsNoTracking()
            .ToListAsync();

        // 2. 找到起點節點
        FileSystemItem? root;
        if (id == null)
            root = allItems.FirstOrDefault(i => i.ParentId == null);
        else
            root = allItems.FirstOrDefault(i => i.Id == id);

        if (root == null) return null;

        // 3. 在記憶體中建立 parent-child 關係
        var lookup = allItems.ToLookup(i => i.ParentId);
        AssembleChildren(root, lookup);

        return root;
    }

    private void AssembleChildren(FileSystemItem item, ILookup<Guid?, FileSystemItem> lookup)
    {
        if (item is Directory directory)
        {
            directory.Items = lookup[item.Id].ToList();
            foreach (var child in directory.Items)
            {
                AssembleChildren(child, lookup);
            }
        }
    }

    public async Task<FileSystemItem?> GetByIdAsync(Guid id)
    {
        return await LoadSubtreeAsync(id);
    }

    public double GetTotalSize(FileSystemItem item)
    {
        return item.GetTotalSize();
    }

    public async Task<SearchResultDto> SearchByExtensionAsync(string extension)
    {
        var root = await LoadSubtreeAsync(null);

        if (root == null)
        {
            return new SearchResultDto { Extension = extension };
        }

        var paths = root.SearchByExtension(extension);
        var files = root.SearchFilesByExtension(extension);

        return new SearchResultDto
        {
            Extension = extension,
            Paths = paths,
            FileIds = files.Select(f => f.Id).ToList(),
            Count = paths.Count
        };
    }

    public TraverseLogDto GetTraverseLog(FileSystemItem item, string operation)
    {
        var logs = new List<string>();
        item.Traverse(logs);

        return new TraverseLogDto
        {
            Operation = operation,
            Logs = logs,
            Timestamp = DateTime.Now
        };
    }

    public async Task<string> GetConsoleOutputAsync()
    {
        var root = await LoadSubtreeAsync(null);
        if (root == null) return "無資料";
        return root.Display();
    }

    public async Task<FileSystemItem> CreateDirectoryAsync(CreateDirectoryDto dto)
    {
        var directory = new Directory
        {
            Name = dto.Name,
            Size = 0,
            ParentId = dto.ParentId,
            CreatedDate = DateTime.Now
        };

        _context.FileSystemItems.Add(directory);
        await _context.SaveChangesAsync();

        return directory;
    }

    public async Task<FileSystemItem> CreateFileAsync(CreateFileDto dto)
    {
        var file = _fileFactory.Create(dto);

        _context.FileSystemItems.Add(file);
        await _context.SaveChangesAsync();

        return file;
    }

    public async Task<FileSystemItem> RenameAsync(Guid id, string newName)
    {
        var item = await _context.FileSystemItems.FindAsync(id);
        if (item == null)
        {
            throw new KeyNotFoundException("找不到指定的項目");
        }

        item.Name = newName;
        await _context.SaveChangesAsync();

        return item;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        // 刪除需要 tracked entities，用單次查詢取得所有後代的 ID
        var allItems = await _context.FileSystemItems.ToListAsync();
        var target = allItems.FirstOrDefault(i => i.Id == id);
        if (target == null) return false;

        // 收集所有要刪除的 ID（含自身）
        var idsToDelete = new HashSet<Guid> { id };
        CollectDescendantIds(id, allItems.ToLookup(i => i.ParentId), idsToDelete);

        // 一次移除所有（子項目先刪，父項目後刪）
        var itemsToDelete = allItems.Where(i => idsToDelete.Contains(i.Id)).ToList();
        _context.FileSystemItems.RemoveRange(itemsToDelete);
        await _context.SaveChangesAsync();

        return true;
    }

    private void CollectDescendantIds(Guid parentId, ILookup<Guid?, FileSystemItem> lookup, HashSet<Guid> result)
    {
        foreach (var child in lookup[parentId])
        {
            result.Add(child.Id);
            CollectDescendantIds(child.Id, lookup, result);
        }
    }

    public async Task<List<TagDto>> GetAllTagsAsync()
    {
        return await _context.Tags
            .Select(t => new TagDto { Id = t.Id, Name = t.Name, Color = t.Color })
            .ToListAsync();
    }

    public async Task ToggleTagAsync(Guid itemId, Guid tagId)
    {
        var item = await _context.FileSystemItems
            .Include(i => i.Tags)
            .FirstOrDefaultAsync(i => i.Id == itemId);

        if (item == null)
            throw new KeyNotFoundException("找不到指定的項目");

        var tag = await _context.Tags.FindAsync(tagId);
        if (tag == null)
            throw new KeyNotFoundException("找不到指定的標籤");

        var existing = item.Tags.FirstOrDefault(t => t.Id == tagId);
        if (existing != null)
            item.Tags.Remove(existing);
        else
            item.Tags.Add(tag);

        await _context.SaveChangesAsync();
    }

    private FileSystemItemDto MapToDto(FileSystemItem item)
    {
        var dto = new FileSystemItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Size = item.Size,
            CreatedDate = item.CreatedDate,
            ParentId = item.ParentId,
            TotalSize = item.GetTotalSize(),
            Extension = item.GetExtension(),
            Tags = item.Tags.Select(t => new TagDto
            {
                Id = t.Id,
                Name = t.Name,
                Color = t.Color
            }).ToList()
        };

        switch (item)
        {
            case WordFile wordFile:
                dto.ItemType = "WordFile";
                dto.Pages = wordFile.Pages;
                break;
            case ImageFile imageFile:
                dto.ItemType = "ImageFile";
                dto.Width = imageFile.Width;
                dto.Height = imageFile.Height;
                break;
            case TextFile textFile:
                dto.ItemType = "TextFile";
                dto.Encoding = textFile.Encoding;
                break;
            case Directory directory:
                dto.ItemType = "Directory";
                dto.Items = directory.Items.Select(MapToDto).ToList();
                break;
        }

        return dto;
    }
}
