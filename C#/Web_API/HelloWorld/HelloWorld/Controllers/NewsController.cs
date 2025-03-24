using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HelloWorld.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private static readonly List<News> NewsList = new List<News>
        {
            new News{id=1,title="凱哥寫程式",content="帶你輕輕鬆鬆學習程式"},
            new News{id=2,title="ASP.NET Core Web API課程",content="帶你輕輕鬆鬆學習ASP.NET Core Web API"},
            new News{id=3,title="ASP.NET Core MVC課程",content="帶你輕輕鬆鬆學習程式ASP.NET Core MVC"},
            new News{id=4,title="Angular課程",content="帶你輕輕鬆鬆學習程式Angular"}
        };

        // GET api/<NewsController> : 查詢全部新聞
        [HttpGet]
        public IEnumerable<News> Get()
        {
            return NewsList;
        }

        // GET api/<NewsController>/5 : 查詢特定新聞
        [HttpGet("{id}")]
        public ActionResult<News> Get(int id)
        {
            var result = NewsList.Where(n=>n.id==id);

            // id 不在result
            //if (result == null)
            if (!result.Any())
            {
                return NotFound();
            }

            return Ok(result);
        }

        // POST api/<NewsController>/5
        [HttpPost("{id}")]
        public ActionResult<IEnumerable<News>> Post(News value)
        {
            NewsList.Add(value);

            return Ok(NewsList);
        }

        // PUT api/<NewsController>/5 : 修改一筆新聞
        [HttpPut("{id}")]
        public ActionResult<IEnumerable<News>> Put(int id, News value)
        {
            var update = (from a in NewsList
                          where a.id == id
                          select a).SingleOrDefault();

            update = NewsList.SingleOrDefault(a => a.id == id);

            if (update != null)
            {
                update.title = value.title;
                update.content = value.content;
            }

            return NewsList;
        }


        // DELETE api/<NewsController>/5 : 刪除一筆新聞
        [HttpDelete("{id}")]
        public ActionResult<IEnumerable<News>> Delete (int id)
        {

            var delete = NewsList.SingleOrDefault(a => a.id == id);

            if (delete != null)
            {
                NewsList.Remove(delete);
            }

            return NewsList;
        }





    }
}
