namespace WebApplication1.Models
{
    public class ITransLog
    {
        public int traceId { get; set; }
        public int rtnCode { get; set; }
        public string msg { get; set; }
        public Info info { get; set; }
    }
}
