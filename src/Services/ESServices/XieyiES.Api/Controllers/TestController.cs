using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using XieyiES.Api.Domain;
using XieyiES.Api.Extensions;
using XieyiESLibrary.Provider;

namespace XieyiES.Api.Controllers
{
    /// <summary>
    ///     测试控制器
    /// </summary>
    [ApiController]
    [Route("api/v1/test")]
    public class TestController : ControllerBase
    {
        private readonly IESRepository _elasticClient;
        private readonly ILogger<TestController> _logger;

        public TestController(IESRepository elasticClient, ILogger<TestController> logger)
        {
            _elasticClient = elasticClient;
            _logger = logger;
        }

        /// <summary>
        ///     新增一条数据
        /// </summary>
        /// <returns></returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>    
        [HttpPost("userwallet")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(UserWallet), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> InsertAsync([FromBody] UserWallet user)
        {
            if (user == null)
            {
                return BadRequest("user can't be null");
            }
            _logger.LogDebug(MyLogEvents.TestItem, $"want to insert data : {JsonSerializer.Serialize(user)}");
            await _elasticClient.InsertAsync(user);

            return Ok(user);
        }

        /// <summary>
        ///     批量新增多条数据
        /// </summary>
        /// <returns></returns>
        /// <response code="201">Returns the newly created itemList</response>
        /// <response code="400">If the item is null</response>    
        [HttpPost("userwallets")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<UserWallet>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> InsertRangeAsync(List<UserWallet> users)
        {
            if (users == null)
            {
                return BadRequest("users can't be null");
            }

            _logger.LogDebug(MyLogEvents.TestItem, $"want to insert lot of data : {JsonSerializer.Serialize(users)}");
            await _elasticClient.InsertRangeAsync(users);
            return Ok(users);
        }
    }
}