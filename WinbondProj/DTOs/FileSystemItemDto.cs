namespace WinbondProj.DTOs;

/// <summary>
/// 檔案系統項目 DTO（資料傳輸物件）
/// </summary>
public class FileSystemItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Size { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid? ParentId { get; set; }
    public string ItemType { get; set; } = string.Empty; // Directory, WordFile, ImageFile, TextFile

    // 特殊屬性（依類型而定）
    public int? Pages { get; set; } // WordFile
    public int? Width { get; set; } // ImageFile
    public int? Height { get; set; } // ImageFile
    public string? Encoding { get; set; } // TextFile

    // 目錄專用
    public List<FileSystemItemDto>? Items { get; set; }

    // 額外資訊
    public double TotalSize { get; set; } // 遞迴計算的總大小
    public string Extension { get; set; } = string.Empty;
}
