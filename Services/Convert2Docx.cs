using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.OpenApi.Models;
using System.Data;

namespace Swagger2Doc.Services
{
    /// <summary>
    /// OpenApiDocument 轉 Word（.docx）：封面 → API 目錄 → 安全性驗證 → API 明細 → 資料模型
    /// 版面統一於此：A4、頁面邊界、標題字級、表格框線與表頭
    /// </summary>
    public class Convert2Docx
    {
        // ===== 版面常數（半點為單位，Word 的 FontSize 以半點計）=====
        private const string FontLatin = "Segoe UI";
        private const string FontEastAsia = "Microsoft JhengHei";
        private const string FontMono = "Consolas";

        private const string SizeBody = "21";      // 10.5pt 內文
        private const string SizeTable = "18";     // 9pt 表格
        private const string SizeCode = "17";      // 8.5pt 程式碼

        private const string ColorHeading = "1F3864";
        private const string ColorBorder = "808080";
        private const string ColorBorderOuter = "595959";
        private const string FillHeader = "E8E8E8";
        private const string FillCode = "F5F5F5";

        public void CreateDoc(OpenApiDocument openApiDocument, string filePath)
        {
            string? directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document();
                AddStyles(mainPart);

                // 文件設定：開啟時更新欄位（頁尾總頁數才會是實際頁數）；
                // 宣告相容性版本，Word 才不會以「相容模式」開啟
                DocumentSettingsPart settingsPart = mainPart.AddNewPart<DocumentSettingsPart>();
                settingsPart.Settings = new Settings(
                    new UpdateFieldsOnOpen { Val = true },
                    new Compatibility(
                        new CompatibilitySetting
                        {
                            Name = CompatSettingNameValues.CompatibilityMode,
                            Uri = "http://schemas.microsoft.com/office/word",
                            Val = "15"
                        }));
                settingsPart.Settings.Save();

                Body body = new Body();
                mainPart.Document.Append(body);

                List<ApiEntry> entries = ApiEntry.Collect(openApiDocument);

                // 封面
                AddCover(body, openApiDocument);
                AddPageBreak(body);

                // 文件索引
                AddIndex(body, entries);
                AddPageBreak(body);

                // 安全性驗證
                AddSecuritySchemes(body, openApiDocument.Components?.SecuritySchemes);

                // API 明細（一支一頁）
                AddHeading(body, "API 明細", "Heading1");
                foreach (ApiEntry entry in entries)
                {
                    AddOperation(body, entry);
                    AddPageBreak(body);
                }

                // 資料模型
                AddSchemas(body, openApiDocument.Components?.Schemas);

                // 版面設定（A4，上下 2cm、左右 1.8cm；須為 body 最後一個元素）
                string footerId = AddFooter(mainPart);
                body.Append(new SectionProperties(
                    new FooterReference { Type = HeaderFooterValues.Default, Id = footerId },
                    new PageSize { Width = 11906U, Height = 16838U },
                    new PageMargin { Top = 1134, Right = 1021U, Bottom = 1134, Left = 1021U, Header = 720U, Footer = 720U, Gutter = 0U }));

                mainPart.Document.Save();
            }

            Console.WriteLine($"docx created: {filePath}");
        }

        #region 章節

        private void AddCover(Body body, OpenApiDocument openApi)
        {
            AddHeading(body, openApi.Info?.Title ?? "API 規格", "Title");

            List<string> meta = new List<string>();
            if (!string.IsNullOrEmpty(openApi.Info?.Version))
            {
                meta.Add($"版本 {openApi.Info.Version}");
            }
            meta.Add($"產生日期 {DateTime.Now:yyyy-MM-dd}");
            AddParagraph(body, string.Join("　｜　", meta));

            string description = OpenApiText.Block(openApi.Info?.Description);
            if (!string.IsNullOrEmpty(description))
            {
                AddParagraph(body, description);
            }

            if (openApi.Servers != null && openApi.Servers.Count > 0)
            {
                AddHeading(body, "服務位址", "Heading2");
                DataTable table = NewTable("位址", "說明");
                foreach (OpenApiServer server in openApi.Servers)
                {
                    table.Rows.Add(OpenApiText.Plain(server.Url), OpenApiText.Plain(server.Description));
                }
                AddTable(body, table, new[] { 45, 55 });
            }

            if (openApi.Info?.Contact != null || openApi.Info?.License != null)
            {
                AddHeading(body, "文件資訊", "Heading2");
                if (openApi.Info?.Contact != null)
                {
                    AddParagraph(body, $"聯絡窗口：{openApi.Info.Contact.Name} {openApi.Info.Contact.Email}".TrimEnd());
                }
                if (openApi.Info?.License != null && !string.IsNullOrEmpty(openApi.Info.License.Name))
                {
                    AddParagraph(body, $"授權：{openApi.Info.License.Name}");
                }
            }
        }

        /// <summary>
        /// 文件索引：一覽全部端點（編號與內文章節編號一致）
        /// </summary>
        private void AddIndex(Body body, List<ApiEntry> entries)
        {
            AddHeading(body, "文件索引", "Heading1");

            DataTable table = NewTable("#", "方法", "路徑", "說明");
            foreach (ApiEntry entry in entries)
            {
                table.Rows.Add(entry.Index.ToString(), entry.Method, entry.Path, entry.Summary);
            }
            AddTable(body, table, new[] { 7, 12, 43, 38 });
        }

        private void AddSecuritySchemes(Body body, IDictionary<string, OpenApiSecurityScheme>? schemes)
        {
            if (schemes == null || schemes.Count == 0)
            {
                return;
            }

            AddHeading(body, "安全性驗證", "Heading1");

            DataTable table = NewTable("名稱", "參數", "類型", "位置", "說明");
            foreach (KeyValuePair<string, OpenApiSecurityScheme> scheme in schemes)
            {
                table.Rows.Add(
                    scheme.Key,
                    OpenApiText.Plain(scheme.Value.Name),
                    scheme.Value.Type.ToString(),
                    scheme.Value.In.ToString(),
                    OpenApiText.Plain(scheme.Value.Description));
            }
            AddTable(body, table, new[] { 16, 16, 12, 12, 44 });
            AddPageBreak(body);
        }

        private void AddOperation(Body body, ApiEntry entry)
        {
            OpenApiOperation operation = entry.Operation;

            AddHeading(body, $"{entry.Index}. [{entry.Method}] {entry.Path}", "Heading2");

            if (!string.IsNullOrEmpty(entry.Summary))
            {
                AddParagraph(body, $"說明：{entry.Summary}");
            }
            if (entry.Tags.Count > 0)
            {
                AddParagraph(body, $"分類：{string.Join("、", entry.Tags)}");
            }
            if (!string.IsNullOrEmpty(operation.OperationId))
            {
                AddParagraph(body, $"代號：{operation.OperationId}");
            }
            if (operation.Deprecated)
            {
                AddParagraph(body, "狀態：已淘汰（deprecated）");
            }

            string detail = OpenApiText.Block(operation.Description);
            if (!string.IsNullOrEmpty(detail))
            {
                AddParagraph(body, detail);
            }

            // 請求參數（Path / Query / Header）：一張表列全部
            if (operation.Parameters != null && operation.Parameters.Count > 0)
            {
                AddHeading(body, "請求參數（Path / Query / Header）", "Heading3");
                DataTable table = NewTable("參數", "位置", "類型", "必填", "說明");
                foreach (OpenApiParameter parameter in operation.Parameters)
                {
                    table.Rows.Add(
                        OpenApiText.Plain(parameter.Name),
                        parameter.In.ToString(),
                        OpenApiText.TypeText(parameter.Schema),
                        parameter.Required ? "是" : string.Empty,
                        OpenApiText.DescriptionCell(parameter.Description, parameter.Schema));
                }
                AddTable(body, table, new[] { 20, 12, 18, 8, 42 });
            }

            // 請求內容
            if (operation.RequestBody?.Content != null && operation.RequestBody.Content.Count > 0)
            {
                KeyValuePair<string, OpenApiMediaType> content = operation.RequestBody.Content.First();
                AddHeading(body, $"請求內容（{content.Key}）", "Heading3");
                AddSchemaTable(body, content.Value?.Schema);
                AddSample(body, "請求範例", content.Value);
            }

            // 回應
            if (operation.Responses != null && operation.Responses.Count > 0)
            {
                AddHeading(body, "回應狀態", "Heading3");
                DataTable table = NewTable("狀態碼", "說明", "內容型別");
                foreach (KeyValuePair<string, OpenApiResponse> response in operation.Responses)
                {
                    string mediaTypes = (response.Value.Content == null || response.Value.Content.Count == 0)
                        ? "—"
                        : string.Join("、", response.Value.Content.Keys);
                    table.Rows.Add(response.Key, OpenApiText.Plain(response.Value.Description), mediaTypes);
                }
                AddTable(body, table, new[] { 14, 56, 30 });

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

                    AddHeading(body, $"回應內容 {response.Key}（{content.Key}）", "Heading3");
                    AddSchemaTable(body, content.Value.Schema);
                    AddSample(body, $"回應範例 {response.Key}", content.Value);
                }
            }
        }

        private void AddSchemas(Body body, IDictionary<string, OpenApiSchema>? schemas)
        {
            if (schemas == null || schemas.Count == 0)
            {
                return;
            }

            AddHeading(body, "資料模型", "Heading1");

            List<KeyValuePair<string, OpenApiSchema>> ordered =
                schemas.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase).ToList();

            DataTable index = NewTable("模型", "欄位數");
            foreach (KeyValuePair<string, OpenApiSchema> schema in ordered)
            {
                index.Rows.Add(schema.Key, (schema.Value.Properties?.Count ?? 0).ToString());
            }
            AddTable(body, index, new[] { 70, 30 });
            AddPageBreak(body);

            foreach (KeyValuePair<string, OpenApiSchema> schema in ordered)
            {
                AddHeading(body, schema.Key, "Heading2");
                string desc = OpenApiText.Plain(schema.Value.Description);
                if (!string.IsNullOrEmpty(desc))
                {
                    AddParagraph(body, desc);
                }
                AddSchemaTable(body, schema.Value);
                AddEmptyParagraph(body);
            }
        }

        private void AddSchemaTable(Body body, OpenApiSchema? schema)
        {
            if (schema == null)
            {
                return;
            }

            if (schema.Properties == null || schema.Properties.Count == 0)
            {
                AddParagraph(body, $"型別：{OpenApiText.TypeText(schema)} {OpenApiText.ConstraintText(schema)}".TrimEnd());
                return;
            }

            DataTable table = NewTable("欄位", "類型", "必填", "說明");
            foreach (KeyValuePair<string, OpenApiSchema> property in schema.Properties)
            {
                bool isRequired = schema.Required != null && schema.Required.Contains(property.Key);
                table.Rows.Add(
                    property.Key,
                    OpenApiText.TypeText(property.Value),
                    isRequired ? "是" : string.Empty,
                    OpenApiText.DescriptionCell(property.Value?.Description, property.Value));
            }
            AddTable(body, table, new[] { 22, 20, 8, 50 });
        }

        private void AddSample(Body body, string caption, OpenApiMediaType? mediaType)
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

            AddHeading(body, $"{caption}：", "Heading3");
            AddCodeBlock(body, sample);
        }

        #endregion

        #region 基礎排版元件

        /// <summary>
        /// 文件預設字型與標題樣式（未定義時 Word 會落回 Calibri，中文顯示不一致）
        /// </summary>
        private void AddStyles(MainDocumentPart mainPart)
        {
            StyleDefinitionsPart stylePart = mainPart.AddNewPart<StyleDefinitionsPart>();
            Styles styles = new Styles();

            styles.Append(new DocDefaults(
                new RunPropertiesDefault(new RunPropertiesBaseStyle(
                    new RunFonts { Ascii = FontLatin, HighAnsi = FontLatin, EastAsia = FontEastAsia },
                    new FontSize { Val = SizeBody },
                    new FontSizeComplexScript { Val = SizeBody })),
                new ParagraphPropertiesDefault(new ParagraphPropertiesBaseStyle(
                    new SpacingBetweenLines { After = "80", Line = "280", LineRule = LineSpacingRuleValues.Auto }))));

            styles.Append(new Style(
                new StyleName { Val = "Normal" },
                new PrimaryStyle())
            { Type = StyleValues.Paragraph, StyleId = "Normal", Default = true });

            // 標題字級：文件標題 20pt、章節 15pt、單支 API 12.5pt、小節 10.5pt
            styles.Append(HeadingStyle("Title", "Title", "40", 0, "360", "160"));
            styles.Append(HeadingStyle("Heading1", "heading 1", "30", 0, "320", "140"));
            styles.Append(HeadingStyle("Heading2", "heading 2", "25", 1, "260", "120"));
            styles.Append(HeadingStyle("Heading3", "heading 3", "21", 2, "200", "100"));

            stylePart.Styles = styles;
            stylePart.Styles.Save();
        }

        private Style HeadingStyle(string styleId, string name, string halfPointSize, int outlineLevel, string spaceBefore, string spaceAfter)
        {
            return new Style(
                new StyleName { Val = name },
                new BasedOn { Val = "Normal" },
                new NextParagraphStyle { Val = "Normal" },
                new PrimaryStyle(),
                new StyleParagraphProperties(
                    new KeepNext(),
                    new SpacingBetweenLines { Before = spaceBefore, After = spaceAfter },
                    new OutlineLevel { Val = outlineLevel }),
                new StyleRunProperties(
                    new RunFonts { Ascii = FontLatin, HighAnsi = FontLatin, EastAsia = FontEastAsia },
                    new Bold(),
                    new Color { Val = ColorHeading },
                    new FontSize { Val = halfPointSize },
                    new FontSizeComplexScript { Val = halfPointSize }))
            { Type = StyleValues.Paragraph, StyleId = styleId };
        }

        /// <summary>
        /// 頁尾頁碼（第 X 頁，共 Y 頁）；回傳 sectPr 參照用的 relationship id
        /// </summary>
        private string AddFooter(MainDocumentPart mainPart)
        {
            FooterPart footerPart = mainPart.AddNewPart<FooterPart>();

            RunProperties FooterRunProperties() => new RunProperties(
                new RunFonts { Ascii = FontLatin, HighAnsi = FontLatin, EastAsia = FontEastAsia },
                new FontSize { Val = "16" },
                new FontSizeComplexScript { Val = "16" },
                new Color { Val = ColorBorder });

            Paragraph paragraph = new Paragraph(
                new ParagraphProperties(new Justification { Val = JustificationValues.Center }),
                new Run(FooterRunProperties(), new Text("第 ") { Space = SpaceProcessingModeValues.Preserve }),
                new Run(FooterRunProperties(), new SimpleField(new Run(new Text("1"))) { Instruction = "PAGE" }),
                new Run(FooterRunProperties(), new Text(" 頁，共 ") { Space = SpaceProcessingModeValues.Preserve }),
                new Run(FooterRunProperties(), new SimpleField(new Run(new Text("1"))) { Instruction = "NUMPAGES" }),
                new Run(FooterRunProperties(), new Text(" 頁") { Space = SpaceProcessingModeValues.Preserve }));

            footerPart.Footer = new Footer(paragraph);
            footerPart.Footer.Save();

            return mainPart.GetIdOfPart(footerPart);
        }

        private void AddHeading(Body body, string text, string styleId)
        {
            Paragraph heading = new Paragraph(
                new ParagraphProperties(new ParagraphStyleId { Val = styleId }),
                new Run(new Text(text) { Space = SpaceProcessingModeValues.Preserve }));
            body.Append(heading);
        }

        private void AddParagraph(Body body, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }
            body.Append(TextParagraph(text, bold: false, fontSize: SizeBody, mono: false));
        }

        private void AddEmptyParagraph(Body body)
        {
            body.Append(new Paragraph());
        }

        private void AddPageBreak(Body body)
        {
            body.Append(new Paragraph(new Run(new Break { Type = BreakValues.Page })));
        }

        /// <summary>
        /// 多行文字段落（換行以 w:br 表示，維持在同一段落內）
        /// </summary>
        private Paragraph TextParagraph(string text, bool bold, string fontSize, bool mono)
        {
            RunProperties runProperties = new RunProperties(
                new RunFonts
                {
                    Ascii = mono ? FontMono : FontLatin,
                    HighAnsi = mono ? FontMono : FontLatin,
                    EastAsia = mono ? FontMono : FontEastAsia
                },
                new FontSize { Val = fontSize },
                new FontSizeComplexScript { Val = fontSize });
            if (bold)
            {
                runProperties.Append(new Bold());
            }

            Run run = new Run();
            run.Append(runProperties);

            string[] lines = (text ?? string.Empty).Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                if (i > 0)
                {
                    run.Append(new Break());
                }
                run.Append(new Text(lines[i]) { Space = SpaceProcessingModeValues.Preserve });
            }

            return new Paragraph(run);
        }

        private DataTable NewTable(params string[] columns)
        {
            DataTable table = new DataTable();
            foreach (string column in columns)
            {
                table.Columns.Add(column);
            }
            return table;
        }

        /// <summary>
        /// 表格：滿版寬度、全框線、表頭灰底並跨頁重複
        /// </summary>
        private void AddTable(Body body, DataTable dataTable, int[] columnPercents)
        {
            Table table = new Table();

            table.AppendChild(new TableProperties(
                new TableWidth { Type = TableWidthUnitValues.Pct, Width = "5000" },
                new TableBorders(
                    new TopBorder { Val = BorderValues.Single, Size = 8, Color = ColorBorderOuter },
                    new BottomBorder { Val = BorderValues.Single, Size = 8, Color = ColorBorderOuter },
                    new LeftBorder { Val = BorderValues.Single, Size = 8, Color = ColorBorderOuter },
                    new RightBorder { Val = BorderValues.Single, Size = 8, Color = ColorBorderOuter },
                    new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4, Color = ColorBorder },
                    new InsideVerticalBorder { Val = BorderValues.Single, Size = 4, Color = ColorBorder }),
                new TableLayout { Type = TableLayoutValues.Fixed }));

            // 表頭
            TableRow headerRow = new TableRow(new TableRowProperties(new TableHeader()));
            int columnIndex = 0;
            foreach (DataColumn column in dataTable.Columns)
            {
                headerRow.Append(BuildCell(column.ColumnName, ColumnWidth(columnPercents, columnIndex), isHeader: true));
                columnIndex++;
            }
            table.Append(headerRow);

            // 資料列
            foreach (DataRow dataRow in dataTable.Rows)
            {
                TableRow row = new TableRow();
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    row.Append(BuildCell(dataRow[i]?.ToString() ?? string.Empty, ColumnWidth(columnPercents, i), isHeader: false));
                }
                table.Append(row);
            }

            body.Append(table);
            AddEmptyParagraph(body);
        }

        private string ColumnWidth(int[] columnPercents, int index)
        {
            int percent = (columnPercents != null && index < columnPercents.Length) ? columnPercents[index] : 0;
            if (percent <= 0)
            {
                percent = 20;
            }
            return (percent * 50).ToString();
        }

        private TableCell BuildCell(string text, string widthPct, bool isHeader)
        {
            TableCellProperties cellProperties = new TableCellProperties(
                new TableCellWidth { Type = TableWidthUnitValues.Pct, Width = widthPct },
                new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Top });

            if (isHeader)
            {
                cellProperties.Append(new Shading { Val = ShadingPatternValues.Clear, Color = "auto", Fill = FillHeader });
            }

            return new TableCell(cellProperties, TextParagraph(text, isHeader, SizeTable, mono: false));
        }

        /// <summary>
        /// 程式碼區塊：等寬字、淺灰底、細框線
        /// </summary>
        private void AddCodeBlock(Body body, string code)
        {
            Paragraph paragraph = TextParagraph(code, bold: false, fontSize: SizeCode, mono: true);

            ParagraphProperties properties = new ParagraphProperties(
                new Shading { Val = ShadingPatternValues.Clear, Color = "auto", Fill = FillCode },
                new ParagraphBorders(
                    new TopBorder { Val = BorderValues.Single, Size = 4, Color = ColorBorder },
                    new BottomBorder { Val = BorderValues.Single, Size = 4, Color = ColorBorder },
                    new LeftBorder { Val = BorderValues.Single, Size = 4, Color = ColorBorder },
                    new RightBorder { Val = BorderValues.Single, Size = 4, Color = ColorBorder }),
                new SpacingBetweenLines { Before = "60", After = "120", Line = "240", LineRule = LineSpacingRuleValues.Auto });

            paragraph.PrependChild(properties);
            body.Append(paragraph);
        }

        #endregion
    }
}
