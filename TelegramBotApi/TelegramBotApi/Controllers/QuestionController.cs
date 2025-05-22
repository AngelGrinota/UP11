using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TelegramBotApi.DataContext;
using TelegramBotApi.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TelegramBotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly VoenkomContext _context;

        public QuestionController(VoenkomContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get() => Ok(await _context.Questions.ToListAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var request = await _context.Questions.FindAsync(id);

            if (request != null)
                return Ok(request);
            else
                return NotFound();
        }

        [HttpPost]

        public async Task<IActionResult> Add([FromBody] QuestionDto questionDto)
        {
            var queryCategory = await _context.Categories.FindAsync(questionDto.CategoryId);

            if (queryCategory == null)
            {
                return BadRequest("Такого CategoryId не существует.");
            }

            var result = new Question
            {
                CategoryId = questionDto.CategoryId,
                QuestionText = questionDto.QuestionText,
                FileName = questionDto.FileName,
                FileData = questionDto.FileData,
                PhotoName = questionDto.PhotoName,
                PhotoData = questionDto.PhotoData
            };

            await _context.Questions.AddAsync(result);
            await _context.SaveChangesAsync();

            return Ok(result);
        }

        [HttpDelete("{id:int}")]

        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var query = await _context.Questions.FindAsync(id);

            if (query != null)
            {
                _context.Questions.Remove(query);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPut("{id:int}")]

        public async Task<IActionResult> Update([FromRoute] int id, QuestionDto newQuestion)
        {
            var query = await _context.Questions.FindAsync(id);

            if (query == null)
            {
                return NotFound();
            }

            var queryCategory = await _context.Categories.FindAsync(newQuestion.CategoryId);

            if (queryCategory == null)
            {
                return BadRequest("Такого CategoryId не существует.");
            }

            query.CategoryId = newQuestion.CategoryId;
            query.QuestionText = newQuestion.QuestionText;
            query.FileName = newQuestion.FileName;
            query.FileData = newQuestion.FileData;
            query.PhotoName = newQuestion.PhotoName;
            query.PhotoData = newQuestion.PhotoData;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
