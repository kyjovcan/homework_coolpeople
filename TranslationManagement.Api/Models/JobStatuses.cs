namespace TranslationManagement.Api.Models
{
    public static class JobStatuses
    {
        internal static readonly string New = "New";
        internal static readonly string InProgress = "InProgress";
        internal static readonly string Completed = "Completed";

        public static bool IsValidStatus(string status)
        {
            return status == New || status == InProgress || status == Completed;
        }

        public static bool IsValidStatusChange(TranslationJob job, string newStatus)
        {
            return (job.Status == New && newStatus == Completed) || (job.Status == Completed && newStatus == New);
        }
    }
}
