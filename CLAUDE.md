# FileSystem-Design — Claude Code 專案導覽

## 專案概述
雲端檔案管理系統，展示多種 Design Pattern（Composite、Factory、Visitor、Observer、Command、Template Method）。
- **後端**：C# .NET 8 Web API + EF Core + SQLite
- **前端**：Vue 3 (Composition API) + Element Plus + Vite
- **通訊**：RESTful JSON API，後端 port 5089，前端 port 5173

## 啟動方式
```bash
# 後端
cd WinbondProj && dotnet run          # http://localhost:5089, Swagger: /swagger

# 前端
cd vue-frontend && npm install && npm run dev   # http://localhost:5173
```

---

## 後端檔案結構 (`WinbondProj/`)

### 進入點
| 檔案 | 說明 |
|------|------|
| `Program.cs` | DI 註冊（DbContext、FileFactory、FileSystemService）、CORS、Swagger、DB 初始化 |

### Models — Composite Pattern 核心
| 檔案 | 角色 | 說明 |
|------|------|------|
| `Models/FileSystemItem.cs` | **Component (抽象基類)** | 共用屬性（Id, Name, Size, CreatedDate, ParentId, Tags）+ 6 個 abstract 方法（GetTotalSize, SearchByExtension, SearchFilesByExtension, ToXml, Traverse, Display） |
| `Models/Directory.cs` | **Composite** | 含 `List<FileSystemItem> Items`，所有方法遞迴遍歷子項目 |
| `Models/File.cs` | **Leaf (抽象)** | 終端節點，不遞迴。定義 `abstract GetFileDetails()` (Template Method) |
| `Models/WordFile.cs` | Leaf 具體類別 | +Pages 屬性 |
| `Models/ImageFile.cs` | Leaf 具體類別 | +Width, Height 屬性 |
| `Models/TextFile.cs` | Leaf 具體類別 | +Encoding 屬性 |
| `Models/Tag.cs` | 獨立實體 | Id, Name, Color；與 FileSystemItem 多對多 |

### Data — EF Core + SQLite
| 檔案 | 說明 |
|------|------|
| `Data/AppDbContext.cs` | TPH 繼承策略（`ItemType` 鑑別欄位）、自參照 FK（ParentId → Id）、多對多 Tag 關聯、ParentId/Name 索引 |
| `Data/DbInitializer.cs` | Seed 範例結構（根目錄 → 專案文件/個人筆記 → 子目錄/檔案）+ 3 個預設標籤（Urgent 紅、Work 藍、Personal 綠） |

### Services — 業務邏輯
| 檔案 | 說明 |
|------|------|
| `Services/IFileSystemService.cs` | 介面：查詢（GetTree, GetTotalSize, Search, XML, Console）、CRUD（Create, Rename, Delete）、標籤（GetAllTags, ToggleTag） |
| `Services/FileSystemService.cs` | 實作：遞迴載入樹、呼叫 Model 層的 Composite 方法、Factory 建檔、遞迴刪除、Tag Toggle |
| `Services/FileFactory.cs` | **Factory Pattern**：switch expression 依 DTO 子型別產生 WordFile/ImageFile/TextFile |

### DTOs — 資料傳輸物件
| 檔案 | 說明 |
|------|------|
| `DTOs/FileSystemItemDto.cs` | 完整樹回傳 DTO（含 Items, Tags, TotalSize, Extension, 各類型專屬欄位） |
| `DTOs/CreateFileDto.cs` | **JSON 多型**（`[JsonPolymorphic]` + `$type` 判別器）→ CreateWordFileDto / CreateImageFileDto / CreateTextFileDto |
| `DTOs/CreateDirectoryDto.cs` | Name + ParentId |
| `DTOs/SearchResultDto.cs` | Extension, Paths, FileIds, Count |
| `DTOs/TraverseLogDto.cs` | Logs, Timestamp, Operation |
| `DTOs/TagDto.cs` | Id, Name, Color |
| `DTOs/RenameDto.cs` | NewName |

### Controllers — REST API
| 檔案 | 說明 |
|------|------|
| `Controllers/FileSystemController.cs` | 路由 `/api/filesystem`，端點：GET tree / {id} / {id}/size / search/extension / {id}/xml / {id}/traverse-log / console / tags，POST directory / file / {id}/tags/{tagId}，PUT {id}/rename，DELETE {id} |

---

## 前端檔案結構 (`vue-frontend/src/`)

### 進入點
| 檔案 | 說明 |
|------|------|
| `main.js` | createApp + Element Plus 全域註冊 |
| `App.vue` | 根元件，僅包含 `<HomePage />` |

### API 層
| 檔案 | 說明 |
|------|------|
| `api/filesystem.js` | Axios client，base URL `http://localhost:5089/api/filesystem`，封裝所有 REST 呼叫（getTree, getTotalSize, searchByExtension, getXml, createDirectory, createFile, delete, toggleTag 等） |

### Composables
| 檔案 | 說明 |
|------|------|
| `composables/useUndoRedo.js` | **Command Pattern**：undoStack/redoStack、executeCommand(cmd)/undo()/redo()、canUndo/canRedo/undoLabel/redoLabel computed |

### Components
| 檔案 | 角色 | 說明 |
|------|------|------|
| `components/HomePage.vue` | **主 Orchestrator** | 三欄 Grid 佈局（450px / 1fr / 400px）。管理全部狀態（treeData, selectedNode, traverseLogs 等）。包含：排序邏輯（sortField/sortOrder + sortedTreeData computed 遞迴排序）、Command 包裝（handleCreate/deleteNode/handleToggleTag）、鍵盤快捷鍵（Ctrl+Z undo, Ctrl+Shift+Z redo）、loadTree 後自動選中根目錄 |
| `components/FileTree.vue` | 左側面板 | Element Plus `el-tree` 樹狀顯示，props: treeData + highlightedIds，自訂 template 顯示 icon/name/tags/info，搜尋結果高亮動畫 |
| `components/ItemInfo.vue` | 中上卡片 | 節點詳細資訊（el-descriptions）+ 標籤 toggle UI + 新增/刪除按鈕 |
| `components/VisitorOperations.vue` | 中下卡片 | 三個操作：計算大小、副檔名搜尋（帶輸入框）、XML 匯出 |
| `components/ConsolePanel.vue` | 右側面板 | 深色 Console（#1e1e1e），顯示 traverseLogs，支援下載/清除/載入 Log |
| `components/CreateItemDialog.vue` | Modal 對話框 | 建立目錄或檔案，依 fileType 動態切換欄位（Word→Pages, Image→Width/Height, Text→Encoding） |
| `components/ObserverPanel.vue` | **Observer Pattern** 浮動面板 | 右下角 fixed，watch processedNodes 顯示進度條 + 當前節點名，操作完成 10 秒後自動隱藏 |

---

## DB Schema 摘要
- **SQLite**，檔案 `filesystem.db`
- **TPH**：單一 `FileSystemItems` 表，`ItemType` 鑑別欄位（Directory/WordFile/ImageFile/TextFile）
- 自參照 FK：`ParentId → Id`（`OnDelete: Restrict`）
- 多對多：`FileSystemItemTags`（FileSystemItemId ↔ TagsId）
- 索引：ParentId, Name
