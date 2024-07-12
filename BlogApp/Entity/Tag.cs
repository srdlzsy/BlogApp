namespace BlogApp.Entity;

public class Tag
{
    public enum TagColors
    {
        primary,danger,info,warning
    }
    public int TagId { get; set; }
    public string? Text { get; set; }
    public string? Url { get; set; }
    public TagColors? Color { get; set; }
    public List<Post> Posts { get; set; } = new List<Post>();
}
