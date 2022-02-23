namespace Netdrop.Interfaces.Requests
{
    public class ListDirRequest : BaseFtpRequest
    {
        public bool New { get; set; }
        public string Connection { get; set; }
    }
}
