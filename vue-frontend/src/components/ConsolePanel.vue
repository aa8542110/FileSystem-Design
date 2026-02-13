<template>
  <el-card class="console-card" shadow="always">
    <template #header>
      <div class="card-header">
        <span>🖥️ CONSOLE</span>
        <el-badge :value="logs.length" type="primary" />
      </div>
    </template>

    <div class="console-content">
      <div
        v-for="(log, index) in logs"
        :key="index"
        class="console-line"
        :class="{ 'highlight': log.includes(currentNode) }"
      >
        {{ log }}
      </div>
      <div v-if="logs.length === 0" class="empty-console">
        等待操作...
      </div>
    </div>

    <div class="console-footer">
      <el-button size="small" type="primary" text @click="$emit('load-log')">
        載入遍歷日誌
      </el-button>
      <el-button size="small" text @click="$emit('download')" v-if="logs.length > 0">
        下載
      </el-button>
      <el-button size="small" text @click="$emit('clear')">
        清空
      </el-button>
    </div>
  </el-card>
</template>

<script setup>
import { defineProps, defineEmits } from 'vue'

const props = defineProps({
  logs: {
    type: Array,
    default: () => []
  },
  currentNode: {
    type: String,
    default: ''
  }
})

const emit = defineEmits(['load-log', 'download', 'clear'])
</script>

<style scoped>
.console-card {
  height: 100%;
  display: flex;
  flex-direction: column;
  border-radius: 8px;
  background: #1e1e1e;
  color: #d4d4d4;
}

.console-card :deep(.el-card__header) {
  background: #2d2d2d;
  color: #d4d4d4;
  border-bottom: 1px solid #3e3e3e;
}

.console-card :deep(.el-card__body) {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-weight: bold;
}

.console-content {
  flex: 1;
  overflow-y: auto;
  font-family: 'Consolas', 'Monaco', monospace;
  font-size: 12px;
  line-height: 1.6;
  padding: 10px;
}

.console-line {
  padding: 2px 0;
  color: #d4d4d4;
  text-align: left;
}

.console-line.highlight {
  background: #264f78;
  color: #4ec9b0;
  padding: 2px 4px;
  border-radius: 2px;
}

.empty-console {
  color: #6a9955;
  text-align: center;
  padding: 20px;
}

.console-footer {
  border-top: 1px solid #3e3e3e;
  padding: 10px;
  display: flex;
  justify-content: space-between;
}

.console-content::-webkit-scrollbar {
  width: 8px;
}

.console-content::-webkit-scrollbar-track {
  background: #2d2d2d;
}

.console-content::-webkit-scrollbar-thumb {
  background: #555;
  border-radius: 4px;
}

.console-content::-webkit-scrollbar-thumb:hover {
  background: #666;
}
</style>
