using System.Xml.Linq;

namespace WinbondProj.Models;

/// <summary>
/// Word 文件檔案
/// </summary>
public class WordFile : File
{
    public int Pages { get; set; }

    public override XElement ToXml()
    {
        var elementName = Name.Replace(".", "_").Replace(" ", "_");

        // XML 標籤名稱不能以數字開頭，需要加前綴
        if (char.IsDigit(elementName[0]))
        {
            elementName = "File_" + elementName;
        }

        return new XElement(elementName, $"頁數: {Pages}, 大小: {Size}KB");
    }

    protected override string GetFileDetails()
    {
        return $"[Word 檔案] (頁數: {Pages}, 大小: {Size}KB)";
    }
}
