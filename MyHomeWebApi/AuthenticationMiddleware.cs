using Microsoft.Extensions.Primitives;
using MyHomeLib;
using System.Net;
using System.Linq;

namespace MyHomeWebApi
{
    public class AuthenticationMiddleware
    {
        private RequestDelegate next;
        private IClientFilter clientFilter;

        public AuthenticationMiddleware(RequestDelegate next, IMyHomeApiService myHomeApiService)
        {
            clientFilter = myHomeApiService.GetClientFilter();
            this.next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var ip = GetRemoteIp(context);
            if (ip == null || !clientFilter.IsClientAllowed(ip.ToString()))
            {
                context.Response.ContentType = "text/plain";
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("This IP is not allowed");
                return;
            }
            await next(context);
        }

        private IPAddress? GetRemoteIp(HttpContext context)
        {
            string header = GetHeaderValue("X-Forwarded-For", context);
            if (header == null)
                return null;

            string ip = header
                .TrimEnd(',')
                .Split(',')
                ?.Select(s => s.Trim())
                ?.ToList()?.FirstOrDefault() ?? "";

            if (string.IsNullOrWhiteSpace(ip) && context.Connection.RemoteIpAddress != null)
                ip = context.Connection.RemoteIpAddress.ToString();

            if (string.IsNullOrWhiteSpace(ip))
                ip = GetHeaderValue("REMOTE_ADDR", context);

            return IPAddress.Parse(ip).MapToIPv4();
        }

        private string GetHeaderValue(string headerName, HttpContext context)
        {
            StringValues values;
            if (context.Request.Headers.TryGetValue(headerName, out values))
            {
                return values.ToString();
            }
            return "";
        }
    }
}
