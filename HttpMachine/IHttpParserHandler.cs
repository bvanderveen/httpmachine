using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpMachine
{
    public interface IHttpParserHandler
    {
        void OnMessageBegin(HttpParser parser);
        void OnMethod(HttpParser parser, ArraySegment<byte> data);
        void OnRequestUri(HttpParser parser, ArraySegment<byte> data);
        void OnQueryString(HttpParser parser, ArraySegment<byte> data);
        void OnFragment(HttpParser parser, ArraySegment<byte> data);
        void OnHeaderName(HttpParser parser, ArraySegment<byte> data);
        void OnHeaderValue(HttpParser parser, ArraySegment<byte> data);
        void OnHeadersEnd(HttpParser parser);
        void OnBody(HttpParser parser, ArraySegment<byte> data);
        void OnMessageEnd(HttpParser parser);
    }
}
