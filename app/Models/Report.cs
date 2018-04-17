namespace app.Models
{
    public class Report
    {
        public Report() {}

        public Report(string error, string userId)
        {
            Error = error;
            UserId = userId;
        }

        public int Id { get; set; }
        public string Error { get; set; }
        public string UserId { get; set; }
    }
}