using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpMachine
{
    public partial class HttpParser
    {
        public int MajorVersion { get { return versionMajor; } }
        public int MinorVersion { get { return versionMinor; } }

        public bool ShouldKeepAlive
        {
            get
            {
                if (versionMajor > 0 && versionMinor > 0)
                    // HTTP/1.1
                    return !gotConnectionClose;
                else
                    // < HTTP/1.1
                    return gotConnectionKeepAlive;
            }
        }
    }
}
