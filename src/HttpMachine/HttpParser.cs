
#line 1 "HttpParser.cs.rl"
using System;
using System.Text;

namespace HttpMachine
{
    public class HttpParser
    {
        public object UserContext { get; set; }
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

        IHttpParserDelegate del;

		// necessary evil?
		StringBuilder sb;
		StringBuilder sb2;
		// Uri uri;

		int versionMajor;
		int versionMinor;
		
        int contentLength;

		// TODO make flags or something, dang
		bool inContentLengthHeader;
		bool inConnectionHeader;
		bool inTransferEncodingHeader;
		bool inUpgradeHeader;
		bool gotConnectionClose;
		bool gotConnectionKeepAlive;
		bool gotTransferEncodingChunked;
		bool gotUpgradeValue;

        int cs;
        // int mark;
        int statusCode;
        string statusReason;

        
#line 362 "HttpParser.cs.rl"

        
        
#line 55 "..\\HttpParser.cs"
static readonly sbyte[] _http_parser_actions =  new sbyte [] {
	0, 1, 0, 1, 8, 1, 10, 1, 
	11, 1, 13, 1, 16, 1, 18, 1, 
	20, 1, 21, 1, 29, 1, 30, 1, 
	31, 1, 32, 1, 33, 1, 34, 2, 
	1, 0, 2, 2, 0, 2, 4, 11, 
	2, 12, 8, 2, 14, 0, 2, 14, 
	13, 2, 15, 0, 2, 15, 13, 2, 
	19, 13, 2, 22, 29, 2, 23, 29, 
	2, 24, 30, 2, 25, 30, 2, 26, 
	29, 2, 27, 30, 2, 28, 29, 3, 
	3, 2, 0, 3, 3, 15, 0, 3, 
	3, 15, 13, 3, 3, 19, 13, 3, 
	4, 1, 0, 3, 9, 1, 0, 3, 
	16, 1, 0, 3, 17, 1, 0, 3, 
	18, 1, 0, 4, 9, 1, 7, 0, 
	4, 9, 1, 7, 13, 5, 9, 1, 
	5, 7, 0, 6, 9, 1, 6, 3, 
	2, 0
};

static readonly short[] _http_parser_key_offsets =  new short [] {
	0, 0, 6, 7, 12, 17, 32, 33, 
	55, 56, 72, 80, 82, 88, 92, 96, 
	100, 104, 108, 110, 114, 118, 122, 124, 
	128, 132, 136, 139, 143, 147, 151, 155, 
	159, 161, 179, 197, 217, 235, 253, 271, 
	289, 307, 325, 341, 359, 377, 395, 411, 
	429, 447, 465, 483, 501, 519, 535, 553, 
	571, 589, 607, 625, 643, 661, 677, 695, 
	713, 731, 749, 767, 785, 803, 821, 837, 
	855, 873, 891, 909, 927, 945, 961, 962, 
	963, 964, 965, 966, 968, 969, 971, 972, 
	987, 993, 999, 1014, 1027, 1040, 1046, 1052, 
	1058, 1064, 1078, 1092, 1098, 1104, 1125, 1146, 
	1159, 1165, 1171, 1176, 1181, 1186, 1191, 1196, 
	1201, 1206, 1211, 1216, 1221, 1226, 1231, 1236, 
	1241, 1246, 1251, 1256, 1261, 1266, 1271, 1276, 
	1281, 1282, 1288, 1294, 1300, 1306, 1308, 1309, 
	1311, 1312, 1314, 1318, 1319, 1341, 1347, 1352, 
	1352, 1352, 1352, 1352, 1352
};

static readonly char[] _http_parser_trans_keys =  new char [] {
	'\u000d', '\u0048', '\u0041', '\u005a', '\u0061', '\u007a', '\u000a', '\u0048', 
	'\u0041', '\u005a', '\u0061', '\u007a', '\u0020', '\u0041', '\u005a', '\u0061', 
	'\u007a', '\u000d', '\u0020', '\u0021', '\u0025', '\u002f', '\u003d', '\u0040', 
	'\u005f', '\u007e', '\u0024', '\u003b', '\u0041', '\u005a', '\u0061', '\u007a', 
	'\u000a', '\u000d', '\u0021', '\u0043', '\u0054', '\u0055', '\u0063', '\u0074', 
	'\u0075', '\u007c', '\u007e', '\u0023', '\u0027', '\u002a', '\u002b', '\u002d', 
	'\u002e', '\u0030', '\u0039', '\u0041', '\u005a', '\u005e', '\u007a', '\u000a', 
	'\u0021', '\u003a', '\u007c', '\u007e', '\u0023', '\u0027', '\u002a', '\u002b', 
	'\u002d', '\u002e', '\u0030', '\u0039', '\u0041', '\u005a', '\u005e', '\u007a', 
	'\u0009', '\u000a', '\u000d', '\u0020', '\u0043', '\u004b', '\u0063', '\u006b', 
	'\u000a', '\u000d', '\u000a', '\u000d', '\u0048', '\u004c', '\u0068', '\u006c', 
	'\u000a', '\u000d', '\u0055', '\u0075', '\u000a', '\u000d', '\u004e', '\u006e', 
	'\u000a', '\u000d', '\u004b', '\u006b', '\u000a', '\u000d', '\u0045', '\u0065', 
	'\u000a', '\u000d', '\u0044', '\u0064', '\u000a', '\u000d', '\u000a', '\u000d', 
	'\u004f', '\u006f', '\u000a', '\u000d', '\u0053', '\u0073', '\u000a', '\u000d', 
	'\u0045', '\u0065', '\u000a', '\u000d', '\u000a', '\u000d', '\u0045', '\u0065', 
	'\u000a', '\u000d', '\u0045', '\u0065', '\u000a', '\u000d', '\u0050', '\u0070', 
	'\u000a', '\u000d', '\u002d', '\u000a', '\u000d', '\u0041', '\u0061', '\u000a', 
	'\u000d', '\u004c', '\u006c', '\u000a', '\u000d', '\u0049', '\u0069', '\u000a', 
	'\u000d', '\u0056', '\u0076', '\u000a', '\u000d', '\u0045', '\u0065', '\u000a', 
	'\u000d', '\u0021', '\u003a', '\u004f', '\u006f', '\u007c', '\u007e', '\u0023', 
	'\u0027', '\u002a', '\u002b', '\u002d', '\u002e', '\u0030', '\u0039', '\u0041', 
	'\u005a', '\u005e', '\u007a', '\u0021', '\u003a', '\u004e', '\u006e', '\u007c', 
	'\u007e', '\u0023', '\u0027', '\u002a', '\u002b', '\u002d', '\u002e', '\u0030', 
	'\u0039', '\u0041', '\u005a', '\u005e', '\u007a', '\u0021', '\u003a', '\u004e', 
	'\u0054', '\u006e', '\u0074', '\u007c', '\u007e', '\u0023', '\u0027', '\u002a', 
	'\u002b', '\u002d', '\u002e', '\u0030', '\u0039', '\u0041', '\u005a', '\u005e', 
	'\u007a', '\u0021', '\u003a', '\u0045', '\u0065', '\u007c', '\u007e', '\u0023', 
	'\u0027', '\u002a', '\u002b', '\u002d', '\u002e', '\u0030', '\u0039', '\u0041', 
	'\u005a', '\u005e', '\u007a', '\u0021', '\u003a', '\u0043', '\u0063', '\u007c', 
	'\u007e', '\u0023', '\u0027', '\u002a', '\u002b', '\u002d', '\u002e', '\u0030', 
	'\u0039', '\u0041', '\u005a', '\u005e', '\u007a', '\u0021', '\u003a', '\u0054', 
	'\u0074', '\u007c', '\u007e', '\u0023', '\u0027', '\u002a', '\u002b', '\u002d', 
	'\u002e', '\u0030', '\u0039', '\u0041', '\u005a', '\u005e', '\u007a', '\u0021', 
	'\u003a', '\u0049', '\u0069', '\u007c', '\u007e', '\u0023', '\u0027', '\u002a', 
	'\u002b', '\u002d', '\u002e', '\u0030', '\u0039', '\u0041', '\u005a', '\u005e', 
	'\u007a', '\u0021', '\u003a', '\u004f', '\u006f', '\u007c', '\u007e', '\u0023', 
	'\u0027', '\u002a', '\u002b', '\u002d', '\u002e', '\u0030', '\u0039', '\u0041', 
	'\u005a', '\u005e', '\u007a', '\u0021', '\u003a', '\u004e', '\u006e', '\u007c', 
	'\u007e', '\u0023', '\u0027', '\u002a', '\u002b', '\u002d', '\u002e', '\u0030', 
	'\u0039', '\u0041', '\u005a', '\u005e', '\u007a', '\u0021', '\u003a', '\u007c', 
	'\u007e', '\u0023', '\u0027', '\u002a', '\u002b', '\u002d', '\u002e', '\u0030', 
	'\u0039', '\u0041', '\u005a', '\u005e', '\u007a', '\u0021', '\u003a', '\u0045', 
	'\u0065', '\u007c', '\u007e', '\u0023', '\u0027', '\u002a', '\u002b', '\u002d', 
	'\u002e', '\u0030', '\u0039', '\u0041', '\u005a', '\u005e', '\u007a', '\u0021', 
	'\u003a', '\u004e', '\u006e', '\u007c', '\u007e', '\u0023', '\u0027', '\u002a', 
	'\u002b', '\u002d', '\u002e', '\u0030', '\u0039', '\u0041', '\u005a', '\u005e', 
	'\u007a', '\u0021', '\u003a', '\u0054', '\u0074', '\u007c', '\u007e', '\u0023', 
	'\u0027', '\u002a', '\u002b', '\u002d', '\u002e', '\u0030', '\u0039', '\u0041', 
	'\u005a', '\u005e', '\u007a', '\u0021', '\u002d', '\u002e', '\u003a', '\u007c', 
	'\u007e', '\u0023', '\u0027', '\u002a', '\u002b', '\u0030', '\u0039', '\u0041', 
	'\u005a', '\u005e', '\u007a', '\u0021', '\u003a', '\u004c', '\u006c', '\u007c', 
	'\u007e', '\u0023', '\u0027', '\u002a', '\u002b', '\u002d', '\u002e', '\u0030', 
	'\u0039', '\u0041', '\u005a', '\u005e', '\u007a', '\u0021', '\u003a', '\u0045', 
	'\u0065', '\u007c', '\u007e', '\u0023', '\u0027', '\u002a', '\u002b', '\u002d', 
	'\u002e', '\u0030', '\u0039', '\u0041', '\u005a', '\u005e', '\u007a', '\u0021', 
	'\u003a', '\u004e', '\u006e', '\u007c', '\u007e', '\u0023', '\u0027', '\u002a', 
	'\u002b', '\u002d', '\u002e', '\u0030', '\u0039', '\u0041', '\u005a', '\u005e', 
	'\u007a', '\u0021', '\u003a', '\u0047', '\u0067', '\u007c', '\u007e', '\u0023', 
	'\u0027', '\u002a', '\u002b', '\u002d', '\u002e', '\u0030', '\u0039', '\u0041', 
	'\u005a', '\u005e', '\u007a', '\u0021', '\u003a', '\u0054', '\u0074', '\u007c', 
	'\u007e', '\u0023', '\u0027', '\u002a', '\u002b', '\u002d', '\u002e', '\u0030', 
	'\u0039', '\u0041', '\u005a', '\u005e', '\u007a', '\u0021', '\u003a', '\u0048', 
	'\u0068', '\u007c', '\u007e', '\u0023', '\u0027', '\u002a', '\u002b', '\u002d', 
	'\u002e', '\u0030', '\u0039', '\u0041', '\u005a', '\u005e', '\u007a', '\u0021', 
	'\u003a', '\u007c', '\u007e', '\u0023', '\u0027', '\u002a', '\u002b', '\u002d', 
	'\u002e', '\u0030', '\u0039', '\u0041', '\u005a', '\u005e', '\u007a', '\u0021', 
	'\u003a', '\u0052', '\u0072', '\u007c', '\u007e', '\u0023', '\u0027', '\u002a', 
	'\u002b', '\u002d', '\u002e', '\u0030', '\u0039', '\u0041', '\u005a', '\u005e', 
	'\u007a', '\u0021', '\u003a', '\u0041', '\u0061', '\u007c', '\u007e', '\u0023', 
	'\u0027', '\u002a', '\u002b', '\u002d', '\u002e', '\u0030', '\u0039', '\u0042', 
	'\u005a', '\u005e', '\u007a', '\u0021', '\u003a', '\u004e', '\u006e', '\u007c', 
	'\u007e', '\u0023', '\u0027', '\u002a', '\u002b', '\u002d', '\u002e', '\u0030', 
	'\u0039', '\u0041', '\u005a', '\u005e', '\u007a', '\u0021', '\u003a', '\u0053', 
	'\u0073', '\u007c', '\u007e', '\u0023', '\u0027', '\u002a', '\u002b', '\u002d', 
	'\u002e', '\u0030', '\u0039', '\u0041', '\u005a', '\u005e', '\u007a', '\u0021', 
	'\u003a', '\u0046', '\u0066', '\u007c', '\u007e', '\u0023', '\u0027', '\u002a', 
	'\u002b', '\u002d', '\u002e', '\u0030', '\u0039', '\u0041', '\u005a', '\u005e', 
	'\u007a', '\u0021', '\u003a', '\u0045', '\u0065', '\u007c', '\u007e', '\u0023', 
	'\u0027', '\u002a', '\u002b', '\u002d', '\u002e', '\u0030', '\u0039', '\u0041', 
	'\u005a', '\u005e', '\u007a', '\u0021', '\u003a', '\u0052', '\u0072', '\u007c', 
	'\u007e', '\u0023', '\u0027', '\u002a', '\u002b', '\u002d', '\u002e', '\u0030', 
	'\u0039', '\u0041', '\u005a', '\u005e', '\u007a', '\u0021', '\u002d', '\u002e', 
	'\u003a', '\u007c', '\u007e', '\u0023', '\u0027', '\u002a', '\u002b', '\u0030', 
	'\u0039', '\u0041', '\u005a', '\u005e', '\u007a', '\u0021', '\u003a', '\u0045', 
	'\u0065', '\u007c', '\u007e', '\u0023', '\u0027', '\u002a', '\u002b', '\u002d', 
	'\u002e', '\u0030', '\u0039', '\u0041', '\u005a', '\u005e', '\u007a', '\u0021', 
	'\u003a', '\u004e', '\u006e', '\u007c', '\u007e', '\u0023', '\u0027', '\u002a', 
	'\u002b', '\u002d', '\u002e', '\u0030', '\u0039', '\u0041', '\u005a', '\u005e', 
	'\u007a', '\u0021', '\u003a', '\u0043', '\u0063', '\u007c', '\u007e', '\u0023', 
	'\u0027', '\u002a', '\u002b', '\u002d', '\u002e', '\u0030', '\u0039', '\u0041', 
	'\u005a', '\u005e', '\u007a', '\u0021', '\u003a', '\u004f', '\u006f', '\u007c', 
	'\u007e', '\u0023', '\u0027', '\u002a', '\u002b', '\u002d', '\u002e', '\u0030', 
	'\u0039', '\u0041', '\u005a', '\u005e', '\u007a', '\u0021', '\u003a', '\u0044', 
	'\u0064', '\u007c', '\u007e', '\u0023', '\u0027', '\u002a', '\u002b', '\u002d', 
	'\u002e', '\u0030', '\u0039', '\u0041', '\u005a', '\u005e', '\u007a', '\u0021', 
	'\u003a', '\u0049', '\u0069', '\u007c', '\u007e', '\u0023', '\u0027', '\u002a', 
	'\u002b', '\u002d', '\u002e', '\u0030', '\u0039', '\u0041', '\u005a', '\u005e', 
	'\u007a', '\u0021', '\u003a', '\u004e', '\u006e', '\u007c', '\u007e', '\u0023', 
	'\u0027', '\u002a', '\u002b', '\u002d', '\u002e', '\u0030', '\u0039', '\u0041', 
	'\u005a', '\u005e', '\u007a', '\u0021', '\u003a', '\u0047', '\u0067', '\u007c', 
	'\u007e', '\u0023', '\u0027', '\u002a', '\u002b', '\u002d', '\u002e', '\u0030', 
	'\u0039', '\u0041', '\u005a', '\u005e', '\u007a', '\u0021', '\u003a', '\u007c', 
	'\u007e', '\u0023', '\u0027', '\u002a', '\u002b', '\u002d', '\u002e', '\u0030', 
	'\u0039', '\u0041', '\u005a', '\u005e', '\u007a', '\u0021', '\u003a', '\u0050', 
	'\u0070', '\u007c', '\u007e', '\u0023', '\u0027', '\u002a', '\u002b', '\u002d', 
	'\u002e', '\u0030', '\u0039', '\u0041', '\u005a', '\u005e', '\u007a', '\u0021', 
	'\u003a', '\u0047', '\u0067', '\u007c', '\u007e', '\u0023', '\u0027', '\u002a', 
	'\u002b', '\u002d', '\u002e', '\u0030', '\u0039', '\u0041', '\u005a', '\u005e', 
	'\u007a', '\u0021', '\u003a', '\u0052', '\u0072', '\u007c', '\u007e', '\u0023', 
	'\u0027', '\u002a', '\u002b', '\u002d', '\u002e', '\u0030', '\u0039', '\u0041', 
	'\u005a', '\u005e', '\u007a', '\u0021', '\u003a', '\u0041', '\u0061', '\u007c', 
	'\u007e', '\u0023', '\u0027', '\u002a', '\u002b', '\u002d', '\u002e', '\u0030', 
	'\u0039', '\u0042', '\u005a', '\u005e', '\u007a', '\u0021', '\u003a', '\u0044', 
	'\u0064', '\u007c', '\u007e', '\u0023', '\u0027', '\u002a', '\u002b', '\u002d', 
	'\u002e', '\u0030', '\u0039', '\u0041', '\u005a', '\u005e', '\u007a', '\u0021', 
	'\u003a', '\u0045', '\u0065', '\u007c', '\u007e', '\u0023', '\u0027', '\u002a', 
	'\u002b', '\u002d', '\u002e', '\u0030', '\u0039', '\u0041', '\u005a', '\u005e', 
	'\u007a', '\u0021', '\u003a', '\u007c', '\u007e', '\u0023', '\u0027', '\u002a', 
	'\u002b', '\u002d', '\u002e', '\u0030', '\u0039', '\u0041', '\u005a', '\u005e', 
	'\u007a', '\u0048', '\u0054', '\u0054', '\u0050', '\u002f', '\u0030', '\u0039', 
	'\u002e', '\u0030', '\u0039', '\u000d', '\u000d', '\u0020', '\u0021', '\u0025', 
	'\u003d', '\u005f', '\u007e', '\u0024', '\u002e', '\u0030', '\u003b', '\u0040', 
	'\u005a', '\u0061', '\u007a', '\u0030', '\u0039', '\u0041', '\u0046', '\u0061', 
	'\u0066', '\u0030', '\u0039', '\u0041', '\u0046', '\u0061', '\u0066', '\u000d', 
	'\u0020', '\u0021', '\u0023', '\u0025', '\u003d', '\u003f', '\u005f', '\u007e', 
	'\u0024', '\u003b', '\u0040', '\u005a', '\u0061', '\u007a', '\u000d', '\u0020', 
	'\u0021', '\u0025', '\u003d', '\u005f', '\u007e', '\u0024', '\u003b', '\u003f', 
	'\u005a', '\u0061', '\u007a', '\u000d', '\u0020', '\u0021', '\u0025', '\u003d', 
	'\u005f', '\u007e', '\u0024', '\u003b', '\u003f', '\u005a', '\u0061', '\u007a', 
	'\u0030', '\u0039', '\u0041', '\u0046', '\u0061', '\u0066', '\u0030', '\u0039', 
	'\u0041', '\u0046', '\u0061', '\u0066', '\u0030', '\u0039', '\u0041', '\u0046', 
	'\u0061', '\u0066', '\u0030', '\u0039', '\u0041', '\u0046', '\u0061', '\u0066', 
	'\u000d', '\u0020', '\u0021', '\u0023', '\u0025', '\u003d', '\u005f', '\u007e', 
	'\u0024', '\u003b', '\u003f', '\u005a', '\u0061', '\u007a', '\u000d', '\u0020', 
	'\u0021', '\u0023', '\u0025', '\u003d', '\u005f', '\u007e', '\u0024', '\u003b', 
	'\u003f', '\u005a', '\u0061', '\u007a', '\u0030', '\u0039', '\u0041', '\u0046', 
	'\u0061', '\u0066', '\u0030', '\u0039', '\u0041', '\u0046', '\u0061', '\u0066', 
	'\u000d', '\u0020', '\u0021', '\u0025', '\u002b', '\u003d', '\u0040', '\u005f', 
	'\u007e', '\u0024', '\u002c', '\u002d', '\u002e', '\u0030', '\u0039', '\u003a', 
	'\u003b', '\u0041', '\u005a', '\u0061', '\u007a', '\u000d', '\u0020', '\u0021', 
	'\u0025', '\u002b', '\u003a', '\u003b', '\u003d', '\u0040', '\u005f', '\u007e', 
	'\u0024', '\u002c', '\u002d', '\u002e', '\u0030', '\u0039', '\u0041', '\u005a', 
	'\u0061', '\u007a', '\u000d', '\u0020', '\u0021', '\u0025', '\u003d', '\u005f', 
	'\u007e', '\u0024', '\u003b', '\u003f', '\u005a', '\u0061', '\u007a', '\u0030', 
	'\u0039', '\u0041', '\u0046', '\u0061', '\u0066', '\u0030', '\u0039', '\u0041', 
	'\u0046', '\u0061', '\u0066', '\u0020', '\u0041', '\u005a', '\u0061', '\u007a', 
	'\u0020', '\u0041', '\u005a', '\u0061', '\u007a', '\u0020', '\u0041', '\u005a', 
	'\u0061', '\u007a', '\u0020', '\u0041', '\u005a', '\u0061', '\u007a', '\u0020', 
	'\u0041', '\u005a', '\u0061', '\u007a', '\u0020', '\u0041', '\u005a', '\u0061', 
	'\u007a', '\u0020', '\u0041', '\u005a', '\u0061', '\u007a', '\u0020', '\u0041', 
	'\u005a', '\u0061', '\u007a', '\u0020', '\u0041', '\u005a', '\u0061', '\u007a', 
	'\u0020', '\u0041', '\u005a', '\u0061', '\u007a', '\u0020', '\u0041', '\u005a', 
	'\u0061', '\u007a', '\u0020', '\u0041', '\u005a', '\u0061', '\u007a', '\u0020', 
	'\u0041', '\u005a', '\u0061', '\u007a', '\u0020', '\u0041', '\u005a', '\u0061', 
	'\u007a', '\u0020', '\u0041', '\u005a', '\u0061', '\u007a', '\u0020', '\u0041', 
	'\u005a', '\u0061', '\u007a', '\u0020', '\u0041', '\u005a', '\u0061', '\u007a', 
	'\u0020', '\u0041', '\u005a', '\u0061', '\u007a', '\u0020', '\u0041', '\u005a', 
	'\u0061', '\u007a', '\u0020', '\u0041', '\u005a', '\u0061', '\u007a', '\u0020', 
	'\u0041', '\u005a', '\u0061', '\u007a', '\u0020', '\u0041', '\u005a', '\u0061', 
	'\u007a', '\u0020', '\u0020', '\u0054', '\u0041', '\u005a', '\u0061', '\u007a', 
	'\u0020', '\u0054', '\u0041', '\u005a', '\u0061', '\u007a', '\u0020', '\u0050', 
	'\u0041', '\u005a', '\u0061', '\u007a', '\u0020', '\u002f', '\u0041', '\u005a', 
	'\u0061', '\u007a', '\u0030', '\u0039', '\u002e', '\u0030', '\u0039', '\u0020', 
	'\u0030', '\u0039', '\u000d', '\u0020', '\u0030', '\u0039', '\u000a', '\u000d', 
	'\u0021', '\u0043', '\u0054', '\u0055', '\u0063', '\u0074', '\u0075', '\u007c', 
	'\u007e', '\u0023', '\u0027', '\u002a', '\u002b', '\u002d', '\u002e', '\u0030', 
	'\u0039', '\u0041', '\u005a', '\u005e', '\u007a', '\u0000', '\u0009', '\u000b', 
	'\u000c', '\u000e', '\u007f', '\u000d', '\u0000', '\u0009', '\u000b', '\u007f', 
	(char) 0
};

static readonly sbyte[] _http_parser_single_lengths =  new sbyte [] {
	0, 2, 1, 1, 1, 9, 1, 10, 
	1, 4, 8, 2, 6, 4, 4, 4, 
	4, 4, 2, 4, 4, 4, 2, 4, 
	4, 4, 3, 4, 4, 4, 4, 4, 
	2, 6, 6, 8, 6, 6, 6, 6, 
	6, 6, 4, 6, 6, 6, 6, 6, 
	6, 6, 6, 6, 6, 4, 6, 6, 
	6, 6, 6, 6, 6, 6, 6, 6, 
	6, 6, 6, 6, 6, 6, 4, 6, 
	6, 6, 6, 6, 6, 4, 1, 1, 
	1, 1, 1, 0, 1, 0, 1, 7, 
	0, 0, 9, 7, 7, 0, 0, 0, 
	0, 8, 8, 0, 0, 9, 11, 7, 
	0, 0, 1, 1, 1, 1, 1, 1, 
	1, 1, 1, 1, 1, 1, 1, 1, 
	1, 1, 1, 1, 1, 1, 1, 1, 
	1, 2, 2, 2, 2, 0, 1, 0, 
	1, 0, 2, 1, 10, 0, 1, 0, 
	0, 0, 0, 0, 0
};

static readonly sbyte[] _http_parser_range_lengths =  new sbyte [] {
	0, 2, 0, 2, 2, 3, 0, 6, 
	0, 6, 0, 0, 0, 0, 0, 0, 
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 6, 6, 6, 6, 6, 6, 6, 
	6, 6, 6, 6, 6, 6, 5, 6, 
	6, 6, 6, 6, 6, 6, 6, 6, 
	6, 6, 6, 6, 6, 5, 6, 6, 
	6, 6, 6, 6, 6, 6, 6, 6, 
	6, 6, 6, 6, 6, 6, 0, 0, 
	0, 0, 0, 1, 0, 1, 0, 4, 
	3, 3, 3, 3, 3, 3, 3, 3, 
	3, 3, 3, 3, 3, 6, 5, 3, 
	3, 3, 2, 2, 2, 2, 2, 2, 
	2, 2, 2, 2, 2, 2, 2, 2, 
	2, 2, 2, 2, 2, 2, 2, 2, 
	0, 2, 2, 2, 2, 1, 0, 1, 
	0, 1, 1, 0, 6, 3, 2, 0, 
	0, 0, 0, 0, 0
};

static readonly short[] _http_parser_index_offsets =  new short [] {
	0, 0, 5, 7, 11, 15, 28, 30, 
	47, 49, 60, 69, 72, 79, 84, 89, 
	94, 99, 104, 107, 112, 117, 122, 125, 
	130, 135, 140, 144, 149, 154, 159, 164, 
	169, 172, 185, 198, 213, 226, 239, 252, 
	265, 278, 291, 302, 315, 328, 341, 353, 
	366, 379, 392, 405, 418, 431, 442, 455, 
	468, 481, 494, 507, 520, 533, 545, 558, 
	571, 584, 597, 610, 623, 636, 649, 660, 
	673, 686, 699, 712, 725, 738, 749, 751, 
	753, 755, 757, 759, 761, 763, 765, 767, 
	779, 783, 787, 800, 811, 822, 826, 830, 
	834, 838, 850, 862, 866, 870, 886, 903, 
	914, 918, 922, 926, 930, 934, 938, 942, 
	946, 950, 954, 958, 962, 966, 970, 974, 
	978, 982, 986, 990, 994, 998, 1002, 1006, 
	1010, 1012, 1017, 1022, 1027, 1032, 1034, 1036, 
	1038, 1040, 1042, 1046, 1048, 1065, 1069, 1073, 
	1074, 1075, 1076, 1077, 1078
};

static readonly byte[] _http_parser_indicies =  new byte [] {
	0, 3, 2, 2, 1, 4, 1, 6, 
	5, 5, 1, 7, 8, 8, 1, 9, 
	10, 11, 12, 13, 11, 11, 11, 11, 
	11, 14, 14, 1, 15, 1, 16, 17, 
	18, 19, 20, 18, 19, 20, 17, 17, 
	17, 17, 17, 17, 17, 17, 1, 21, 
	1, 22, 23, 22, 22, 22, 22, 22, 
	22, 22, 22, 1, 25, 1, 1, 25, 
	26, 27, 26, 27, 24, 1, 29, 28, 
	1, 29, 30, 31, 30, 31, 28, 1, 
	29, 32, 32, 28, 1, 29, 33, 33, 
	28, 1, 29, 34, 34, 28, 1, 29, 
	35, 35, 28, 1, 29, 36, 36, 28, 
	1, 37, 28, 1, 29, 38, 38, 28, 
	1, 29, 39, 39, 28, 1, 29, 40, 
	40, 28, 1, 41, 28, 1, 29, 42, 
	42, 28, 1, 29, 43, 43, 28, 1, 
	29, 44, 44, 28, 1, 29, 45, 28, 
	1, 29, 46, 46, 28, 1, 29, 47, 
	47, 28, 1, 29, 48, 48, 28, 1, 
	29, 49, 49, 28, 1, 29, 50, 50, 
	28, 1, 51, 28, 22, 23, 52, 52, 
	22, 22, 22, 22, 22, 22, 22, 22, 
	1, 22, 23, 53, 53, 22, 22, 22, 
	22, 22, 22, 22, 22, 1, 22, 23, 
	54, 55, 54, 55, 22, 22, 22, 22, 
	22, 22, 22, 22, 1, 22, 23, 56, 
	56, 22, 22, 22, 22, 22, 22, 22, 
	22, 1, 22, 23, 57, 57, 22, 22, 
	22, 22, 22, 22, 22, 22, 1, 22, 
	23, 58, 58, 22, 22, 22, 22, 22, 
	22, 22, 22, 1, 22, 23, 59, 59, 
	22, 22, 22, 22, 22, 22, 22, 22, 
	1, 22, 23, 60, 60, 22, 22, 22, 
	22, 22, 22, 22, 22, 1, 22, 23, 
	61, 61, 22, 22, 22, 22, 22, 22, 
	22, 22, 1, 22, 62, 22, 22, 22, 
	22, 22, 22, 22, 22, 1, 22, 23, 
	63, 63, 22, 22, 22, 22, 22, 22, 
	22, 22, 1, 22, 23, 64, 64, 22, 
	22, 22, 22, 22, 22, 22, 22, 1, 
	22, 23, 65, 65, 22, 22, 22, 22, 
	22, 22, 22, 22, 1, 22, 66, 22, 
	23, 22, 22, 22, 22, 22, 22, 22, 
	1, 22, 23, 67, 67, 22, 22, 22, 
	22, 22, 22, 22, 22, 1, 22, 23, 
	68, 68, 22, 22, 22, 22, 22, 22, 
	22, 22, 1, 22, 23, 69, 69, 22, 
	22, 22, 22, 22, 22, 22, 22, 1, 
	22, 23, 70, 70, 22, 22, 22, 22, 
	22, 22, 22, 22, 1, 22, 23, 71, 
	71, 22, 22, 22, 22, 22, 22, 22, 
	22, 1, 22, 23, 72, 72, 22, 22, 
	22, 22, 22, 22, 22, 22, 1, 22, 
	73, 22, 22, 22, 22, 22, 22, 22, 
	22, 1, 22, 23, 74, 74, 22, 22, 
	22, 22, 22, 22, 22, 22, 1, 22, 
	23, 75, 75, 22, 22, 22, 22, 22, 
	22, 22, 22, 1, 22, 23, 76, 76, 
	22, 22, 22, 22, 22, 22, 22, 22, 
	1, 22, 23, 77, 77, 22, 22, 22, 
	22, 22, 22, 22, 22, 1, 22, 23, 
	78, 78, 22, 22, 22, 22, 22, 22, 
	22, 22, 1, 22, 23, 79, 79, 22, 
	22, 22, 22, 22, 22, 22, 22, 1, 
	22, 23, 80, 80, 22, 22, 22, 22, 
	22, 22, 22, 22, 1, 22, 81, 22, 
	23, 22, 22, 22, 22, 22, 22, 22, 
	1, 22, 23, 82, 82, 22, 22, 22, 
	22, 22, 22, 22, 22, 1, 22, 23, 
	83, 83, 22, 22, 22, 22, 22, 22, 
	22, 22, 1, 22, 23, 84, 84, 22, 
	22, 22, 22, 22, 22, 22, 22, 1, 
	22, 23, 85, 85, 22, 22, 22, 22, 
	22, 22, 22, 22, 1, 22, 23, 86, 
	86, 22, 22, 22, 22, 22, 22, 22, 
	22, 1, 22, 23, 87, 87, 22, 22, 
	22, 22, 22, 22, 22, 22, 1, 22, 
	23, 88, 88, 22, 22, 22, 22, 22, 
	22, 22, 22, 1, 22, 23, 89, 89, 
	22, 22, 22, 22, 22, 22, 22, 22, 
	1, 22, 90, 22, 22, 22, 22, 22, 
	22, 22, 22, 1, 22, 23, 91, 91, 
	22, 22, 22, 22, 22, 22, 22, 22, 
	1, 22, 23, 92, 92, 22, 22, 22, 
	22, 22, 22, 22, 22, 1, 22, 23, 
	93, 93, 22, 22, 22, 22, 22, 22, 
	22, 22, 1, 22, 23, 94, 94, 22, 
	22, 22, 22, 22, 22, 22, 22, 1, 
	22, 23, 95, 95, 22, 22, 22, 22, 
	22, 22, 22, 22, 1, 22, 23, 96, 
	96, 22, 22, 22, 22, 22, 22, 22, 
	22, 1, 22, 97, 22, 22, 22, 22, 
	22, 22, 22, 22, 1, 98, 1, 99, 
	1, 100, 1, 101, 1, 102, 1, 103, 
	1, 104, 1, 105, 1, 106, 1, 107, 
	108, 109, 110, 109, 109, 109, 109, 109, 
	109, 109, 1, 111, 111, 111, 1, 109, 
	109, 109, 1, 112, 113, 114, 115, 116, 
	114, 117, 114, 114, 114, 114, 114, 1, 
	118, 119, 120, 121, 120, 120, 120, 120, 
	120, 120, 1, 122, 123, 124, 125, 124, 
	124, 124, 124, 124, 124, 1, 126, 126, 
	126, 1, 124, 124, 124, 1, 127, 127, 
	127, 1, 114, 114, 114, 1, 128, 129, 
	130, 131, 132, 130, 130, 130, 130, 130, 
	130, 1, 133, 134, 135, 136, 137, 135, 
	135, 135, 135, 135, 135, 1, 138, 138, 
	138, 1, 135, 135, 135, 1, 107, 108, 
	109, 110, 139, 109, 109, 109, 109, 109, 
	139, 139, 109, 139, 139, 1, 107, 108, 
	109, 110, 139, 140, 109, 109, 109, 109, 
	109, 109, 139, 139, 139, 139, 1, 107, 
	108, 140, 141, 140, 140, 140, 140, 140, 
	140, 1, 142, 142, 142, 1, 140, 140, 
	140, 1, 7, 143, 143, 1, 7, 144, 
	144, 1, 7, 145, 145, 1, 7, 146, 
	146, 1, 7, 147, 147, 1, 7, 148, 
	148, 1, 7, 149, 149, 1, 7, 150, 
	150, 1, 7, 151, 151, 1, 7, 152, 
	152, 1, 7, 153, 153, 1, 7, 154, 
	154, 1, 7, 155, 155, 1, 7, 156, 
	156, 1, 7, 157, 157, 1, 7, 158, 
	158, 1, 7, 159, 159, 1, 7, 160, 
	160, 1, 7, 161, 161, 1, 7, 162, 
	162, 1, 7, 163, 163, 1, 7, 164, 
	164, 1, 7, 1, 7, 165, 8, 8, 
	1, 7, 166, 143, 143, 1, 7, 167, 
	144, 144, 1, 7, 168, 145, 145, 1, 
	169, 1, 170, 1, 171, 1, 172, 1, 
	173, 1, 174, 175, 176, 1, 177, 1, 
	178, 179, 180, 181, 182, 180, 181, 182, 
	179, 179, 179, 179, 179, 179, 179, 179, 
	1, 183, 183, 183, 1, 185, 184, 184, 
	1, 186, 187, 1, 186, 188, 1, 0
};

static readonly byte[] _http_parser_trans_targs =  new byte [] {
	2, 0, 4, 129, 3, 4, 129, 5, 
	106, 6, 78, 87, 88, 90, 101, 7, 
	8, 9, 33, 54, 71, 145, 9, 10, 
	11, 10, 12, 23, 11, 6, 13, 19, 
	14, 15, 16, 17, 18, 6, 20, 21, 
	22, 6, 24, 25, 26, 27, 28, 29, 
	30, 31, 32, 6, 34, 35, 36, 43, 
	37, 38, 39, 40, 41, 42, 10, 44, 
	45, 46, 47, 48, 49, 50, 51, 52, 
	53, 10, 55, 56, 57, 58, 59, 60, 
	61, 62, 63, 64, 65, 66, 67, 68, 
	69, 70, 10, 72, 73, 74, 75, 76, 
	77, 10, 79, 80, 81, 82, 83, 84, 
	85, 86, 6, 6, 78, 87, 88, 89, 
	6, 78, 90, 91, 95, 97, 6, 78, 
	92, 93, 6, 78, 92, 93, 94, 96, 
	6, 78, 98, 91, 99, 6, 78, 98, 
	91, 99, 100, 102, 103, 104, 105, 107, 
	108, 109, 110, 111, 112, 113, 114, 115, 
	116, 117, 118, 119, 120, 121, 122, 123, 
	124, 125, 126, 127, 128, 130, 131, 132, 
	133, 134, 135, 136, 137, 138, 139, 141, 
	138, 140, 8, 9, 33, 54, 71, 142, 
	142, 139, 146, 148, 147
};

static readonly byte[] _http_parser_trans_actions =  new byte [] {
	37, 0, 95, 95, 7, 31, 31, 40, 
	1, 120, 120, 115, 115, 131, 125, 0, 
	0, 31, 31, 31, 31, 23, 1, 19, 
	31, 0, 31, 31, 1, 21, 1, 1, 
	1, 1, 1, 1, 1, 73, 1, 1, 
	1, 64, 1, 1, 1, 1, 1, 1, 
	1, 1, 1, 67, 1, 1, 1, 1, 
	1, 1, 1, 1, 1, 1, 61, 1, 
	1, 1, 1, 1, 1, 1, 1, 1, 
	1, 58, 1, 1, 1, 1, 1, 1, 
	1, 1, 1, 1, 1, 1, 1, 1, 
	1, 1, 70, 1, 1, 1, 1, 1, 
	1, 76, 0, 0, 0, 0, 0, 15, 
	0, 17, 0, 9, 9, 1, 1, 1, 
	46, 46, 34, 43, 34, 43, 91, 91, 
	79, 79, 55, 55, 34, 34, 34, 34, 
	87, 87, 79, 83, 79, 52, 52, 34, 
	49, 34, 34, 1, 1, 1, 1, 1, 
	1, 1, 1, 1, 1, 1, 1, 1, 
	1, 1, 1, 1, 1, 1, 1, 1, 
	1, 1, 1, 1, 1, 1, 1, 1, 
	0, 15, 0, 17, 3, 99, 103, 11, 
	1, 1, 13, 111, 111, 111, 111, 31, 
	1, 107, 25, 0, 27
};

static readonly byte[] _http_parser_from_state_actions =  new byte [] {
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 0, 0, 0, 29
};

static readonly byte[] _http_parser_eof_actions =  new byte [] {
	0, 0, 0, 0, 0, 5, 0, 0, 
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 5, 0, 0, 0, 0, 0, 0, 
	0, 0, 0, 27, 0
};

const int http_parser_start = 1;
const int http_parser_first_final = 145;
const int http_parser_error = 0;

const int http_parser_en_main = 1;
const int http_parser_en_body_identity = 143;
const int http_parser_en_body_identity_eof = 147;
const int http_parser_en_dead = 144;


#line 365 "HttpParser.cs.rl"
        
        public HttpParser(IHttpParserDelegate del)
        {
            this.del = del;
			sb = new StringBuilder();
            
#line 587 "..\\HttpParser.cs"
	{
	cs = http_parser_start;
	}

#line 371 "HttpParser.cs.rl"
        }

        public int Execute(ArraySegment<byte> buf)
        {
            byte[] data = buf.Array;
            int p = buf.Offset;
            int pe = buf.Offset + buf.Count;
            int eof = buf.Count == 0 ? buf.Offset : -1;
            //int eof = pe;
            // mark = 0;
            
			//if (p == pe)
			//	Console.WriteLine("Parser executing on p == pe (EOF)");

            
#line 604 "..\\HttpParser.cs"
	{
	sbyte _klen;
	short _trans;
	int _acts;
	int _nacts;
	short _keys;

	if ( p == pe )
		goto _test_eof;
	if ( cs == 0 )
		goto _out;
_resume:
	_acts = _http_parser_from_state_actions[cs];
	_nacts = _http_parser_actions[_acts++];
	while ( _nacts-- > 0 ) {
		switch ( _http_parser_actions[_acts++] ) {
	case 34:
#line 356 "HttpParser.cs.rl"
	{
			throw new Exception("Parser is dead; there shouldn't be more data. Client is bogus? fpc =" + p);
		}
	break;
#line 625 "..\\HttpParser.cs"
		default: break;
		}
	}

	_keys = _http_parser_key_offsets[cs];
	_trans = (short)_http_parser_index_offsets[cs];

	_klen = _http_parser_single_lengths[cs];
	if ( _klen > 0 ) {
		short _lower = _keys;
		short _mid;
		short _upper = (short) (_keys + _klen - 1);
		while (true) {
			if ( _upper < _lower )
				break;

			_mid = (short) (_lower + ((_upper-_lower) >> 1));
			if ( data[p] < _http_parser_trans_keys[_mid] )
				_upper = (short) (_mid - 1);
			else if ( data[p] > _http_parser_trans_keys[_mid] )
				_lower = (short) (_mid + 1);
			else {
				_trans += (short) (_mid - _keys);
				goto _match;
			}
		}
		_keys += (short) _klen;
		_trans += (short) _klen;
	}

	_klen = _http_parser_range_lengths[cs];
	if ( _klen > 0 ) {
		short _lower = _keys;
		short _mid;
		short _upper = (short) (_keys + (_klen<<1) - 2);
		while (true) {
			if ( _upper < _lower )
				break;

			_mid = (short) (_lower + (((_upper-_lower) >> 1) & ~1));
			if ( data[p] < _http_parser_trans_keys[_mid] )
				_upper = (short) (_mid - 2);
			else if ( data[p] > _http_parser_trans_keys[_mid+1] )
				_lower = (short) (_mid + 2);
			else {
				_trans += (short)((_mid - _keys)>>1);
				goto _match;
			}
		}
		_trans += (short) _klen;
	}

_match:
	_trans = (short)_http_parser_indicies[_trans];
	cs = _http_parser_trans_targs[_trans];

	if ( _http_parser_trans_actions[_trans] == 0 )
		goto _again;

	_acts = _http_parser_trans_actions[_trans];
	_nacts = _http_parser_actions[_acts++];
	while ( _nacts-- > 0 )
	{
		switch ( _http_parser_actions[_acts++] )
		{
	case 0:
#line 57 "HttpParser.cs.rl"
	{
			sb.Append((char)data[p]);
		}
	break;
	case 1:
#line 61 "HttpParser.cs.rl"
	{
			sb.Length = 0;
		}
	break;
	case 2:
#line 65 "HttpParser.cs.rl"
	{
			sb2.Append((char)data[p]);
		}
	break;
	case 3:
#line 69 "HttpParser.cs.rl"
	{
			if (sb2 == null)
				sb2 = new StringBuilder();
			sb2.Length = 0;
		}
	break;
	case 4:
#line 75 "HttpParser.cs.rl"
	{
			//Console.WriteLine("message_begin");
			versionMajor = 0;
			versionMinor = 9;
			contentLength = -1;

			inContentLengthHeader = false;
			inConnectionHeader = false;
			inTransferEncodingHeader = false;
			inUpgradeHeader = false;

			gotConnectionClose = false;
			gotConnectionKeepAlive = false;
			gotTransferEncodingChunked = false;
			gotUpgradeValue = false;
			del.OnMessageBegin(this);
		}
	break;
	case 5:
#line 93 "HttpParser.cs.rl"
	{
            //Console.WriteLine("matched absolute_uri");
        }
	break;
	case 6:
#line 96 "HttpParser.cs.rl"
	{
            //Console.WriteLine("matched abs_path");
        }
	break;
	case 7:
#line 99 "HttpParser.cs.rl"
	{
            //Console.WriteLine("matched authority");
        }
	break;
	case 8:
#line 102 "HttpParser.cs.rl"
	{
            //Console.WriteLine("matched first space");
        }
	break;
	case 9:
#line 105 "HttpParser.cs.rl"
	{
            //Console.WriteLine("leave_first_space");
        }
	break;
	case 11:
#line 114 "HttpParser.cs.rl"
	{
			//Console.WriteLine("matched_leading_crlf");
		}
	break;
	case 12:
#line 124 "HttpParser.cs.rl"
	{
			del.OnMethod(this, sb.ToString());
		}
	break;
	case 13:
#line 128 "HttpParser.cs.rl"
	{
			del.OnRequestUri(this, sb.ToString());
		}
	break;
	case 14:
#line 133 "HttpParser.cs.rl"
	{
			del.OnPath(this, sb2.ToString());
		}
	break;
	case 15:
#line 138 "HttpParser.cs.rl"
	{
			del.OnQueryString(this, sb2.ToString());
		}
	break;
	case 16:
#line 143 "HttpParser.cs.rl"
	{
			statusCode = int.Parse(sb.ToString());
		}
	break;
	case 17:
#line 148 "HttpParser.cs.rl"
	{
			statusReason = sb.ToString();
		}
	break;
	case 18:
#line 153 "HttpParser.cs.rl"
	{
			del.OnResponseCode(this, statusCode, statusReason);
			statusReason = null;
			statusCode = 0;
		}
	break;
	case 19:
#line 170 "HttpParser.cs.rl"
	{
			del.OnFragment(this, sb2.ToString());
		}
	break;
	case 20:
#line 184 "HttpParser.cs.rl"
	{
			versionMajor = (char)data[p] - '0';
		}
	break;
	case 21:
#line 188 "HttpParser.cs.rl"
	{
			versionMinor = (char)data[p] - '0';
		}
	break;
	case 22:
#line 192 "HttpParser.cs.rl"
	{
            if (contentLength != -1) throw new Exception("Already got Content-Length. Possible attack?");
			//Console.WriteLine("Saw content length");
			contentLength = 0;
			inContentLengthHeader = true;
        }
	break;
	case 23:
#line 199 "HttpParser.cs.rl"
	{
			//Console.WriteLine("header_connection");
			inConnectionHeader = true;
		}
	break;
	case 24:
#line 204 "HttpParser.cs.rl"
	{
			//Console.WriteLine("header_connection_close");
			if (inConnectionHeader)
				gotConnectionClose = true;
		}
	break;
	case 25:
#line 210 "HttpParser.cs.rl"
	{
			//Console.WriteLine("header_connection_keepalive");
			if (inConnectionHeader)
				gotConnectionKeepAlive = true;
		}
	break;
	case 26:
#line 216 "HttpParser.cs.rl"
	{
			//Console.WriteLine("Saw transfer encoding");
			inTransferEncodingHeader = true;
		}
	break;
	case 27:
#line 221 "HttpParser.cs.rl"
	{
			if (inTransferEncodingHeader)
				gotTransferEncodingChunked = true;
		}
	break;
	case 28:
#line 226 "HttpParser.cs.rl"
	{
			inUpgradeHeader = true;
		}
	break;
	case 29:
#line 230 "HttpParser.cs.rl"
	{
			del.OnHeaderName(this, sb.ToString());
		}
	break;
	case 30:
#line 234 "HttpParser.cs.rl"
	{
			var str = sb.ToString();
			//Console.WriteLine("on_header_value '" + str + "'");
			//Console.WriteLine("inContentLengthHeader " + inContentLengthHeader);
			if (inContentLengthHeader)
				contentLength = int.Parse(str);

			inConnectionHeader = inTransferEncodingHeader = inContentLengthHeader = false;
			
			del.OnHeaderValue(this, str);
		}
	break;
	case 31:
#line 246 "HttpParser.cs.rl"
	{
			
			if (data[p] == 10)
			{
				//Console.WriteLine("leave_headers contentLength = " + contentLength);
				del.OnHeadersEnd(this);

				// if chunked transfer, ignore content length and parse chunked (but we can't yet so bail)
				// if content length given but zero, read next request
				// if content length is given and non-zero, we should read that many bytes
				// if content length is not given
				//   if should keep alive, assume next request is coming and read it
				//   else 
				//		if chunked transfer read body until EOF
				//   	else read next request

				if (contentLength == 0)
				{
					del.OnMessageEnd(this);
					//fhold;
					{cs = 1; if (true) goto _again;}
				}
				else if (contentLength > 0)
				{
					//fhold;
					{cs = 143; if (true) goto _again;}
				}
				else
				{
					//Console.WriteLine("Request had no content length.");
					if (ShouldKeepAlive)
					{
						del.OnMessageEnd(this);
						//Console.WriteLine("Should keep alive, will read next message.");
						//fhold;
						{cs = 1; if (true) goto _again;}
					}
					else
					{
						if (gotTransferEncodingChunked) {
							//Console.WriteLine("Not keeping alive, will read until eof. Will hold, but currently fpc = " + fpc);
							//fhold;
							{cs = 147; if (true) goto _again;}
						}
		
						del.OnMessageEnd(this);
						//fhold;
						{cs = 1; if (true) goto _again;}
					}
				}
			}
        }
	break;
	case 32:
#line 299 "HttpParser.cs.rl"
	{
			var toRead = Math.Min(pe - p, contentLength);
			//Console.WriteLine("body_identity: reading " + toRead + " bytes from body.");
			if (toRead > 0)
			{
				del.OnBody(this, new ArraySegment<byte>(data, p, toRead));
				p += toRead - 1;
				contentLength -= toRead;
				//Console.WriteLine("content length is now " + contentLength);

				if (contentLength == 0)
				{
					del.OnMessageEnd(this);

					if (ShouldKeepAlive)
					{
						//Console.WriteLine("Transitioning from identity body to next message.");
						//fhold;
						{cs = 1; if (true) goto _again;}
					}
					else
					{
						//fhold;
						{cs = 144; if (true) goto _again;}
					}
				}
				else
				{
					{p++; if (true) goto _out; }
				}
			}
		}
	break;
	case 33:
#line 332 "HttpParser.cs.rl"
	{
			var toRead = pe - p;
			//Console.WriteLine("body_identity_eof: reading " + toRead + " bytes from body.");
			if (toRead > 0)
			{
				del.OnBody(this, new ArraySegment<byte>(data, p, toRead));
				p += toRead - 1;
				{p++; if (true) goto _out; }
			}
			else
			{
				del.OnMessageEnd(this);
				
				if (ShouldKeepAlive)
					{cs = 1; if (true) goto _again;}
				else
				{
					//Console.WriteLine("body_identity_eof: going to dead");
					p--;
					{cs = 144; if (true) goto _again;}
				}
			}
		}
	break;
#line 989 "..\\HttpParser.cs"
		default: break;
		}
	}

_again:
	if ( cs == 0 )
		goto _out;
	if ( ++p != pe )
		goto _resume;
	_test_eof: {}
	if ( p == eof )
	{
	int __acts = _http_parser_eof_actions[cs];
	int __nacts = _http_parser_actions[__acts++];
	while ( __nacts-- > 0 ) {
		switch ( _http_parser_actions[__acts++] ) {
	case 10:
#line 108 "HttpParser.cs.rl"
	{
            //Console.WriteLine("eof_leave_first_space");
        }
	break;
	case 33:
#line 332 "HttpParser.cs.rl"
	{
			var toRead = pe - p;
			//Console.WriteLine("body_identity_eof: reading " + toRead + " bytes from body.");
			if (toRead > 0)
			{
				del.OnBody(this, new ArraySegment<byte>(data, p, toRead));
				p += toRead - 1;
				{p++; if (true) goto _out; }
			}
			else
			{
				del.OnMessageEnd(this);
				
				if (ShouldKeepAlive)
					{cs = 1; if (true) goto _again;}
				else
				{
					//Console.WriteLine("body_identity_eof: going to dead");
					p--;
					{cs = 144; if (true) goto _again;}
				}
			}
		}
	break;
#line 1035 "..\\HttpParser.cs"
		default: break;
		}
	}
	}

	_out: {}
	}

#line 386 "HttpParser.cs.rl"
            
            var result = p - buf.Offset;

			if (result != buf.Count)
			{
				Console.WriteLine("error on character " + p);
				Console.WriteLine("('" + buf.Array[p] + "')");
				Console.WriteLine("('" + (char)buf.Array[p] + "')");
			}

			return p - buf.Offset;
        }
    }
}