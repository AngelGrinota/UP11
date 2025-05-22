using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TelegramBotApi.DataContext;

namespace TelegramBotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserQuestionHistoryController : ControllerBase
    {
        private readonly VoenkomContext _context;

        public UserQuestionHistoryController(VoenkomContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get() => Ok(await _context.UserQuestionHistories.ToListAsync());
    }
}
