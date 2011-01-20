using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpMachine
{
    public interface IHttpParserHandler
    {
        void OnMessageBegin();
        void OnMethod(ArraySegment<byte> data);
        void OnRequestUri(ArraySegment<byte> data);
        void OnQueryString(ArraySegment<byte> data);
        void OnFragment(ArraySegment<byte> data);
        void OnVersionMajor(int major);
        void OnVersionMinor(int minor);
        void OnHeaderName(ArraySegment<byte> data);
        void OnHeaderValue(ArraySegment<byte> data);
        void OnHeadersEnd();
        void OnBody(ArraySegment<byte> data);
        void OnMessageEnd();
    }
}
