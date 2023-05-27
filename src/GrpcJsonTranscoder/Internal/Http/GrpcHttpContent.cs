using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GrpcJsonTranscoder.Internal.Http
{
    public class GrpcHttpContent : HttpContent
    {
        public readonly string _result;

        public GrpcHttpContent(string result)
        {
            _result = result;
        }

        public GrpcHttpContent(object result)
        {
            _result = JsonConvert.SerializeObject(result);
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            var writer = new StreamWriter(stream);

            await writer.WriteAsync(_result);

            await writer.FlushAsync();
            stream.Position = 0;
        }

        public long GetResultLength()
        {
            long length = 0;
            using(MemoryStream ms = new())
            {
                var writer = new StreamWriter(ms);
                try {
                    writer.Write(_result);
                    writer.Flush();
                    ms.Seek(0, SeekOrigin.Begin);
                    length = ms.Length;
                }
                finally {
                    ms.Dispose();
                }
            }
            return length;
        }

        protected override bool TryComputeLength(out long length)
        {
            length = GetResultLength();

            //length = _result.Length;

            return true;
        }
    }
}
