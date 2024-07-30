
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Swagger2Doc.Helpers;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Markdig;

namespace Swagger2Doc.Services
{
    public class CoreService
    {
        private readonly ILogger<CoreService> _logger;

        private readonly IConfiguration _config;

        private readonly IConfigurationSection _storedProcedures;

        private readonly DapperHelper _dapperHelper;

        private readonly IEmailService _emailService;

        public CoreService(IConfiguration config, ILogger<CoreService> logger, DapperHelper dapperHelper, IEmailService emailService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _storedProcedures = _config.GetSection("StoredProcedures") ?? throw new ArgumentNullException("StoredProcedures Section not found in appsetting.json");

            _dapperHelper = dapperHelper ?? throw new ArgumentNullException(nameof(dapperHelper));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public async Task Run()
        {

            // Note that the first element in the array contains the path of the executable
            // and the arguments passed to the program start with the second element, i.e. at index 1.
            // string? tdate = String.Empty; // 20221024
            try
            {
                //tdate = Environment.GetCommandLineArgs()[1];

                string jsonText = this.ReadJsonFile();

                // 创建 OpenApiDocument 读取器
                var openApiReader = new OpenApiStringReader();

                // 解析 JSON 内容到 OpenApiDocument 对象
                OpenApiDocument openApiDocument = openApiReader.Read(jsonText, out var diagnostic);

                // 现在你可以使用 openApiDocument 对象
                Console.WriteLine("OpenAPI Document Title: " + openApiDocument.Info.Title);


                //Markdown to HTML
                string markdownText = Convert2MarkDown.CreateMD(openApiDocument, diagnostic);
                // Configure the pipeline with all advanced extensions active
                var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
                string htmlContent = Markdown.ToHtml(markdownText, pipeline);
                string htmlFileText = $@"<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>Markdown to HTML</title>
    <link rel='stylesheet' href='https://cdnjs.cloudflare.com/ajax/libs/github-markdown-css/4.0.0/github-markdown.min.css'>
    <style>
        body {{
            font-family: Arial, sans-serif;
            margin: 40px;
        }}
        .markdown-body {{
            box-sizing: border-box;
            min-width: 200px;
            max-width: 980px;
            margin: 0 auto;
            padding: 45px;
        }}
    </style>
</head>
<body>
    <article class='markdown-body'>
        {htmlContent}
    </article>
</body>
</html>"; 
                SaveToFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "swagger.html"), htmlFileText);


                Convert2Docx convert2Docx = new Convert2Docx();
                convert2Docx.CreateDoc(openApiDocument);
                //CreateDoc(openApiDocument);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }
        }

        public static void SaveToFile(string filePath, string text)
        {
            File.WriteAllText(filePath, text);
        }

        private string ReadJsonFile()
        {
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string jsonPath = Path.Combine(currentDirectory, "swagger.json");
            return File.ReadAllText(jsonPath);
        }

    
    }
}
