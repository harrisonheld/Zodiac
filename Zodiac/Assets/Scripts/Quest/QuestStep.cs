namespace QuestNamespace
{
    public class QuestStep
    {
        public string Id { get; set; }
        public string Title { get; set; } = "[Default title]";
        public string Description { get; set; } = "[Default description]";
        public bool IsComplete { get; set; } = false;
    }
}