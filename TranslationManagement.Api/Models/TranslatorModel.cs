namespace TranslationManagement.Api.Models
{
    public class TranslatorModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string HourlyRate { get; set; }
        public string Status { get; set; }
        public string CreditCardNumber { get; set; }

        public TranslatorModel() { }

        public TranslatorModel(int id, string name, string hourlyRate, string status, string creditCardNumber)
        {
            Id = id;
            Name = name;
            HourlyRate = hourlyRate;
            Status = status;
            CreditCardNumber = creditCardNumber;
        }
    }
}
