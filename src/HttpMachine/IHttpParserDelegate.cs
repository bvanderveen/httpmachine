using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpMachine
{
    public interface IHttpParserDelegate
    {
        void OnMessageBegin(HttpParser parser);
        void OnHeaderName(HttpParser parser, string name);
        void OnHeaderValue(HttpParser parser, string value);
        void OnHeadersEnd(HttpParser parser);
        void OnBody(HttpParser parser, ArraySegment<byte> data);
        void OnMessageEnd(HttpParser parser);
    }

}
