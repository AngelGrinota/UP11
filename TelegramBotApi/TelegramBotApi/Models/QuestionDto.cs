namespace TelegramBotApi.Models
{
    public class QuestionDto
    {
        public int QuestionId { get; set; }

        public string QuestionText { get; set; } = null!;

        public int CategoryId { get; set; }

        public string FileName { get; set; } = null!;

        public string FileData { get; set; } = null!;

        public string PhotoName { get; set; } = null!;

        public byte[] PhotoData { get; set; } = null!;

        public DateTime UploadDate { get; set; }
    }
}
