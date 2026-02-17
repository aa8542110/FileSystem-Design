<template>
  <el-card class="info-card" shadow="hover">
    <template #header>
      <div class="card-header">
        <span>ℹ️ 項目資訊</span>
        <div v-if="selectedNode" class="header-actions">
          <template v-if="selectedNode.itemType === 'Directory'">
            <el-button type="primary" size="small" @click="$emit('create-directory')">📁 新增目錄</el-button>
            <el-button size="small" @click="$emit('create-file')">📄 新增檔案</el-button>
          </template>
          <el-button type="danger" size="small" @click="$emit('delete')">刪除</el-button>
        </div>
      </div>
    </template>

    <template v-if="selectedNode">
      <el-descriptions :column="2" size="small" border>
        <el-descriptions-item label="名稱">{{ selectedNode.name }}</el-descriptions-item>
        <el-descriptions-item label="類型">{{ getItemTypeLabel(selectedNode.itemType) }}</el-descriptions-item>
        <el-descriptions-item label="大小">{{ getDisplaySize(selectedNode) }} KB</el-descriptions-item>
        <el-descriptions-item label="建立時間">{{ formatDate(selectedNode.createdDate) }}</el-descriptions-item>

        <!-- 特殊屬性 -->
        <el-descriptions-item label="頁數" v-if="selectedNode.pages">
          {{ selectedNode.pages }}
        </el-descriptions-item>
        <el-descriptions-item label="解析度" v-if="selectedNode.width && selectedNode.height">
          {{ selectedNode.width }} x {{ selectedNode.height }}
        </el-descriptions-item>
        <el-descriptions-item label="編碼" v-if="selectedNode.encoding">
          {{ selectedNode.encoding }}
        </el-descriptions-item>
      </el-descriptions>

      <!-- 標籤管理 -->
      <div v-if="allTags && allTags.length" class="tag-section">
        <span class="tag-label">標籤：</span>
        <el-tag
          v-for="tag in allTags"
          :key="tag.id"
          :color="isTagActive(tag.id) ? tag.color : ''"
          :effect="isTagActive(tag.id) ? 'dark' : 'plain'"
          :style="!isTagActive(tag.id) ? { borderColor: tag.color, color: tag.color } : { borderColor: tag.color }"
          class="tag-toggle"
          @click="$emit('toggle-tag', selectedNode.id, tag.id)"
        >
          {{ tag.name }}
        </el-tag>
      </div>
    </template>

    <template v-else>
      <el-empty description="請在左側點選一個項目" :image-size="60" />
    </template>
  </el-card>
</template>

<script setup>
import { defineProps, defineEmits } from 'vue'

const props = defineProps({
  selectedNode: {
    type: Object,
    default: null
  },
  allTags: {
    type: Array,
    default: () => []
  }
})

const emit = defineEmits(['delete', 'create-directory', 'create-file', 'toggle-tag'])

const isTagActive = (tagId) => {
  return props.selectedNode?.tags?.some(t => t.id === tagId) || false
}

const tagStyle = (tag, active) => {
  if (active) {
    return {
      backgroundColor: tag.color,
      borderColor: tag.color,
      color: '#fff'
    }
  }
  return {
    borderColor: tag.color,
    color: tag.color
  }
}

const formatDate = (dateString) => {
  return new Date(dateString).toLocaleString('zh-TW')
}

const getItemTypeLabel = (type) => {
  const labels = {
    'Directory': '目錄',
    'WordFile': 'Word 文件',
    'ImageFile': '圖片',
    'TextFile': '純文字檔'
  }
  return labels[type] || type
}

const getDisplaySize = (node) => {
  if (node.itemType === 'Directory') {
    return node.totalSize.toFixed(2)
  }
  return node.size.toFixed(2)
}
</script>

<style scoped>
.info-card {
  border-radius: 8px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-weight: bold;
}

.header-actions {
  display: flex;
  gap: 5px;
}

.directory-actions {
  margin-top: 12px;
  display: flex;
  align-items: center;
  gap: 8px;
}

.actions-label {
  font-size: 13px;
  color: #606266;
}

.tag-section {
  margin-top: 12px;
  display: flex;
  align-items: center;
  gap: 8px;
}

.tag-label {
  font-size: 13px;
  color: #606266;
  white-space: nowrap;
}

.tag-toggle {
  cursor: pointer;
}
</style>
