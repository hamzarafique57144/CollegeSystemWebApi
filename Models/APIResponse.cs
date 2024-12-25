using System.Net;
using System.Security.Permissions;

namespace CollegeAppWebAPI.Models
{
    public class APIResponse
    {
        public bool Status { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public dynamic Data { get; set; }
        public List<string> Error { get; set; }
    }
}
