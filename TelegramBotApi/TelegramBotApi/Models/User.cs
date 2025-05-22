using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TelegramBotApi.Models;

public partial class User
{
    public long ChatId { get; set; }

    public string Username { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime LastActive { get; set; }

    [JsonIgnore]

    public virtual ICollection<History> Histories { get; set; } = new List<History>();

    [JsonIgnore]

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
