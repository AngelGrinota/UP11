using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using TelegramBotApi.DataContext;
using TelegramBotApi.Models;

namespace TelegramBotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        private readonly VoenkomContext _context;
        private readonly string _connectionString = "server=127.0.0.1;port=3306;userid=root;password=root;database=voenkom;";

        public HistoryController(VoenkomContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get() => Ok(await _context.Histories.ToListAsync());

        [HttpPost]
        public async Task<IActionResult> AddHistory([FromBody] HistoryDto history)
        {
            if (history == null)
            {
                return BadRequest();
            }

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new MySqlCommand("add_record_to_history", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@p_chat_id", history.ChatId);
                    command.Parameters.AddWithValue("@p_question_id", history.QuestionId);

                    await command.ExecuteNonQueryAsync();
                }
            }

            return Ok();
        }
    }
}
