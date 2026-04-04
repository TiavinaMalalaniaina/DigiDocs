using BackOffice.Models.Enums;

namespace BackOffice.Models
{
    public class EditDocumentModel
    {

        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Category { get; set; }
        public string? Description { get; set; }
        public string? AccessLevel { get; set; }
    }
}
