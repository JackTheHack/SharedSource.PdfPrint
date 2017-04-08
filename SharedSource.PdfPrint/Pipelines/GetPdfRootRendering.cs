using System.Diagnostics;
using Sitecore.Data;
using Sitecore.Mvc.Pipelines.Response.GetPageRendering;

namespace SharedSource.PdfPrint.Pipelines
{
    public class GetPdfRootRendering : GetPageRenderingProcessor
    {
        private readonly ID _pdfRenderingId = ID.Parse("{7987A8BB-B7CF-4260-A6E2-08F1C52392A7}");

        public override void Process(GetPageRenderingArgs args)
        {
            if (Sitecore.Context.Device.ID == _pdfRenderingId &&
                !(Sitecore.Context.PageMode.IsPreview || Sitecore.Context.PageMode.IsExperienceEditor))
            {            
                Trace.WriteLine("Creating PDF rendering...");
                args.Result = new PdfRendering(args.Result);
            }
        }
    }
}