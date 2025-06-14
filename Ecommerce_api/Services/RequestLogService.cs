using Ecommerce_api.Data;
using Ecommerce_api.Models;

namespace Ecommerce.Services
{
    public class RequestLogService
    {
        Ecommerce_apiDBContext _context;
        public RequestLogService(Ecommerce_apiDBContext context)
        {
            _context = context;
        }

        public async Task LogSuceededRequest(string message, int StatusCode)
        {
            var succesfulRequest = new RequestLog
            {
                RequestDescription = message,
                RequestType = RequestType.Succeeded,
                ResponseCode = StatusCode,
                TimeStamp = DateTime.Now
            };

            _context.Add(succesfulRequest);
            await _context.SaveChangesAsync();
        }

        public async Task LogFailedRequest(string message, int StatusCode)
        {
            var succesfulRequest = new RequestLog
            {
                RequestDescription = message,
                RequestType = RequestType.Failed,
                ResponseCode = StatusCode,
                TimeStamp = DateTime.Now
            };

            _context.Add(succesfulRequest);
            await _context.SaveChangesAsync();
        }
    }
}
