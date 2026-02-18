using WinbondProj.DTOs;

namespace WinbondProj.Services;

public interface ITagService
{
    Task<List<TagDto>> GetAllTagsAsync();
    Task ToggleTagAsync(Guid itemId, Guid tagId);
}
