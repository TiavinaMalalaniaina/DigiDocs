namespace FrontOffice.Models
{
    public class Document
    {
        public int Id { get; set; }
        public DateTime UploadDate { get; set; }
        public int DownloadCount { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public byte[] FileData { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }

        public string AccessLevel { get; set; }
        public string Category { get; set; }
    }
}
