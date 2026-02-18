using WinbondProj.DTOs;
using WinbondProj.Models;

namespace WinbondProj.Services;

public interface ICommandService
{
    Task<FileSystemItem> CreateDirectoryAsync(CreateDirectoryDto dto);
    Task<FileSystemItem> CreateFileAsync(CreateFileDto dto);
    Task<FileSystemItem> RenameAsync(Guid id, string newName);
    Task<bool> DeleteAsync(Guid id);
}
