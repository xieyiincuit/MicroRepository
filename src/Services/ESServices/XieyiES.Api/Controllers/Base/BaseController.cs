using Microsoft.AspNetCore.Mvc;

namespace XieyiES.Api.Controllers.Base
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected virtual OkObjectResult Success(string message = "success")
        {
            return Ok(new
            {
                IsSuccess = true
            });
        }

        protected virtual OkObjectResult Success<T>(T data, string message = "success")
        {
            return Ok(new
            {
                IsSuccess = true,
                Message = message,
                Data = data
            });
        }
        protected virtual BadRequestObjectResult FailWithBadRequest(string message = "request fail")
        {
            return BadRequest(new
            {
                IsSuccess = false
            });
        }

        protected virtual BadRequestObjectResult FailWithBadRequest<T>(T data, string message = "request fail")
        {
            return BadRequest(new
            {
                IsSuccess = false,
                Message = message,
                Data = data
            });
        }

        protected virtual NotFoundObjectResult FailWithNotFound(string message = "fail to find")
        {
            return NotFound(new
            {
                IsSuccess = false
            });
        }

        protected virtual NotFoundObjectResult FailWithNotFound<T>(T data, string message = "fail to find")
        {
            return NotFound(new
            {
                IsSuccess = false,
                Message = message,
                Data = data
            });
        }

    }
}
