using HelloWorld.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HelloWorld.Dtos;

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
    }
}
