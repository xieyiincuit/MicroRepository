using System;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Serilog;
using XieyiESLibrary.Interfaces;

namespace XieyiES.Api.Controllers
{
    [ApiController]
    [Route("api/v1/nest")]
    public class RegistryController : ControllerBase
    {
        /// <summary>
        ///     NEST Client
        /// </summary>
        private readonly IElasticClient _elasticClient;

        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public RegistryController(IESClientProvider elasticClient, ILogger logger, IMapper mapper)
        {
            _elasticClient = elasticClient.ElasticClient ?? throw new ArgumentNullException(nameof(elasticClient.ElasticClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }


    }
}