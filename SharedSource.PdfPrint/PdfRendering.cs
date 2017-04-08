using System.Diagnostics;
using Sitecore.Mvc.Presentation;

namespace SharedSource.PdfPrint
{
    public class PdfRendering : Rendering
    {
        private readonly Rendering _baseRendering;

        public PdfRendering(Rendering baseRendering)
        {
            _baseRendering = baseRendering;
        }

        protected override Renderer GetRenderer()
        {
            var baseRenderer = _baseRendering.Renderer;
            Trace.WriteLine("Get renderer - " + baseRenderer?.GetType().Name);
            return new PdfRenderer(baseRenderer);
        }
    }
}