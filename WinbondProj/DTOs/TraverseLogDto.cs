namespace WinbondProj.DTOs;

/// <summary>
/// 遍歷日誌 DTO
/// </summary>
public class TraverseLogDto
{
    public List<string> Logs { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string Operation { get; set; } = string.Empty; // "CalculateSize", "Search", "Traverse"
}
