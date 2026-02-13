<template>
  <div class="file-tree">
    <el-button type="primary" @click="$emit('refresh')" style="margin-bottom: 10px;">
      重新載入
    </el-button>

    <el-tree
      v-if="treeData"
      :data="[treeData]"
      :props="treeProps"
      node-key="id"
      default-expand-all
      @node-click="handleNodeClick"
    >
      <template #default="{ node, data }">
        <span class="custom-tree-node" :class="{ 'highlighted-node': isHighlighted(data.id) }">
          <span class="node-icon">{{ getIcon(data.itemType) }}</span>
          <span class="node-label">{{ data.name }}</span>
          <span class="node-info">{{ getNodeInfo(data) }}</span>
        </span>
      </template>
    </el-tree>

    <el-empty v-else description="無資料" />
  </div>
</template>

<script setup>
import { defineProps, defineEmits } from 'vue'

const props = defineProps({
  treeData: {
    type: Object,
    default: null
  },
  highlightedIds: {
    type: Array,
    default: () => []
  }
})

const emit = defineEmits(['node-click', 'refresh'])

const treeProps = {
  children: 'items',
  label: 'name'
}

const handleNodeClick = (data) => {
  emit('node-click', data)
}

const getIcon = (type) => {
  const icons = {
    'Directory': '📁',
    'WordFile': '📄',
    'ImageFile': '🖼️',
    'TextFile': '📝'
  }
  return icons[type] || '📦'
}

const getNodeInfo = (data) => {
  if (data.itemType === 'Directory') {
    return `[目錄] (${data.totalSize.toFixed(2)} KB)`
  } else if (data.itemType === 'WordFile') {
    return `[Word] (頁數: ${data.pages}, ${data.size} KB)`
  } else if (data.itemType === 'ImageFile') {
    return `[圖片] (${data.width}x${data.height}, ${data.size} KB)`
  } else if (data.itemType === 'TextFile') {
    return `[文字] (${data.encoding}, ${data.size} KB)`
  }
  return `(${data.size} KB)`
}

const isHighlighted = (nodeId) => {
  return props.highlightedIds.includes(nodeId)
}
</script>

<style scoped>
.file-tree {
  padding: 10px;
}

.custom-tree-node {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 14px;
}

.node-icon {
  font-size: 18px;
}

.node-label {
  font-weight: 500;
}

.node-info {
  color: #909399;
  font-size: 12px;
  margin-left: auto;
}

:deep(.el-tree-node__content) {
  height: 36px;
  padding: 4px 0;
}

:deep(.el-tree-node__content:hover) {
  background-color: #f5f7fa;
}

/* 高亮搜尋到的檔案 */
.highlighted-node {
  background: linear-gradient(90deg, #fef0cd 0%, #fff9e6 100%);
  border-left: 3px solid #e6a23c;
  padding-left: 5px;
  font-weight: 600;
  animation: highlight-pulse 1.5s ease-in-out infinite;
}

@keyframes highlight-pulse {
  0%, 100% {
    box-shadow: 0 0 5px rgba(230, 162, 60, 0.3);
  }
  50% {
    box-shadow: 0 0 15px rgba(230, 162, 60, 0.6);
  }
}
</style>
