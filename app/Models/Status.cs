namespace app.Models
{
    public class Status {

        public const int AVAILABLE = 1;
        public const int BUSY = 2;
        public const int UNAVAILABLE = 3;
        
        public Status() {}

        public Status(string statusName, string style, string glyphicon)
        {
            StatusName = statusName;
            Style = style;
            Glyphicon = glyphicon;
            IsDeleteable = true;
        }

        public int Id { get; set; }
        public string StatusName { get; set; }
        public string Style { get; set; }
        public string Glyphicon { get; set; }
        public bool IsDeleteable { get; set; } = true;
    }
}