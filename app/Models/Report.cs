namespace app.Models
{
    public class Report
    {
        public Report() {}

        public Report(string error)
        {
            Error = error;
        }

        public int Id { get; set; }
        public string Error { get; set; }
    }
}