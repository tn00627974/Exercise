using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Todo.Dtos;
using Todo.Models;
using Todo.Parameter;

namespace Todo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly TodoContext _todoContext;
        //private readonly IMapper _mapper;

        public TodoController(TodoContext todoContext)
        {
            _todoContext = todoContext;
        }

        // GET: api/<TodoController>
        [HttpGet]
        public IEnumerable<TodoListSelectDto> Get( [FromQuery] TodoSelectParameter TSPvalue)
        {


            var result = _todoContext.TodoList
                .Include(a => a.InsertEmployee)
                .Include(a => a.UpdateEmployee)
                //.Include(a => a.UploadFiles)
                .Select(t => new TodoListSelectDto
                {
                    Enable = t.Enable,
                    InsertEmployeeName = t.InsertEmployee != null ? t.InsertEmployee.Name : null,
                    InsertTime = t.InsertTime,
                    Name = t.Name,
                    Orders = t.Orders,
                    TodoId = t.TodoId,
                    UpdateEmployeeName = t.UpdateEmployee != null ? t.UpdateEmployee.Name : null,
                    UpdateTime = t.UpdateTime
                });

            Console.WriteLine(result.Count());

            if (!string.IsNullOrWhiteSpace(TSPvalue.name))
            {
                result = result.Where(a => a.Name.Contains(TSPvalue.name));
            }

            if (TSPvalue.enable != null)
            {
                result = result.Where(a => a.Enable == TSPvalue.enable);
            }

            if (TSPvalue.InsertTime != null)
            {
                result = result.Where(a => a.InsertTime.Date == TSPvalue.InsertTime);
            }

            if (TSPvalue.minOrder != null && TSPvalue.maxOrder != null)
            {
                result = result
                    .Where(a => a.Orders >= TSPvalue.minOrder && a.Orders <= TSPvalue.maxOrder)
                    .OrderBy(t => t.Orders);
            }
            return result;
        }
    }
}

