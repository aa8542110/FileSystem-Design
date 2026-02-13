using WinbondProj.Models;
using Directory = WinbondProj.Models.Directory;

namespace WinbondProj.Data;

/// <summary>
/// 資料庫初始化：建立範例資料結構
/// </summary>
public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        // 確保資料庫已建立
        context.Database.EnsureCreated();

        // 如果已有資料，則不重複初始化
        if (context.FileSystemItems.Any())
        {
            return;
        }

        // 建立根目錄
        var root = new Directory
        {
            Name = "根目錄 (Root)",
            Size = 0,
            CreatedDate = DateTime.Now
        };

        // 建立專案文件目錄
        var projectDocs = new Directory
        {
            Name = "專案文件 (Project_Docs)",
            Size = 0,
            CreatedDate = DateTime.Now,
            Parent = root
        };

        var requirementDoc = new WordFile
        {
            Name = "需求規格書.docx",
            Size = 500,
            Pages = 15,
            CreatedDate = DateTime.Now,
            Parent = projectDocs
        };

        var architectureImage = new ImageFile
        {
            Name = "系統架構圖.png",
            Size = 2048, // 2MB = 2048KB
            Width = 1920,
            Height = 1080,
            CreatedDate = DateTime.Now,
            Parent = projectDocs
        };

        // 建立個人筆記目錄
        var personalNotes = new Directory
        {
            Name = "個人筆記 (Personal_Notes)",
            Size = 0,
            CreatedDate = DateTime.Now,
            Parent = root
        };

        var todoList = new TextFile
        {
            Name = "待辦清單.txt",
            Size = 1,
            Encoding = "UTF-8",
            CreatedDate = DateTime.Now,
            Parent = personalNotes
        };

        // 建立 2025 備份子目錄
        var archive2025 = new Directory
        {
            Name = "2025備份 (Archive_2025)",
            Size = 0,
            CreatedDate = DateTime.Now,
            Parent = personalNotes
        };

        var oldMeetingDoc = new WordFile
        {
            Name = "舊會議記錄.docx",
            Size = 200,
            Pages = 5,
            CreatedDate = DateTime.Now,
            Parent = archive2025
        };

        // 根目錄下的 README 檔案
        var readme = new TextFile
        {
            Name = "README.txt",
            Size = 0.5,
            Encoding = "ASCII",
            CreatedDate = DateTime.Now,
            Parent = root
        };

        // 組裝目錄結構
        projectDocs.Items.Add(requirementDoc);
        projectDocs.Items.Add(architectureImage);

        archive2025.Items.Add(oldMeetingDoc);

        personalNotes.Items.Add(todoList);
        personalNotes.Items.Add(archive2025);

        root.Items.Add(projectDocs);
        root.Items.Add(personalNotes);
        root.Items.Add(readme);

        // 儲存到資料庫
        context.FileSystemItems.Add(root);
        context.SaveChanges();
    }

    /// <summary>
    /// 取得根目錄（用於測試和顯示）
    /// </summary>
    public static Directory? GetRootDirectory(AppDbContext context)
    {
        return context.Directories
            .FirstOrDefault(d => d.ParentId == null);
    }
}
