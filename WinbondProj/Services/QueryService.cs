using Microsoft.EntityFrameworkCore;
using WinbondProj.Data;
using WinbondProj.DTOs;
using WinbondProj.Models;
using Directory = WinbondProj.Models.Directory;

namespace WinbondProj.Services;

public class QueryService : IQueryService
{
    private readonly AppDbContext _context;
    private readonly ItemMapper _mapper;

    public QueryService(AppDbContext context, ItemMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<FileSystemItemDto?> GetTreeAsync()
    {
        var root = await LoadSubtreeAsync(null);
        if (root == null) return null;
        return _mapper.MapToDto(root);
    }

    public async Task<FileSystemItem?> GetByIdAsync(Guid id)
    {
        return await LoadSubtreeAsync(id);
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

    public async Task<string> GetConsoleOutputAsync()
    {
        var root = await LoadSubtreeAsync(null);
        if (root == null) return "無資料";
        return root.Display();
    }

    private async Task<FileSystemItem?> LoadSubtreeAsync(Guid? id)
    {
        var allItems = await _context.FileSystemItems
            .Include(i => i.Tags)
            .AsNoTracking()
            .ToListAsync();

        FileSystemItem? root;
        if (id == null)
            root = allItems.FirstOrDefault(i => i.ParentId == null);
        else
            root = allItems.FirstOrDefault(i => i.Id == id);

        if (root == null) return null;

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
}