using System.Text;
using System.Xml.Linq;

namespace WinbondProj.Models;

/// <summary>
/// 目錄 (Composite Pattern - Composite)
/// </summary>
public class Directory : FileSystemItem
{
    public List<FileSystemItem> Items { get; set; } = new();

    public override double GetTotalSize()
    {
        // 遞迴計算：自身大小 + 所有子項目的總大小
        return Size + Items.Sum(item => item.GetTotalSize());
    }

    public override List<string> SearchByExtension(string extension, string currentPath = "")
    {
        var results = new List<string>();
        var fullPath = string.IsNullOrEmpty(currentPath) ? Name : $"{currentPath}/{Name}";

        // 遞迴搜尋所有子項目
        foreach (var item in Items)
        {
            results.AddRange(item.SearchByExtension(extension, fullPath));
        }

        return results;
    }

    public override List<FileSystemItem> SearchFilesByExtension(string extension)
    {
        var results = new List<FileSystemItem>();

        // 遞迴搜尋所有子項目
        foreach (var item in Items)
        {
            results.AddRange(item.SearchFilesByExtension(extension));
        }

        return results;
    }

    public override XElement ToXml()
    {
        var elementName = Name.Replace(" ", "_").Replace("(", "").Replace(")", "").Replace(".", "_");

        // XML 標籤名稱不能以數字開頭，需要加前綴
        if (char.IsDigit(elementName[0]))
        {
            elementName = "Dir_" + elementName;
        }

        var element = new XElement(elementName);

        // 遞迴建立子項目的 XML
        foreach (var item in Items)
        {
            element.Add(item.ToXml());
        }

        return element;
    }

    public override void Traverse(List<string> log, string currentPath = "")
    {
        var fullPath = string.IsNullOrEmpty(currentPath) ? Name : $"{currentPath} -> {Name}";
        log.Add($"Visiting: {fullPath}");

        // 遞迴遍歷所有子項目
        foreach (var item in Items)
        {
            item.Traverse(log, fullPath);
        }
    }

    public override string Display(string indent = "", bool isLast = true)
    {
        var sb = new StringBuilder();
        var connector = isLast ? "└── " : "├── ";

        sb.AppendLine($"{indent}{connector}{Name} [目錄]");

        // 更新下一層的縮排
        var newIndent = indent + (isLast ? "    " : "│   ");

        for (int i = 0; i < Items.Count; i++)
        {
            var isLastItem = i == Items.Count - 1;
            sb.Append(Items[i].Display(newIndent, isLastItem));
            if (i < Items.Count - 1 || !isLastItem)
            {
                sb.AppendLine();
            }
        }

        return sb.ToString();
    }
}
