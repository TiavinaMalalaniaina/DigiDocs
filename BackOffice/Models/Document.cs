using BackOffice.Models.Enums;

namespace BackOffice.Models
{
    public class Document
    {
        public int Id { get; set; }
        public DateTime UploadDate { get; set; }
        public string? Title { get; set; }
        public int DownloadCount { get; set; }
        public byte[]? FileData { get; set; }
        public string? FileName { get; set; }
        public string? Category { get; set; }
        public ContentTypeDocument ContentType { get; set; }
        public string? Description { get; set; }
        public string? AccessLevel { get; set; }
    }
}
