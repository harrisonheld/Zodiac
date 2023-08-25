namespace QuestNamespace
{
    public class QuestStep
    {
        public string Id { get; set; }
        public string Description { get; set; } = "[Default description]";
        public bool IsComplete { get; set; } = false;
    }
}