using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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

        // GET: api/todos
        [HttpGet("todos")]
        public ActionResult<TodoListSelectDto> GetAll( [FromQuery] TodoSelectParameter TSPvalue)
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
            var resultCount = result.Count();
            var resultList = result.ToList();
            Console.WriteLine(resultCount);
            Console.WriteLine(resultList.Count());
            return Ok(result);
        }

        // Get : api/todos/id
        [HttpGet("todos/{id}")]
        public ActionResult<TodoListSelectDto> GetById(Guid id)
        {
            // LINQ 關聯JOIN寫法 (用 Include )
            //var result = _todoContext.TodoList
            //    .Include(a => a.InsertEmployee)
            //    .Include(a => a.UpdateEmployee)
            //    //.Include(a => a.UploadFiles)
            //    .Select(t => new TodoListSelectDto
            //    {
            //        Enable = t.Enable,
            //        InsertEmployeeName = t.InsertEmployee != null ? t.InsertEmployee.Name : null,
            //        InsertTime = t.InsertTime,
            //        Name = t.Name,
            //        Orders = t.Orders,
            //        TodoId = t.TodoId,
            //        UpdateEmployeeName = t.UpdateEmployee != null ? t.UpdateEmployee.Name : null,
            //        UpdateTime = t.UpdateTime
            //    }).SingleOrDefault();

            // LinQ 非關聯JOIN寫法
            var result = (from t in _todoContext.TodoList
                          join b in _todoContext.Employee on t.InsertEmployeeId equals b.EmployeeId
                          join c in _todoContext.Employee on t.UpdateEmployeeId equals c.EmployeeId
                          where t.TodoId == id
                          //.Include(a => a.UploadFiles)
                          select new TodoListSelectDto
                          {
                              Enable = t.Enable,
                              InsertEmployeeName = b.Name,
                              InsertTime = t.InsertTime,
                              Name = t.Name,
                              Orders = t.Orders,
                              TodoId = t.TodoId,
                              UpdateEmployeeName = c.Name,
                              UpdateTime = t.UpdateTime
                          }).SingleOrDefault();

            return Ok(result);
        }

        // Post : api/todos
        [HttpPost("todos")]
        public IActionResult CreateTodo ( [FromBody ] CreateDto dto )
        {
            // 查詢 Employee 名字 
            var insertEmployee = _todoContext.Employee.FirstOrDefault(e => e.Name == dto.InsertEmployeeName);
            var updateEmployee = _todoContext.Employee.FirstOrDefault(e => e.Name == dto.UpdateEmployeeName);

            if (insertEmployee == null || updateEmployee == null)
            {
                return BadRequest("InsertEmployee 或 UpdateEmployee 不存在");
            }

            var newTodo = new TodoList
            {
                //TodoId = Guid.NewGuid(),
                Name = dto.Name,
                InsertTime = dto.InsertTime,
                UpdateTime = dto.UpdateTime,
                Enable = dto.Enable,
                Orders = dto.Orders,
                InsertEmployeeId = insertEmployee.EmployeeId,
                UpdateEmployeeId = updateEmployee.EmployeeId
            };
            _todoContext.TodoList.Add(newTodo);
            _todoContext.SaveChanges();
            return Ok();
        }


        [HttpGet("Route/{id}")]
        public dynamic GetFrom(string id)
        {
            return id;
        }

    }
}

