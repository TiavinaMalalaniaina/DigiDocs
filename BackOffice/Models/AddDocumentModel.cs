namespace BackOffice.Models
{
    public class AddDocumentModel
    {
        public IFormFile? FileData { get; set; }
        public string? Title { get; set; }
        public string? Category { get; set; }
        public string? Description { get; set; }
        public string? AccessLevel { get; set; }
    }
}
