using Microsoft.EntityFrameworkCore;
using WinbondProj.Data;
using WinbondProj.DTOs;

namespace WinbondProj.Services;

public class TagService : ITagService
{
    private readonly AppDbContext _context;

    public TagService(AppDbContext context)
    {
        _context = context;
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
}
