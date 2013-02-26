using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpMachine
{
    public class HttpMessageParser : IHttpParserDelegate
    {
        HttpParser _parser;
        HttpMessage _message;
        Action<IHttpMessage> _receiver;
        string _header;
        readonly Queue<IHttpMessage> _messages;

        public HttpMessageParser()
        {
            _parser = new HttpParser(this);
            _message = new HttpMessage();
            _messages = new Queue<IHttpMessage>();
        }

        public void OnMessageBegin(HttpParser parser)
        {
            // Ignore
        }

        public void OnMethod(HttpParser parser, string method)
        {
            _message.Method = method;
        }

        public void OnRequestUri(HttpParser parser, string requestUri)
        {
            _message.RequestUri = requestUri;
        }

        public void OnPath(HttpParser parser, string path)
        {
            _message.RequestPath = path;
        }

        public void OnFragment(HttpParser parser, string fragment)
        {
            _message.Fragment = fragment;
        }

        public void OnQueryString(HttpParser parser, string queryString)
        {
            _message.QueryString = queryString;
        }

        public void OnHeaderName(HttpParser parser, string name)
        {
            _header = name;
        }

        public void OnHeaderValue(HttpParser parser, string value)
        {
            _message.Headers.Add(new KeyValuePair<string, string>(_header, value));
        }

        public void OnHeadersEnd(HttpParser parser)
        {
            // Ignore
        }

        public void OnBody(HttpParser parser, ArraySegment<byte> data)
        {
            _message.BodyBytes.Add(data);
        }

        public void OnMessageEnd(HttpParser parser)
        {
            // Create stream
            var length = _message.BodyBytes.Aggregate(0, (s, b) => s + b.Count);
            byte[] bs = new byte[length];
            if (length > 0)
            {
                int where = 0;
                foreach (var buf in _message.BodyBytes)
                {
                    Buffer.BlockCopy(buf.Array, buf.Offset, bs, where, buf.Count);
                    where += buf.Count;
                }
            }
            _message.Body = new MemoryStream(bs, 0, bs.Length, false, false);
            _message.Body.Position = 0;
            _message.BodyBytes = null;


            _message.HttpVersion = new Version(parser.MajorVersion, parser.MinorVersion);
            _message.ShouldKeepAlive = parser.ShouldKeepAlive;
            _messages.Enqueue(_message);
            _message = new HttpMessage();
        }

        public IEnumerable<IHttpMessage> Execute(ArraySegment<byte> arraySegment)
        {
            long totalLength = 0;
            List<IHttpMessage> result = new List<IHttpMessage>();
            while (true)
            {
                totalLength += _parser.Execute(arraySegment);
                while (_messages.Count > 0)
                    result.Add(_messages.Dequeue());

                if (totalLength >= arraySegment.Count)
                    return result;
            }
        }
    }

    public interface IHttpMessage
    {
        string Method { get; }
        string RequestUri { get; }
        string RequestPath { get; }
        string Fragment { get; }
        string QueryString { get; }
        IList<KeyValuePair<string, string>> Headers { get; }
        Stream Body { get; }

        Version HttpVersion { get; }
        bool ShouldKeepAlive { get; }
    }

    class HttpMessage : IHttpMessage
    {
        public HttpMessage()
        {
            Headers = new List<KeyValuePair<string, string>>();
            BodyBytes = new List<ArraySegment<byte>>();
        }

        public List<ArraySegment<byte>> BodyBytes { get; internal set; }

        public string Method { get; internal set; }
        public string RequestUri { get; internal set; }
        public string RequestPath { get; internal set; }
        public string Fragment { get; internal set; }
        public string QueryString { get; internal set; }
        public IList<KeyValuePair<string, string>> Headers { get; private set; }
        public Stream Body { get; internal set; }

        public Version HttpVersion { get; internal set; }
        public bool ShouldKeepAlive { get; internal set; }
    }
}
