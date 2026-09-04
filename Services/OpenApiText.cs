using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace Swagger2Doc.Services
{
    /// <summary>
    /// OpenApi 物件轉文件文字的共用工具：型別、限制、描述清洗、範例 JSON、錨點
    /// （Markdown 與 Docx 產生器共用，避免兩邊規則不一致）
    /// </summary>
    public static class OpenApiText
    {
        private static readonly Regex BrTag = new Regex(@"</?br\s*/?>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex InlineTag = new Regex(@"</?(strong|b|em|i|u|code|span|p|div|small)\s*/?>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex MultiSpace = new Regex(@"[ \t]{2,}", RegexOptions.Compiled);
        private static readonly Regex MultiBreak = new Regex(@"\n{2,}", RegexOptions.Compiled);

        /// <summary>
        /// 型別文字：$ref 顯示模型名稱、陣列顯示 array&lt;T&gt;、其餘顯示 type(format 或長度)
        /// </summary>
        public static string TypeText(OpenApiSchema? schema)
        {
            if (schema == null)
            {
                return string.Empty;
            }

            // $ref：直接顯示模型名稱（模型明細另見「資料模型」章節）
            if (schema.Reference != null && !string.IsNullOrEmpty(schema.Reference.Id))
            {
                return schema.Reference.Id;
            }

            // allOf / oneOf / anyOf 包裝：Swashbuckle 對「可為 null 之參考型別」的常見寫法，
            // 不解開會讓型別欄整格空白
            if (string.IsNullOrEmpty(schema.Type))
            {
                OpenApiSchema? wrapped = schema.AllOf?.FirstOrDefault()
                    ?? schema.OneOf?.FirstOrDefault()
                    ?? schema.AnyOf?.FirstOrDefault();
                if (wrapped != null)
                {
                    return TypeText(wrapped);
                }
            }

            if (string.Equals(schema.Type, "array", StringComparison.OrdinalIgnoreCase))
            {
                string itemType = TypeText(schema.Items);
                return string.IsNullOrEmpty(itemType) ? "array" : $"array<{itemType}>";
            }

            string text = string.IsNullOrEmpty(schema.Type) ? "object" : schema.Type;

            if (!string.IsNullOrEmpty(schema.Format))
            {
                text += $"({schema.Format})";
            }
            else if (schema.MaxLength.HasValue)
            {
                string min = (schema.MinLength.HasValue && schema.MinLength.Value > 0) ? $"{schema.MinLength}.." : string.Empty;
                text += $"({min}{schema.MaxLength})";
            }

            return text;
        }

        /// <summary>
        /// 限制文字：列舉值／預設值／格式／可為 null／唯讀／已淘汰／數值範圍
        /// </summary>
        public static string ConstraintText(OpenApiSchema? schema)
        {
            if (schema == null)
            {
                return string.Empty;
            }

            List<string> notes = new List<string>();

            if (schema.Enum != null && schema.Enum.Count > 0)
            {
                notes.Add("可選值：" + string.Join("｜", schema.Enum.Select(AnyText).Where(x => !string.IsNullOrEmpty(x))));
            }
            if (schema.Default != null)
            {
                string def = AnyText(schema.Default);
                if (!string.IsNullOrEmpty(def))
                {
                    notes.Add($"預設 {def}");
                }
            }
            if (!string.IsNullOrEmpty(schema.Pattern))
            {
                notes.Add($"格式 {schema.Pattern}");
            }
            if (schema.Minimum.HasValue || schema.Maximum.HasValue)
            {
                notes.Add($"範圍 {schema.Minimum?.ToString() ?? "-"} ~ {schema.Maximum?.ToString() ?? "-"}");
            }
            if (schema.Nullable)
            {
                notes.Add("可為 null");
            }
            if (schema.ReadOnly)
            {
                notes.Add("唯讀");
            }
            if (schema.Deprecated)
            {
                notes.Add("已淘汰");
            }

            return notes.Count == 0 ? string.Empty : "〔" + string.Join("；", notes) + "〕";
        }

        /// <summary>
        /// 說明欄文字＝描述 ＋ 限制註記（單行，供表格使用）
        /// </summary>
        public static string DescriptionCell(string? description, OpenApiSchema? schema)
        {
            string desc = Plain(description);
            string constraint = ConstraintText(schema);
            if (string.IsNullOrEmpty(constraint))
            {
                return desc;
            }
            return string.IsNullOrEmpty(desc) ? constraint : $"{desc} {constraint}";
        }

        /// <summary>
        /// OpenApi Any 值轉文字
        /// </summary>
        public static string AnyText(IOpenApiAny? any)
        {
            return any switch
            {
                null => string.Empty,
                OpenApiString s => s.Value ?? string.Empty,
                OpenApiInteger i => i.Value.ToString(),
                OpenApiLong l => l.Value.ToString(),
                OpenApiFloat f => f.Value.ToString(),
                OpenApiDouble d => d.Value.ToString(),
                OpenApiBoolean b => b.Value ? "true" : "false",
                OpenApiDate dt => dt.Value.ToString("yyyy-MM-dd"),
                OpenApiDateTime dtm => dtm.Value.ToString("yyyy-MM-ddTHH:mm:ss"),
                OpenApiNull => "null",
                _ => any.ToString() ?? string.Empty
            };
        }

        /// <summary>
        /// 單行純文字：移除 HTML 標籤與換行（表格儲存格用）
        /// </summary>
        public static string Plain(string? text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            string s = BrTag.Replace(text, " ");
            s = InlineTag.Replace(s, string.Empty);
            s = s.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");
            return MultiSpace.Replace(s, " ").Trim();
        }

        /// <summary>
        /// 多行純文字：HTML 換行標籤轉真換行、移除其他行內標籤（段落用）
        /// </summary>
        public static string Block(string? text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            string s = BrTag.Replace(text, "\n");
            s = InlineTag.Replace(s, string.Empty);
            s = s.Replace("\r\n", "\n").Replace("\r", "\n");
            s = string.Join("\n", s.Split('\n').Select(line => MultiSpace.Replace(line, " ").TrimEnd()));
            // 連續換行（如來源寫成 </br></br>）收斂為單一換行，避免文件出現整片空行
            return MultiBreak.Replace(s, "\n").Trim();
        }

        /// <summary>
        /// Markdown 表格儲存格：單行純文字，並跳脫欄位分隔符號
        /// </summary>
        public static string MdCell(string? text)
        {
            return Plain(text).Replace("|", "\\|");
        }

        /// <summary>
        /// 必填標記
        /// </summary>
        public static string RequiredHint(bool isRequired)
        {
            return isRequired ? "( * )" : string.Empty;
        }

        /// <summary>
        /// 產生範例 JSON（$ref 循環自我保護；同層重複引用不誤判）
        /// </summary>
        public static string GenerateExampleJson(OpenApiSchema? schema)
        {
            if (schema == null)
            {
                return "{}";
            }
            object? sample = BuildExample(schema, new HashSet<string>(), 0);
            return JsonConvert.SerializeObject(sample, Formatting.Indented);
        }

        /// <summary>巢狀深度上限（避免深層模型把範例撐爆）</summary>
        private const int MaxDepth = 6;

        private static object? BuildExample(OpenApiSchema? schema, HashSet<string> path, int depth)
        {
            if (schema == null || depth > MaxDepth)
            {
                return null;
            }

            // 沿著目前遞迴路徑判斷循環；回溯時移除，
            // 才不會把「同一層出現兩次的同型別」誤判為循環
            string? refId = schema.Reference?.Id;
            if (!string.IsNullOrEmpty(refId))
            {
                if (refId.Equals("JToken", StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }
                if (path.Contains(refId))
                {
                    return $"[Circular: {refId}]";
                }
                path.Add(refId);
            }

            try
            {
                if (schema.Example != null)
                {
                    return AnyText(schema.Example);
                }

                // allOf / oneOf / anyOf 包裝
                if (string.IsNullOrEmpty(schema.Type) && (schema.Properties == null || schema.Properties.Count == 0))
                {
                    OpenApiSchema? wrapped = schema.AllOf?.FirstOrDefault()
                        ?? schema.OneOf?.FirstOrDefault()
                        ?? schema.AnyOf?.FirstOrDefault();
                    if (wrapped != null)
                    {
                        return BuildExample(wrapped, path, depth + 1);
                    }
                }

                switch (schema.Type)
                {
                    case "array":
                        return new List<object?> { BuildExample(schema.Items, path, depth + 1) };
                    case "string":
                        if (schema.Enum != null && schema.Enum.Count > 0)
                        {
                            return AnyText(schema.Enum.First());
                        }
                        return schema.Format switch
                        {
                            "uuid" => Guid.Empty.ToString(),
                            "date" => "2024-01-01",
                            "date-time" => "2024-01-01T00:00:00",
                            "byte" => "c3RyaW5n",
                            _ => "string"
                        };
                    case "integer":
                    case "number":
                        return 0;
                    case "boolean":
                        return true;
                    case "object":
                    default:
                        if (schema.Properties != null && schema.Properties.Count > 0)
                        {
                            return schema.Properties.ToDictionary(
                                prop => prop.Key,
                                prop => BuildExample(prop.Value, path, depth + 1));
                        }
                        return schema.Type == null ? null : new Dictionary<string, object?>();
                }
            }
            finally
            {
                if (!string.IsNullOrEmpty(refId))
                {
                    path.Remove(refId);
                }
            }
        }

        /// <summary>
        /// 縮排文字區塊
        /// </summary>
        public static string Indent(string text, int indentSize)
        {
            string indent = new string(' ', indentSize);
            string[] lines = text.Replace("\r\n", "\n").Split('\n');
            return string.Join(Environment.NewLine, lines.Select(line => indent + line));
        }

        /// <summary>
        /// 目錄錨點 id（HTML/Markdown 共用）
        /// </summary>
        public static string ApiAnchor(int index)
        {
            return $"api-{index}";
        }

        /// <summary>
        /// 資料模型錨點 id
        /// </summary>
        public static string SchemaAnchor(string name)
        {
            return "schema-" + Regex.Replace(name, @"[^A-Za-z0-9_-]", "-");
        }
    }
}
