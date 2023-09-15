namespace WebUI.DTOs
{
    public class ChatVM
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public string CourseId { get; set; }
        public string OutlineId { get; set; }
        public string UserId { get; set; }
        public DateTime? Date_Time { get; set; }
        public bool IsYours { get; set; }
        public string? Image { get; set; }
    }
}
