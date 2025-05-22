using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TelegramBotApi.Models;

public partial class History
{
    public long ChatId { get; set; }

    public int QuestionId { get; set; }

    public DateTime RequestDate { get; set; }

    [JsonIgnore]
    public virtual User Chat { get; set; } = null!;

    [JsonIgnore]

    public virtual Question Question { get; set; } = null!;
}
