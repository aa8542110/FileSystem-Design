import axios from 'axios'

const API_BASE_URL = 'http://localhost:5089/api/filesystem'

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json'
  }
})

export default {
  // 取得完整目錄樹
  getTree() {
    return apiClient.get('/tree')
  },

  // 取得單一項目
  getById(id) {
    return apiClient.get(`/${id}`)
  },

  // 計算目錄總大小（遞迴）
  getTotalSize(id) {
    return apiClient.get(`/${id}/size`)
  },

  // 副檔名搜尋
  searchByExtension(extension) {
    return apiClient.get('/search/extension', {
      params: { ext: extension }
    })
  },

  // 取得 XML 結構
  getXml(id) {
    return apiClient.get(`/${id}/xml`)
  },

  // 取得遍歷日誌
  getTraverseLog(id, operation = 'Traverse') {
    return apiClient.get(`/${id}/traverse-log`, {
      params: { operation }
    })
  },

  // 取得 Console 輸出
  getConsoleOutput() {
    return apiClient.get('/console')
  },

  // 建立目錄
  createDirectory(data) {
    return apiClient.post('/directory', data)
  },

  // 建立檔案
  createFile(data) {
    return apiClient.post('/file', data)
  },

  // 重新命名
  rename(id, newName) {
    return apiClient.put(`/${id}/rename`, { newName })
  },

  // 刪除項目
  delete(id) {
    return apiClient.delete(`/${id}`)
  },

  // 取得所有標籤
  getAllTags() {
    return apiClient.get('/tags')
  },

  // 切換標籤
  toggleTag(itemId, tagId) {
    return apiClient.post(`/${itemId}/tags/${tagId}`)
  }
}
