namespace control.Models;

public class FirebaseToken
{
    public long timestamp { get; set; }
    public string userAgent { get; set; }
    public List<string> topics { get; set;}
    public string name { get; set;}
}