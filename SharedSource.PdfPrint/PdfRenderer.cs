using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Web;
using NReco.PdfGenerator;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Mvc.Presentation;

namespace SharedSource.PdfPrint
{
    public class PdfRenderer : Renderer
    {
        private readonly Renderer _baseRenderer;

        public PdfRenderer(Renderer baseRenderer)
        {
            _baseRenderer = baseRenderer;
        }

        public override void Render(TextWriter writer)
        {
            var httpWriter = writer as HttpWriter;

            if (httpWriter == null)
            {
                return;
            }

            Trace.WriteLine("Starting Pdf Renderer");

            var textWriter = new StringWriter();
            _baseRenderer.Render(textWriter);
            string renderedView = textWriter.ToString();

            Trace.WriteLine("Rendered HTML to string");

            var htmlToPdf = CreatePdfConverter();
            var pdfContents = htmlToPdf.GeneratePdf(renderedView);

            Trace.WriteLine("Produced PDF");            

            HttpContext.Current.Response.AddHeader("Content-Type", "application/pdf");
            HttpContext.Current.Response.AddHeader("Content-Disposition", $"attachment; filename ={Sitecore.Context.Item.Name}.pdf");
            HttpContext.Current.Response.AddHeader("Content-Length", pdfContents.Length.ToString());

            httpWriter.WriteBytes(pdfContents, 0, pdfContents.Length);

            Trace.WriteLine("Render completed");
        }

        public HtmlToPdfConverter CreatePdfConverter()
        {
            var settings = Sitecore.Context.Database.GetItem(ID.Parse("{C190212A-4AE6-40E7-B451-649F314E7611}"));

            var marginsField = settings["PageMargins"];
            var marginValues = marginsField?.Split(',');

            return new HtmlToPdfConverter()
            {
                GenerateToc = ((CheckboxField)settings.Fields["GenerateTOC"]).Checked,
                TocHeaderText = !string.IsNullOrEmpty(settings["TOCHeader"]) ? settings["TOCHeader"] : null,
                Margins = marginValues!= null && marginValues.Length ==4 ? new PageMargins() { Top = int.Parse(marginValues[0]), Right = int.Parse(marginValues[1]), Bottom = int.Parse(marginValues[2]), Left = int.Parse(marginValues[3])}  : null,
                Size = !string.IsNullOrEmpty(settings["PageSize"]) ? (PageSize)Enum.Parse(typeof(PageSize), settings["PageSize"]) : PageSize.Default,
                Orientation = !string.IsNullOrEmpty(settings["Orientation"])?(PageOrientation)Enum.Parse(typeof(PageOrientation), settings["Orientation"]): PageOrientation.Default,
                PageHeaderHtml = !string.IsNullOrEmpty(settings["PageHeader"]) ? settings["PageHeader"] : null,
                PageFooterHtml= !string.IsNullOrEmpty(settings["PageFooter"]) ? settings["PageFooter"] : null,
                PageWidth = !string.IsNullOrEmpty(settings["PageWidth"]) ? int.Parse(settings["PageWidth"]) : (float?)null,
                PageHeight= !string.IsNullOrEmpty(settings["PageHeight"]) ? int.Parse(settings["PageHeight"]) : (float?)null,
            };
        }
    }
}