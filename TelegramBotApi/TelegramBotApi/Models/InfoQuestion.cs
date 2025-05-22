using System;
using System.Collections.Generic;

namespace TelegramBotApi.Models;

public partial class InfoQuestion
{
    public int QuestionId { get; set; }

    public string QuestionText { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public DateTime UploadDate { get; set; }
}
