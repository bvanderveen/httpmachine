
#line 1 "HttpMachine.cs.rl"
using System;

namespace HttpSharp
{
    public interface IHttpRequestParser
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
    }

    public class HttpMachine
    {
        int cs;
        int mark;
		int qsMark;
		int fragMark;
        IHttpRequestParser parser;

        
#line 148 "HttpMachine.cs.rl"

        
        
#line 34 "HttpMachine.cs"
static readonly sbyte[] _http_parser_actions =  new sbyte [] {
	0, 1, 0, 1, 1, 1, 2, 1, 
	3, 1, 4, 1, 5, 1, 6, 1, 
	7, 1, 8, 1, 10, 1, 11, 1, 
	12, 1, 13, 1, 14, 1, 15, 1, 
	16, 1, 17, 1, 18, 1, 19, 1, 
	20, 2, 7, 4, 2, 7, 5, 2, 
	9, 4, 2, 9, 5
};

static readonly byte[] _http_parser_key_offsets =  new byte [] {
	0, 0, 4, 9, 12, 15, 16, 17, 
	18, 19, 20, 22, 25, 27, 30, 31, 
	47, 48, 64, 66, 67, 68, 69, 71, 
	73, 78, 83, 88, 93, 98, 103, 108, 
	113, 118, 123, 128, 133, 138, 143, 148, 
	153, 158, 163, 168, 173, 178, 183, 184
};

static readonly char[] _http_parser_trans_keys =  new char [] {
	'\u0041', '\u005a', '\u0061', '\u007a', '\u0020', '\u0041', '\u005a', '\u0061', 
	'\u007a', '\u0020', '\u0023', '\u003f', '\u0020', '\u0023', '\u003f', '\u0048', 
	'\u0054', '\u0054', '\u0050', '\u002f', '\u0030', '\u0039', '\u002e', '\u0030', 
	'\u0039', '\u0030', '\u0039', '\u000d', '\u0030', '\u0039', '\u000a', '\u000d', 
	'\u0021', '\u007c', '\u007e', '\u0023', '\u0027', '\u002a', '\u002b', '\u002d', 
	'\u002e', '\u0030', '\u0039', '\u0041', '\u005a', '\u005e', '\u007a', '\u000a', 
	'\u0021', '\u003a', '\u007c', '\u007e', '\u0023', '\u0027', '\u002a', '\u002b', 
	'\u002d', '\u002e', '\u0030', '\u0039', '\u0041', '\u005a', '\u005e', '\u007a', 
	'\u0009', '\u0020', '\u000d', '\u0020', '\u0020', '\u0020', '\u0023', '\u0020', 
	'\u0023', '\u0020', '\u0041', '\u005a', '\u0061', '\u007a', '\u0020', '\u0041', 
	'\u005a', '\u0061', '\u007a', '\u0020', '\u0041', '\u005a', '\u0061', '\u007a', 
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
	(char) 0
};

static readonly sbyte[] _http_parser_single_lengths =  new sbyte [] {
	0, 0, 1, 3, 3, 1, 1, 1, 
	1, 1, 0, 1, 0, 1, 1, 4, 
	1, 4, 2, 1, 1, 1, 2, 2, 
	1, 1, 1, 1, 1, 1, 1, 1, 
	1, 1, 1, 1, 1, 1, 1, 1, 
	1, 1, 1, 1, 1, 1, 1, 0
};

static readonly sbyte[] _http_parser_range_lengths =  new sbyte [] {
	0, 2, 2, 0, 0, 0, 0, 0, 
	0, 0, 1, 1, 1, 1, 0, 6, 
	0, 6, 0, 0, 0, 0, 0, 0, 
	2, 2, 2, 2, 2, 2, 2, 2, 
	2, 2, 2, 2, 2, 2, 2, 2, 
	2, 2, 2, 2, 2, 2, 0, 0
};

static readonly byte[] _http_parser_index_offsets =  new byte [] {
	0, 0, 3, 7, 11, 15, 17, 19, 
	21, 23, 25, 27, 30, 32, 35, 37, 
	48, 50, 61, 64, 66, 68, 70, 73, 
	76, 80, 84, 88, 92, 96, 100, 104, 
	108, 112, 116, 120, 124, 128, 132, 136, 
	140, 144, 148, 152, 156, 160, 164, 166
};

static readonly sbyte[] _http_parser_indicies =  new sbyte [] {
	0, 0, 1, 2, 3, 3, 1, 1, 
	1, 1, 4, 6, 7, 8, 5, 9, 
	1, 10, 1, 11, 1, 12, 1, 13, 
	1, 14, 1, 15, 16, 1, 17, 1, 
	18, 19, 1, 20, 1, 21, 22, 22, 
	22, 22, 22, 22, 22, 22, 22, 1, 
	23, 1, 24, 25, 24, 24, 24, 24, 
	24, 24, 24, 24, 1, 27, 27, 26, 
	29, 28, 6, 30, 32, 31, 6, 7, 
	33, 35, 36, 34, 2, 37, 37, 1, 
	2, 38, 38, 1, 2, 39, 39, 1, 
	2, 40, 40, 1, 2, 41, 41, 1, 
	2, 42, 42, 1, 2, 43, 43, 1, 
	2, 44, 44, 1, 2, 45, 45, 1, 
	2, 46, 46, 1, 2, 47, 47, 1, 
	2, 48, 48, 1, 2, 49, 49, 1, 
	2, 50, 50, 1, 2, 51, 51, 1, 
	2, 52, 52, 1, 2, 53, 53, 1, 
	2, 54, 54, 1, 2, 55, 55, 1, 
	2, 56, 56, 1, 2, 57, 57, 1, 
	2, 58, 58, 1, 2, 1, 1, 0
};

static readonly sbyte[] _http_parser_trans_targs =  new sbyte [] {
	2, 0, 3, 24, 4, 4, 5, 20, 
	22, 6, 7, 8, 9, 10, 11, 12, 
	11, 13, 14, 13, 15, 16, 17, 47, 
	17, 18, 19, 18, 19, 14, 21, 21, 
	5, 23, 23, 5, 20, 25, 26, 27, 
	28, 29, 30, 31, 32, 33, 34, 35, 
	36, 37, 38, 39, 40, 41, 42, 43, 
	44, 45, 46
};

static readonly sbyte[] _http_parser_trans_actions =  new sbyte [] {
	1, 0, 5, 0, 7, 0, 11, 0, 
	0, 0, 0, 0, 0, 0, 19, 23, 
	0, 25, 29, 0, 0, 0, 31, 0, 
	0, 33, 35, 0, 0, 37, 17, 0, 
	50, 13, 0, 44, 15, 0, 0, 0, 
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 0, 0, 0, 0, 0, 0, 0, 
	0, 0, 0
};

static readonly sbyte[] _http_parser_eof_actions =  new sbyte [] {
	0, 0, 3, 0, 9, 0, 0, 0, 
	0, 0, 0, 21, 0, 27, 0, 0, 
	0, 33, 0, 37, 9, 47, 9, 41, 
	3, 3, 3, 3, 3, 3, 3, 3, 
	3, 3, 3, 3, 3, 3, 3, 3, 
	3, 3, 3, 3, 3, 3, 3, 39
};

const int http_parser_start = 1;
const int http_parser_first_final = 47;
const int http_parser_error = 0;

const int http_parser_en_main = 1;


#line 151 "HttpMachine.cs.rl"
        
        public HttpMachine(IHttpRequestParser parser)
        {
			this.parser = parser;
            
#line 176 "HttpMachine.cs"
	{
	cs = http_parser_start;
	}

#line 156 "HttpMachine.cs.rl"
        }

        public int Execute(ArraySegment<byte> buf)
        {
            byte[] data = buf.Array;
            int p = buf.Offset;
            int pe = buf.Offset + buf.Count;
            //int eof = pe == 0 ? 0 : -1;
			int eof = pe;
			mark = 0;
			qsMark = 0;
			fragMark = 0;
            
            
#line 196 "HttpMachine.cs"
	{
	sbyte _klen;
	byte _trans;
	sbyte _acts;
	sbyte _nacts;
	byte _keys;

	if ( p == pe )
		goto _test_eof;
	if ( cs == 0 )
		goto _out;
_resume:
	_keys = _http_parser_key_offsets[cs];
	_trans = (byte)_http_parser_index_offsets[cs];

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
				_trans += (byte) (_mid - _keys);
				goto _match;
			}
		}
		_keys += (byte) _klen;
		_trans += (byte) _klen;
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
				_trans += (byte)((_mid - _keys)>>1);
				goto _match;
			}
		}
		_trans += (byte) _klen;
	}

_match:
	_trans = (byte)_http_parser_indicies[_trans];
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
#line 44 "HttpMachine.cs.rl"
	{
            mark = p;
        }
	break;
	case 2:
#line 53 "HttpMachine.cs.rl"
	{
			//Console.WriteLine("leave_method fpc " + fpc + " mark " + mark);
            parser.OnMethod(new ArraySegment<byte>(data, mark, p - mark));
        }
	break;
	case 3:
#line 58 "HttpMachine.cs.rl"
	{
			//Console.WriteLine("enter_request_uri fpc " + fpc);
            mark = p;
        }
	break;
	case 5:
#line 68 "HttpMachine.cs.rl"
	{
			//Console.WriteLine("leave_request_uri fpc " + fpc + " mark " + mark);
            parser.OnRequestUri(new ArraySegment<byte>(data, mark, p - mark));
        }
	break;
	case 6:
#line 73 "HttpMachine.cs.rl"
	{
			//Console.WriteLine("enter_query_string fpc " + fpc);
            qsMark = p;
        }
	break;
	case 7:
#line 78 "HttpMachine.cs.rl"
	{
			//Console.WriteLine("leave_query_string fpc " + fpc + " qsMark " + qsMark);
            parser.OnQueryString(new ArraySegment<byte>(data, qsMark, p - qsMark));
        }
	break;
	case 8:
#line 82 "HttpMachine.cs.rl"
	{
			//Console.WriteLine("enter_fragment fpc " + fpc);
            fragMark = p;
        }
	break;
	case 9:
#line 87 "HttpMachine.cs.rl"
	{
			//Console.WriteLine("leave_fragment fpc " + fpc + " fragMark " + fragMark);
            parser.OnFragment(new ArraySegment<byte>(data, fragMark, p - fragMark));
        }
	break;
	case 10:
#line 92 "HttpMachine.cs.rl"
	{
			//Console.WriteLine("enter_version_major fpc " + fpc);
            mark = p;
        }
	break;
	case 12:
#line 102 "HttpMachine.cs.rl"
	{
			//Console.WriteLine("leave_version_major fpc " + fpc + " mark " + mark);
            parser.OnVersionMajor(new ArraySegment<byte>(data, mark, p - mark));
        }
	break;
	case 13:
#line 107 "HttpMachine.cs.rl"
	{
			//Console.WriteLine("enter_request_uri fpc " + fpc);
            mark = p;
        }
	break;
	case 15:
#line 117 "HttpMachine.cs.rl"
	{
			//Console.WriteLine("leave_version_minor fpc " + fpc + " mark " + mark);
            parser.OnVersionMinor(new ArraySegment<byte>(data, mark, p - mark));
        }
	break;
	case 16:
#line 122 "HttpMachine.cs.rl"
	{
			//Console.WriteLine("enter_header_name fpc " + fpc + " fc " + (char)fc);
            mark = p;
        }
	break;
	case 17:
#line 127 "HttpMachine.cs.rl"
	{
			//Console.WriteLine("leave_header_name fpc " + fpc + " fc " + (char)fc);
            parser.OnHeaderName(new ArraySegment<byte>(data, mark, p - mark));
        }
	break;
	case 18:
#line 132 "HttpMachine.cs.rl"
	{
			//Console.WriteLine("enter_header_value fpc " + fpc + " fc " + (char)fc);
            mark = p;
        }
	break;
	case 19:
#line 137 "HttpMachine.cs.rl"
	{
			//Console.WriteLine("leave_header_value fpc " + fpc + " fc " + (char)fc);
            parser.OnHeaderValue(new ArraySegment<byte>(data, mark, p - mark));
        }
	break;
#line 381 "HttpMachine.cs"
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
	sbyte __acts = _http_parser_eof_actions[cs];
	sbyte __nacts = _http_parser_actions[__acts++];
	while ( __nacts-- > 0 ) {
		switch ( _http_parser_actions[__acts++] ) {
	case 1:
#line 48 "HttpMachine.cs.rl"
	{
			//Console.WriteLine("eof_leave_method fpc " + fpc + " mark " + mark);
            parser.OnMethod(new ArraySegment<byte>(data, mark, p - mark));
        }
	break;
	case 4:
#line 63 "HttpMachine.cs.rl"
	{
			//Console.WriteLine("eof_leave_request_uri!! fpc " + fpc + " mark " + mark);
            parser.OnRequestUri(new ArraySegment<byte>(data, mark, p - mark));
        }
	break;
	case 7:
#line 78 "HttpMachine.cs.rl"
	{
			//Console.WriteLine("leave_query_string fpc " + fpc + " qsMark " + qsMark);
            parser.OnQueryString(new ArraySegment<byte>(data, qsMark, p - qsMark));
        }
	break;
	case 9:
#line 87 "HttpMachine.cs.rl"
	{
			//Console.WriteLine("leave_fragment fpc " + fpc + " fragMark " + fragMark);
            parser.OnFragment(new ArraySegment<byte>(data, fragMark, p - fragMark));
        }
	break;
	case 11:
#line 97 "HttpMachine.cs.rl"
	{
			//Console.WriteLine("eof_leave_version_major fpc " + fpc + " mark " + mark);
            parser.OnVersionMajor(new ArraySegment<byte>(data, mark, p - mark));
        }
	break;
	case 14:
#line 112 "HttpMachine.cs.rl"
	{
			//Console.WriteLine("eof_leave_version_minor!! fpc " + fpc + " mark " + mark);
            parser.OnVersionMinor(new ArraySegment<byte>(data, mark, p - mark));
        }
	break;
	case 17:
#line 127 "HttpMachine.cs.rl"
	{
			//Console.WriteLine("leave_header_name fpc " + fpc + " fc " + (char)fc);
            parser.OnHeaderName(new ArraySegment<byte>(data, mark, p - mark));
        }
	break;
	case 19:
#line 137 "HttpMachine.cs.rl"
	{
			//Console.WriteLine("leave_header_value fpc " + fpc + " fc " + (char)fc);
            parser.OnHeaderValue(new ArraySegment<byte>(data, mark, p - mark));
        }
	break;
	case 20:
#line 142 "HttpMachine.cs.rl"
	{
			parser.OnHeadersComplete();
		}
	break;
#line 460 "HttpMachine.cs"
		default: break;
		}
	}
	}

	_out: {}
	}

#line 170 "HttpMachine.cs.rl"
            
            return p - buf.Offset;
        }
    }
}