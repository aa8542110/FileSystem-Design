using System.Xml.Linq;

namespace WinbondProj.Models;

/// <summary>
/// 抽象檔案基類 (Composite Pattern - Leaf)
/// </summary>
public abstract class File : FileSystemItem
{
    public override double GetTotalSize()
    {
        // 檔案直接回傳自己的大小
        return Size;
    }

    public override List<string> SearchByExtension(string extension, string currentPath = "")
    {
        var results = new List<string>();
        var fullPath = string.IsNullOrEmpty(currentPath) ? Name : $"{currentPath}/{Name}";

        if (GetExtension().Equals(extension, StringComparison.OrdinalIgnoreCase))
        {
            results.Add(fullPath);
        }

        return results;
    }

    public override List<FileSystemItem> SearchFilesByExtension(string extension)
    {
        var results = new List<FileSystemItem>();

        if (GetExtension().Equals(extension, StringComparison.OrdinalIgnoreCase))
        {
            results.Add(this);
        }

        return results;
    }

    public override void Traverse(List<string> log, string currentPath = "")
    {
        var fullPath = string.IsNullOrEmpty(currentPath) ? Name : $"{currentPath} -> {Name}";
        log.Add($"Visiting: {fullPath}");
    }

    protected abstract string GetFileDetails();

    public override string Display(string indent = "", bool isLast = true)
    {
        var connector = isLast ? "└── " : "├── ";
        return $"{indent}{connector}{Name} {GetFileDetails()}";
    }
}
