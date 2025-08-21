//using HelloWorld.Models;
using HelloWorld.MysqlModels; // 引用 MySQL Models
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HelloWorld.Dtos;
using Microsoft.EntityFrameworkCore;

namespace HelloWorld.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly WebContext _webContext;

        public NewsController(WebContext webContext)
        {
            _webContext = webContext;
        }

        // GET api/<NewsController> : 查詢全部新聞
        [HttpGet] 
        public IEnumerable<NewsDto> Get()
        {
            // LINQ 查詢語法
            var newsResult = from n in _webContext.News
                             select new NewsDto
                             {
                                 NewsId = n.NewsId,
                                 Title = n.Title,
                                 Content = n.Content,
                                 StartDateTime = n.StartDateTime,
                                 EndDateTime = n.EndDateTime,
                                 Click = n.Click
                             };

            return newsResult;

            //// LINQ 方法語法
            //var newsResult = _webContext.News.Select
            //    (n => new NewsDto 
            //    { NewsId = n.NewsId, 
            //      Title = n.Title, 
            //      Content = n.Content,
            //      StartDateTime = n.StartDateTime ,
            //      EndDateTime = n.EndDateTime,
            //      Click = n.Click
            //    }).ToList();

            //return newsResult;
        }

        [HttpGet("{id}")]
        public News Get(Guid id)
        {
            var newsResult = _webContext.News.Find(id);

            return newsResult;
        }

        [HttpGet("search")]
        public IEnumerable<NewsDto> Get(string title, string content, DateTime? startDateTime)
        {
            var newsResult = from n in _webContext.News
                             select new NewsDto
                             {
                                 NewsId = n.NewsId,
                                 Title = n.Title,
                                 Content = n.Content,
                                 StartDateTime = n.StartDateTime,
                                 EndDateTime = n.EndDateTime,
                                 Click = n.Click
                             };

            if (!string.IsNullOrEmpty(title))
            {
                newsResult = newsResult.Where(n => n.Title.Contains(title));
            }
            if (!string.IsNullOrEmpty(content))
            {
                newsResult = newsResult.Where(n => n.Content.Contains(content));
            }
            if (startDateTime != null)
            {
                //newsResult = newsResult.Where(n => n.StartDateTime.Date == ((DateTime)startDateTime).Date); // 2025/01/01 00:00:00 只取日期 2025/01/01
                // OR 這種
                newsResult = newsResult.Where(n => n.StartDateTime.Date == startDateTime.Value.Date); // 2025/01/01 00:00:00 只取日期 2025/01/01
            }
            return newsResult;
        }
    }
}
