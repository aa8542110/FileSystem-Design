using System.Xml.Linq;

namespace WinbondProj.Models;

/// <summary>
/// 圖片檔案
/// </summary>
public class ImageFile : File
{
    public int Width { get; set; }
    public int Height { get; set; }

    public override XElement ToXml()
    {
        var elementName = Name.Replace(".", "_").Replace(" ", "_");

        // XML 標籤名稱不能以數字開頭，需要加前綴
        if (char.IsDigit(elementName[0]))
        {
            elementName = "File_" + elementName;
        }

        return new XElement(elementName, $"解析度: {Width}x{Height}, 大小: {Size}KB");
    }

    protected override string GetFileDetails()
    {
        return $"[圖片] (解析度: {Width}x{Height}, 大小: {Size}KB)";
    }
}
