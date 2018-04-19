namespace app.Models
{
    public class Status {
        
        public Status() {}

        public Status(string statusName, string style, string glyphicon)
        {
            StatusName = statusName;
            Style = style;
            Glyphicon = glyphicon;
        }

        public int Id { get; set; }
        public string StatusName { get; set; }
        public string Style { get; set; }
        public string Glyphicon { get; set; }
    }
}