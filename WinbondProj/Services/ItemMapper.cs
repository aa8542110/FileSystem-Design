using WinbondProj.DTOs;
using WinbondProj.Models;
using Directory = WinbondProj.Models.Directory;

namespace WinbondProj.Services;

/// <summary>
/// 負責 FileSystemItem ↔ DTO 的映射轉換
/// </summary>
public class ItemMapper
{
    public FileSystemItemDto MapToDto(FileSystemItem item)
    {
        var dto = new FileSystemItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Size = item.Size,
            CreatedDate = item.CreatedDate,
            ParentId = item.ParentId,
            Extension = item.GetExtension(),
            Tags = item.Tags.Select(t => new TagDto
            {
                Id = t.Id,
                Name = t.Name,
                Color = t.Color
            }).ToList()
        };

        switch (item)
        {
            case WordFile wordFile:
                dto.ItemType = "WordFile";
                dto.Pages = wordFile.Pages;
                dto.TotalSize = dto.Size;
                break;
            case ImageFile imageFile:
                dto.ItemType = "ImageFile";
                dto.Width = imageFile.Width;
                dto.Height = imageFile.Height;
                dto.TotalSize = dto.Size;
                break;
            case TextFile textFile:
                dto.ItemType = "TextFile";
                dto.Encoding = textFile.Encoding;
                dto.TotalSize = dto.Size;
                break;
            case Directory directory:
                dto.ItemType = "Directory";
                dto.Items = directory.Items.Select(MapToDto).ToList();
                dto.TotalSize = dto.Size + dto.Items.Sum(child => child.TotalSize);
                break;
        }

        return dto;
    }
}
