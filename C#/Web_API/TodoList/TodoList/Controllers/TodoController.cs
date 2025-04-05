using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoList.Dtos;
using TodoList.Models;
using TodoList.Parameter;

namespace TodoList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly TodoContext _todoContext;

        public TodoController(TodoContext todoContext)
        {
            _todoContext = todoContext;
        }

        //[HttpGet]
        //public IEnumerable<TodoListSelectDto> Get([FromQuery] TodoSelectParameter TSPvalue )
        //{
        //var result = _todoContext.TodoLists.Select(x => new TodoListSelectDto
        //return;
        //}



        //}
    }
}
