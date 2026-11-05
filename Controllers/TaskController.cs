using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using trpo8.Services;
using Task = trpo8.Models.Task;

namespace trpo8.Controllers
{
    [Route("api/v1/tasks")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly TaskService _taskService;
        
        public TaskController()
        {
            _taskService = new TaskService();
        }
        
        [HttpGet]
        [Authorize(Roles = "User, Admin")]
        public IActionResult GetTasks()
        {
            return Ok (_taskService.GetAll().Where(p => p.OwnerLogin == User.Identity?.Name));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "User, Admin")]
        public IActionResult GetTask([FromRoute] int id)
        {
            if (_taskService.GetAll().Any(p => p.Id == id))
            {
                if (User.IsInRole("Admin"))
                    return Ok(_taskService.GetAll().Where(p => p.Id == id));
                
                if (_taskService.GetAll().Any(p => p.OwnerLogin == User.Identity?.Name && p.Id == id))
                    return Ok(_taskService.GetAll().Where(p => p.OwnerLogin == User.Identity?.Name  && p.Id == id));
                
                return Forbid();
            }
            return NotFound();
        }

        public record TaskAddDto(string title, string description);
        [HttpPost]
        [Authorize(Roles = "User, Admin")]
        public IActionResult CreateTask(TaskAddDto taskDto)
        {
            if (String.IsNullOrWhiteSpace(taskDto.title) || String.IsNullOrWhiteSpace(taskDto.description))
                return Conflict();
            var task = new Task
            {
                Title = taskDto.title,
                Description = taskDto.description,
                OwnerLogin = User.Identity?.Name,
                IsCompleted = false,
            };

            _taskService.Add(task);
            return Created($"/api/v1/tasks/{task.Id}", task);
        }

        public record TaskUpdateDto(string title, string description, bool isCompleted);
        [HttpPut("{id}")]
        [Authorize(Roles = "User, Admin")]
        public IActionResult PutTask([FromRoute] int id, TaskUpdateDto taskDto)
        {
            if (String.IsNullOrWhiteSpace(taskDto.title) || String.IsNullOrWhiteSpace(taskDto.description))
                return Conflict();
            if (_taskService.GetAll().Any(p => p.Id == id))
            {
                if (User.IsInRole("Admin") || _taskService.GetAll().Any(p => p.OwnerLogin == User.Identity?.Name && p.Id == id))
                {
                    var task = _taskService.GetAll().FirstOrDefault(p => p.Id == id);
                    task.Title = taskDto.title;
                    task.Description = taskDto.description;
                    task.IsCompleted = taskDto.isCompleted;
                    _taskService.Commit();
                    return Ok(task);
                }
                return Forbid();
            }
            return NotFound();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "User, Admin")]
        public IActionResult DeleteTask([FromRoute] int id)
        {
            if (_taskService.GetAll().Any(p => p.Id == id))
            {
                if (User.IsInRole("Admin") ||
                    _taskService.GetAll().Any(p => p.OwnerLogin == User.Identity?.Name && p.Id == id))
                {
                    var task =  _taskService.GetAll().FirstOrDefault(p => p.Id == id);
                    _taskService.Delete(task);
                    _taskService.Commit();
                    return NoContent();
                }
                return Forbid();
            }
            return NotFound();
        }
        
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll()
        {
            return Ok(_taskService.GetAll());
        }
    }
}