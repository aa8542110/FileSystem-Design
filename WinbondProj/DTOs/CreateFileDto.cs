using System.Text.Json.Serialization;

namespace WinbondProj.DTOs;

/// <summary>
/// 建立檔案的基類 DTO（使用 JSON 多型反序列化）
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(CreateWordFileDto), "word")]
[JsonDerivedType(typeof(CreateImageFileDto), "image")]
[JsonDerivedType(typeof(CreateTextFileDto), "text")]
public class CreateFileDto
{
    public string Name { get; set; } = string.Empty;
    public double Size { get; set; }
    public Guid? ParentId { get; set; }
}

/// <summary>
/// 建立 Word 檔案的 DTO
/// </summary>
public class CreateWordFileDto : CreateFileDto
{
    public int Pages { get; set; }
}

/// <summary>
/// 建立圖片檔案的 DTO
/// </summary>
public class CreateImageFileDto : CreateFileDto
{
    public int Width { get; set; }
    public int Height { get; set; }
}

/// <summary>
/// 建立純文字檔案的 DTO
/// </summary>
public class CreateTextFileDto : CreateFileDto
{
    public string Encoding { get; set; } = "UTF-8";
}
