using Microsoft.AspNetCore.Mvc;

namespace XieyiES.Api.Controllers.Base
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected virtual OkObjectResult Success()
        {
            return Ok(new
            {
                IsSuccess = true
            });
        }

        protected virtual OkObjectResult Success<T>(T data, string message = "")
        {
            return Ok(new
            {
                IsSuccess = true,
                Message = message,
                Data = data
            });
        }
    }
}
