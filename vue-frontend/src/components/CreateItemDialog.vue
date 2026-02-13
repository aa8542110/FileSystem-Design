<template>
  <el-dialog
    v-model="visible"
    :title="dialogTitle"
    width="500px"
    @close="handleClose"
  >
    <el-form :model="form" label-width="100px">
      <el-form-item label="名稱">
        <el-input v-model="form.name" placeholder="請輸入名稱" />
      </el-form-item>

      <el-form-item label="父目錄">
        <el-select v-model="form.parentId" placeholder="選擇父目錄" filterable>
          <el-option
            v-for="dir in directories"
            :key="dir.id"
            :label="dir.path"
            :value="dir.id"
          />
        </el-select>
      </el-form-item>

      <template v-if="form.type === 'file'">
        <el-form-item label="檔案類型">
          <el-select v-model="form.fileType">
            <el-option label="Word 文件" value="word" />
            <el-option label="圖片" value="image" />
            <el-option label="純文字" value="text" />
          </el-select>
        </el-form-item>

        <el-form-item label="大小 (KB)">
          <el-input-number v-model="form.size" :min="0" />
        </el-form-item>

        <!-- Word 專用 -->
        <el-form-item label="頁數" v-if="form.fileType === 'word'">
          <el-input-number v-model="form.pages" :min="1" />
        </el-form-item>

        <!-- Image 專用 -->
        <template v-if="form.fileType === 'image'">
          <el-form-item label="寬度">
            <el-input-number v-model="form.width" :min="1" />
          </el-form-item>
          <el-form-item label="高度">
            <el-input-number v-model="form.height" :min="1" />
          </el-form-item>
        </template>

        <!-- Text 專用 -->
        <el-form-item label="編碼" v-if="form.fileType === 'text'">
          <el-select v-model="form.encoding">
            <el-option label="UTF-8" value="UTF-8" />
            <el-option label="ASCII" value="ASCII" />
            <el-option label="Big5" value="Big5" />
          </el-select>
        </el-form-item>
      </template>
    </el-form>

    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleSubmit">確定</el-button>
    </template>
  </el-dialog>
</template>

<script setup>
import { ref, computed, watch, defineProps, defineEmits } from 'vue'

const props = defineProps({
  modelValue: {
    type: Boolean,
    default: false
  },
  formData: {
    type: Object,
    required: true
  },
  rootId: {
    type: String,
    default: null
  },
  directories: {
    type: Array,
    default: () => []
  }
})

const emit = defineEmits(['update:modelValue', 'submit'])

const visible = ref(props.modelValue)
const form = ref({ ...props.formData })

watch(() => props.modelValue, (newVal) => {
  visible.value = newVal
})

watch(visible, (newVal) => {
  emit('update:modelValue', newVal)
})

watch(() => props.formData, (newVal) => {
  form.value = { ...newVal }
}, { deep: true })

const dialogTitle = computed(() => {
  return form.value.type === 'directory' ? '新增目錄' : '新增檔案'
})

const handleClose = () => {
  visible.value = false
}

const handleSubmit = () => {
  emit('submit', form.value)
  visible.value = false
}
</script>

<style scoped>
/* 對話框樣式由 Element Plus 提供 */
</style>
