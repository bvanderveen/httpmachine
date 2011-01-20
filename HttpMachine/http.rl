%%{

machine http;

include uri "uri.rl"; # query, fragment

http_crlf = "\r\n";
http_cntrl = (cntrl | 127);
http_separators = "(" | ")" | "<" | ">" | "@" | "," | ";" | ":" | "\\" | "\"" | "/" | "[" | "]" | "?" | "=" | "{" | "}" | " " | "\t";
http_token = (ascii -- (http_cntrl | http_separators))+;


# not as picky as the spec at the moment, accept any method name less than 24 chars
http_request_method = (alpha {1,24} >enter_method %/eof_leave_method %leave_method);

query_string = (uri_query >enter_query_string %/leave_query_string %leave_query_string);
fragment = (uri_fragment >enter_fragment %/leave_fragment %leave_fragment);

absolute_uri = uri_absolute_uri >matched_absolute_uri;
abs_path = (uri_abs_path ("?" query_string)? ("#" fragment?)?) >matched_abs_path;
authority = uri_authority >matched_authority;

http_request_uri = ("*" | absolute_uri | abs_path | authority) >enter_request_uri %/eof_leave_request_uri %leave_request_uri;

http_version = "HTTP/" (digit{1} $version_major) "." (digit{1} $version_minor);

http_request_line = http_crlf? http_request_method " " http_request_uri (" " http_version)? http_crlf;

# not getting fancy with header values, just reading everything until CRLF and calling it good. 
# thus we don't support line folding. fuck that noise.
http_header_value = any+ $header_value_char >enter_header_value %/leave_header_value %leave_header_value;

http_header_content_length = "content-length"i %leave_header_content_length;
http_header_transfer_encoding = "transfer-encoding"i %leave_header_transfer_encoding;
http_header_connection = "connection"i %leave_header_connection;
http_header_upgrade = "upgrade"i %leave_header_upgrade;

http_interesting_headers = (http_header_content_length | http_header_transfer_encoding | http_header_connection | http_header_upgrade);

http_header_name = (http_token | http_interesting_headers) >enter_header_name %/leave_header_name %leave_header_name;
http_header_separator = (":" (" " | "\t")*);
http_header =  http_header_name http_header_separator <: http_header_value :> http_crlf;

http_request_headers = http_request_line (http_header)* http_crlf %/leave_headers %leave_headers;

main := http_request_headers >message_begin;

body_identity := any+ %/eof_leave_body_identity;
body_identity_eof := any* $in_body_identity_eof %/eof_leave_body_identity_eof;
# body_chunked := ...

dead := any <*enter_dead;

}%%