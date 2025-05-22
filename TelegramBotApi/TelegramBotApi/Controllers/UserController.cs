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
    public class UserController : ControllerBase
    {
        private readonly VoenkomContext _context;
        private readonly string _connectionString = "server=127.0.0.1;port=3306;userid=root;password=root;database=voenkom;";

        public UserController(VoenkomContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get() => Ok(await _context.Users.ToListAsync());

        [HttpGet("count")]
        public async Task<IActionResult> GetUserCount() => Ok(await _context.Users.CountAsync());

        [HttpPost]

        public async Task<IActionResult> AddAndUpdate([FromBody] UserDto userDto)
        {
            if (userDto == null)
            {
                return BadRequest();
            }

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new MySqlCommand("insert_or_update_user", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@p_username", userDto.Username);
                    command.Parameters.AddWithValue("@p_chat_id", userDto.ChatId);
                    command.Parameters.AddWithValue("@p_last_active", DateTime.Now);

                    await command.ExecuteNonQueryAsync();
                }
            }

            return Ok();
        }
    }
}
