using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TelegramBotApi.Models;

public partial class Question
{
    public int QuestionId { get; set; }

    public string QuestionText { get; set; } = null!;

    public int CategoryId { get; set; }

    public string FileName { get; set; } = null!;

    public string FileData { get; set; } = null!;

    public string PhotoName { get; set; } = null!;

    public byte[] PhotoData { get; set; } = null!;

    public DateTime UploadDate { get; set; }

    [JsonIgnore]
    public virtual Category Category { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<History> Histories { get; set; } = new List<History>();
}
