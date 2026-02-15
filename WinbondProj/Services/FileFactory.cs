using WinbondProj.DTOs;
using WinbondProj.Models;
using File = WinbondProj.Models.File;

namespace WinbondProj.Services;

/// <summary>
/// 檔案工廠 (Factory Pattern) — 根據 DTO 類型建立對應的 File 物件
/// </summary>
public class FileFactory
{
    public File Create(CreateFileDto dto) => dto switch
    {
        CreateWordFileDto w => new WordFile
        {
            Name = w.Name,
            Size = w.Size,
            Pages = w.Pages,
            ParentId = w.ParentId,
            CreatedDate = DateTime.Now
        },
        CreateImageFileDto i => new ImageFile
        {
            Name = i.Name,
            Size = i.Size,
            Width = i.Width,
            Height = i.Height,
            ParentId = i.ParentId,
            CreatedDate = DateTime.Now
        },
        CreateTextFileDto t => new TextFile
        {
            Name = t.Name,
            Size = t.Size,
            Encoding = t.Encoding,
            ParentId = t.ParentId,
            CreatedDate = DateTime.Now
        },
        _ => throw new ArgumentException("不支援的檔案類型")
    };
}
