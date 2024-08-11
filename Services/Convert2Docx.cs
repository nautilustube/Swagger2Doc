using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using Microsoft.OpenApi.Models;
using System.Data;
using System.Text;
using System.Security.Cryptography.Xml;

namespace Swagger2Doc.Services
{
    public class Convert2Docx
    {
        public void CreateDoc(OpenApiDocument openApiDocument)
        {

            string currentDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
            currentDirectory = Path.Combine(currentDirectory, "Output");
            string filePath = Path.Combine(currentDirectory, "swagger.docx");

            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {

                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = new Body();
                mainPart.Document.Append(body);

                // 新增標題頁
                CreateDocBlock1(body, openApiDocument);
                AddPageBreak(body);

                // SecuritySchemes 
                AddHeading(body, $"SecuritySchemes", "Heading2", 36, "000000");
                AddLineBreak(body);
                foreach (KeyValuePair<string, OpenApiSecurityScheme> securityScheme in openApiDocument.Components.SecuritySchemes)
                {
                    string name = securityScheme.Key;
                    GenSecuritySchemeTable(body, name, securityScheme.Value);
                }
                AddPageBreak(body);

                // Api Endpoints 
                int pathCount = 1;
                foreach (var pathItem in openApiDocument.Paths)
                {
                    string path = pathItem.Key;
                    var operations = pathItem.Value;

                    // KeyValuePair<OperationType,OpenApiOperation>
                    foreach (KeyValuePair<OperationType, OpenApiOperation> operation in operations.Operations)
                    {
                        string httpMethod = operation.Key.ToString().ToUpper();
                        string url = path;
                        string todo = operation.Value.Summary;

                        // title 
                        AddHeading(body, $"{pathCount}. [{httpMethod}]", "Heading2", 24, "000000");
                        AddHeading(body, $"{url}", "Heading2", 18, "000000");
                        AddLineBreak(body);
                        AddParagraph(body, $"Todo : {todo}");
                        AddLineBreak(body);

                        // 描述
                        if (!string.IsNullOrEmpty(operations.Summary))
                        {
                            AddParagraph(body, $"Summary: {operations.Summary}");

                        }
                        else if (!string.IsNullOrEmpty(operations.Description))
                        {
                            AddParagraph(body, $"Description: {operations.Description}");
                        }


                        // Get parameters
                        if (operation.Value.Parameters != null)
                        {
                            AddHeading(body, $"Request Parameter(From Url) Form:", "Heading3", 24, "000000");
                            DataTable dataColName = new DataTable();
                            dataColName.Columns.Add("Name");
                            dataColName.Columns.Add("Type");
                            dataColName.Columns.Add("Description");

                            foreach (var content in operation.Value.Parameters)
                            {

                                if (content.Schema == null)
                                { 
                                    continue;
                                }

                                bool isRequired = content.Required;
                                string requiredHint = isRequired ? "( * )" : "";
                                DataRow row = dataColName.NewRow();
                                row["Name"] = $"{requiredHint} {content.Name}";
                                row["Type"] = $"{content.Schema?.Type} {content.Schema?.Format} {content.Schema?.Items?.Format} {content.Reference?.Id}";
                                row["Description"] = $"{content.Description}";
                                dataColName.Rows.Add(row);

                                AddTableWithFirstRowHeader(body, dataColName);
                                // sample JSON
                                AddHeading(body, $"Request Sample:", "Heading3", 24, "000000");
                                string exampleJson = content.Example?.ToString() ?? String.Empty;
                                if (content.Schema != null && !content.Examples.Any())
                                {
                                    exampleJson = Convert2MarkDown.GenerateExampleFromSchema(content.Schema);
                                }
                                AddCodeBlock(body, exampleJson);
                                AddLineBreak(body);
                                break;
                            }
                        }

                        // Request
                        if (operation.Value.RequestBody != null && operation.Value.RequestBody.Content != null)
                        {
                            foreach (KeyValuePair<string,OpenApiMediaType> content in operation.Value.RequestBody.Content)
                            {

                                AddHeading(body, $"RequestBody Parameter Form:", "Heading3", 24, "000000");
                                DataTable dataColName = new DataTable();
                                dataColName.Columns.Add("Name");
                                dataColName.Columns.Add("Type");
                                dataColName.Columns.Add("Description");

                                foreach (KeyValuePair<string, OpenApiSchema> param in content.Value.Schema.Properties)
                                {
                                    OpenApiSchema property = param.Value;
                                    bool isRequired = content.Value.Schema.Required.Where(x => x.Equals(param.Key)).Any();
                                    string requiredHint = isRequired ? "( * )" : "";
                                    string refernce = (property.Items?.Reference?.Id != null) ? $"<{property.Items?.Reference?.Id}>" : "";
                                    DataRow row = dataColName.NewRow();
                                    row["Name"] = $"{requiredHint} {param.Key}";
                                    row["Type"] = $"{property.Type} {property.Format} {property.Items?.Format} {property.Reference?.Id} {refernce} ";
                                    row["Description"] = $"{property.Description}";
                                    dataColName.Rows.Add(row);
                                }
                                AddTableWithFirstRowHeader(body, dataColName);


                                // sample JSON
                                AddHeading(body, $"Request Sample JSON:", "Heading3", 24, "000000");
                                string exampleJson = content.Value.Example?.ToString() ?? String.Empty;
                                if (content.Value.Schema != null && !content.Value.Examples.Any())
                                {
                                    exampleJson = Convert2MarkDown.GenerateExampleFromSchema(content.Value.Schema);
                                }
                                AddCodeBlock(body, exampleJson);
                                AddLineBreak(body);
                                break;
                            }
                        }

                        // Response
                        if (operation.Value.Responses != null)
                        {
                            foreach (var response in operation.Value.Responses)
                            {

                                /*AddHeading(body, $"回應狀態表格:", "Heading3", 24, "000000");
                                DataTable dataColName = new DataTable();
                                dataColName.Columns.Add("Code");
                                dataColName.Columns.Add("Description");
                                DataRow row = dataColName.NewRow();
                                row["Code"] = $"{response.Key}";
                                row["Description"] = $"{response.Value.Description}";
                                AddTableWithFirstRowHeader(body, dataColName);
                                AddLineBreak(body);*/

                                // sample JSON
                                if (response.Value.Content != null)
                                {
                                    AddHeading(body, $"Response Parameter Form:", "Heading3", 24, "000000");
                                    DataTable respDataColName = new DataTable();
                                    respDataColName.Columns.Add("Name");
                                    respDataColName.Columns.Add("Type");
                                    respDataColName.Columns.Add("Description");
                                    var schema = response.Value.Content.FirstOrDefault().Value?.Schema;
                                    if (schema != null) 
                                    {
                                        foreach (var param in schema.Properties)
                                        {
                                            var property = param.Value;
                                            bool isRequired = schema.Required.Where(x => x.Equals(param.Key)).Any();
                                            string requiredHint = isRequired ? "( * )" : "";
                                            string refernce = (property.Items?.Reference?.Id != null) ? $"<{property.Items?.Reference?.Id}>" : "";
                                            DataRow respRow = respDataColName.NewRow();
                                            respRow["Name"] = $"{requiredHint} {param.Key}";
                                            respRow["Type"] = $"{property.Type} {property.Format} {property.Items?.Format} {property.Reference?.Id} {refernce} ";
                                            respRow["Description"] = $"{property.Description}";
                                            respDataColName.Rows.Add(respRow);
                                        }
                                        AddTableWithFirstRowHeader(body, respDataColName);
                                        AddLineBreak(body);
                                    }
                                    

                                    foreach (KeyValuePair<string, OpenApiMediaType> content in response.Value.Content)
                                    {

                                        // sample JSON
                                        AddHeading(body, $"Response Sample JSON:", "Heading3", 24, "000000");
                                        string exampleJson = content.Value.Example?.ToString() ?? String.Empty;
                                        if (content.Value.Schema != null && !content.Value.Examples.Any())
                                        {
                                            exampleJson = Convert2MarkDown.GenerateExampleFromSchema(content.Value.Schema);
                                        }
                                        AddCodeBlock(body, exampleJson);
                                        AddLineBreak(body);
                                        break;

                                    }
                                }
                            }
                        }
                    }
                    pathCount++;
                    AddPageBreak(body);
                }


                // Components Schemas
                AddHeading(body, $"Schemes", "Heading2", 36, "000000");
                AddLineBreak(body);
                foreach (KeyValuePair<string, OpenApiSchema> schemas in openApiDocument.Components.Schemas)
                {
                    string name = schemas.Key;
                    GenSchemesTable(body, name, schemas.Value);
                    AddLineBreak(body);
                    AddLineBreak(body);
                }


                // 保存更改
                mainPart.Document.Save();
            }

            Console.WriteLine("Custom .docx file with table created successfully!");

        }

        private void CreateDocBlock1(Body body, OpenApiDocument openApi)
        {
            // 生成Markdown內容
            StringBuilder markdown = new StringBuilder();

            // 標題
            AddHeading(body, openApi.Info.Title, "Heading1", 48, "000000");
            AddLineBreak(body);
            AddLineBreak(body);
            if (String.IsNullOrEmpty(openApi.Info.Description) == false) 
            {
                AddParagraph(body, openApi.Info.Description.Replace("</br>", "\r\n"));
            }
            AddLineBreak(body);

            // 版本
            AddHeading(body, openApi.Info.Version ?? String.Empty, "Heading3", 24, "000000");
            AddLineBreak(body);

            // 服務條款
            if (openApi.Info.TermsOfService != null)
            {
                AddHeading(body, "Terms of service", "Heading3", 24, "000000");
                AddParagraph(body, openApi.Info.License.Name ?? String.Empty);
                AddLineBreak(body);
            }

            // 聯繫信息
            if (openApi.Info.Contact != null)
            {
                AddHeading(body, "Contact information", "Heading4", 18, "000000");
                AddParagraph(body, openApi.Info.Contact.Name ?? String.Empty);
                AddParagraph(body,openApi.Info.Contact.Email ?? String.Empty);
                AddLineBreak(body);
            }

            // 許可證
            if (openApi.Info.License != null)
            {
                AddHeading(body, $"**License:** [{openApi.Info.License}]({openApi.Info.License.Url})", "Heading4", 18, "000000");
                AddLineBreak(body);
            }

        }

        /// <summary>
        /// 產生 SchemesTable
        /// </summary>
        /// <param name="body"></param>
        /// <param name="markdownBuilder"></param>
        /// <param name="name"></param>
        /// <param name="schema"></param>
        private void GenSchemesTable(Body body, string name, OpenApiSchema schema)
        {
            AddHeading(body, name, "Heading3", 20, "000000");
            AddLineBreak(body);
            DataTable dataColName = new DataTable();
            dataColName.Columns.Add("Name");
            dataColName.Columns.Add("Type");
            dataColName.Columns.Add("Description");

            foreach (var param in schema.Properties)
            {
                var property = param.Value;
                bool isRequired = schema.Required.Where(x => x.Equals(param.Key)).Any();
                string requiredHint = isRequired ? "( * )" : "";
                string refernce = (property.Items?.Reference?.Id != null) ? $"<{property.Items?.Reference?.Id}>" : "";
                DataRow row = dataColName.NewRow();
                row["Name"] = $"{requiredHint} {param.Key}";
                row["Type"] = $"{property.Type} {property.Format} {property.Items?.Format} {property.Reference?.Id} {refernce} ";
                row["Description"] = $"{property.Description}";
                dataColName.Rows.Add(row);
            }
            AddTableWithFirstRowHeader(body, dataColName);
        }

        /// <summary>
        /// 產生  Security SchemesTable
        /// </summary>
        /// <param name="body"></param>
        /// <param name="name"></param>
        /// <param name="schema"></param>
        private void GenSecuritySchemeTable(Body body, string name, OpenApiSecurityScheme schema)
        {
            AddHeading(body, $"請求驗證限制:", "Heading3", 24, "000000");
            DataTable dataColName = new DataTable();
            dataColName.Columns.Add("Name");
            dataColName.Columns.Add("Type");
            dataColName.Columns.Add("Description");
            dataColName.Columns.Add("In");

            DataRow row = dataColName.NewRow();
            row["Name"] = $"{schema.Name}";
            row["Type"] = $"{schema.Type}";
            row["Description"] = $"{schema.Description}";
            row["In"] = $"{schema.In}";
            dataColName.Rows.Add(row);
            AddTableWithFirstRowHeader(body, dataColName);
        }

        #region Common Basic Fn
        private void AddHeading(Body body, string text, string styleId, int fontSize, string colorHex)
        {
            Paragraph heading = new Paragraph();
            Run headingRun = new Run();
            Text headingText = new Text(text);
            headingRun.Append(headingText);

            // 字體樣式
            RunProperties runProperties = new RunProperties();
            runProperties.Append(new FontSize() { Val = (fontSize * 2).ToString() });
            runProperties.Append(new Color() { Val = colorHex });
            runProperties.RunFonts = new RunFonts() { Ascii = "Microsoft JhengHei" };
            headingRun.PrependChild(runProperties);

            heading.Append(headingRun);

            ParagraphProperties headingProps = new ParagraphProperties();
            headingProps.Append(new ParagraphStyleId() { Val = styleId });
            heading.Append(headingProps);

            body.Append(heading);
        }
        private Paragraph GetParagraph(string text)
        {
            if (String.IsNullOrEmpty(text)) 
            {
                return new Paragraph();
            }
            RunProperties runProperties = new RunProperties();
            RunFonts runFont = new RunFonts() { Ascii = "Microsoft JhengHei" };
            runProperties.Append(runFont);

            Paragraph paragraph = new Paragraph();
            Run run = new Run();
            run.Append(runProperties);
            string[] lines = text.Split(new[] { "<br>", "</br>", "\n", "\r", "\r\n", "\n\r" }, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                run.Append(new Text(line) { Space = SpaceProcessingModeValues.Preserve });
                // 添加换行符
                run.Append(new Break());
            }
            paragraph.Append(run);
            return paragraph;
        }

        private void AddParagraph(Body body, string text)
        {
            body.Append(GetParagraph(text));
        }

        private void AddTableWithFirstRowHeader(Body body, System.Data.DataTable dataTable)
        {
            Table table = new Table();

            // 表格樣式
            TableProperties tblProps = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                    new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 }
                )
            );
            table.AppendChild(tblProps);

            // add標題
            RunFonts runFont = new RunFonts() { Ascii = "Microsoft JhengHei" };

            TableRow headerRow = new TableRow();
            foreach (DataColumn column in dataTable.Columns)
            {
                TableCell headerCell = new TableCell();

                Paragraph p = GetParagraph(column.ColumnName);

                // 標題樣式
                RunProperties pProps = new RunProperties();
                pProps.Bold = new Bold();
                pProps.Append(new Color() { Val = "000000" }); 
                TableCellWidth tcw = new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "2400" };
                Shading shading = new Shading() { Val = ShadingPatternValues.Clear, Fill = "d1d1d1" }; // background
                headerCell.Append(new TableCellProperties(tcw,shading));
                p.PrependChild(pProps);

                headerCell.Append(p);
                headerRow.Append(headerCell);
            }
            table.Append(headerRow);

            // 資料行
            foreach (DataRow dataRow in dataTable.Rows)
            {
                TableRow tr = new TableRow();
                foreach (var item in dataRow.ItemArray)
                {

                    TableCell tc = new TableCell(GetParagraph(item?.ToString() ?? String.Empty));
                    TableCellWidth tcw1 = new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "2400" };
                    Shading shading1 = new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "FFCCCC" };
                    tc.Append(tcw1, shading1);
                    tr.Append(tc);
                }
                table.Append(tr);
            }

            body.Append(table);
        }

        private void AddLineBreak(Body body)
        {
            body.AppendChild(new Break());
        }

        private void AddPageBreak(Body body)
        {
            Paragraph pageBreakParagraph = new Paragraph(new Run(new Break() { Type = BreakValues.Page }));
            body.Append(pageBreakParagraph);
        }

        private void AddCodeBlock(Body body, string code)
        {
            Paragraph codeParagraph = new Paragraph();
            Run codeRun = new Run();

            // 樣式設定
            RunProperties runProps = new RunProperties();
            RunFonts runFont = new RunFonts() { Ascii = "Microsoft JhengHei,Courier New" }; // 使用等宽字体
            runProps.Append(runFont);
            codeRun.Append(runProps);
            codeRun.Append(new Break());

            string[] lines = code.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                codeRun.Append(new Text(line) { Space = SpaceProcessingModeValues.Preserve });
                // 添加换行符
                codeRun.Append(new Break());
            }

            // 设置背景颜色和边框
            ParagraphProperties paragraphProps = new ParagraphProperties();
            Shading shading = new Shading()
            {
                Color = "auto",
                Fill = "e9e9e9", // 背景颜色 (灰色)
                Val = ShadingPatternValues.Clear
            };
            paragraphProps.Append(shading);

            // 添加边框
            ParagraphBorders paragraphBorders = new ParagraphBorders();
            paragraphBorders.TopBorder = new TopBorder() { Val = BorderValues.Single, Size = 4 };
            paragraphBorders.BottomBorder = new BottomBorder() { Val = BorderValues.Single, Size = 4 };
            paragraphBorders.LeftBorder = new LeftBorder() { Val = BorderValues.Single, Size = 4 };
            paragraphBorders.RightBorder = new RightBorder() { Val = BorderValues.Single, Size = 4 };
            paragraphProps.Append(paragraphBorders);

            // 模拟内边距
            //SpacingBetweenLines spacingBetweenLines = new SpacingBetweenLines() { Before = "200", After = "200" };
            // Indentation indentation = new Indentation() { Left = "200", Right = "200" };
            //paragraphProps.Append(spacingBetweenLines);
            // paragraphProps.Append(indentation);

            codeParagraph.Append(paragraphProps);
            codeParagraph.Append(codeRun);
            body.Append(codeParagraph);
        }
        #endregion

    }
}
