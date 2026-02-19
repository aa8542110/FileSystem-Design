### 後端
```bash
cd WinbondProj
dotnet run
# 預設啟動於 http://localhost:5089
# Swagger UI: http://localhost:5089/swagger
```

### 前端
```bash
cd vue-frontend
npm install
npm run dev
# 預設啟動於 http://localhost:5173
```

首次啟動後端時，`DbInitializer` 會自動建立 SQLite 資料庫並 Seed 範例資料。
