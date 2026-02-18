using Microsoft.EntityFrameworkCore;
using WinbondProj.Data;
using WinbondProj.DTOs;
using WinbondProj.Models;
using Directory = WinbondProj.Models.Directory;

namespace WinbondProj.Services;

public class CommandService : ICommandService
{
    private readonly AppDbContext _context;
    private readonly FileFactory _fileFactory;

    public CommandService(AppDbContext context, FileFactory fileFactory)
    {
        _context = context;
        _fileFactory = fileFactory;
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
        var allItems = await _context.FileSystemItems.ToListAsync();
        var target = allItems.FirstOrDefault(i => i.Id == id);
        if (target == null) return false;

        var idsToDelete = new HashSet<Guid> { id };
        CollectDescendantIds(id, allItems.ToLookup(i => i.ParentId), idsToDelete);

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
}
