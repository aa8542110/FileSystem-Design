<template>
  <div class="file-system-container">
    <!-- 頂部標題 -->
    <div class="header">
      <h1>雲端檔案管理系統 - Design Pattern 實作</h1>
    </div>

    <!-- 主要內容區 -->
    <div class="main-content">
      <!-- 左側：檔案樹 -->
      <div class="left-panel">
        <div class="toolbar">
          <el-button size="small" @click="showSortMenu = !showSortMenu">
            名稱 ▼
          </el-button>
          <el-button size="small">大小</el-button>
          <el-button size="small">類型</el-button>
          <el-button size="small">排序</el-button>
        </div>

        <div class="tree-section">
          <h3>📦 檔案階層 (Composite)</h3>
          <FileTree
            :tree-data="treeData"
            :highlighted-ids="highlightedFileIds"
            @node-click="handleNodeClick"
            @refresh="loadTree"
          />
        </div>
      </div>

      <!-- 中間：訪問者操作與監控 -->
      <div class="middle-panel">
        <ItemInfo
          :selected-node="selectedNode"
          :all-tags="allTags"
          @delete="deleteNode"
          @create-directory="showCreateDialog('directory')"
          @create-file="showCreateDialog('file')"
          @toggle-tag="handleToggleTag"
        />

        <VisitorOperations
          :selected-node="selectedNode"
          :search-extension="searchExtension"
          @calculate-size="calculateSize"
          @search="handleSearch"
          @export-xml="exportXmlStructure"
        />

      </div>

      <!-- 右側：Console 面板 -->
      <div class="right-panel">
        <ConsolePanel
          :logs="traverseLogs"
          :current-node="currentProcessingNode"
          @load-log="loadTraverseLog"
          @download="downloadConsoleContent"
          @clear="clearConsole"
        />
      </div>
    </div>

    <!-- 右下角懸浮監控面板 -->
    <ObserverPanel
      :current-node="currentProcessingNode"
      :processed-nodes="processedNodes"
      :total-nodes="totalNodes"
    />

    <!-- 新增/編輯對話框 -->
    <CreateItemDialog
      v-model="createDialogVisible"
      :form-data="createForm"
      :root-id="rootId"
      :directories="availableDirectories"
      @submit="handleCreate"
    />
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import FileTree from './FileTree.vue'
import VisitorOperations from './VisitorOperations.vue'
import ObserverPanel from './ObserverPanel.vue'
import ItemInfo from './ItemInfo.vue'
import ConsolePanel from './ConsolePanel.vue'
import CreateItemDialog from './CreateItemDialog.vue'
import filesystemApi from '../api/filesystem'

// State
const treeData = ref(null)
const selectedNode = ref(null)
const traverseLogs = ref([])
const currentProcessingNode = ref('')
const processedNodes = ref(0)
const totalNodes = ref(0)
const searchExtension = ref('.docx')
const allTags = ref([])
const showSortMenu = ref(false)
const createDialogVisible = ref(false)
const createForm = ref({
  type: 'directory',
  name: '',
  parentId: null,
  fileType: 'word',
  size: 0,
  pages: 1,
  width: 1920,
  height: 1080,
  encoding: 'UTF-8'
})

const rootId = computed(() => treeData.value?.id || null)

// 收集所有目錄（遞迴）
const collectDirectories = (node, path = '') => {
  const directories = []

  if (!node) return directories

  if (node.itemType === 'Directory') {
    const currentPath = path ? `${path}/${node.name}` : node.name
    directories.push({
      id: node.id,
      name: node.name,
      path: currentPath
    })

    // 遞迴收集子目錄
    if (node.items && node.items.length > 0) {
      node.items.forEach(item => {
        if (item.itemType === 'Directory') {
          directories.push(...collectDirectories(item, currentPath))
        }
      })
    }
  }

  return directories
}

// 所有可用的目錄列表
const availableDirectories = computed(() => {
  return collectDirectories(treeData.value)
})

// 載入所有標籤
const loadTags = async () => {
  try {
    const response = await filesystemApi.getAllTags()
    allTags.value = response.data
  } catch (error) {
    console.error('載入標籤失敗:', error)
  }
}

// 切換標籤
const handleToggleTag = async (itemId, tagId) => {
  try {
    await filesystemApi.toggleTag(itemId, tagId)
    await loadTree()
    // 重新選取同一節點以更新 ItemInfo 顯示
    if (selectedNode.value) {
      const findNode = (node, id) => {
        if (node.id === id) return node
        if (node.items) {
          for (const child of node.items) {
            const found = findNode(child, id)
            if (found) return found
          }
        }
        return null
      }
      selectedNode.value = findNode(treeData.value, itemId)
    }
  } catch (error) {
    console.error('切換標籤失敗:', error)
    ElMessage.error('切換標籤失敗')
  }
}

// 載入目錄樹
const loadTree = async () => {
  try {
    const response = await filesystemApi.getTree()
    treeData.value = response.data
    highlightedFileIds.value = []  // 清除高亮

    // 在 Console 面板輸出樹狀結構
    const consoleRes = await filesystemApi.getConsoleOutput()
    if (consoleRes.data?.output) {
      traverseLogs.value = consoleRes.data.output.split('\n').filter(line => line.trim() !== '')
      totalNodes.value = traverseLogs.value.length
      processedNodes.value = traverseLogs.value.length
    }

    ElMessage.success('目錄樹載入成功')
  } catch (error) {
    console.error('載入目錄樹失敗:', error)
    ElMessage.error('載入目錄樹失敗')
  }
}

// 處理節點點擊
const handleNodeClick = (node) => {
  selectedNode.value = node
  currentProcessingNode.value = node.name
}

// 計算大小
const calculateSize = async () => {
  if (!selectedNode.value) return

  try {
    processedNodes.value = 0
    totalNodes.value = 0
    traverseLogs.value = []

    const response = await filesystemApi.getTotalSize(selectedNode.value.id)

    traverseLogs.value = response.data.traverseLog || []
    totalNodes.value = traverseLogs.value.length

    for (let i = 0; i < totalNodes.value; i++) {
      await new Promise(resolve => setTimeout(resolve, 100))
      processedNodes.value = i + 1
      currentProcessingNode.value = traverseLogs.value[i].replace('Visiting: ', '')
    }

    ElMessage.success(`總大小: ${response.data.totalSize} KB`)
    selectedNode.value.totalSize = response.data.totalSize
  } catch (error) {
    console.error('計算大小失敗:', error)
    ElMessage.error('計算大小失敗')
  }
}

// 格式化 XML
const formatXml = (xmlString) => {
  const PADDING = '  '
  const reg = /(>)(<)(\/*)/g
  let formatted = ''
  let pad = 0

  xmlString = xmlString.replace(reg, '$1\n$2$3')

  xmlString.split('\n').forEach((line) => {
    let indent = 0
    if (line.match(/.+<\/\w[^>]*>$/)) {
      indent = 0
    } else if (line.match(/^<\/\w/)) {
      if (pad !== 0) pad -= 1
    } else if (line.match(/^<\w([^>]*[^\/])?>.*$/)) {
      indent = 1
    } else {
      indent = 0
    }

    formatted += PADDING.repeat(pad) + line + '\n'
    pad += indent
  })

  return formatted.trim()
}

// 匯出 XML
const exportXmlStructure = async () => {
  if (!treeData.value) {
    ElMessage.warning('請先載入目錄樹')
    return
  }

  try {
    processedNodes.value = 0
    totalNodes.value = 0
    currentProcessingNode.value = '正在生成 XML...'

    const response = await filesystemApi.getXml(treeData.value.id)
    traverseLogs.value = []

    let xmlContent = response.data
    try {
      xmlContent = formatXml(xmlContent)
    } catch (e) {
      console.warn('XML 格式化失敗，使用原始格式', e)
    }

    const xmlLines = xmlContent.split('\n').filter(line => line.trim() !== '')
    totalNodes.value = xmlLines.length

    for (let i = 0; i < xmlLines.length; i++) {
      await new Promise(resolve => setTimeout(resolve, 30))
      traverseLogs.value.push(xmlLines[i])
      processedNodes.value = i + 1

      const openTagMatch = xmlLines[i].match(/<([^/>!\?][^>\s]*)/)
      if (openTagMatch) {
        currentProcessingNode.value = openTagMatch[1].replace(/_/g, '.')
      }
    }

    currentProcessingNode.value = 'XML 輸出完成'
    ElMessage.success(`XML 結構已輸出到 Console (共 ${xmlLines.length} 行)`)
  } catch (error) {
    console.error('匯出 XML 失敗:', error)
    ElMessage.error('匯出 XML 失敗')
  }
}

// 搜尋結果高亮的檔案 ID 列表
const highlightedFileIds = ref([])

// 搜尋
const handleSearch = async (extension) => {
  if (!extension) return

  try {
    processedNodes.value = 0
    traverseLogs.value = []
    highlightedFileIds.value = []

    const response = await filesystemApi.searchByExtension(extension)

    // 簡單格式顯示搜尋結果
    traverseLogs.value = [
      `🔍 副檔名搜尋結果: .${extension}`,
      `找到 ${response.data.count} 個檔案`,
      '─────────────────────────────────'
    ]

    if (response.data.count > 0) {
      response.data.paths.forEach((path, index) => {
        traverseLogs.value.push(`${index + 1}. ${path}`)
      })

      // 提取搜尋到的檔案 ID 用於高亮顯示
      highlightedFileIds.value = response.data.fileIds || []
    } else {
      traverseLogs.value.push('沒有找到符合的檔案')
    }

    totalNodes.value = response.data.count
    processedNodes.value = response.data.count

    ElMessage.success(`找到 ${response.data.count} 個檔案，已在檔案樹中高亮`)
  } catch (error) {
    console.error('搜尋失敗:', error)
    ElMessage.error('搜尋失敗')
  }
}

// 載入遍歷日誌
const loadTraverseLog = async (nodeId) => {
  try {
    const id = nodeId || (treeData.value ? treeData.value.id : null)
    if (!id) return

    const response = await filesystemApi.getTraverseLog(id, 'Traverse')
    traverseLogs.value = response.data.logs
    totalNodes.value = traverseLogs.value.length
    processedNodes.value = traverseLogs.value.length
  } catch (error) {
    console.error('載入遍歷日誌失敗:', error)
  }
}

// 清空 Console
const clearConsole = () => {
  traverseLogs.value = []
  processedNodes.value = 0
  totalNodes.value = 0
  currentProcessingNode.value = ''
  highlightedFileIds.value = []  // 清除高亮
}

// 下載 Console 內容
const downloadConsoleContent = () => {
  if (traverseLogs.value.length === 0) return

  const content = traverseLogs.value.join('\n')
  const blob = new Blob([content], { type: 'text/plain;charset=utf-8' })
  const url = URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url

  const isXml = content.includes('<') && content.includes('>')
  a.download = isXml ? 'filesystem-structure.xml' : 'console-output.txt'

  a.click()
  URL.revokeObjectURL(url)

  ElMessage.success('已下載')
}

// 顯示新增對話框
const showCreateDialog = (type) => {
  createForm.value.type = type
  createForm.value.parentId = selectedNode.value?.id || rootId.value
  createDialogVisible.value = true
}

// 建立項目
const handleCreate = async (formData) => {
  try {
    if (formData.type === 'directory') {
      await filesystemApi.createDirectory({
        name: formData.name,
        parentId: formData.parentId
      })
    } else {
      // 根據檔案類型組裝對應的 DTO（Factory Pattern - 前端帶 $type discriminator）
      const payload = {
        $type: formData.fileType,  // "word" | "image" | "text"
        name: formData.name,
        size: formData.size,
        parentId: formData.parentId
      }

      // 依類型附加專屬屬性
      switch (formData.fileType) {
        case 'word':
          payload.pages = formData.pages
          break
        case 'image':
          payload.width = formData.width
          payload.height = formData.height
          break
        case 'text':
          payload.encoding = formData.encoding
          break
      }

      await filesystemApi.createFile(payload)
    }

    ElMessage.success('建立成功')
    await loadTree()
  } catch (error) {
    console.error('建立失敗:', error)
    ElMessage.error('建立失敗')
  }
}

// 刪除節點
const deleteNode = async () => {
  if (!selectedNode.value) return

  try {
    await ElMessageBox.confirm(
      `確定要刪除 "${selectedNode.value.name}" 嗎？`,
      '警告',
      {
        confirmButtonText: '確定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )

    await filesystemApi.delete(selectedNode.value.id)
    ElMessage.success('刪除成功')
    selectedNode.value = null
    await loadTree()
  } catch (error) {
    if (error !== 'cancel') {
      console.error('刪除失敗:', error)
      ElMessage.error('刪除失敗')
    }
  }
}

// 初始化
onMounted(() => {
  loadTags()
  loadTree()
})
</script>

<style scoped>
.file-system-container {
  height: 100vh;
  display: flex;
  flex-direction: column;
  background-color: #f0f2f5;
}

.header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 20px;
  text-align: center;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
}

.header h1 {
  margin: 0;
  font-size: 24px;
}

.main-content {
  flex: 1;
  display: grid;
  grid-template-columns: 450px 1fr 400px;
  gap: 15px;
  padding: 15px;
  overflow: hidden;
}

/* 左側面板 */
.left-panel {
  display: flex;
  flex-direction: column;
  gap: 10px;
  overflow: hidden;
}

.toolbar {
  background: white;
  padding: 10px;
  border-radius: 8px;
  display: flex;
  gap: 5px;
  flex-wrap: wrap;
  box-shadow: 0 2px 4px rgba(0,0,0,0.05);
}

.tree-section {
  flex: 1;
  background: white;
  border-radius: 8px;
  padding: 15px;
  overflow-y: auto;
  box-shadow: 0 2px 4px rgba(0,0,0,0.05);
}

.tree-section h3 {
  margin: 0 0 15px 0;
  font-size: 16px;
  color: #303133;
}

/* 中間面板 */
.middle-panel {
  display: flex;
  flex-direction: column;
  gap: 15px;
  overflow-y: auto;
}

/* 右側面板 */
.right-panel {
  overflow: hidden;
}

/* 滾動條美化 */
::-webkit-scrollbar {
  width: 8px;
  height: 8px;
}

::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 4px;
}

::-webkit-scrollbar-thumb {
  background: #c1c1c1;
  border-radius: 4px;
}

::-webkit-scrollbar-thumb:hover {
  background: #a8a8a8;
}
</style>
