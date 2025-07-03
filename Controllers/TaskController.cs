using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskApi.Data;
using TaskApi.Models;

namespace TaskApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly AppDbContext _context;
        public TaskController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetTasks()
        {
            var tasks = await _context.TaskItems.ToListAsync();
            return tasks.Select(MapToDto).ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItemDto>> GetTask(int id)
        {
            var task = await _context.TaskItems.FindAsync(id);
            if (task == null) return NotFound();
            return MapToDto(task);
        }
        [HttpPost]
        public async Task<ActionResult<TaskItem>> CreateTask(TaskItemDto taskDto)
        {
            var task = new TaskItem
            {
                Title = taskDto.Title,
                Description = taskDto.Description,
                DueDate = taskDto.DueDate,
                IsComplete = taskDto.IsComplete
            };

            _context.TaskItems.Add(task);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, TaskItemDto taskDto)
        {
            var task = await _context.TaskItems.FindAsync(id);
            if (task == null) return NotFound();

            // Only update modifiable fields
            task.Title = taskDto.Title;
            task.Description = taskDto.Description;
            task.DueDate = taskDto.DueDate;
            task.IsComplete = taskDto.IsComplete;

            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.TaskItems.FindAsync(id);
            if (task == null) return NotFound();
            _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        private static TaskItemDto MapToDto(TaskItem task)
        {
            return new TaskItemDto
            {
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                IsComplete = task.IsComplete
            };
        }

        private static TaskItem MapToEntity(TaskItemDto dto)
        {
            return new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                IsComplete = dto.IsComplete
            };
        }

    }
}
