using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using XieyiES.Api.Model;
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
        private readonly ILogger _logger;


        public TestController(IESRepository elasticClient, ILogger logger)
        {
            _elasticClient = elasticClient ?? throw new ArgumentNullException(nameof(elasticClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            _logger.Debug($"to insert data : {JsonSerializer.Serialize(userWallet)}");
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

            _logger.Debug($"to insert lot of data : {JsonSerializer.Serialize(userWallets)}");
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
            _logger.Debug("to delete userwallet index");
            await _elasticClient.DeleteIndexAsync<UserWallet>();
            return NoContent();
        }

        /// <summary>
        ///     根据Id删除索引中的文件
        /// </summary>
        /// <param name="id">doc's Id</param>
        /// <param name="index"></param>
        /// <response code="204">Delete Returns NoContent</response>
        /// <response code="400">If the id is null</response> 
        /// <returns></returns>
        [HttpDelete("userwallet/{id}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteIndexAsync(string id, string index)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("id can't be null");
            }
            _logger.Debug($"to delete doc Item [Id:{id}] ");
            await _elasticClient.DeleteEntityByIdAsync<UserWallet>(id, index);
            return NoContent();
        }


        [HttpDelete("userwallet/query")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteByQueryAsync()
        {
            await _elasticClient.DeleteByQuery<UserWallet>(x=>x.UserName == "xieyi");
            return Ok();
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
            _logger.Debug($"want to update userwallet:[{id}] to: {JsonSerializer.Serialize(userWallet)}");
            await _elasticClient.UpdateAsync(id, userWallet);
            return Ok(userWallet);
        }
    }
}