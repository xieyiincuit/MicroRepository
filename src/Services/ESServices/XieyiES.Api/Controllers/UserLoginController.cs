using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Serilog;
using XieyiES.Api.Model;
using XieyiESLibrary.Interfaces;

namespace XieyiES.Api.Controllers
{
    [ApiController]
    [Route("api/v1/student")]
    public class UserLoginController : ControllerBase
    {
        /// <summary>
        ///     NEST Client
        /// </summary>
        private readonly IElasticClient _elasticClient;

        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public UserLoginController(IESClientProvider elasticClient, ILogger logger, IMapper mapper)
        {
            _elasticClient = elasticClient.ElasticClient ?? throw new ArgumentNullException(nameof(elasticClient.ElasticClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost("record")]
        public async Task<IActionResult> InsertUserLoginInfo([FromBody] UserLogin userLogin)
        {
            if (userLogin == null)
            {
                return BadRequest();
            }
            await _elasticClient.IndexAsync(userLogin, x => x.Index<UserLogin>());
            return Ok(userLogin);
        }

    }
}