using System;
using System.Collections.Generic;

namespace TelegramBotApi.Models;

public partial class UserQuestionHistory
{
    public string Username { get; set; } = null!;

    public string QuestionText { get; set; } = null!;

    public DateTime RequestDate { get; set; }
}
