using System;
using System.Collections.Generic;

namespace TelegramBotApi.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
