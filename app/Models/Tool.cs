namespace app.Models
{
    public class Tool
    {
        public Tool() {}

        public Tool(string name, string status)
        {
            Name = name;
            Status = status;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
    }
}