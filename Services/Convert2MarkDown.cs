using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace Swagger2Doc.Services
{
    public class Convert2MarkDown
    {

        public static string CreateMD(OpenApiDocument openApiDocument, OpenApiDiagnostic diagnostic)
        {
            // 診斷訊息
            if (diagnostic.Errors.Count > 0)
            {
                foreach (var error in diagnostic.Errors)
                {
                    Console.WriteLine($"Error: {error}");
                }
                return String.Empty;
            }

            // Markdown Sb
            StringBuilder markdownBuilder = new StringBuilder();

            markdownBuilder.AppendLine($"# {openApiDocument.Info.Title}");
            markdownBuilder.AppendLine($"{openApiDocument.Info.Description}");

            int pathCount = 1;
            foreach (KeyValuePair<string, OpenApiPathItem> pathItem in openApiDocument.Paths)
            {
                string path = pathItem.Key;
                OpenApiPathItem operations = pathItem.Value;

                // KeyValuePair<OperationType,OpenApiOperation>
                foreach (KeyValuePair<OperationType, OpenApiOperation> operation in operations.Operations)
                {
                    string httpMethod = operation.Key.ToString().ToUpper();
                    string url = path;

                    // title 
                    markdownBuilder.AppendLine($"## {pathCount}. **[{httpMethod}]** {url}");


                    // 描述
                    if (!string.IsNullOrEmpty(operations.Summary))
                    {
                        markdownBuilder.AppendLine($"- 描述: {operations.Summary}");
                    }
                    else if (!string.IsNullOrEmpty(operations.Description))
                    {
                        markdownBuilder.AppendLine($"- 描述: {operations.Description}");
                    }

                    // Request
                    if (operation.Value.RequestBody != null && operation.Value.RequestBody.Content != null)
                    {
                        foreach (var content in operation.Value.RequestBody.Content)
                        {

                            markdownBuilder.AppendLine($"### - 請求參數表格:");
                            markdownBuilder.AppendLine($"| Name | Type | Description |");
                            markdownBuilder.AppendLine($"|------|------|-------------|");

                            foreach (var param in content.Value.Schema.Properties)
                            {
                                var property = param.Value;
                                bool isRequired = content.Value.Schema.Required.Where(x => x.Equals(param.Key)).Any();
                                string refernce = (property.Items?.Reference?.Id != null) ? $"<{property.Items?.Reference?.Id}>" : "";
                                string requiredHint = isRequired ? "( * )" : "";
                                markdownBuilder.AppendLine($"| {requiredHint} {param.Key} | {property.Type} {property.Format} {property.Items?.Format} {property.Reference?.Id} {refernce} | {property.Description} |");
                            }

                            // sample JSON
                            string exampleJson = content.Value.Example?.ToString() ?? String.Empty;
                            if (content.Value.Schema != null && !content.Value.Examples.Any())
                            {
                                exampleJson = GenerateExampleFromSchema(content.Value.Schema);
                            }
                            markdownBuilder.AppendLine($"- {content.Key} Request Sample JSON:");
                            markdownBuilder.AppendLine($"```json");
                            markdownBuilder.AppendLine($"{IndentJson(exampleJson, 3)}");
                            markdownBuilder.AppendLine($"```");
                            break;
                        }
                    }


                    // Response
                    if (operation.Value.Responses != null)
                    {
                        foreach (var response in operation.Value.Responses)
                        {
                            markdownBuilder.AppendLine($"### - 回應参数表格:");
                            markdownBuilder.AppendLine($"| Code | Description |");
                            markdownBuilder.AppendLine($"|------|-------------|");
                            markdownBuilder.AppendLine($"| {response.Key} | {response.Value.Description} |");

                            // sample JSON
                            if (response.Value.Content != null)
                            {
                                GenResponseMarkDownTable(markdownBuilder, response.Value.Content.First().Value.Schema);

                                foreach (KeyValuePair<string, OpenApiMediaType> content in response.Value.Content)
                                {
                                    string exampleJson = content.Value.Example?.ToString() ?? String.Empty;
                                    if (content.Value.Schema != null && !content.Value.Examples.Any())
                                    {
                                        exampleJson = GenerateExampleFromSchema(content.Value.Schema);
                                    }
                                    markdownBuilder.AppendLine($"- {content.Key} 回應Sample JSON:");
                                    markdownBuilder.AppendLine($"```json");
                                    markdownBuilder.AppendLine($"{IndentJson(exampleJson, 3)}");
                                    markdownBuilder.AppendLine($"```");
                                    break;
                                }
                            }
                        }
                    }
                
                }
                pathCount++;
                markdownBuilder.AppendLine($"\n<br><br>\n");

            }
            // Components Schemas
            // title 
            markdownBuilder.AppendLine($"##  Schemas");
            markdownBuilder.AppendLine($"\n<br><br>\n");
            foreach (KeyValuePair<string, OpenApiSchema> schemas in openApiDocument.Components.Schemas) 
            {
                string name = schemas.Key;
                GenSchemasMarkDownTable(markdownBuilder, name, schemas.Value);
            }
            markdownBuilder.AppendLine($"\n<br><br>\n");


            // title 
            markdownBuilder.AppendLine($"##  SecurityScheme");
            markdownBuilder.AppendLine($"\n<br><br>\n");
            foreach (KeyValuePair<string, OpenApiSecurityScheme> securityScheme in openApiDocument.Components.SecuritySchemes)
            {
                string name = securityScheme.Key;
                GenSecuritySchemeMarkDownTable(markdownBuilder, name, securityScheme.Value);
            }
            markdownBuilder.AppendLine($"\n<br><br>\n");

            // output Markdown
            Console.WriteLine(markdownBuilder.ToString());
            CoreService.SaveToFile("swagger.md", markdownBuilder.ToString());
            return markdownBuilder.ToString();
        }

        private static void GenSchemasMarkDownTable(StringBuilder markdownBuilder, string name,OpenApiSchema schema)
        {
            markdownBuilder.AppendLine($"### - {name}");
            // 示例参数表格
            markdownBuilder.AppendLine($"| Name | Type | Description |");
            markdownBuilder.AppendLine($"|------|------|-------------|");

            foreach (var param in schema.Properties)
            {
                var property = param.Value;
                bool isRequired = schema.Required.Where(x => x.Equals(param.Key)).Any();
                string refernce = (property.Items?.Reference?.Id != null) ? $"<{property.Items?.Reference?.Id}>" : "";
                string requiredHint = isRequired ? "( * )" : "";
                markdownBuilder.AppendLine($"| {requiredHint} {param.Key} | {property.Type} {property.Format} {property.Reference?.Id} {refernce} | {property.Description} |");
            }
        }

        private static void GenSecuritySchemeMarkDownTable(StringBuilder markdownBuilder, string name, OpenApiSecurityScheme schema)
        {
            markdownBuilder.AppendLine($"### - {name}(SecurityScheme)");

            markdownBuilder.AppendLine($"| Name | Type | Description | In |");
            markdownBuilder.AppendLine($"|------|------|-------------|----|");
            markdownBuilder.AppendLine($"| ( * ) {schema.Name} | {schema.Type} | {schema.Description} | {schema.In} |");
        }


        private static void GenResponseMarkDownTable(StringBuilder markdownBuilder, OpenApiSchema schema)
        {
            markdownBuilder.AppendLine($"### - 回應参数表格:");
            // 示例参数表格
            markdownBuilder.AppendLine($"| Name | Type | Description |");
            markdownBuilder.AppendLine($"|------|------|-------------|");

            
            foreach (var param in schema.Properties)
            {
                var property = param.Value;
                bool isRequired = schema.Required.Where(x => x.Equals(param.Key)).Any();
                string refernce = (property.Items?.Reference?.Id != null) ? $"<{property.Items?.Reference?.Id}>" : "";
                string requiredHint = isRequired ? "( * )" : "";
                markdownBuilder.AppendLine($"| {requiredHint} {param.Key} | {property.Type} {property.Format} {property.Reference?.Id} {refernce} | {property.Description} |");
            }
        }


        public static string GenerateExampleFromSchema(OpenApiSchema schema)
        {
            return JsonConvert.SerializeObject(GenerateExample(schema), Newtonsoft.Json.Formatting.Indented);
        }

        public static object GenerateExample(OpenApiSchema schema)
        {
            if (schema == null) return null;
            if (schema.Reference != null && schema.Reference.Id.Equals("JToken")) { return null; }
            switch (schema.Type)
            {
                case "object":
                    return schema.Properties.ToDictionary(
                        prop => prop.Key,
                        prop => GenerateExample(prop.Value)
                    );
                case "array":
                    return new List<object> { GenerateExample(schema.Items) };
                case "string":
                    if (schema.Format != null && schema.Format.Equals("uuid"))
                    {
                        return Guid.NewGuid().ToString();
                    }
                    return schema.Example?.ToString() ?? "string";
                case "number":
                case "integer":
                    return 1;
                case "boolean":
                    return true;
                default:
                    return schema.Example;
            }
        }


        public static string IndentJson(string json, int indentSize)
        {
            var indent = new string(' ', indentSize);
            var lines = json.Split(Environment.NewLine);
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = indent + lines[i];
            }
            return string.Join(Environment.NewLine, lines);
        }


    }


}
