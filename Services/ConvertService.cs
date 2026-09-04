using Markdig;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

namespace Swagger2Doc.Services
{
    /// <summary>
    /// 轉檔主流程：讀 swagger.json → 產生 .md / .html / .docx
    /// <para>
    /// 用法：Swagger2Doc.exe [輸入 json 或資料夾] [-o 輸出資料夾] [-n 輸出檔名]
    /// 不帶參數時沿用預設：Resources\swagger.json → Resources\Output\swagger.{md,html,docx}
    /// </para>
    /// </summary>
    public class ConvertService
    {
        private readonly ILogger<ConvertService> _logger;

        private readonly IConfiguration _config;

        public ConvertService(IConfiguration config, ILogger<ConvertService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public void Run()
        {
            // Program 以 Host 啟動、未把 args 傳進 DI，此處直接取命令列（第 0 個是執行檔本身）
            string[] args = Environment.GetCommandLineArgs().Skip(1).ToArray();
            ConvertOptions options = ConvertOptions.Parse(args, DefaultResourceDirectory());

            if (options.ShowHelp)
            {
                Console.WriteLine(ConvertOptions.HelpText);
                return;
            }

            List<string> inputs = options.ResolveInputFiles();
            if (inputs.Count == 0)
            {
                _logger.LogError("找不到任何 swagger json：{Input}", string.Join("、", options.Inputs));
                return;
            }

            Directory.CreateDirectory(options.OutputDirectory);

            foreach (string input in inputs)
            {
                string baseName = (inputs.Count == 1 && !string.IsNullOrEmpty(options.OutputName))
                    ? options.OutputName!
                    : Path.GetFileNameWithoutExtension(input);

                ConvertOne(input, options.OutputDirectory, baseName);
            }
        }

        private void ConvertOne(string inputPath, string outputDirectory, string baseName)
        {
            try
            {
                string jsonText = File.ReadAllText(inputPath);

                OpenApiStringReader reader = new OpenApiStringReader();
                OpenApiDocument document = reader.Read(jsonText, out OpenApiDiagnostic diagnostic);

                if (diagnostic != null && diagnostic.Errors.Count > 0)
                {
                    // 規格瑕疵不一定致命（多數為 $ref 或欄位缺漏），仍嘗試輸出並把問題留在 log
                    foreach (OpenApiError error in diagnostic.Errors)
                    {
                        _logger.LogWarning("{File} 規格檢核：{Error}", Path.GetFileName(inputPath), error.ToString());
                    }
                }

                if (document == null)
                {
                    _logger.LogError("{File} 無法解析為 OpenApi 文件", inputPath);
                    return;
                }

                // 1. docx
                string docxPath = Path.Combine(outputDirectory, $"{baseName}.docx");
                new Convert2Docx().CreateDoc(document, docxPath);

                // 2. markdown（診斷錯誤不阻擋輸出，故不再回傳空字串）
                string markdown = new Convert2MarkDown().CreateMD(document, new OpenApiDiagnostic());
                string mdPath = Path.Combine(outputDirectory, $"{baseName}.md");
                SaveToFile(mdPath, markdown);

                // 3. html（由 markdown 轉出，套用內附樣板）
                MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
                string htmlContent = Markdown.ToHtml(markdown, pipeline);
                string template = ReadTemplate();
                string title = document.Info?.Title ?? baseName;
                string html = template
                    .Replace(":htmlTitle", System.Net.WebUtility.HtmlEncode(title))
                    .Replace(":htmlContent", htmlContent);
                string htmlPath = Path.Combine(outputDirectory, $"{baseName}.html");
                SaveToFile(htmlPath, html);

                _logger.LogInformation("{File} → {Base}.md / .html / .docx", Path.GetFileName(inputPath), baseName);
                Console.WriteLine($"[OK] {Path.GetFileName(inputPath)} → {outputDirectory}\\{baseName}.(md|html|docx)");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "轉檔失敗：{File}", inputPath);
                Console.WriteLine($"[FAIL] {inputPath}：{ex.Message}");
            }
        }

        private static string DefaultResourceDirectory()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
        }

        private string ReadTemplate()
        {
            return ReadFile("swagger_html_template.txt");
        }

        public static void SaveToFile(string filePath, string text)
        {
            string? directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.WriteAllText(filePath, text);
        }

        public static string ReadFile(string fileName, string? filePath = null)
        {
            string path = Path.Combine(filePath ?? DefaultResourceDirectory(), fileName);
            return File.ReadAllText(path);
        }
    }

    /// <summary>
    /// 命令列參數
    /// </summary>
    public class ConvertOptions
    {
        public const string HelpText =
            "Swagger2Doc — 將 swagger.json 轉為 Markdown / HTML / Word\n" +
            "\n" +
            "用法：\n" +
            "  Swagger2Doc.exe                          使用 Resources\\swagger.json，輸出至 Resources\\Output\n" +
            "  Swagger2Doc.exe <json 或資料夾> [...]     指定輸入（資料夾時轉換其中所有 .json）\n" +
            "\n" +
            "選項：\n" +
            "  -o, --output <資料夾>   輸出資料夾（預設 Resources\\Output）\n" +
            "  -n, --name   <檔名>     輸出檔名（僅單一輸入時有效，預設沿用輸入檔名）\n" +
            "  -h, --help              顯示說明\n";

        public List<string> Inputs { get; private set; } = new List<string>();

        public string OutputDirectory { get; private set; } = string.Empty;

        public string? OutputName { get; private set; }

        public bool ShowHelp { get; private set; }

        public static ConvertOptions Parse(string[] args, string resourceDirectory)
        {
            ConvertOptions options = new ConvertOptions
            {
                OutputDirectory = Path.Combine(resourceDirectory, "Output")
            };

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                switch (arg.ToLowerInvariant())
                {
                    case "-h":
                    case "--help":
                        options.ShowHelp = true;
                        break;
                    case "-o":
                    case "--output":
                        if (i + 1 < args.Length)
                        {
                            options.OutputDirectory = args[++i];
                        }
                        break;
                    case "-n":
                    case "--name":
                        if (i + 1 < args.Length)
                        {
                            options.OutputName = Path.GetFileNameWithoutExtension(args[++i]);
                        }
                        break;
                    default:
                        // Host.CreateDefaultBuilder 的組態參數（--key=value）不視為輸入路徑
                        if (!arg.StartsWith("-"))
                        {
                            options.Inputs.Add(arg);
                        }
                        break;
                }
            }

            if (options.Inputs.Count == 0)
            {
                options.Inputs.Add(Path.Combine(resourceDirectory, "swagger.json"));
            }

            return options;
        }

        /// <summary>
        /// 展開輸入清單：資料夾取其中所有 .json，檔案則直接採用
        /// </summary>
        public List<string> ResolveInputFiles()
        {
            List<string> files = new List<string>();

            foreach (string input in Inputs)
            {
                if (Directory.Exists(input))
                {
                    files.AddRange(Directory.GetFiles(input, "*.json", SearchOption.TopDirectoryOnly));
                }
                else if (File.Exists(input))
                {
                    files.Add(input);
                }
            }

            return files.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }
    }
}
