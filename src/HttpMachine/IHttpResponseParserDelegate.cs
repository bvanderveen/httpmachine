namespace HttpMachine
{
    public interface IHttpResponseParserDelegate : IHttpParserDelegate
    {
        void OnResponseCode(HttpParser parser, int statusCode, string statusReason); 
    }
}