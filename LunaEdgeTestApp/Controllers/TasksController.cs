using LunaEdgeTestApp.Dtos;
using LunaEdgeTestApp.Enums;
using LunaEdgeTestApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.IO;

namespace LunaEdgeTestApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITasksService _taskService;

        public TasksController(ITasksService taskService)
        {
            _taskService = taskService;
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateTask(TaskDto taskDto)
        {
            var userId = GetUserIdFromJwt();

            try
            {
                _taskService.CreateTask(taskDto, userId);
            }
            catch (Exception ex) 
            { 
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<Models.Task>> GetAllTasks(
            [FromQuery] Enums.TaskStatus? status,
            [FromQuery] DateTime? dueDate,
            [FromQuery] TaskPriority? priority,
            [FromQuery] string? sortBy,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var userId = GetUserIdFromJwt();

            var result = _taskService.GetAllTasks(userId, status, dueDate, priority, sortBy, page, pageSize);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("{id}")]
        public ActionResult<Models.Task> GetTaskById(Guid id)
        {
            var userId = GetUserIdFromJwt();
            Models.Task result;

            try
            {
                result = _taskService.GetTaskById(id, userId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPut("{id}")]
        public IActionResult UpdateTask(TaskDto taskDto, Guid id) 
        {
            var userId = GetUserIdFromJwt();

            try
            {
                _taskService.UpdateTask(taskDto, userId, id);
            }
            catch (Exception ex) 
            { 
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult DeleteTask(Guid id)
        {
            var userId = GetUserIdFromJwt();

            try
            {
                _taskService.DeleteTask(id, userId);
            }
            catch (Exception ex) 
            { 
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        private Guid GetUserIdFromJwt()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var decryptedToken = handler.ReadToken(token);

            var jwtSecurityToken = decryptedToken as JwtSecurityToken;
            return Guid.Parse(jwtSecurityToken!.Claims.First(c => c.Type == "UserId").Value);
        }
    }
}
