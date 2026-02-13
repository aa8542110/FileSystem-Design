namespace WinbondProj.DTOs;

/// <summary>
/// 建立目錄的 DTO
/// </summary>
public class CreateDirectoryDto
{
    public string Name { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
}
