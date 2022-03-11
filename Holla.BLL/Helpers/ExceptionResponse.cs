using System.Collections.Generic;

namespace Holla.BLL.Helpers
{
    public class ExceptionResponse
    {
        public ExceptionResponse()
        {
        }
        public ExceptionResponse(string message)
        {
            Message = message;
        }
        public string Message { get; set; }
        public string Details { get; set; }
        public List<string> Errors { get; set; }
    }
}
