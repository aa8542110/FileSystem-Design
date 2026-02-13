<template>
  <el-card v-if="selectedNode" class="info-card" shadow="hover">
    <template #header>
      <div class="card-header">
        <span>ℹ️ 項目資訊</span>
        <el-button type="danger" size="small" @click="$emit('delete')">刪除</el-button>
      </div>
    </template>

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
  </el-card>
</template>

<script setup>
import { defineProps, defineEmits } from 'vue'

const props = defineProps({
  selectedNode: {
    type: Object,
    default: null
  }
})

const emit = defineEmits(['delete'])

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
</style>
