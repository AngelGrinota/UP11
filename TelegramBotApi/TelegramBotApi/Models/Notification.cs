using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TelegramBotApi.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public string NotificationText { get; set; } = null!;

    public DateTime SendTime { get; set; }

    public string Status { get; set; } = null!;

    public byte[] PhotoNotification { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<User> Chats { get; set; } = new List<User>();
}
