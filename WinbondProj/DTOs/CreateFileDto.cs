namespace WinbondProj.DTOs;

/// <summary>
/// 建立檔案的 DTO
/// </summary>
public class CreateFileDto
{
    public string Name { get; set; } = string.Empty;
    public double Size { get; set; }
    public Guid? ParentId { get; set; }
    public string FileType { get; set; } = string.Empty; // "Word", "Image", "Text"

    // WordFile 專用
    public int? Pages { get; set; }

    // ImageFile 專用
    public int? Width { get; set; }
    public int? Height { get; set; }

    // TextFile 專用
    public string? Encoding { get; set; }
}
