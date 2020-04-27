using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Threading;

namespace ChatSample
{
    public delegate void ProgressDelegate(long sendBytes, long totalBytes);

    public class ProgressStreamContent : HttpContent
    {
        ProgressDelegate _progress;
        public ProgressDelegate Progress
        {
            get { return _progress; }
            set
            {
                if (value == null) _progress = delegate { };
                else _progress = value;
            }
        }

        private const int defaultBufferSize = 5120;

        private Stream content;
        private int bufferSize;
        private bool contentConsumed;
        int total;

        public ProgressStreamContent(Stream _content)
        {
            content = _content;
            bufferSize = defaultBufferSize;
            total = 0;
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        { 
            PrepareContent();

            return Task.Run(() =>
            {
                var buffer = new Byte[this.bufferSize];
                var size = content.Length;

                using (content) while (true)
                    {
                        var length = content.Read(buffer, 0, buffer.Length);
                        if (length <= 0) break;

                        stream.Write(buffer, 0, length);
                        total += length;
                        Progress(total, size);
                    }
            });
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                content.Dispose();
                Progress = null;
            }
            base.Dispose(disposing);
        }


        private void PrepareContent()
        {
            if (contentConsumed)
            {
                // If the content needs to be written to a target stream a 2nd time, then the stream must support
                // seeking (e.g. a FileStream), otherwise the stream can't be copied a second time to a target 
                // stream (e.g. a NetworkStream).
                if (content.CanSeek)
                {
                    content.Position = 0;
                }
                else
                {
                    throw new InvalidOperationException("SR.net_http_content_stream_already_read");
                }
            }

            contentConsumed = true;
        }

        protected override bool TryComputeLength(out long length)
        {
            //throw new NotImplementedException();
            length = content.Length;
            return true;
        }
    }
}