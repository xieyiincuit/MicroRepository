using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using XieyiES.Api.Model;
using XieyiES.Api.Model.DtoModel;
using XieyiESLibrary.Entity;
using XieyiESLibrary.Provider;


namespace XieyiES.Api.Controllers
{
    /// <summary>
    ///     测试控制器
    /// </summary>
    [ApiController]
    [Route("api/v1/test")]
    public class UserController : ControllerBase
    {
        private readonly IESRepository _elasticClient;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;


        public UserController(IESRepository elasticClient, ILogger logger, IMapper mapper)
        {
            _elasticClient = elasticClient ?? throw new ArgumentNullException(nameof(elasticClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        ///     新增一条数据
        /// </summary>
        /// <param name="addDto"></param>
        /// <returns></returns>
        /// <response code="200">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>    
        [HttpPost("user")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<IActionResult> InsertUserAsync([FromBody] UserUpdateOrAddDto addDto)
        {
            if (addDto == null)
            {
                return BadRequest("user can't be null");
            }
            var entity = _mapper.Map<User>(addDto);

            _logger.Debug($"to insert data : {JsonSerializer.Serialize(entity)}");
            await _elasticClient.InsertAsync(entity);
            return Ok(entity);
        }

        /// <summary>
        ///     批量新增多条数据
        /// </summary>
        /// <param name="addDtos"></param>
        /// <returns></returns>
        /// <response code="200">Returns the newly created itemList</response>
        /// <response code="400">If the item is null</response>
        [HttpPost("users")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<User>), StatusCodes.Status200OK)]
        public async Task<IActionResult> InsertUsersAsync([FromBody] List<UserUpdateOrAddDto> addDtos)
        {
            if (addDtos == null)
            {
                return BadRequest("users can't be null");
            }

            var entities = _mapper.Map<List<User>>(addDtos);

            _logger.Debug($"to insert lot of data : {JsonSerializer.Serialize(entities)}");
            await _elasticClient.InsertRangeAsync(entities);
            return Ok(entities);
        }

        /// <summary>
        ///     删除user索引
        /// </summary>
        /// <returns></returns>
        /// <response code="204">delete return NoContent</response>
        [HttpDelete("user")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteIndexAsync()
        {
            _logger.Debug("to delete userwallet index");
            await _elasticClient.DeleteIndexAsync<User>();
            return NoContent();
        }

        /// <summary>
        ///     根据Id删除索引中的文件
        /// </summary>
        /// <param name="id"></param>
        /// <param name="index"></param>
        /// <response code="204">Delete Returns NoContent</response>
        /// <response code="400">If the id is null</response> 
        /// <returns></returns>
        [HttpDelete("user/{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteUserAsync([FromRoute] string id, [FromQuery]string index)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("id can't be null");
            }
            _logger.Debug($"to delete doc Item [Id:{id}] ");
            await _elasticClient.DeleteEntityByIdAsync<User>(id, index);
            return NoContent();
        }


        /// <summary>
        ///     通过数据唯一标识批量删除数据
        /// </summary>
        /// <param name="ids"></param>
        /// <response code="204">Delete Returns NoContent</response>
        /// <response code="400">If the ids is null</response> 
        /// <returns></returns>
        [HttpDelete("users")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteUsersAsync([FromBody] IReadOnlyList<string> ids)
        {
            if (ids == null)
            {
                return BadRequest("ids is null");
            }

            var deleteUsers = ids.Select(id => new User() { Id = id }).ToList();
            _logger.Debug($"want to delete users -> userIds:{JsonSerializer.Serialize(ids)}");
            await _elasticClient.DeleteManyAsync<User>(deleteUsers);
            return NoContent();
        }

        /// <summary>
        ///     根据检索条件删除
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userId"></param>
        /// <param name="queryType">0-And 1-Or</param>
        /// <param name="constraintType">0-Loose 1-Tight</param>
        /// <response code="400">if query item is null</response>
        /// <response code="204">Delete Return NoContent</response>
        /// <returns></returns>
        [HttpDelete("user/query")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteUserByQueryAsync(
            [FromQuery] string userName, [FromQuery] string userId, 
            [FromQuery] QueryType queryType = QueryType.And, 
            [FromQuery] ConstraintType constraintType = ConstraintType.Tight)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("search item can't be null");
            }

            Expression<Func<User, bool>> expression = null;
            if (queryType == QueryType.And && constraintType == ConstraintType.Loose)
                expression = x => x.UserName.Contains(userName) && x.UserId.Contains(userId);
            if (queryType == QueryType.And && constraintType == ConstraintType.Tight)
                expression = x => x.UserName == userName && x.UserId == userId;
            if (queryType == QueryType.Or && constraintType == ConstraintType.Loose)
                expression = x => x.UserName.Contains(userName) || x.UserId.Contains(userId);
            if (queryType == QueryType.Or && constraintType == ConstraintType.Tight)
                expression = x => x.UserName == userName || x.UserId == userId;
            _logger.Debug(
                $"use query to delete -> query:[userName:{userName},userId:{userId}] queryType:{queryType} constraintType:{constraintType}");
            await _elasticClient.DeleteByQuery<User>(expression);
            return NoContent();
        }

        /// <summary>
        ///     通过id更新实体信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateDto"></param>
        /// <returns></returns>
        /// <response code="200">Return newly Update Entity</response>
        /// <response code="400">If the id is null</response> 
        [HttpPut("user/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(int), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateUserAsync([FromRoute]string id, [FromBody] UserUpdateOrAddDto updateDto)
        {
            if (id == null)
            {
                return BadRequest("can't find this id");
            }

            var entity = _mapper.Map<User>(updateDto);
            _logger.Debug($"want to update user:[id:{id}] to: {JsonSerializer.Serialize(updateDto)}");
            await _elasticClient.UpdateAsync(id, entity);
            return Ok(entity);
        }
    }
}