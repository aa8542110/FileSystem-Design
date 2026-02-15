using Microsoft.EntityFrameworkCore;
using System.Text;
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
        // 取得根目錄（ParentId 為 null）
        var root = await _context.FileSystemItems
            .Include(i => ((Directory)i).Items)
            .FirstOrDefaultAsync(i => i.ParentId == null);

        if (root == null) return null;

        // 遞迴載入所有子項目
        await LoadChildrenRecursiveAsync(root);

        return MapToDto(root);
    }

    private async Task LoadChildrenRecursiveAsync(FileSystemItem item)
    {
        if (item is Directory directory)
        {
            // 載入當前目錄的所有子項目
            await _context.Entry(directory)
                .Collection(d => d.Items)
                .LoadAsync();

            // 遞迴載入每個子項目
            foreach (var child in directory.Items)
            {
                await LoadChildrenRecursiveAsync(child);
            }
        }
    }

    public async Task<FileSystemItem?> GetByIdAsync(Guid id)
    {
        var item = await _context.FileSystemItems.FindAsync(id);
        if (item is Directory directory)
        {
            await LoadChildrenRecursiveAsync(directory);
        }
        return item;
    }

    public async Task<double> GetTotalSizeAsync(Guid id)
    {
        var item = await GetByIdAsync(id);
        return item?.GetTotalSize() ?? 0;
    }

    public async Task<SearchResultDto> SearchByExtensionAsync(string extension)
    {
        var root = await _context.FileSystemItems
            .FirstOrDefaultAsync(i => i.ParentId == null);

        if (root == null)
        {
            return new SearchResultDto { Extension = extension };
        }

        await LoadChildrenRecursiveAsync(root);

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

    public async Task<TraverseLogDto> GetTraverseLogAsync(Guid id, string operation)
    {
        var item = await GetByIdAsync(id);
        if (item == null)
        {
            return new TraverseLogDto { Operation = operation };
        }

        var logs = new List<string>();
        item.Traverse(logs);

        return new TraverseLogDto
        {
            Operation = operation,
            Logs = logs,
            Timestamp = DateTime.Now
        };
    }

    public async Task<string> GetXmlAsync(Guid id)
    {
        var item = await GetByIdAsync(id);
        if (item == null) return string.Empty;

        var xml = item.ToXml();
        return xml.ToString();
    }

    public string GetConsoleOutput()
    {
        var root = _context.FileSystemItems
            .Include(i => ((Directory)i).Items)
            .FirstOrDefault(i => i.ParentId == null);

        if (root == null) return "無資料";

        LoadChildrenRecursiveAsync(root).Wait();

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
        var item = await GetByIdAsync(id);
        if (item == null) return false;

        // 如果是目錄，遞迴刪除所有子項目
        if (item is Directory directory)
        {
            await DeleteChildrenRecursiveAsync(directory);
        }

        _context.FileSystemItems.Remove(item);
        await _context.SaveChangesAsync();

        return true;
    }

    private async Task DeleteChildrenRecursiveAsync(Directory directory)
    {
        foreach (var child in directory.Items.ToList())
        {
            if (child is Directory childDirectory)
            {
                await DeleteChildrenRecursiveAsync(childDirectory);
            }
            _context.FileSystemItems.Remove(child);
        }
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
            Extension = item.GetExtension()
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
