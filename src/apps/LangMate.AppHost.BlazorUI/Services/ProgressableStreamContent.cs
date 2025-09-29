using System.Net;

namespace LangMate.AppHost.BlazorUI.Services
{
    public class ProgressableStreamContent(Stream content, long contentSize, Action<double> progress) : HttpContent
    {
        private const int defaultBufferSize = 4096;
        private readonly Stream content = content;
        private readonly long contentSize = contentSize;
        private readonly Action<double> progress = progress;

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext? context)
        {
            var buffer = new byte[defaultBufferSize];
            long uploaded = 0;
            int read;

            while ((read = await content.ReadAsync(buffer)) > 0)
            {
                await stream.WriteAsync(buffer.AsMemory(0, read));
                uploaded += read;
                double progressPercentage = uploaded * 100.0 / contentSize;
                progress(progressPercentage);
            }
        }

        protected override bool TryComputeLength(out long length)
        {
            length = contentSize;
            return true;
        }
    }

}