using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HelloWorld.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelloWorldController : ControllerBase
    {
        // GET: api/<HelloWorldController>
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()  // 強類型回傳（讓 Swagger / OpenAPI 更好解析）
        {
            return new string[] { "HelloWorld1", "HelloWorld2", "HelloWorld3" };
            //return BadRequest("錯誤發生了");
        }

        // GET api/<HelloWorldController>/5
        [HttpGet("IEGet")]
        public IEnumerable<string> IEGet()
        {
            return new string[] { "HelloWorld1", "HelloWorld2", "HelloWorld3" };
        }

        // GET api/<HelloWorldController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            string result = "HelloWorld1";
            if (id == 2)
            {
                result = "HelloWorld2";
            }
            else if (id == 3)
            {
                result = "HelloWorld3";
            }
            else if (id == 4)
            {
                result = "HelloWorld4";
            }
            return result;
        }

        // POST api/<HelloWorldController>
        [HttpPost]
        public string Post([FromBody] string value)
        {
            return $"Put + ID {value}";
        }

        // PUT api/<HelloWorldController>/5
        [HttpPut("{id}")]
        public string Put(int id, [FromBody] string value)
        {
            return "Put【" + value + "】ID：" + id;
        }

        // DELETE api/<HelloWorldController>/5
        [HttpDelete("{id}")]
        public string Delete(int id)
        {
            return "Delete ID：" + id;
        }



    }
}
