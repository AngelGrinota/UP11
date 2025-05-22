using System.ComponentModel.DataAnnotations;

namespace TelegramBotApi.Models
{
    public class NotificationDto
    {
        public int NotificationId { get; set; }

        [Required]
        public string NotificationText { get; set; } = null!;

        public DateTime SendTime { get; set; }

        public string Status { get; set; } = null!;

        public byte[] PhotoNotification { get; set; } = null!;
    }
}
