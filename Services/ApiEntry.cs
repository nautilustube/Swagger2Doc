using Microsoft.OpenApi.Models;

namespace Swagger2Doc.Services
{
    /// <summary>
    /// 一支 API（路徑 + HTTP 方法）的攤平資料；編號即目錄與錨點的依據，
    /// Markdown 與 Docx 兩邊共用同一份順序，確保索引編號一致
    /// </summary>
    public class ApiEntry
    {
        public int Index { get; set; }

        public string Method { get; set; } = string.Empty;

        public string Path { get; set; } = string.Empty;

        public string Summary { get; set; } = string.Empty;

        public List<string> Tags { get; set; } = new List<string>();

        public OpenApiOperation Operation { get; set; } = new OpenApiOperation();

        /// <summary>
        /// 攤平文件中所有 path 與 operation（含 PathItem 層級的共用參數）
        /// </summary>
        public static List<ApiEntry> Collect(OpenApiDocument document)
        {
            List<ApiEntry> entries = new List<ApiEntry>();

            if (document?.Paths == null)
            {
                return entries;
            }

            int index = 1;
            foreach (KeyValuePair<string, OpenApiPathItem> pathItem in document.Paths)
            {
                if (pathItem.Value?.Operations == null)
                {
                    continue;
                }

                foreach (KeyValuePair<OperationType, OpenApiOperation> operation in pathItem.Value.Operations)
                {
                    OpenApiOperation value = operation.Value ?? new OpenApiOperation();

                    // PathItem 層級的共用參數併入該 operation（規格允許，Swashbuckle 少見但其他產生器常見）
                    if (pathItem.Value.Parameters != null && pathItem.Value.Parameters.Count > 0)
                    {
                        foreach (OpenApiParameter shared in pathItem.Value.Parameters)
                        {
                            bool exists = value.Parameters != null
                                && value.Parameters.Any(x => x.Name == shared.Name && x.In == shared.In);
                            if (!exists)
                            {
                                value.Parameters ??= new List<OpenApiParameter>();
                                value.Parameters.Add(shared);
                            }
                        }
                    }

                    string summary = value.Summary;
                    if (string.IsNullOrEmpty(summary))
                    {
                        summary = pathItem.Value.Summary ?? string.Empty;
                    }

                    entries.Add(new ApiEntry
                    {
                        Index = index++,
                        Method = operation.Key.ToString().ToUpperInvariant(),
                        Path = pathItem.Key,
                        Summary = OpenApiText.Plain(summary),
                        Tags = value.Tags?.Select(tag => tag.Name).Where(name => !string.IsNullOrEmpty(name)).ToList() ?? new List<string>(),
                        Operation = value
                    });
                }
            }

            return entries;
        }
    }
}
