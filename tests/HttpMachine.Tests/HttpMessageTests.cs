using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HttpMachine;
using NUnit.Framework;
using System.Diagnostics;
using System.IO;

// TODO
// parse request path
// allow ? in query
// extract transfer-encoding header, decode chunked encoding
// extract upgrade header, indicate upgrade
// line folding?

// error conditions
// - too-long method
// - data after dead
// - fuzz

// not in scope (clients responsibility)
// 
// - too-long request uri
// - too-long headers
// - too-long body (or, will error out on next read)

namespace HttpMachine.Tests
{
    public class HttpMessageTests
    {
        static void AssertRequest(TestRequest[] expected, IHttpMessage[] actual)
        {
            for (int i = 0; i < expected.Length; i++)
            {
                
                Assert.IsTrue(i <= actual.Length - 1, "Expected more requests than received");

                var expectedRequest = expected[i];
                var actualRequest = actual[i];
                //Console.WriteLine("Asserting request " + expectedRequest.Name);
                Assert.AreEqual(expectedRequest.Method, actualRequest.Method, "Unexpected method.");
                Assert.AreEqual(expectedRequest.RequestUri, actualRequest.RequestUri, "Unexpected request URI.");
                Assert.AreEqual(expectedRequest.VersionMajor, actualRequest.HttpVersion.Major, "Unexpected major version.");
                Assert.AreEqual(expectedRequest.VersionMinor, actualRequest.HttpVersion.Minor, "Unexpected minor version.");
                Assert.AreEqual(expectedRequest.RequestPath, actualRequest.RequestPath, "Unexpected request path.");
                Assert.AreEqual(expectedRequest.QueryString, actualRequest.QueryString, "Unexpected query string.");
                Assert.AreEqual(expectedRequest.Fragment, actualRequest.Fragment, "Unexpected fragment.");
                //Assert.AreEqual(expected.RequestPath, test.RequestPath, "Unexpected path.");

                Assert.AreEqual(expectedRequest.ShouldKeepAlive, actualRequest.ShouldKeepAlive, "Wrong value for ShouldKeepAlive");

                foreach (var pair in expectedRequest.Headers)
                {
                    Assert.IsTrue(actualRequest.Headers.Any(h => string.Equals(h.Key, pair.Key, StringComparison.OrdinalIgnoreCase)), "Actual headers did not contain key '" + pair.Key + "'");
                    Assert.AreEqual(pair.Value, actualRequest.Headers.Where(h => string.Equals(h.Key, pair.Key, StringComparison.OrdinalIgnoreCase)).Select(h => h.Value).First(), "Actual headers had wrong value for key '" + pair.Key + "'");
                }

                foreach (var pair in actualRequest.Headers)
                {
                    Assert.IsTrue(expectedRequest.Headers.ContainsKey(pair.Key), "Unexpected header named '" + pair.Key + "'");
                }

                if (expectedRequest.Body != null)
                {
                    var expectedBody = Encoding.UTF8.GetString(expectedRequest.Body);
                    Assert.IsNotNull(actualRequest.Body, "Expected non-null request body");
                    StreamReader sr = new StreamReader(actualRequest.Body, Encoding.UTF8);
                    var actualBody = sr.ReadToEnd();
                    Assert.AreEqual(expectedBody, actualBody, "Body differs");
                }
                else
                    Assert.AreEqual(0, actualRequest.Body.Length);
            }
        }

        [Test]
        public void SingleChunk()
        {
            // read each request as a single block and parse

            foreach (var request in TestRequest.Requests)
            {
                var parser = new HttpMessageParser();
                Console.WriteLine("----- Testing request: '" + request.Name + "' -----");

                var parsed = parser.Execute(new ArraySegment<byte>(request.Raw));

                parser.Execute(default(ArraySegment<byte>));

                AssertRequest(new TestRequest[] { request }, parsed.ToArray());
            }
        }

        [Test]
        public void RequestsSingle()
        {
            foreach (var request in TestRequest.Requests/*.Where(r => r.Name == "1.0 post")*/)
            {
                ThreeChunkScan(new TestRequest[] { request });
            }
        }

        [Test]
        public void RequestsWithDigits() {
            foreach (var request in TestRequest.Requests.Where(r => r.Name.StartsWith("digits in "))) {
                var parser = new HttpMessageParser();
                Console.WriteLine("----- Testing request: '" + request.Name + "' -----");

                var parsed = parser.Execute(new ArraySegment<byte>(request.Raw));

                AssertRequest(new TestRequest[] { request }, parsed.ToArray());
            }
        }

        [TestFixture]
        public class OneOhTests
        {
            [Test]
            public void PostKeepAlivePostEof()
            {
                PipelineAndScan("1.0 post keep-alive with content length", "1.0 post");
            }

            [Test]
            public void GetKeepAlivePostEof()
            {
                PipelineAndScan("1.0 get keep-alive", "1.0 post");
            }

            [Test]
            public void PostEof()
            {
                PipelineAndScan("1.0 post");
            }

            [Test]
            public void PostNoContentLength()
            {
                PipelineAndScan("1.0 post no content length");
            }

			[Test]
            public void Get()
            {
                PipelineAndScan("1.0 get");
            }

            [Test]
            public void GetKeepAlive()
            {
                PipelineAndScan("1.0 get keep-alive");
            }

            [Test]
            public void PostKeepAliveGet()
            {
                PipelineAndScan("1.0 post keep-alive with content length", "1.0 get");
            }

            [Test]
            public void GetKeepAliveGet()
            {
                PipelineAndScan("1.0 get keep-alive", "1.0 get keep-alive", "1.0 get");
            }

            [Test]
            public void OneOhPostKeepAlivePost()
            {
                PipelineAndScan("1.0 post keep-alive with content length", "1.0 post");
            }

            [Test]
            public void GetKeepAlivePost()
            {
                PipelineAndScan("1.0 get keep-alive", "1.0 post");
            }
        }

        [TestFixture]
        public class OneOneTests
        {
            [Test]
            public void Get()
            {
                PipelineAndScan("1.1 get");
            }

            [Test]
            public void GetGet()
            {
                PipelineAndScan("1.1 get", "1.1 get");
            }

            [Test]
            public void GetGetGetClose()
            {
                PipelineAndScan("1.1 get", "1.1 get", "1.1 get close");
            }

            [Test]
            public void Post()
            {
                PipelineAndScan("1.1 post");
            }

            [Test]
            public void PostPost()
            {
                PipelineAndScan("1.1 post", "1.1 post");
            }

            [Test]
            public void PostPostPostClose()
            {
                PipelineAndScan("1.1 post", "1.1 post", "1.1 post close");
            }

            [Test]
            public void GetClose()
            {
                PipelineAndScan("1.1 get close");
            }

            [Test]
            public void PostClose()
            {
                PipelineAndScan("1.1 post close");
            }

            [Test]
            public void GetPost()
            {
                PipelineAndScan("1.1 get", "1.1 post");
            }

            [Test]
            public void GetPostClose()
            {
                PipelineAndScan("1.1 get", "1.1 post close");
            }

            [Test]
            public void GetPostGetClose()
            {
                PipelineAndScan("1.1 get", "1.1 post", "1.1 get close");
            }
        }

        static void PipelineAndScan(params string[] requests)
        {
            ThreeChunkScan(MakePipelined(requests));
        }

        static IEnumerable<TestRequest> MakePipelined(string[] requestNames)
        {
            List<TestRequest> result = new List<TestRequest>();
            foreach (var n in requestNames)
                result.Add(TestRequest.Requests.Where(r => r.Name == n).First());
            return result;
        }

        static void ThreeChunkScan(IEnumerable<TestRequest> requests)
        {
            // read each sequence of requests as three blocks, with the breaks in every possible combination.

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
            int buffer1Length = 0, buffer2Length = 0, buffer3Length = 0;

            Console.WriteLine("----- Testing requests: " +
                requests.Aggregate("", (s, r) => s + "; " + r.Name).TrimStart(';', ' ') +
                " (" + totalOperations + " ops) -----");

            int lastI = 0;
            int lastJ = 0;

            try
            {
                for (int j = 2; j < raw.Length; j++)
                    for (int i = 1; i < j; i++)
                    {
                        lastI = i; lastJ = j;
                        //Console.WriteLine();
                        if (operationsCompleted % 1000 == 0)
                            Console.WriteLine("  " + (100.0 * ((float)operationsCompleted / (float)totalOperations)));

                        operationsCompleted++;
                        //Console.WriteLine(operationsCompleted + " / " + totalOperations);

                        var parser = new HttpMessageParser();

                        buffer1Length = i;
                        Buffer.BlockCopy(raw, 0, buffer1, 0, buffer1Length);
                        buffer2Length = j - i;
                        Buffer.BlockCopy(raw, i, buffer2, 0, buffer2Length);
                        buffer3Length = raw.Length - j;
                        Buffer.BlockCopy(raw, j, buffer3, 0, buffer3Length);

                        List<IHttpMessage> messages = new List<IHttpMessage>();

                        //Console.WriteLine("Parsing buffer 1.");
                        messages.AddRange(parser.Execute(new ArraySegment<byte>(buffer1, 0, buffer1Length)));

                        //Console.WriteLine("Parsing buffer 2.");
                        messages.AddRange(parser.Execute(new ArraySegment<byte>(buffer2, 0, buffer2Length)));

                        //Console.WriteLine("Parsing buffer 3.");
                        messages.AddRange(parser.Execute(new ArraySegment<byte>(buffer3, 0, buffer3Length)));
                        
                        AssertRequest(requests.ToArray(), messages.ToArray());
                    }
            }
            catch
            {
                Console.WriteLine("Problem while parsing chunks:");
                Console.WriteLine("---");
                Console.WriteLine(Encoding.UTF8.GetString(buffer1, 0, buffer1Length));
                Console.WriteLine("---");
                Console.WriteLine(Encoding.UTF8.GetString(buffer2, 0, buffer2Length));
                Console.WriteLine("---");
                Console.WriteLine(Encoding.UTF8.GetString(buffer3, 0, buffer3Length));
                Console.WriteLine("---");
                Console.WriteLine("Failed on i = " + lastI + " j = " + lastJ);
                throw;
            }
        }
    }
}
