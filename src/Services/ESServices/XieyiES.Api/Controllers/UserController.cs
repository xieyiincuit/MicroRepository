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
using XieyiESLibrary.Interfaces;


namespace XieyiES.Api.Controllers
{
    /// <summary>
    ///     测试控制器
    /// </summary>
    [ApiController]
    [Route("api/v1/xieyi")]
    public class UserController : ControllerBase
    {
        /// <summary>
        ///     自定义仓储
        /// </summary>
        private readonly IESRepository _elasticRepository;

        /// <summary>
        ///     自定义搜索
        /// </summary>
        private readonly IESSearch _elasticSearch;

        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        public UserController(IESRepository elasticRepository, IESSearch elasticSearch, ILogger logger, IMapper mapper)
        {
            _elasticRepository = elasticRepository ?? throw new ArgumentNullException(nameof(elasticRepository));
            _elasticSearch = elasticSearch ?? throw new ArgumentNullException(nameof(elasticSearch));
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

            _logger.Information($"to insert data : {JsonSerializer.Serialize(entity)}");
            await _elasticRepository.InsertAsync(entity);
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

            _logger.Information($"to insert lot of data : {JsonSerializer.Serialize(entities)}");
            await _elasticRepository.InsertRangeAsync(entities);
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
            _logger.Warning("to delete user index, all userInfo will be delete ");
            await _elasticRepository.DeleteIndexAsync<User>();
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteUserAsync([FromRoute] string id, [FromQuery] string index)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("id can't be null");
            }
            _logger.Information($"to delete doc Item [Id:{id}] ");
            await _elasticRepository.DeleteEntityByIdAsync<User>(id, index);
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteUsersAsync([FromBody] IReadOnlyList<string> ids)
        {
            if (ids == null)
            {
                return BadRequest("ids is null");
            }

            var deleteUsers = ids.Select(id => new User() { Id = id }).ToList();
            _logger.Information($"want to delete users -> userIds:{JsonSerializer.Serialize(ids)}");
            await _elasticRepository.DeleteManyAsync<User>(deleteUsers);
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
            _logger.Information(
                $"use query to delete -> query:[userName:{userName},userId:{userId}] queryType:{queryType} constraintType:{constraintType}");
            await _elasticRepository.DeleteByQuery<User>(expression);
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
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateUserAsync([FromRoute] string id, [FromBody] UserUpdateOrAddDto updateDto)
        {
            if (id == null)
            {
                return BadRequest("id can't be null");
            }

            var entity = _mapper.Map<User>(updateDto);
            _logger.Debug($"want to update user:[id:{id}] to: {JsonSerializer.Serialize(updateDto)}");
            await _elasticRepository.UpdateAsync(id, entity);
            return Ok(entity);
        }

        /// <summary>
        ///     查询用户信息
        /// </summary>
        /// <returns></returns>
        /// <response code="200">return users info</response>
        /// <response code="404">users info is empty</response>
        [HttpGet("users")]
        [ProducesResponseType(typeof(List<User>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsersAsync()
        {
            var users = await _elasticSearch.Queryable<User>().OrderBy(x => x.Money).ToListAsync();
            if (users == null || users.Count == 0)
            {
                return NotFound();
            }
            return Ok(users);
        }

        /// <summary>
        ///     分页查询用户信息
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// <response code="200">return users pageInfo</response>
        /// <response code="404">users info is empty</response>
        [HttpGet("users/page")]
        [ProducesResponseType(typeof(List<User>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsersPageAsync([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 5)
        {
            var users = await _elasticSearch.Queryable<User>().OrderBy(x => x.Money).ToPageListAsync(pageIndex, pageSize);
            if (users == null || users.Count == 0)
            {
                return NotFound();
            }
            return Ok(users);
        }

        /// <summary>
        ///     分页查询用户信息 返回信息总数
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// <response code="200">return users pageInfo</response>
        /// <response code="404">users info is empty</response>
        [HttpGet("users/pageTotal")]
        [ProducesResponseType(typeof(List<User>), StatusCodes.Status200OK)]
        public IActionResult GetUsersPageWithTotalNumber([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 5)
        {
            var data = _elasticSearch.Queryable<User>().ToPageList(pageIndex, pageSize, out var totalNumber);
            if (data == null | data.Count == 0)
            {
                return NotFound();
            }
            return Ok(new
            {
                data,
                totalNumber
            });
        }

        /// <summary>
        ///     通过id查询用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">return users contain this name</response>
        /// <response code="404">users info is empty</response>
        [HttpGet("user/{id}")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchUserAsync([FromRoute] string id)
        {
            var data = await _elasticSearch.Queryable<User>().Where(x => x.Id == id).FirstAsync();
            if (data == null )
            {
                return NotFound();
            }
            return Ok(data);
        }

        /// <summary>
        ///     通过姓名模糊查询用户
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        /// <response code="200">return users contain this name</response>
        /// <response code="404">users info is empty</response>
        [HttpGet("user/by")]
        [ProducesResponseType(typeof(List<User>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchByQueryAsync([FromQuery] string userName)
        {
            var data = await _elasticSearch.Queryable<User>().Where(x=>x.UserName.Contains(userName)).ToListAsync();
            if (data == null || data.Count == 0)
            {
                return NotFound();
            }
            return Ok(data);
        }

        /// <summary>
        ///     通过时间分组
        /// </summary>
        /// <returns></returns>
        /// <response code="200">return users group</response>
        /// <response code="404">users info is empty</response>
        [HttpGet("user/group")]
        [ProducesResponseType(typeof(List<User>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GroupAsync()
        {
            var data = await _elasticSearch.Queryable<User>().GroupBy(x => x.CreateTime).ToListAsync();
            if (data == null || data.Count == 0)
            {
                return NotFound();
            }
            return Ok(data);
        }
    }
}