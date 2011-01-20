%%{

machine http;

include uri "uri.rl";

http_crlf = "\r\n";
http_cntrl = (cntrl | 127);
http_separators = "(" | ")" | "<" | ">" | "@" | "," | ";" | ":" | "\\" | "\"" | "/" | "[" | "]" | "?" | "=" | "{" | "}" | " " | "\t";
http_token = (ascii -- (http_cntrl | http_separators))+;


# accept any method name less than 24 chars
http_request_method = alpha {1,24};
#http_request_uri = ("*" | absolute_uri >matched_absolute_uri | abs_path >matched_abs_path | authority >matched_authority);
http_request_uri = "*" | ((any -- (" " | "#" | "?" | http_crlf))+ ("?" ((any -- (" " | "#" | http_crlf))+ >enter_query_string %/leave_query_string %leave_query_string)?)? ("#" ((any -- (" " | http_crlf))+ >enter_fragment %/leave_fragment %leave_fragment)?)?);

# http_version = "HTTP/" (digit{1} >enter_version_major %/eof_leave_version_major %leave_version_major) "." (digit{1} >enter_version_minor %/eof_leave_version_minor %leave_version_minor);
http_version = "HTTP/" (digit{1} $version_major) "." (digit{1} $version_minor);

http_request_line = (http_request_method >enter_method %/eof_leave_method %leave_method) " " (http_request_uri >enter_request_uri %/eof_leave_request_uri %leave_request_uri) " "? http_version? http_crlf;

http_header_name = http_token;

# not getting fancy with header values, just reading everything until CRLF and calling it good. 
# thus we don't support line folding. fuck that noise.
http_header_value = any+;

http_content_length = "content-length"i %leave_content_length;
http_header = (http_header_name | http_content_length) >enter_header_name %/leave_header_name %leave_header_name ((":" (" " | "\t")*) ) <: http_header_value $header_value_char >enter_header_value %/leave_header_value %leave_header_value :> http_crlf;

http_request = http_request_line (http_header)* http_crlf %/leave_headers %leave_headers (any+ >enter_body %/leave_body)?;

main := >message_begin http_request;

}%%