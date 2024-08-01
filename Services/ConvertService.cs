
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Markdig;

namespace Swagger2Doc.Services
{
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
            try
            {
                string jsonText = this.ReadJsonFile();

                // OpenApiDocument Reader
                OpenApiStringReader openApiReader = new OpenApiStringReader();
                OpenApiDocument openApiDocument = openApiReader.Read(jsonText, out var diagnostic);

                // 1.OpenApiDocument To Docx
                Convert2Docx convert2Docx = new Convert2Docx();
                convert2Docx.CreateDoc(openApiDocument);

                // 2. OpenApiDocument To Markdown
                Convert2MarkDown convert2MarkDown = new Convert2MarkDown();
                string markdownText = convert2MarkDown.CreateMD(openApiDocument, diagnostic);

                // 2.OpenApiDocument Markdown to HTML
                // Configure the pipeline with all advanced extensions active
                var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
                string htmlContent = Markdown.ToHtml(markdownText, pipeline);
                string _htmlFileText = ConvertService.ReadFile("swagger_html_template.txt");
                string htmlFileText = _htmlFileText.Replace(":htmlContent", htmlContent);

                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
                filePath = Path.Combine(filePath, "Output");
                SaveToFile(Path.Combine(filePath,"swagger.html"), htmlFileText);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }
        }
        private string ReadJsonFile()
        {
            string currentDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
            string jsonPath = Path.Combine(currentDirectory, "swagger.json");
            return File.ReadAllText(jsonPath);
        }

        public static void SaveToFile(string filePath, string text)
        {
            File.WriteAllText(filePath, text);
        }
        public static string ReadFile(string fileName, string? filePath= null )
        {
            string currentDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Resources");
            string jsonPath = Path.Combine(currentDirectory, fileName);
            if (filePath != null)
            {
                jsonPath = Path.Combine(filePath, fileName);
            }
            return File.ReadAllText(jsonPath);
        }



    
    }
}
