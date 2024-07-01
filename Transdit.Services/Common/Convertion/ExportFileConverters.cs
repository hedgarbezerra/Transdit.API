using HtmlAgilityPack;
using NPOI.SS.Formula;
using NPOI.XWPF.UserModel;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Text;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using Transdit.Core.Contracts;
using Transdit.Core.Domain;
using Transdit.Core.Models.Transcription;

namespace Transdit.Services.Common.Convertion
{
    [ExcludeFromCodeCoverage]
    public partial class ExportFileConvertion
    {
        private class Pdf : ExportFileConverter
        {
            private IEmailFromFileGenerator _mailGenerator;

            public Pdf(IEmailFromFileGenerator mailGenerator)
            {
                _mailGenerator = mailGenerator;
            }

            public override MemoryStream Convert(TranscriptionOperationResult transcription, bool detailed = false)
            {
                var ms = new MemoryStream();
                string html = _mailGenerator.Generate("\\Html\\transcricao-resultado.html");
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);
                var htmlTitle = GetHeader(transcription);
                var htmlContent = GetContent(transcription, detailed);
                html = HandleHtml(htmlDoc, htmlTitle, htmlContent);

                var config = new PdfGenerateConfig() { MarginBottom = 40, MarginLeft = 25, MarginRight = 25, MarginTop = 30, PageSize = PdfSharp.PageSize.A4 };
                PdfDocument pdf = PdfGenerator.GeneratePdf(html, config);
                AddInfo(pdf, transcription);
                AddPages(pdf);
                pdf.Save(ms, false);

                return ms;
            }

            public override MemoryStream Convert(string content)
            {
                var ms = new MemoryStream();
                string html = _mailGenerator.Generate("\\Html\\transcricao-resultado.html");
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);
                var htmlTitle = $"Transcrição exportada em {DateTime.Now}";
                html = HandleHtml(htmlDoc, htmlTitle, content);

                var config = new PdfGenerateConfig() { MarginBottom = 40, MarginLeft = 25, MarginRight = 25, MarginTop = 30, PageSize = PdfSharp.PageSize.A4 };
                PdfDocument pdf = PdfGenerator.GeneratePdf(html, config);
                AddInfo(pdf);
                AddPages(pdf);
                pdf.Save(ms, false);

                return ms;
            }
            private string HandleHtml(HtmlDocument html, string header, string content)
            {
                var textoPrincipal = html.DocumentNode.Descendants("div").FirstOrDefault(a => a.HasClass("replace-titulo"));
                if (textoPrincipal is not null)
                    textoPrincipal.InnerHtml = header;

                var conteudo = html.DocumentNode.Descendants("div").FirstOrDefault(a => a.HasClass("replace-conteudo"));
                if (conteudo is not null)
                    conteudo.InnerHtml = content;


                return html.DocumentNode.OuterHtml;
            }
            private void AddPages(PdfDocument pdf)
            {
                XFont font = new XFont("Arial", 10);
                XBrush brush = XBrushes.Black;
                for (int i = 0; i < pdf.Pages.Count; ++i)
                {
                    PdfPage page = pdf.Pages[i];
                    XRect layoutRectangle = new XRect(0, page.Height - font.Height, page.Width, font.Height);

                    using (XGraphics gfx = XGraphics.FromPdfPage(page))
                    {
                        gfx.DrawString($"Página {i+1} de {pdf.PageCount}", font, brush, layoutRectangle, XStringFormats.Center);
                    }
                }
            }
            private void AddInfo(PdfDocument pdf, TranscriptionOperationResult transcription = null)
            {
                pdf.Info.Author = "Transdit";
                pdf.Info.CreationDate = DateTime.Now;
                if(transcription != null)
                {
                    pdf.Info.Subject = $"Transcription on {transcription.FileName}";
                    pdf.Info.Title = GetHeader(transcription);
                }
            }

        }

        private class Docx : ExportFileConverter
        {
            private readonly string _rootPath;

            public Docx(string rootPath)
            {
                _rootPath = Path.Combine(rootPath, "Temp", "Export");
            }

            public override MemoryStream Convert(TranscriptionOperationResult transcription, bool detailed = false)
            {
                string tempFilePath = Path.Combine(_rootPath, Path.GetRandomFileName());
                Stream tempFile = File.OpenWrite(tempFilePath);
                using (XWPFDocument doc = new XWPFDocument())
                {
                    var title = GetHeader(transcription);
                    var content = GetContent(transcription, detailed);
                    AddContentToDocument(doc, title, content);

                    doc.Write(tempFile);
                }
                var bytes = File.ReadAllBytes(tempFilePath);
                var output = new MemoryStream(bytes);

                File.Delete(tempFilePath);
                return output;
            }

            public override MemoryStream Convert(string content)
            {
                string tempFilePath = Path.Combine(_rootPath, Path.GetRandomFileName());
                using (Stream tempFile = File.OpenWrite(tempFilePath))
                using (XWPFDocument doc = new XWPFDocument())
                {
                    var title = $"Transcrição exportada em {DateTime.Now}";
                    AddContentToDocument(doc, title, content);

                    doc.Write(tempFile);
                }
                var bytes = File.ReadAllBytes(tempFilePath);
                var output = new MemoryStream(bytes);

                File.Delete(tempFilePath);
                return output;
            }
            private void AddContentToDocument(XWPFDocument doc, string title, string content)
            {
                XWPFParagraph header = doc.CreateParagraph();
                header.Alignment = ParagraphAlignment.LEFT;
                var headerContent = header.CreateRun();
                headerContent.FontFamily = DefaultFont;
                headerContent.FontSize = 14;
                headerContent.SetText(title);

                XWPFParagraph body = doc.CreateParagraph();
                body.Alignment = ParagraphAlignment.LEFT;
                var bodyContent = header.CreateRun();
                bodyContent.FontFamily = DefaultFont;
                bodyContent.FontSize = 12;
                bodyContent.SetText($"\n\n{content} \n");

                XWPFFootnote footer = doc.CreateFootnote();
                var footerParagraph = footer.CreateParagraph();
                footerParagraph.Alignment = ParagraphAlignment.CENTER;
                var footerContent = footerParagraph.CreateRun();
                footerContent.FontFamily = DefaultFont;
                footerContent.FontSize = 12;
                footerContent.SetText(Footer);
            }
        }

        private class Txt : ExportFileConverter
        {
            public override MemoryStream Convert(TranscriptionOperationResult transcription, bool detailed = false)
            {
                var sb = new StringBuilder();
                sb.AppendLine(GetHeader(transcription));
                sb.AppendLine();
                sb.AppendLine(GetContent(transcription, detailed));
                sb.Append("\n\n");
                sb.AppendLine(Footer);

                byte[] byteArray = Encoding.UTF8.GetBytes(sb.ToString());
                MemoryStream stream = new MemoryStream(byteArray);

                return stream;
            }
           
            public override MemoryStream Convert(string content)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Transcrição exportada em {DateTime.Now}");
                sb.AppendLine();
                sb.AppendLine(content);
                sb.Append("\n\n");
                sb.AppendLine(Footer);

                byte[] byteArray = Encoding.UTF8.GetBytes(sb.ToString());
                MemoryStream stream = new MemoryStream(byteArray);

                return stream;
            }
        }
    }
    internal abstract class ExportFileConverter
    {
        protected static string DefaultFont => "Times New Roman";
        protected virtual string Footer => $"Conteúdo gerado por Transdit LTDA. {DateTime.Now.Year}";
        public abstract MemoryStream Convert(TranscriptionOperationResult transcription, bool detailed = false);
        public abstract MemoryStream Convert(string content);
        protected virtual string GetHeader(TranscriptionOperationResult transcription) 
            => $"Transcrição do arquivo {transcription.FileName} feita em: {transcription.Date.ToString("dd/MM/yyyy")}";
        protected virtual string GetContent(TranscriptionOperationResult transcription, bool detailed)
        {
            var sb = new StringBuilder();
            foreach (var item in transcription.Data)
            {
                if (detailed)
                {
                    var startTime = TimeSpan.FromSeconds(item.StartTimeSeconds);
                    var endTime = TimeSpan.FromSeconds(item.EndTimeSeconds);
                    sb.AppendLine($"Participante {item.SpeakerTag.ToString()} entre: {startTime.ToString("c")} - {endTime.ToString("c")}");
                }
                sb.AppendLine($"({item.Precision * 100}%) - {item.Text}");
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
