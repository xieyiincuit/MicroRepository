using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using XieyiES.Api.Interfaces;
using XieyiES.Api.Models;

namespace XieyiES.Api.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class StudentController : ControllerBase
    {
        private readonly ILogger<StudentController> _logger;
        private readonly IESProvider _esServer;

        public StudentController(ILogger<StudentController> logger, IESProvider esServer)
        {
            _logger = logger;
            _esServer = esServer;
        }

        [HttpGet("students")]
        [ProducesResponseType(typeof(List<Student>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetStudents()
        {
            var response = await _esServer.ElasticClient.SearchAsync<Student>(
                s => s.Index("Student"));
            if (response.Documents is null)
            {
                return NotFound();
            }

            return Ok(response.Documents);
        }

        [HttpPost("student")]
        [ProducesResponseType(typeof(Student), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetStudent([FromBody] Student student)
        {
            if (student is null)
            {
                return BadRequest("Student entity can't be null");
            }

            await _esServer.ElasticClient.IndexDocumentAsync(student);

            return Ok(student);
        }

    }
}
