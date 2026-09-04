# Swagger2Doc

將 swagger.json（OpenAPI）轉成 **Markdown / HTML / Word（.docx）** 三種 API 規格文件。

## 使用方式

```powershell
# 1. 重新編譯（產出 publish\Swagger2Doc.exe）
.\compile.ps1

# 2. 轉檔
cd publish

# 預設：讀 Resources\swagger.json，輸出到 Resources\Output\swagger.{md,html,docx}
.\Swagger2Doc.exe

# 指定輸入檔，輸出到指定資料夾（檔名沿用輸入檔名）
.\Swagger2Doc.exe "D:\spec\my-api.json" -o "D:\out"

# 一次轉多個檔，或直接給資料夾（轉換其中所有 .json）
.\Swagger2Doc.exe "D:\spec\api.json" "D:\spec\web.json" -o "D:\out"
.\Swagger2Doc.exe "D:\spec" -o "D:\out"

# 自訂輸出檔名（僅單一輸入時有效）
.\Swagger2Doc.exe "D:\spec\my-api.json" -o "D:\out" -n "API規格書"
```

| 選項 | 說明 |
| --- | --- |
| `-o`, `--output` | 輸出資料夾（預設 `Resources\Output`） |
| `-n`, `--name` | 輸出檔名，不含副檔名（僅單一輸入時有效） |
| `-h`, `--help` | 顯示說明 |

## 文件結構

三種格式內容一致，章節順序：

1. **封面** — 標題、版本、產生日期、服務說明、服務位址（servers）、聯絡窗口與授權
2. **文件索引** — 全部端點一覽（編號、方法、路徑、說明），編號與內文章節一致；Markdown/HTML 可點擊跳轉
3. **安全性驗證** — securitySchemes（名稱、參數、類型、位置、說明）
4. **API 明細** — 每支一節：說明、分類（tags）、operationId、是否淘汰、請求參數（Path/Query/Header）、請求內容、回應狀態與回應內容，各含欄位表與範例 JSON
5. **資料模型** — components.schemas 的欄位表

## 支援的 OpenAPI 內容

- 參數位置（path / query / header / cookie）與必填標示
- `$ref` 模型名稱；`allOf` / `oneOf` / `anyOf` 包裝會解開取得型別（Swashbuckle 對可為 null 的參考型別常見寫法）
- 陣列顯示為 `array<T>`
- 欄位限制：列舉值、預設值、`pattern`、數值範圍、長度、`nullable`、`readOnly`、`deprecated`
- 多狀態碼回應，逐一列出說明、內容型別、欄位表與範例
- PathItem 層級的共用參數會併入各 operation
- 描述文字中的 HTML 標籤（`<br>`、`<strong>`、`<code>` 等）會自動清除，不會在 Word 裡顯示成字面標籤
- 範例 JSON 具循環引用保護與深度上限；同層重複引用不會被誤判為循環

## 版面

- **Word**：A4、上下 2cm／左右 1.8cm；標題 20pt、章節 15pt、單支 API 12.5pt、小節 10.5pt；表格滿版、全框線、表頭灰底並跨頁重複標題列；頁尾頁碼；每支 API 獨立一頁；標題套 Word 大綱層級，可用「功能窗格」導覽
- **HTML**：樣式內嵌，不依賴外部 CDN，離線可正常顯示；表格全框線，並含列印樣式
- **Markdown**：標準表格語法，索引以錨點連結

## 專案結構

```
Services/
├── ConvertService.cs     轉檔主流程與命令列參數
├── ApiEntry.cs           攤平 path × method，統一編號與順序
├── OpenApiText.cs        型別／限制／描述清洗／範例 JSON 等共用轉換
├── Convert2MarkDown.cs   Markdown 產生器
└── Convert2Docx.cs       Word 產生器（版面設定集中於此）
Resources/
├── swagger.json                 預設輸入
├── swagger_html_template.txt    HTML 樣板（含內嵌樣式）
└── Output/                      預設輸出
```
