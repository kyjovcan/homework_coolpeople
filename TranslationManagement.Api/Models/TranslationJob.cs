namespace TranslationManagement.Api.Models
{
    public class TranslationJob
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string Status { get; set; }
        public string OriginalContent { get; set; }
        public string TranslatedContent { get; set; }
        public double Price { get; set; }

        public TranslationJob() { }

        public TranslationJob(int id, string customerName, string status, string originalContent, string translatedContent, double price)
        {
            Id = id;
            CustomerName = customerName;
            Status = status;
            OriginalContent = originalContent;
            TranslatedContent = translatedContent;
            Price = price;
        }
    }
}
