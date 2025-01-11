namespace Rise.Data.Messages
{
    /// <summary>
    /// Represents a simple notification with text content,
    /// a title, and an icon.
    /// </summary>
    public sealed record BasicNotification(string Title, string Content, string Icon)
    {
    }
}
