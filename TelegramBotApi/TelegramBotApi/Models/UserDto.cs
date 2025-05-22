namespace TelegramBotApi.Models
{
    public class UserDto
    {
        public long ChatId { get; set; }

        public string Username { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime LastActive { get; set; }
    }
}
