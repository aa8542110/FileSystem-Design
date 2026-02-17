<template>
  <el-card class="visitor-card" shadow="hover">
    <template #header>
      <div class="card-header">
        <span>👤 訪問者操作 (Visitor)</span>
      </div>
    </template>

    <!-- 計算大小 -->
    <el-button
      class="operation-btn"
      type="primary"
      @click="$emit('calculate-size')"
      :disabled="!selectedNode || selectedNode.itemType !== 'Directory'"
    >
      📊 計算目錄總大小
    </el-button>

    <!-- 匯出 XML -->
    <el-button
      class="operation-btn"
      type="warning"
      @click="$emit('export-xml')"
    >
      📄 匯出 XML 結構
    </el-button>

    <!-- 副檔名搜尋 -->
    <div class="search-section">
      <el-input
        v-model="localSearchExtension"
        placeholder=".docx"
        size="default"
        @keyup.enter="handleSearch"
      >
        <template #prepend>🔍 副檔名搜尋</template>
        <template #append>
          <el-button @click="handleSearch">搜尋</el-button>
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

const emit = defineEmits(['calculate-size', 'search', 'export-xml'])

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

.operation-btn {
  width: 100%;
  margin-bottom: 10px;
  margin-left: 0;
}

.search-section {
  margin-top: 2px;
}
</style>
