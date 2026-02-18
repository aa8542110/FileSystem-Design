using WinbondProj.DTOs;
using WinbondProj.Models;

namespace WinbondProj.Services;

public interface IQueryService
{
    Task<FileSystemItemDto?> GetTreeAsync();
    Task<FileSystemItem?> GetByIdAsync(Guid id);
    Task<SearchResultDto> SearchByExtensionAsync(string extension);
    Task<string> GetConsoleOutputAsync();
}
