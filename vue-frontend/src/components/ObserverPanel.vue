<template>
  <el-card class="observer-card" shadow="hover">
    <template #header>
      <div class="card-header">
        <span>⚡ 監控 (Observer)</span>
        <el-tag type="info" size="small">LIVE</el-tag>
      </div>
    </template>

    <div class="observer-content">
      <div class="current-node">
        <label>目前節點</label>
        <div class="node-name">{{ currentNode || '無' }}</div>
      </div>

      <div class="progress-section">
        <label>搜尋進度</label>
        <el-progress
          :percentage="progressPercentage"
          :status="progressStatus"
          :stroke-width="20"
        />
        <div class="progress-info">{{ processedNodes }} / {{ totalNodes }} Nodes</div>
      </div>
    </div>
  </el-card>
</template>

<script setup>
import { computed, defineProps } from 'vue'

const props = defineProps({
  currentNode: {
    type: String,
    default: ''
  },
  processedNodes: {
    type: Number,
    default: 0
  },
  totalNodes: {
    type: Number,
    default: 0
  }
})

const progressPercentage = computed(() => {
  if (props.totalNodes === 0) return 0
  return Math.round((props.processedNodes / props.totalNodes) * 100)
})

const progressStatus = computed(() => {
  if (progressPercentage.value === 100) return 'success'
  return undefined
})
</script>

<style scoped>
.observer-card {
  border-radius: 8px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-weight: bold;
}

.observer-content {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.current-node label,
.progress-section label {
  display: block;
  font-size: 12px;
  color: #909399;
  margin-bottom: 8px;
}

.node-name {
  background: #ecf5ff;
  padding: 8px 12px;
  border-radius: 4px;
  font-family: 'Consolas', monospace;
  color: #409eff;
  font-weight: 500;
}

.progress-info {
  text-align: right;
  font-size: 12px;
  color: #909399;
  margin-top: 5px;
}
</style>
