using System.Xml.Linq;

namespace WinbondProj.Models;

/// <summary>
/// 純文字檔案
/// </summary>
public class TextFile : File
{
    public string Encoding { get; set; } = "UTF-8";

    public override XElement ToXml()
    {
        var elementName = Name.Replace(".", "_").Replace(" ", "_");

        // XML 標籤名稱不能以數字開頭，需要加前綴
        if (char.IsDigit(elementName[0]))
        {
            elementName = "File_" + elementName;
        }

        return new XElement(elementName, $"編碼: {Encoding}, 大小: {Size}KB");
    }

    protected override string GetFileDetails()
    {
        return $"[純文字檔] (編碼: {Encoding}, 大小: {Size}KB)";
    }
}
