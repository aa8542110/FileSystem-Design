using Microsoft.AspNetCore.Mvc;
using WinbondProj.DTOs;
using WinbondProj.Services;

namespace WinbondProj.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileSystemController : ControllerBase
{
    private readonly IFileSystemService _service;
    private readonly ILogger<FileSystemController> _logger;

    public FileSystemController(IFileSystemService service, ILogger<FileSystemController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// 取得完整目錄樹
    /// </summary>
    [HttpGet("tree")]
    public async Task<ActionResult<FileSystemItemDto>> GetTree()
    {
        var tree = await _service.GetTreeAsync();
        if (tree == null)
        {
            return NotFound("找不到根目錄");
        }
        return Ok(tree);
    }

    /// <summary>
    /// 取得單一項目
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(Guid id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound($"找不到 ID 為 {id} 的項目");
        }
        return Ok(item);
    }

    /// <summary>
    /// 計算目錄總大小（遞迴）
    /// </summary>
    [HttpGet("{id}/size")]
    public async Task<ActionResult<object>> GetTotalSize(Guid id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound($"找不到 ID 為 {id} 的項目");
        }

        var totalSize = await _service.GetTotalSizeAsync(id);
        var log = await _service.GetTraverseLogAsync(id, "CalculateSize");

        return Ok(new
        {
            Id = id,
            Name = item.Name,
            TotalSize = totalSize,
            Unit = "KB",
            TraverseLog = log.Logs
        });
    }

    /// <summary>
    /// 副檔名搜尋
    /// </summary>
    [HttpGet("search/extension")]
    public async Task<ActionResult<SearchResultDto>> SearchByExtension([FromQuery] string ext)
    {
        if (string.IsNullOrWhiteSpace(ext))
        {
            return BadRequest("請提供副檔名");
        }

        // 確保副檔名格式正確
        if (!ext.StartsWith("."))
        {
            ext = "." + ext;
        }

        var result = await _service.SearchByExtensionAsync(ext);
        return Ok(result);
    }

    /// <summary>
    /// 取得 XML 結構
    /// </summary>
    [HttpGet("{id}/xml")]
    public async Task<ActionResult<string>> GetXml(Guid id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound($"找不到 ID 為 {id} 的項目");
        }

        var xml = await _service.GetXmlAsync(id);
        return Content(xml, "application/xml");
    }

    /// <summary>
    /// 取得遍歷日誌
    /// </summary>
    [HttpGet("{id}/traverse-log")]
    public async Task<ActionResult<TraverseLogDto>> GetTraverseLog(Guid id, [FromQuery] string operation = "Traverse")
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound($"找不到 ID 為 {id} 的項目");
        }

        var log = await _service.GetTraverseLogAsync(id, operation);
        return Ok(log);
    }

    /// <summary>
    /// 取得 Console 輸出（樹狀結構）
    /// </summary>
    [HttpGet("console")]
    public ActionResult<object> GetConsoleOutput()
    {
        var output = _service.GetConsoleOutput();
        return Ok(new { Output = output });
    }

    /// <summary>
    /// 建立目錄
    /// </summary>
    [HttpPost("directory")]
    public async Task<ActionResult> CreateDirectory([FromBody] CreateDirectoryDto dto)
    {
        try
        {
            var directory = await _service.CreateDirectoryAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = directory.Id }, directory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "建立目錄失敗");
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// 建立檔案
    /// </summary>
    [HttpPost("file")]
    public async Task<ActionResult> CreateFile([FromBody] CreateFileDto dto)
    {
        try
        {
            var file = await _service.CreateFileAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = file.Id }, file);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "建立檔案失敗");
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// 重新命名
    /// </summary>
    [HttpPut("{id}/rename")]
    public async Task<ActionResult> Rename(Guid id, [FromBody] RenameDto dto)
    {
        try
        {
            var item = await _service.RenameAsync(id, dto.NewName);
            return Ok(item);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "重新命名失敗");
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// 刪除項目
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success)
        {
            return NotFound($"找不到 ID 為 {id} 的項目");
        }
        return NoContent();
    }
}
