using WinbondProj.DTOs;
using WinbondProj.Models;

namespace WinbondProj.Services;

public interface IFileSystemService
{
    // 查詢
    Task<FileSystemItemDto?> GetTreeAsync();
    Task<FileSystemItem?> GetByIdAsync(Guid id);
    Task<double> GetTotalSizeAsync(Guid id);
    Task<SearchResultDto> SearchByExtensionAsync(string extension);
    Task<TraverseLogDto> GetTraverseLogAsync(Guid id, string operation);
    Task<string> GetXmlAsync(Guid id);
    string GetConsoleOutput();

    // 建立
    Task<FileSystemItem> CreateDirectoryAsync(CreateDirectoryDto dto);
    Task<FileSystemItem> CreateFileAsync(CreateFileDto dto);

    // 更新
    Task<FileSystemItem> RenameAsync(Guid id, string newName);

    // 刪除
    Task<bool> DeleteAsync(Guid id);

    // 標籤
    Task<List<TagDto>> GetAllTagsAsync();
    Task ToggleTagAsync(Guid itemId, Guid tagId);
}
