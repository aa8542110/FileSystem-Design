<template>
  <transition name="observer-fade">
    <div v-if="visible" class="observer-floating">
      <div class="observer-header">
        <span>⚡ 監控 (Observer)</span>
        <button class="close-btn" @click="close">✕</button>
      </div>
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
            :stroke-width="14"
          />
          <div class="progress-info">{{ processedNodes }} / {{ totalNodes }} Nodes</div>
        </div>
      </div>
    </div>
  </transition>
</template>

<script setup>
import { ref, computed, watch, onBeforeUnmount } from 'vue'

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

const visible = ref(false)
let autoCloseTimer = null

const startAutoClose = () => {
  clearTimeout(autoCloseTimer)
  autoCloseTimer = setTimeout(() => {
    visible.value = false
  }, 10000)
}

const close = () => {
  visible.value = false
  clearTimeout(autoCloseTimer)
}

// 當有新的操作觸發時自動顯示面板
watch(
  () => props.processedNodes,
  (newVal, oldVal) => {
    if (newVal !== oldVal) {
      visible.value = true
      startAutoClose()
    }
  }
)

watch(
  () => props.currentNode,
  (newVal, oldVal) => {
    if (newVal && newVal !== oldVal) {
      visible.value = true
      startAutoClose()
    }
  }
)

onBeforeUnmount(() => {
  clearTimeout(autoCloseTimer)
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
.observer-floating {
  position: fixed;
  bottom: 20px;
  right: 20px;
  width: 280px;
  background: white;
  border-radius: 10px;
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.15);
  z-index: 1000;
  overflow: hidden;
}

.observer-header {
  padding: 10px 14px;
  font-weight: bold;
  font-size: 13px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.close-btn {
  background: none;
  border: none;
  color: rgba(255, 255, 255, 0.8);
  font-size: 14px;
  cursor: pointer;
  padding: 0 2px;
  line-height: 1;
}

.close-btn:hover {
  color: white;
}

/* 淡入淡出動畫 */
.observer-fade-enter-active,
.observer-fade-leave-active {
  transition: opacity 0.3s ease, transform 0.3s ease;
}

.observer-fade-enter-from,
.observer-fade-leave-to {
  opacity: 0;
  transform: translateY(10px);
}

.observer-content {
  padding: 12px 14px;
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.current-node label,
.progress-section label {
  display: block;
  font-size: 11px;
  color: #909399;
  margin-bottom: 4px;
}

.node-name {
  background: #ecf5ff;
  padding: 6px 10px;
  border-radius: 4px;
  font-family: 'Consolas', monospace;
  color: #409eff;
  font-weight: 500;
  font-size: 13px;
  word-break: break-all;
}

.progress-info {
  text-align: right;
  font-size: 11px;
  color: #909399;
  margin-top: 3px;
}
</style>
