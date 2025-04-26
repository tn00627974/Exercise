using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Todo.Dtos;
using Todo.Models;
using Todo.Parameter;
using Todo.Profiles;
using AutoMapper;

namespace Todo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly TodoContext _todoContext;
        private readonly IMapper _mapper;

        public TodoController(TodoContext todoContext , IMapper mapper)
        {
            _todoContext = todoContext;
            _mapper = mapper;
        }

        // GET: api/todos
        [HttpGet("todos")]
        public ActionResult<TodoListSelectDto> GetAll( [FromQuery] TodoSelectParameter TSPvalue)
        {
            var result = _todoContext.TodoList
                .Include(a => a.InsertEmployee)
                .Include(a => a.UpdateEmployee)
                //.Include(a => a.UploadFiles)
                .Select(a => new TodoListSelectDto
                {
                    Enable = a.Enable,
                    InsertEmployeeName = a.InsertEmployee.Name,
                    InsertTime = a.InsertTime,
                    Name = a.Name,
                    Orders = a.Orders,
                    TodoId = a.TodoId,
                    UpdateEmployeeName = a.UpdateEmployee.Name,
                    UpdateTime = a.UpdateTime
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
            // LINQ 關聯JOIN寫法 (用 Include + aDto )
            var result = _todoContext.TodoList
                .Include(a => a.InsertEmployee)
                .Include(a => a.UpdateEmployee)
                //.Include(a => a.UploadFiles)
                .Where(a => a.TodoId == id)
                .Select(a =>itemDto(a)).SingleOrDefault();

            return Ok(result);

            // LinQ 非關聯JOIN寫法 + itemDto 
            //var result = (from t in _todoContext.TodoList
            //              join b in _todoContext.Employee on t.InsertEmployeeId equals b.EmployeeId
            //              join c in _todoContext.Employee on t.UpdateEmployeeId equals c.EmployeeId
            //              where t.TodoId == id
            //              //.Include(a => a.UploadFiles)
            //              select new TodoList
            //              {
            //                  Enable = t.Enable,
            //                  InsertEmployee = b,
            //                  InsertTime = t.InsertTime,
            //                  Name = t.Name,
            //                  Orders = t.Orders,
            //                  TodoId = t.TodoId,
            //                  UpdateEmployee = c,
            //                  UpdateTime = t.UpdateTime
            //              }).SingleOrDefault();

            //return Ok(itemDto(result));
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


        [HttpGet("AutoMapper")]
        public ActionResult<TodoListSelectDto> GetAutoMapper()
        {
            // AutoMapper 寫法
            var result = _todoContext.TodoList
                .Include(a => a.InsertEmployee)
                .Include(a => a.UpdateEmployee)
                .ToList();
            var map = _mapper.Map<List<TodoListSelectDto>>(result);
            return Ok(result);

            // 原始寫法 
            //var result = _todoContext.TodoList
            //.Include(a => a.InsertEmployee)
            //.Include(a => a.UpdateEmployee)
            ////.Include(a => a.UploadFiles)
            //    .Select(a => new TodoListSelectDto
            //    {
            //        Enable = a.Enable,
            //        InsertEmployeeName = a.InsertEmployee.Name,
            //        InsertTime = a.InsertTime,
            //        Name = a.Name,
            //        Orders = a.Orders,
            //        TodoId = a.TodoId,
            //        UpdateEmployeeName = a.UpdateEmployee.Name,
            //        UpdateTime = a.UpdateTime
            //    });

            //return Ok(map);
        }


        // 將DTO轉換部分函式化
        public static TodoListSelectDto itemDto(TodoList item)
        {
            return new TodoListSelectDto
            {
                Enable = item.Enable,
                InsertEmployeeName = item.InsertEmployee.Name,
                InsertTime = item.InsertTime,
                Name = item.Name,
                Orders = item.Orders,
                TodoId = item.TodoId,
                UpdateEmployeeName = item.UpdateEmployee.Name,
                UpdateTime = item.UpdateTime
            };
        }

    }
}

