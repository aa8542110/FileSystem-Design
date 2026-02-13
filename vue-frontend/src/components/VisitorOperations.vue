<template>
  <el-card class="visitor-card" shadow="hover">
    <template #header>
      <div class="card-header">
        <span>👤 訪問者操作 (Visitor)</span>
      </div>
    </template>

    <!-- 計算大小 -->
    <div class="operation-section">
      <div class="operation-header">
        <h4>計算大小</h4>
        <el-button
          type="primary"
          size="small"
          @click="$emit('calculate-size')"
          :disabled="!selectedNode || selectedNode.itemType !== 'Directory'"
        >
          執行
        </el-button>
      </div>
    </div>

    <el-divider />

    <!-- 搜尋功能 -->
    <div class="operation-section">
      <div class="operation-header">
        <h4>副檔名搜尋</h4>
      </div>
      <el-input
        v-model="localSearchExtension"
        placeholder=".docx"
        size="small"
        @keyup.enter="handleSearch"
      >
        <template #suffix>
          <el-button
            type="primary"
            size="small"
            text
            @click="handleSearch"
          >
            搜尋 🔍
          </el-button>
        </template>
      </el-input>
    </div>
  </el-card>
</template>

<script setup>
import { ref, defineProps, defineEmits } from 'vue'

const props = defineProps({
  selectedNode: {
    type: Object,
    default: null
  },
  searchExtension: {
    type: String,
    default: '.docx'
  }
})

const emit = defineEmits(['calculate-size', 'search'])

const localSearchExtension = ref(props.searchExtension)

const handleSearch = () => {
  emit('search', localSearchExtension.value)
}
</script>

<style scoped>
.visitor-card {
  border-radius: 8px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-weight: bold;
}

.operation-section {
  margin-bottom: 15px;
}

.operation-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 10px;
}

.operation-header h4 {
  margin: 0;
  font-size: 14px;
  color: #606266;
}
</style>
