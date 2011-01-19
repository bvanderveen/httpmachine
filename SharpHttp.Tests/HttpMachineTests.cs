using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HttpSharp;
using NUnit.Framework;

namespace SharpHttp.Tests
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

        public static TestRequest[] Requests = new TestRequest[] {
            // TODO
            // parse request path
            // leading \r\n, ? in query, requests from common clients
            
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
                Body = null
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
                Body = null
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
                Body = null
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
                Body = null
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
                Body = null
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
                Body = null
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
                Body = null
            }
        };
    }


    public class Handler : IHttpRequestParser
    {
        StringBuilder method, requestUri, queryString, fragment, headerName, headerValue, versionMajor, versionMinor;
        Dictionary<string, string> headers;
        List<ArraySegment<byte>> body;

        public string Method
        {
            get { return method.ToString(); }
        }

        public string RequestUri
        {
            get { return requestUri.ToString(); }
        }

        public int VersionMajor
        {
            get { return int.Parse(versionMajor.ToString()); }
        }

        public int VersionMinor
        {
            get { return int.Parse(versionMinor.ToString()); }
        }

        public string QueryString
        {
            get { return queryString.ToString(); }
        }
        public string Fragment
        {
            get { return fragment.ToString(); }
        }

        public Dictionary<string, string> Headers
        {
            get { return headers; }
        }

        public List<ArraySegment<byte>> Body
        {
            get { return body; }
        }

        public Handler()
        {
            method = new StringBuilder();
            requestUri = new StringBuilder();
            queryString = new StringBuilder();
            fragment = new StringBuilder();
            headerName = new StringBuilder();
            headerValue = new StringBuilder();
            versionMajor = new StringBuilder(1);
            versionMinor = new StringBuilder(1);
            headers = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            body = new List<ArraySegment<byte>>();
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

        public void OnVersionMajor(ArraySegment<byte> data)
        {
            var str = Encoding.ASCII.GetString(data.Array, data.Offset, data.Count);
            //Console.WriteLine("OnVersionMajor:  '" + str + "'");
            versionMajor.Append(str);
        }

        public void OnVersionMinor(ArraySegment<byte> data)
        {
            var str = Encoding.ASCII.GetString(data.Array, data.Offset, data.Count);
            //Console.WriteLine("OnVersionMinor:  '" + str + "'");
            versionMinor.Append(str);
        }

        public void OnHeadersComplete()
        {
            //Console.WriteLine("OnHeadersComplete");
            if (headerValue.Length != 0)
                CommitHeader();
        }

        void CommitHeader()
        {
            //Console.WriteLine("Committing header '" + headerName.ToString() + "' : '" + headerValue.ToString() + "'");
            headers[headerName.ToString()] = headerValue.ToString();
            headerName.Length = headerValue.Length = 0;
        }

        public void OnBody(ArraySegment<byte> data)
        {
            body.Add(data);
        }
    }

    public class HttpMachineTests
    {

        void AssertRequest(TestRequest expected, Handler test, HttpMachine machine)
        {
            Assert.AreEqual(expected.Method, test.Method, "Unexpected method.");
            Assert.AreEqual(expected.RequestUri, test.RequestUri, "Unexpected request URI.");
            Assert.AreEqual(expected.VersionMajor, test.VersionMajor, "Unexpected major version.");
            Assert.AreEqual(expected.VersionMinor, test.VersionMinor, "Unexpected minor version.");
            Assert.AreEqual(expected.QueryString, test.QueryString, "Unexpected query string.");
            Assert.AreEqual(expected.Fragment, test.Fragment, "Unexpected fragment.");
            //Assert.AreEqual(expected.RequestPath, test.RequestPath, "Unexpected path.");

            if (expected.Headers.Keys.Any(k => k.ToLowerInvariant() == "content-length"))
            {
                //Console.WriteLine("verifying content length");
                Assert.IsTrue(machine.gotContentLength);
                Assert.AreEqual(int.Parse(expected.Headers["content-length"]), machine.contentLength);
            }

            foreach (var pair in expected.Headers)
            {
                Assert.IsTrue(test.Headers.ContainsKey(pair.Key), "Tested headers did not contain key '" + pair.Key + "'");
                Assert.AreEqual(pair.Value, test.Headers[pair.Key], "Tested headers had wrong value for key '" + pair.Key + "'");
            }

            foreach (var pair in test.Headers)
            {
                Assert.IsTrue(expected.Headers.ContainsKey(pair.Key), "Unexpected header named '" + pair.Key + "'");
            }
        }


        [Test]
        public void SingleChunk()
        {
            // read each request as a single block and parse
            // read each request as three blocks, with the breaks in every possible combination

            foreach (var request in TestRequest.Requests)
            {
                var handler = new Handler();
                var parser = new HttpMachine(handler);
                Console.WriteLine("----- Testing request: '" + request.Name + "' -----");

                parser.Execute(new ArraySegment<byte>(request.Raw));
                AssertRequest(request, handler, parser);
            }
        }

        [Test]
        public void ThreeChunkScan()
        {
            foreach (var request in TestRequest.Requests)
            {
                var raw = request.Raw;
                int totalOperations = (raw.Length - 1) * (raw.Length - 2) / 2;
                int operationsCompleted = 0;
                byte[] buffer1 = new byte[80 * 1024];
                byte[] buffer2 = new byte[80 * 1024];
                byte[] buffer3 = new byte[80 * 1024];

                Console.WriteLine("total ops: " + totalOperations);
                for (int j = 2; j < raw.Length; j++)
                    for (int i = 1; i < j; i++)
                    {
                        if (operationsCompleted % 100 == 0)
                            Console.WriteLine("  " + (100.0 * ((float)operationsCompleted / (float)totalOperations)));

                        operationsCompleted++;
                        //Console.WriteLine("----- Testing request: '" + request.Name + "' (" + operationsCompleted + ") -----");


                        var handler = new Handler();
                        var parser = new HttpMachine(handler);

                        //Console.WriteLine("--- Parsing Chunk 1 --- ");
                        var buffer1Length = i;
                        Buffer.BlockCopy(raw, 0, buffer1, 0, buffer1Length);
                        //Console.WriteLine(Encoding.ASCII.GetString(buffer1, 0, buffer1Length));
                        //Console.WriteLine("---");

                        parser.Execute(new ArraySegment<byte>(buffer1, 0, buffer1Length));

                        //Console.WriteLine("--- Parsing Chunk 2 --- ");
                        var buffer2Length = j - i;
                        //Console.WriteLine("j - i = " + (j - i));
                        Buffer.BlockCopy(raw, i, buffer2, 0, buffer2Length);
                        //Console.WriteLine(Encoding.ASCII.GetString(buffer2, 0, buffer2Length));
                        //Console.WriteLine("---");

                        parser.Execute(new ArraySegment<byte>(buffer2, 0, buffer2Length));

                        //Console.WriteLine("--- Parsing Chunk 3 --- ");
                        var buffer3Length = raw.Length - j;
                        Buffer.BlockCopy(raw, j, buffer3, 0, buffer3Length);
                        //Console.WriteLine(Encoding.ASCII.GetString(buffer3, 0, buffer3Length));
                        //Console.WriteLine("---");

                        parser.Execute(new ArraySegment<byte>(buffer3, 0, buffer3Length));

                        AssertRequest(request, handler, parser);

                    }
            }
        }

        // TODO 
        // extract content-length/parse body
        // error conditions/fuzz
        // decode chunked encoding
        // upgrade?
        // keepalive
        [Test]
        public void Upgrade()
        {
        }
    }
}
