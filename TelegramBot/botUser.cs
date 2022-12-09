namespace TelegramBot
{
    public class botUser
    {
        public botUser(long id, string tags)
        {
            Id = id;
            Tags = tags;
        }

        public long Id { get; set; }
        public string Tags { get; set; }
    }
}