namespace WinbondProj.DTOs;

/// <summary>
/// 搜尋結果 DTO
/// </summary>
public class SearchResultDto
{
    public string Extension { get; set; } = string.Empty;
    public List<string> Paths { get; set; } = new();
    public List<Guid> FileIds { get; set; } = new();
    public int Count { get; set; }
}
