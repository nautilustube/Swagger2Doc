using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using System.Text;

namespace Swagger2Doc.Services
{
    /// <summary>
    /// OpenApiDocument 轉 Markdown：封面資訊 → 服務位址 → 安全性驗證 → API 目錄 → API 明細 → 資料模型
    /// </summary>
    public class Convert2MarkDown
    {
        public string CreateMD(OpenApiDocument openApiDocument, OpenApiDiagnostic diagnostic)
        {
            if (diagnostic != null && diagnostic.Errors.Count > 0)
            {
                foreach (OpenApiError error in diagnostic.Errors)
                {
                    Console.WriteLine($"Error: {error}");
                }
                return string.Empty;
            }

            StringBuilder md = new StringBuilder();

            // ===== 封面資訊 =====
            md.AppendLine($"# {openApiDocument.Info?.Title ?? "API 規格"}");
            md.AppendLine();

            List<string> meta = new List<string>();
            if (!string.IsNullOrEmpty(openApiDocument.Info?.Version))
            {
                meta.Add($"版本 {openApiDocument.Info.Version}");
            }
            meta.Add($"產生日期 {DateTime.Now:yyyy-MM-dd}");
            md.AppendLine(string.Join("｜", meta));
            md.AppendLine();

            AppendBlock(md, openApiDocument.Info?.Description);
            AppendContact(md, openApiDocument.Info);
            AppendServers(md, openApiDocument.Servers);
            AppendSecuritySchemes(md, openApiDocument.Components?.SecuritySchemes);

            // ===== API 目錄 =====
            List<ApiEntry> entries = ApiEntry.Collect(openApiDocument);

            md.AppendLine("## API 目錄");
            md.AppendLine();
            md.AppendLine("| # | 方法 | 路徑 | 說明 |");
            md.AppendLine("|---|------|------|------|");
            foreach (ApiEntry entry in entries)
            {
                md.AppendLine($"| {entry.Index} | {entry.Method} | [{OpenApiText.MdCell(entry.Path)}](#{OpenApiText.ApiAnchor(entry.Index)}) | {OpenApiText.MdCell(entry.Summary)} |");
            }
            md.AppendLine();
            md.AppendLine("---");
            md.AppendLine();

            // ===== API 明細 =====
            md.AppendLine("## API 明細");
            md.AppendLine();
            foreach (ApiEntry entry in entries)
            {
                AppendOperation(md, entry);
            }

            // ===== 資料模型 =====
            AppendSchemas(md, openApiDocument.Components?.Schemas);

            return md.ToString();
        }

        /// <summary>
        /// 多行文字：每行之間留空行，避免 Markdown 併成同一段
        /// </summary>
        private void AppendBlock(StringBuilder md, string? text)
        {
            string block = OpenApiText.Block(text);
            if (string.IsNullOrEmpty(block))
            {
                return;
            }

            char[] separator = new char[] { '\n' };
            foreach (string line in block.Split(separator))
            {
                md.AppendLine(line);
                md.AppendLine();
            }
        }

        private void AppendContact(StringBuilder md, OpenApiInfo? info)
        {
            if (info == null)
            {
                return;
            }

            List<string> lines = new List<string>();
            if (info.Contact != null && (!string.IsNullOrEmpty(info.Contact.Name) || !string.IsNullOrEmpty(info.Contact.Email)))
            {
                lines.Add($"- 聯絡窗口：{info.Contact.Name} {info.Contact.Email}".TrimEnd());
            }
            if (info.License != null && !string.IsNullOrEmpty(info.License.Name))
            {
                lines.Add($"- 授權：{info.License.Name}");
            }
            if (lines.Count == 0)
            {
                return;
            }

            foreach (string line in lines)
            {
                md.AppendLine(line);
            }
            md.AppendLine();
        }

        private void AppendServers(StringBuilder md, IList<OpenApiServer>? servers)
        {
            if (servers == null || servers.Count == 0)
            {
                return;
            }

            md.AppendLine("## 服務位址");
            md.AppendLine();
            md.AppendLine("| 位址 | 說明 |");
            md.AppendLine("|------|------|");
            foreach (OpenApiServer server in servers)
            {
                md.AppendLine($"| {OpenApiText.MdCell(server.Url)} | {OpenApiText.MdCell(server.Description)} |");
            }
            md.AppendLine();
        }

        private void AppendSecuritySchemes(StringBuilder md, IDictionary<string, OpenApiSecurityScheme>? schemes)
        {
            if (schemes == null || schemes.Count == 0)
            {
                return;
            }

            md.AppendLine("## 安全性驗證");
            md.AppendLine();
            md.AppendLine("| 名稱 | 參數 | 類型 | 位置 | 說明 |");
            md.AppendLine("|------|------|------|------|------|");
            foreach (KeyValuePair<string, OpenApiSecurityScheme> scheme in schemes)
            {
                md.AppendLine($"| {OpenApiText.MdCell(scheme.Key)} | {OpenApiText.MdCell(scheme.Value.Name)} | {scheme.Value.Type} | {scheme.Value.In} | {OpenApiText.MdCell(scheme.Value.Description)} |");
            }
            md.AppendLine();
        }

        private void AppendOperation(StringBuilder md, ApiEntry entry)
        {
            OpenApiOperation operation = entry.Operation;

            md.AppendLine($"### <a id=\"{OpenApiText.ApiAnchor(entry.Index)}\"></a>{entry.Index}. [{entry.Method}] {entry.Path}");
            md.AppendLine();

            if (!string.IsNullOrEmpty(entry.Summary))
            {
                md.AppendLine($"- 說明：{OpenApiText.Plain(entry.Summary)}");
            }
            if (entry.Tags.Count > 0)
            {
                md.AppendLine($"- 分類：{string.Join("、", entry.Tags)}");
            }
            if (!string.IsNullOrEmpty(operation.OperationId))
            {
                md.AppendLine($"- 代號：`{operation.OperationId}`");
            }
            if (operation.Deprecated)
            {
                md.AppendLine("- 狀態：已淘汰（deprecated）");
            }
            md.AppendLine();

            AppendBlock(md, operation.Description);

            // 路徑／查詢／標頭參數：一張表列全部
            if (operation.Parameters != null && operation.Parameters.Count > 0)
            {
                md.AppendLine("#### 請求參數（Path / Query / Header）");
                md.AppendLine();
                md.AppendLine("| 參數 | 位置 | 類型 | 必填 | 說明 |");
                md.AppendLine("|------|------|------|------|------|");
                foreach (OpenApiParameter parameter in operation.Parameters)
                {
                    string required = parameter.Required ? "是" : string.Empty;
                    md.AppendLine($"| {OpenApiText.MdCell(parameter.Name)} | {parameter.In} | {OpenApiText.MdCell(OpenApiText.TypeText(parameter.Schema))} | {required} | {OpenApiText.MdCell(OpenApiText.DescriptionCell(parameter.Description, parameter.Schema))} |");
                }
                md.AppendLine();
            }

            // 請求內容
            if (operation.RequestBody?.Content != null && operation.RequestBody.Content.Count > 0)
            {
                KeyValuePair<string, OpenApiMediaType> content = operation.RequestBody.Content.First();
                md.AppendLine($"#### 請求內容（{content.Key}）");
                md.AppendLine();
                AppendSchemaTable(md, content.Value?.Schema);
                AppendSample(md, "請求範例", content.Value);
            }

            // 回應
            if (operation.Responses != null && operation.Responses.Count > 0)
            {
                md.AppendLine("#### 回應狀態");
                md.AppendLine();
                md.AppendLine("| 狀態碼 | 說明 | 內容型別 |");
                md.AppendLine("|--------|------|----------|");
                foreach (KeyValuePair<string, OpenApiResponse> response in operation.Responses)
                {
                    string mediaTypes = (response.Value.Content == null || response.Value.Content.Count == 0)
                        ? "—"
                        : string.Join("、", response.Value.Content.Keys);
                    md.AppendLine($"| {response.Key} | {OpenApiText.MdCell(response.Value.Description)} | {mediaTypes} |");
                }
                md.AppendLine();

                foreach (KeyValuePair<string, OpenApiResponse> response in operation.Responses)
                {
                    if (response.Value.Content == null || response.Value.Content.Count == 0)
                    {
                        continue;
                    }

                    KeyValuePair<string, OpenApiMediaType> content = response.Value.Content.First();
                    if (content.Value?.Schema == null)
                    {
                        continue;
                    }

                    md.AppendLine($"#### 回應內容 {response.Key}（{content.Key}）");
                    md.AppendLine();
                    AppendSchemaTable(md, content.Value.Schema);
                    AppendSample(md, $"回應範例 {response.Key}", content.Value);
                }
            }

            md.AppendLine("---");
            md.AppendLine();
        }

        /// <summary>
        /// 欄位表：物件展開屬性；陣列或單一 $ref 則以一行標示型別
        /// </summary>
        private void AppendSchemaTable(StringBuilder md, OpenApiSchema? schema)
        {
            if (schema == null)
            {
                return;
            }

            if (schema.Properties == null || schema.Properties.Count == 0)
            {
                md.AppendLine($"型別：`{OpenApiText.TypeText(schema)}` {OpenApiText.ConstraintText(schema)}".TrimEnd());
                md.AppendLine();
                return;
            }

            md.AppendLine("| 欄位 | 類型 | 必填 | 說明 |");
            md.AppendLine("|------|------|------|------|");
            foreach (KeyValuePair<string, OpenApiSchema> property in schema.Properties)
            {
                bool isRequired = schema.Required != null && schema.Required.Contains(property.Key);
                string required = isRequired ? "是" : string.Empty;
                md.AppendLine($"| {OpenApiText.MdCell(property.Key)} | {OpenApiText.MdCell(OpenApiText.TypeText(property.Value))} | {required} | {OpenApiText.MdCell(OpenApiText.DescriptionCell(property.Value?.Description, property.Value))} |");
            }
            md.AppendLine();
        }

        private void AppendSample(StringBuilder md, string caption, OpenApiMediaType? mediaType)
        {
            if (mediaType == null)
            {
                return;
            }

            string sample = mediaType.Example != null
                ? OpenApiText.AnyText(mediaType.Example)
                : OpenApiText.GenerateExampleJson(mediaType.Schema);

            if (string.IsNullOrWhiteSpace(sample))
            {
                return;
            }

            md.AppendLine($"{caption}：");
            md.AppendLine();
            md.AppendLine("```json");
            md.AppendLine(sample);
            md.AppendLine("```");
            md.AppendLine();
        }

        private void AppendSchemas(StringBuilder md, IDictionary<string, OpenApiSchema>? schemas)
        {
            if (schemas == null || schemas.Count == 0)
            {
                return;
            }

            md.AppendLine("## 資料模型");
            md.AppendLine();
            md.AppendLine("| 模型 | 欄位數 |");
            md.AppendLine("|------|--------|");
            foreach (KeyValuePair<string, OpenApiSchema> schema in schemas.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase))
            {
                md.AppendLine($"| [{OpenApiText.MdCell(schema.Key)}](#{OpenApiText.SchemaAnchor(schema.Key)}) | {schema.Value.Properties?.Count ?? 0} |");
            }
            md.AppendLine();

            foreach (KeyValuePair<string, OpenApiSchema> schema in schemas.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase))
            {
                md.AppendLine($"### <a id=\"{OpenApiText.SchemaAnchor(schema.Key)}\"></a>{schema.Key}");
                md.AppendLine();

                string desc = OpenApiText.Plain(schema.Value.Description);
                if (!string.IsNullOrEmpty(desc))
                {
                    md.AppendLine(desc);
                    md.AppendLine();
                }

                AppendSchemaTable(md, schema.Value);
            }
        }
    }
}
