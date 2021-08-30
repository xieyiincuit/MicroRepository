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
        /// <param name="userWallet"></param>
        /// <returns></returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>    
        [HttpPost("userwallet")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(UserWallet), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> InsertAsync([FromBody] UserWallet userWallet)
        {
            if (userWallet == null)
            {
                return BadRequest("user can't be null");
            }
            _logger.LogDebug(MyLogEvents.TestItem, $"to insert data : {JsonSerializer.Serialize(userWallet)}");
            await _elasticClient.InsertAsync(userWallet);

            return Ok(userWallet);
        }

        /// <summary>
        ///     批量新增多条数据
        /// </summary>
        /// <param name="userWallets"></param>
        /// <returns></returns>
        /// <response code="201">Returns the newly created itemList</response>
        /// <response code="400">If the item is null</response>
        [HttpPost("userwallets")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<UserWallet>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> InsertRangeAsync([FromBody] List<UserWallet> userWallets)
        {
            if (userWallets == null)
            {
                return BadRequest("users can't be null");
            }

            _logger.LogDebug(MyLogEvents.TestItem, $"to insert lot of data : {JsonSerializer.Serialize(userWallets)}");
            await _elasticClient.InsertRangeAsync(userWallets);
            return Ok(userWallets);
        }

        /// <summary>
        ///     删除userwallet索引
        /// </summary>
        /// <returns></returns>
        /// <response code="204">delete return NoContent</response>
        [HttpDelete("userwallet")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteIndexAsync()
        {
            _logger.LogDebug(MyLogEvents.DeleteItem, "to delete userwallet index");
            await _elasticClient.DeleteIndexAsync<UserWallet>();
            return NoContent();
        }

        /// <summary>
        ///     根据Id删除索引中的文件
        /// </summary>
        /// <param name="id">doc's Id</param>
        /// <param name="indexName"></param>
        /// <response code="204">Returns NoContent</response>
        /// <response code="400">If the id is null</response> 
        /// <returns></returns>
        [HttpDelete("userwallet/{id}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteIndexAsync(string id, [FromQuery] string indexName = "")
        {
            if (id == null)
            {
                return BadRequest("can't find this id");
            }
            _logger.LogDebug(MyLogEvents.DeleteItem, $"to delete doc Item [Id:{id}] in [{indexName}]");
            await _elasticClient.DeleteEntityByIdAsync<UserWallet>(id);
            return NoContent();
        }

        /// <summary>
        ///     通过id更新实体信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userWallet"></param>
        /// <returns></returns>
        [HttpPut("userwallet/{id}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UserWallet), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateEntityAsync(string id, [FromBody] UserWallet userWallet)
        {
            if (id == null)
            {
                return BadRequest("can't find this id");
            }

            _logger.LogDebug(MyLogEvents.UpdateItem, $"want to update userwallet:[{id}] to: {JsonSerializer.Serialize(User)}");
            await _elasticClient.UpdateAsync(id, userWallet);
            return Ok(userWallet);
        }
    }
}