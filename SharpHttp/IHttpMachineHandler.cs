using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpMachine
{
    public interface IHttpParserHandler
    {
        void OnMethod(ArraySegment<byte> data);
        void OnRequestUri(ArraySegment<byte> data);
        void OnQueryString(ArraySegment<byte> data);
        void OnFragment(ArraySegment<byte> data);
        void OnVersionMajor(ArraySegment<byte> data);
        void OnVersionMinor(ArraySegment<byte> data);
        void OnHeaderName(ArraySegment<byte> data);
        void OnHeaderValue(ArraySegment<byte> data);
        void OnHeadersComplete();
        void OnBody(ArraySegment<byte> data);
    }
}
