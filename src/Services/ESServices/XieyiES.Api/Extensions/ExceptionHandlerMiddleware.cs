using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;

namespace XieyiES.Api.Extensions
{
    /// <summary>
    ///     自定义异常中间件
    /// </summary>
    public class ExceptionHandlerMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var result = new ErrorResult
                {
                    ErrorCode = StatusCodes.Status500InternalServerError,
                    ErrorMessage = ex.Message,
                    ErrorStack = ex.StackTrace
                };
                _logger.Error("exceptionHandlerMiddleware catch a error, please fix it");
                context.Response.ContentType = "application/json;charset=utf-8";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
            }
        }
    }

    public class ErrorResult
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorStack { get; set; }
    }
}