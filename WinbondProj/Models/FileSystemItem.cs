using System.Xml.Linq;

namespace WinbondProj.Models;

/// <summary>
/// 抽象基類：檔案系統項目 (Composite Pattern - Component)
/// </summary>
public abstract class FileSystemItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Size { get; set; } // KB
    public DateTime CreatedDate { get; set; }
    public Guid? ParentId { get; set; }

    // 導航屬性
    public Directory? Parent { get; set; }

    protected FileSystemItem()
    {
        Id = Guid.NewGuid();
        CreatedDate = DateTime.Now;
    }

    /// <summary>
    /// 取得總大小（遞迴計算）
    /// </summary>
    public abstract double GetTotalSize();

    /// <summary>
    /// 依副檔名搜尋（遞迴）
    /// </summary>
    public abstract List<string> SearchByExtension(string extension, string currentPath = "");

    /// <summary>
    /// 依副檔名搜尋檔案物件（遞迴）- 用於取得檔案 ID
    /// </summary>
    public abstract List<FileSystemItem> SearchFilesByExtension(string extension);

    /// <summary>
    /// 轉換為 XML 格式
    /// </summary>
    public abstract XElement ToXml();

    /// <summary>
    /// 遍歷訪問（用於記錄日誌）
    /// </summary>
    public abstract void Traverse(List<string> log, string currentPath = "");

    /// <summary>
    /// 顯示樹狀結構（Console 輸出用）
    /// </summary>
    public abstract string Display(string indent = "", bool isLast = true);

    /// <summary>
    /// 取得檔案副檔名
    /// </summary>
    public string GetExtension()
    {
        var parts = Name.Split('.');
        return parts.Length > 1 ? "." + parts[^1] : "";
    }
}
