using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System.Data;
using TelegramBotApi.DataContext;
using TelegramBotApi.Models;

namespace TelegramBotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly VoenkomContext _context;
        private readonly string _connectionString = "server=127.0.0.1;port=3306;userid=root;password=root;database=voenkom;";

        public NotificationController(VoenkomContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get() => Ok(await _context.Notifications.ToListAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var notification = await _context.Notifications.FindAsync(id);

            if (notification != null)
                return Ok(notification);
            else
                return NotFound();
        }

        [HttpPost]

        public async Task<IActionResult> Add([FromBody] NotificationDto notificationDto)
        {
            var notification = new Notification
            {
                NotificationText = notificationDto.NotificationText,
                SendTime = notificationDto.SendTime,
                Status = notificationDto.Status,
                PhotoNotification = notificationDto.PhotoNotification,
            };

            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();

            return Ok(notification);
        }

        [HttpDelete("{id:int}")]

        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var notification = await _context.Notifications.FindAsync(id);

            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPut("{id:int}")]

        public async Task<IActionResult> Update([FromRoute] int id, NotificationDto newNotification)
        {
            var notification = await _context.Notifications.FindAsync(id);

            if (notification == null)
            {
                return NotFound();
            }

            notification.NotificationText = newNotification.NotificationText;
            notification.SendTime = newNotification.SendTime;
            notification.Status = newNotification.Status;
            notification.PhotoNotification = newNotification.PhotoNotification;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("mark-assent/{id:int}")]
        public async Task<IActionResult> UpdateStatus([FromRoute] int id)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new MySqlCommand("update_notification_status", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("p_notification_id", id);

                    try
                    {
                        await command.ExecuteNonQueryAsync();
                        return Ok();
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, $"Error occurred: {ex.Message}");
                    }
                }
            }
        }

        [HttpGet("{id:int}/users")]
        public async Task<IActionResult> GetUsersByNotificationId([FromRoute] int id)
        {
            var chatIds = new List<long>();

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new MySqlCommand("get_users_by_notification_id", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("p_notification_id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                chatIds.Add(reader.GetInt64("chat_id"));
                            }
                        }
                    }
                }

                return Ok(chatIds);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error occurred: {ex.Message}");
            }
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentNotifications()
        {
            var notifications = new List<NotificationDto>();

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new MySqlCommand("get_current_notifications", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var notification = new NotificationDto
                                {
                                    NotificationId = reader.GetInt32("notification_id"),
                                    NotificationText = reader.GetString("notification_text"),
                                    SendTime = reader.GetDateTime("send_time"),
                                    Status = reader.GetString("status"),
                                    PhotoNotification = (byte[])reader["photo_notification"]
                                };

                                notifications.Add(notification);
                            }
                        }
                    }
                }

                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error occurred: {ex.Message}");
            }
        }
    }
}
