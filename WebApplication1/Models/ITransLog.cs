namespace WebApplication1.Models
{
    public class ITransLog
    {
        public string traceId { get; set; }
        public string rtnCode { get; set; }
        public string msg { get; set; }
        public Info info { get; set; }
    }
}
