using Microsoft.AspNetCore.Builder;

namespace XieyiES.Api.Extensions
{
    /// <summary>
    ///     异常处理中间件
    /// </summary>
    public static class ExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}