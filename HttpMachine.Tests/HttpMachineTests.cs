using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HttpMachine;
using NUnit.Framework;

// TODO
// reset state after body is read (add OnBeginMessage, OnEndMessage)
// extract connection header, indicate keepalive
// parse request path
// allow leading \r\n
// allow ? in query
// test requests from common clients (firefox etc)
// extract transfer-encoding header, decode chunked encoding
// extract upgrade header, indicate upgrade
// line folding?
// no http version numbers default to 0.9

// error conditions
// - too-long method
// - fuzz

// not in scope (clients responsibility)
// 
// - too-long request uri
// - too-long headers
// - too-long body (or, will error out on next read)

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
                Raw = Encoding.ASCII.GetBytes("GET /foo HTTP/1.1\r\n\r\n"),
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
            }
        };
    }


    class Handler : IHttpParserHandler
    {
        public List<TestRequest> Requests = new List<TestRequest>();

        StringBuilder method, requestUri, queryString, fragment, headerName, headerValue;
        int versionMajor = -1, versionMinor = -1;
        Dictionary<string, string> headers;
        List<ArraySegment<byte>> body;
        bool onHeadersEndCalled;

        public void OnMessageBegin()
        {
            // defer creation of buffers until message is created so 
            // NullRef will be thrown if OnMessageBegin is not called.

            method = new StringBuilder();
            requestUri = new StringBuilder();
            queryString = new StringBuilder();
            fragment = new StringBuilder();
            headerName = new StringBuilder();
            headerValue = new StringBuilder();
            headers = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            body = new List<ArraySegment<byte>>();
        }

        public void OnMessageEnd()
        {
            TestRequest request = new TestRequest();
            request.Method = method.ToString();
            request.RequestUri = requestUri.ToString();
            request.QueryString = queryString.ToString();
            request.Fragment = fragment.ToString();
            request.Headers = headers;
            request.OnHeadersEndCalled = onHeadersEndCalled;
            // aggregate body chunks into one big chunk
            request.Body = new byte[body.Aggregate(0, (s, b) => s + b.Count)];
            int where = 0;
            foreach (var buf in body)
            {
                Buffer.BlockCopy(buf.Array, buf.Offset, request.Body, where, buf.Count);
                where += buf.Count;
            }

            // add it to the list of requests recieved.
            Requests.Add(request);

            // reset our internal state
            versionMajor = versionMinor = -1;
            method = requestUri = queryString = fragment = headerName = headerValue = null;
            headers = null;
            body = null;
            onHeadersEndCalled = false;
        }

        public void OnMethod(ArraySegment<byte> data)
        {
            var str = Encoding.ASCII.GetString(data.Array, data.Offset, data.Count);
            ///Console.WriteLine("OnMethod: '" + str + "'");
            method.Append(str);
        }

        public void OnRequestUri(ArraySegment<byte> data)
        {
            var str = Encoding.ASCII.GetString(data.Array, data.Offset, data.Count);
            //Console.WriteLine("OnRequestUri:  '" + str + "'");
            requestUri.Append(str);
        }

        public void OnQueryString(ArraySegment<byte> data)
        {
            var str = Encoding.ASCII.GetString(data.Array, data.Offset, data.Count);
            //Console.WriteLine("OnQueryString:  '" + str + "'");
            queryString.Append(str);
        }

        public void OnFragment(ArraySegment<byte> data)
        {
            var str = Encoding.ASCII.GetString(data.Array, data.Offset, data.Count);
            //Console.WriteLine("OnFragment:  '" + str + "'");
            fragment.Append(str);
        }

        public void OnHeaderName(ArraySegment<byte> data)
        {
            var str = Encoding.ASCII.GetString(data.Array, data.Offset, data.Count);
            //Console.WriteLine("OnHeaderName:  '" + str + "'");

            if (headerValue.Length != 0)
                CommitHeader();

            headerName.Append(str);
        }

        public void OnHeaderValue(ArraySegment<byte> data)
        {
            var str = Encoding.ASCII.GetString(data.Array, data.Offset, data.Count);
            //Console.WriteLine("OnHeaderValue:  '" + str + "'");

            if (headerName.Length == 0)
                throw new Exception("Got header value without name.");

            headerValue.Append(str);
        }

        public void OnVersionMajor(int major)
        {
            //Console.WriteLine("OnVersionMajor:  '" + major + "'");
            versionMajor = major;
        }

        public void OnVersionMinor(int minor)
        {
            //Console.WriteLine("OnVersionMinor:  '" + minor + "'");
            versionMinor = minor;
        }

        public void OnHeadersEnd()
        {
            //Console.WriteLine("OnHeadersEnd");

            if (headerValue.Length != 0)
                CommitHeader();


            onHeadersEndCalled = true;
        }

        void CommitHeader()
        {
            //Console.WriteLine("Committing header '" + headerName.ToString() + "' : '" + headerValue.ToString() + "'");
            headers[headerName.ToString()] = headerValue.ToString();
            headerName.Length = headerValue.Length = 0;
        }

        public void OnBody(ArraySegment<byte> data)
        {
            var str = Encoding.ASCII.GetString(data.Array, data.Offset, data.Count);
            //Console.WriteLine("OnBody:  '" + str + "'");
            body.Add(data);
        }
    }

    public class HttpParserTests
    {

        void AssertRequest(TestRequest[] expected, TestRequest[] actual, HttpParser machine)
        {
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.IsTrue(i <= actual.Length - 1, "Expected more requests than received");

                var expectedRequest = expected[i];
                var actualRequest = actual[i];

                Assert.AreEqual(expectedRequest.Method, actualRequest.Method, "Unexpected method.");
                Assert.AreEqual(expectedRequest.RequestUri, actualRequest.RequestUri, "Unexpected request URI.");
                Assert.AreEqual(expectedRequest.VersionMajor, actualRequest.VersionMajor, "Unexpected major version.");
                Assert.AreEqual(expectedRequest.VersionMinor, actualRequest.VersionMinor, "Unexpected minor version.");
                Assert.AreEqual(expectedRequest.QueryString, actualRequest.QueryString, "Unexpected query string.");
                Assert.AreEqual(expectedRequest.Fragment, actualRequest.Fragment, "Unexpected fragment.");
                //Assert.AreEqual(expected.RequestPath, test.RequestPath, "Unexpected path.");

                Assert.IsTrue(expectedRequest.OnHeadersEndCalled, "OnHeadersEnd was not called.");

                if (expectedRequest.Headers.Keys.Any(k => k.ToLowerInvariant() == "content-length"))
                {
                    //Console.WriteLine("verifying content length");
                    Assert.IsTrue(machine.gotContentLength);
                    Assert.AreEqual(int.Parse(actualRequest.Headers["content-length"]), machine.contentLength);
                }

                foreach (var pair in expectedRequest.Headers)
                {
                    Assert.IsTrue(actualRequest.Headers.ContainsKey(pair.Key), "Actual headers did not contain key '" + pair.Key + "'");
                    Assert.AreEqual(pair.Value, actualRequest.Headers[pair.Key], "Actual headers had wrong value for key '" + pair.Key + "'");
                }

                foreach (var pair in actualRequest.Headers)
                {
                    Assert.IsTrue(expectedRequest.Headers.ContainsKey(pair.Key), "Unexpected header named '" + pair.Key + "'");
                }

                if (expectedRequest.Body != null)
                {
                    var expectedBody = Encoding.UTF8.GetString(expectedRequest.Body);
                    var actualBody = Encoding.UTF8.GetString(actualRequest.Body);
                    Assert.AreEqual(expectedBody, actualBody, "Body differs");
                }
                else
                    Assert.IsNull(actualRequest.Body);
            }
        }


        [Test]
        public void SingleChunk()
        {
            // read each request as a single block and parse

            foreach (var request in TestRequest.Requests)
            {
                var handler = new Handler();
                var parser = new HttpParser(handler);
                Console.WriteLine("----- Testing request: '" + request.Name + "' -----");

                parser.Execute(new ArraySegment<byte>(request.Raw));
                AssertRequest(new TestRequest[] { request }, handler.Requests.ToArray(), parser);
            }
        }

        [Test]
        public void RequestsSingle()
        {
            foreach (var request in TestRequest.Requests)
            {
                ThreeChunkScan(new TestRequest[] { request });
            }
        }

        [Test]
        public void RequestsPipelined()
        {
            ThreeChunkScan(new TestRequest[] { 
        }

        void ThreeChunkScan(TestRequest[] requests)
        {
            // read each sequence of requests as three blocks, with the breaks in every possible combination.
            //
            // roughly O(n^2) where n is number of bytes in the request sequence. D:

            // one buffer to rule them all
            var raw = new byte[requests.Aggregate(0, (s, b) => s + b.Raw.Length)];
            int where = 0;
            foreach (var r in requests)
            {
                Buffer.BlockCopy(r.Raw, 0, raw, where, r.Raw.Length);
                where += r.Raw.Length;
            }

            int totalOperations = (raw.Length - 1) * (raw.Length - 2) / 2;
            int operationsCompleted = 0;
            byte[] buffer1 = new byte[80 * 1024];
            byte[] buffer2 = new byte[80 * 1024];
            byte[] buffer3 = new byte[80 * 1024];

            Console.WriteLine("----- Testing requests: " + 
                requests.Aggregate("", (s, r) => s + ", " + r.Name).TrimStart(',',' ') + 
                " (" + totalOperations + " ops) -----");

            for (int j = 2; j < raw.Length; j++)
                for (int i = 1; i < j; i++)
                {
                    //Console.WriteLine();
                    if (operationsCompleted % 1000 == 0)
                        Console.WriteLine("  " + (100.0 * ((float)operationsCompleted / (float)totalOperations)));

                    operationsCompleted++;

                    var handler = new Handler();
                    var parser = new HttpParser(handler);

                    var buffer1Length = i;
                    Buffer.BlockCopy(raw, 0, buffer1, 0, buffer1Length);

                    parser.Execute(new ArraySegment<byte>(buffer1, 0, buffer1Length));

                    var buffer2Length = j - i;
                    Buffer.BlockCopy(raw, i, buffer2, 0, buffer2Length);

                    parser.Execute(new ArraySegment<byte>(buffer2, 0, buffer2Length));

                    var buffer3Length = raw.Length - j;
                    Buffer.BlockCopy(raw, j, buffer3, 0, buffer3Length);

                    parser.Execute(new ArraySegment<byte>(buffer3, 0, buffer3Length));

                    try
                    {
                        AssertRequest(request, handler, parser);
                    }
                    catch (AssertionException e)
                    {
                        Console.WriteLine("Problem while parsing chunks:");
                        Console.WriteLine("---");
                        Console.WriteLine(Encoding.UTF8.GetString(buffer1, 0, buffer1Length));
                        Console.WriteLine("---");
                        Console.WriteLine(Encoding.UTF8.GetString(buffer2, 0, buffer2Length));
                        Console.WriteLine("---");
                        Console.WriteLine(Encoding.UTF8.GetString(buffer3, 0, buffer3Length));
                        Console.WriteLine("---");
                        throw;
                    }
            }
        }
    }
}
