using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpMachine.Tests
{
    class TestRequest
    {
        public string Name;
        public byte[] Raw;
        public string Method;
        public string RequestUri;
        public string RequestPath;
        public string QueryString;
        public string Fragment;
        public int VersionMajor;
        public int VersionMinor;
        public Dictionary<string, string> Headers;
        public byte[] Body;

        public bool ShouldKeepAlive; // if the message is 1.1 and !Connection:close, or message is < 1.1 and Connection:keep-alive
        public bool OnHeadersEndCalled;

        public static TestRequest[] Requests = new TestRequest[] {
            
            new TestRequest() {
                Name = "No headers, no body",
                Raw = Encoding.ASCII.GetBytes("\r\nGET /foo HTTP/1.1\r\n\r\n"),
                Method = "GET",
                RequestUri = "/foo",
                RequestPath = "/foo",
                QueryString = "",
                Fragment = "",
                VersionMajor = 1,
                VersionMinor = 1,
                Headers = new Dictionary<string,string>() {
                },
                Body = null,
                ShouldKeepAlive = true
            },
            new TestRequest() {
                Name = "No headers, no body, no version",
                Raw = Encoding.ASCII.GetBytes("GET /foo\r\n\r\n"),
                Method = "GET",
                RequestUri = "/foo",
                RequestPath = "/foo",
                QueryString = "",
                Fragment = "",
                VersionMajor = 0,
                VersionMinor = 9,
                Headers = new Dictionary<string,string>() {
                },
                Body = null,
                ShouldKeepAlive = false
            },
            new TestRequest() {
                Name = "no body",
                Raw = Encoding.ASCII.GetBytes("GET /foo HTTP/1.1\r\nFoo: Bar\r\n\r\n"),
                Method = "GET",
                RequestUri = "/foo",
                RequestPath = "/foo",
                QueryString = "",
                Fragment = "",
                VersionMajor = 1,
                VersionMinor = 1,
                Headers = new Dictionary<string,string>() {
                    { "Foo", "Bar" }
                },
                Body = null,
                ShouldKeepAlive = true
            },
            new TestRequest() {
                Name = "no body no version",
                Raw = Encoding.ASCII.GetBytes("GET /foo\r\nFoo: Bar\r\n\r\n"),
                Method = "GET",
                RequestUri = "/foo",
                RequestPath = "/foo",
                QueryString = "",
                Fragment = "",
                VersionMajor = 0,
                VersionMinor = 9,
                Headers = new Dictionary<string,string>() {
                    { "Foo", "Bar" }
                },
                Body = null,
                ShouldKeepAlive = false
            },
            new TestRequest() {
                Name = "query string",
                Raw = Encoding.ASCII.GetBytes("GET /foo?asdf=jklol HTTP/1.1\r\nFoo: Bar\r\nBaz-arse: Quux\r\n\r\n"),
                Method = "GET",
                RequestUri = "/foo?asdf=jklol",
                RequestPath = "/foo",
                QueryString = "asdf=jklol",
                Fragment = "",
                VersionMajor = 1,
                VersionMinor = 1,
                Headers = new Dictionary<string,string>() {
                    { "Foo", "Bar" },
                    { "Baz-arse", "Quux" }
                },
                Body = null,
                ShouldKeepAlive = true
            },
            new TestRequest() {
                Name = "fragment",
                Raw = Encoding.ASCII.GetBytes("POST /foo?asdf=jklol#poopz HTTP/1.1\r\nFoo: Bar\r\nBaz: Quux\r\n\r\n"),
                Method = "POST",
                RequestUri = "/foo?asdf=jklol#poopz",
                RequestPath = "/foo",
                QueryString = "asdf=jklol",
                Fragment = "poopz",
                VersionMajor = 1,
                VersionMinor = 1,
                Headers = new Dictionary<string,string>() {
                    { "Foo", "Bar" },
                    { "Baz", "Quux" }
                },
                Body = null,
                ShouldKeepAlive = true
            },
            new TestRequest() {
                Name = "zero content length",
                Raw = Encoding.ASCII.GetBytes("POST /foo HTTP/1.1\r\nFoo: Bar\r\nContent-Length: 0\r\n\r\n"),
                Method = "POST",
                RequestUri = "/foo",
                RequestPath = "/foo",
                QueryString = "",
                Fragment = "",
                VersionMajor = 1,
                VersionMinor = 1,
                Headers = new Dictionary<string,string>(StringComparer.InvariantCultureIgnoreCase) {
                    { "Foo", "Bar" },
                    { "Content-Length", "0" }
                },
                Body = null,
                ShouldKeepAlive = true
            },
            new TestRequest() {
                Name = "some content length",
                Raw = Encoding.ASCII.GetBytes("POST /foo HTTP/1.1\r\nFoo: Bar\r\nContent-Length: 5\r\n\r\nhello"),
                Method = "POST",
                RequestUri = "/foo",
                RequestPath = "/foo",
                QueryString = "",
                Fragment = "",
                VersionMajor = 1,
                VersionMinor = 1,
                Headers = new Dictionary<string,string>(StringComparer.InvariantCultureIgnoreCase) {
                    { "Foo", "Bar" },
                    { "Content-Length", "5" }
                },
                Body = Encoding.UTF8.GetBytes("hello"),
                ShouldKeepAlive = true
            },
            new TestRequest() {
                Name = "more content length",
                Raw = Encoding.ASCII.GetBytes("POST /foo HTTP/1.1\r\nFoo: Bar\r\nContent-Length: 15\r\n\r\nhelloworldhello"),
                Method = "POST",
                RequestUri = "/foo",
                RequestPath = "/foo",
                QueryString = "",
                Fragment = "",
                VersionMajor = 1,
                VersionMinor = 1,
                Headers = new Dictionary<string,string>(StringComparer.InvariantCultureIgnoreCase) {
                    { "Foo", "Bar" },
                    { "Content-Length", "15" }
                },
                Body = Encoding.UTF8.GetBytes("helloworldhello"),
                ShouldKeepAlive = true
            },
            new TestRequest() {
                Name = "safari",
                Raw = Encoding.ASCII.GetBytes(@"GET /portfolio HTTP/1.1
Host: bvanderveen.com
User-Agent: Mozilla/5.0 (Macintosh; U; Intel Mac OS X 10_6_5; en-us) AppleWebKit/533.18.1 (KHTML, like Gecko) Version/5.0.2 Safari/533.18.5
Accept: application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5
Referer: http://bvanderveen.com/
Accept-Language: en-us
Accept-Encoding: gzip, deflate
Cookie: __utmb=251228503.1.12.12338288212; __utmc=393939923957034890; __utma=1847567578628673.487437863768.2382773744.4893288295.485438934.99; __utmz=838388383.23433321112905.23.96.utmcsr=google|utmccn=(organic)|utmcmd=organic|utmctr=fooobnn%20ittszz
Connection: keep-alive

"),
                Method = "GET",
                RequestUri = "/portfolio",
                RequestPath = "/portfolio",
                QueryString = "",
                Fragment = "",
                VersionMajor = 1,
                VersionMinor = 1,
                Headers = new Dictionary<string,string>(StringComparer.InvariantCultureIgnoreCase) {
                    { "Host", "bvanderveen.com" },
                    { "User-Agent", "Mozilla/5.0 (Macintosh; U; Intel Mac OS X 10_6_5; en-us) AppleWebKit/533.18.1 (KHTML, like Gecko) Version/5.0.2 Safari/533.18.5" },
                    { "Accept", "application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5" },
                    { "Referer", "http://bvanderveen.com/" },
                    { "Accept-Language", "en-us" },
                    { "Accept-Encoding", "gzip, deflate" },
                    { "Cookie", "__utmb=251228503.1.12.12338288212; __utmc=393939923957034890; __utma=1847567578628673.487437863768.2382773744.4893288295.485438934.99; __utmz=838388383.23433321112905.23.96.utmcsr=google|utmccn=(organic)|utmcmd=organic|utmctr=fooobnn%20ittszz" },
                    { "Connection", "keep-alive" }
                },
                Body = null,
                ShouldKeepAlive = true
            },
            // because it has no content-length, it's not keep alive anyway? TODO 
            new TestRequest() {
                Name = "get connection close",
                Raw = Encoding.ASCII.GetBytes("GET /foo?asdf=jklol#poopz HTTP/1.1\r\nFoo: Bar\r\nBaz: Quux\r\nConnection: close\r\n\r\n"),
                Method = "GET",
                RequestUri = "/foo?asdf=jklol#poopz",
                RequestPath = "/foo",
                QueryString = "asdf=jklol",
                Fragment = "poopz",
                VersionMajor = 1,
                VersionMinor = 1,
                Headers = new Dictionary<string,string>() {
                    { "Foo", "Bar" },
                    { "Baz", "Quux" },
                    { "Connection", "close" }
                },
                Body = null,
                ShouldKeepAlive = false
            },
            new TestRequest() {
                Name = "get 1.0",
                Raw = Encoding.ASCII.GetBytes("GET /foo?asdf=jklol#poopz HTTP/1.0\r\nFoo: Bar\r\nBaz: Quux\r\n\r\n"),
                Method = "GET",
                RequestUri = "/foo?asdf=jklol#poopz",
                RequestPath = "/foo",
                QueryString = "asdf=jklol",
                Fragment = "poopz",
                VersionMajor = 1,
                VersionMinor = 0,
                Headers = new Dictionary<string,string>() {
                    { "Foo", "Bar" },
                    { "Baz", "Quux" }
                },
                Body = null,
                ShouldKeepAlive = false
            },
            new TestRequest() {
                Name = "get 1.0 connection keep-alive",
                Raw = Encoding.ASCII.GetBytes("GET /foo?asdf=jklol#poopz HTTP/1.0\r\nFoo: Bar\r\nBaz: Quux\r\nConnection: keep-alive\r\n\r\n"),
                Method = "GET",
                RequestUri = "/foo?asdf=jklol#poopz",
                RequestPath = "/foo",
                QueryString = "asdf=jklol",
                Fragment = "poopz",
                VersionMajor = 1,
                VersionMinor = 0,
                Headers = new Dictionary<string,string>() {
                    { "Foo", "Bar" },
                    { "Baz", "Quux" },
                    { "Connection", "keep-alive" }
                },
                Body = null,
                ShouldKeepAlive = true
            }
        };
    }

}
