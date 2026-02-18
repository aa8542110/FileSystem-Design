using WinbondProj.DTOs;
using WinbondProj.Models;

namespace WinbondProj.Services;

public interface IFileSystemService
{
    // 查詢
    Task<FileSystemItemDto?> GetTreeAsync();
    Task<FileSystemItem?> GetByIdAsync(Guid id);
    Task<SearchResultDto> SearchByExtensionAsync(string extension);
    Task<string> GetConsoleOutputAsync();

    // 基於已載入 item 的操作（避免重複查詢）
    double GetTotalSize(FileSystemItem item);
    TraverseLogDto GetTraverseLog(FileSystemItem item, string operation);

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
