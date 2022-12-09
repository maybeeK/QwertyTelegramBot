namespace TelegramBot
{
    internal class CourseInfo
    {
        public CourseInfo(string name, string discr, string link)
        {
            Name = name;
            Discr = discr;
            Link = link;
        }

        public string Name { get; set; }
        public string Discr { get; set; }
        public string Link { get; set; }
        public override string ToString()
        {
            return $"{Name} - {Discr} - {Link}";
        }
    }
}