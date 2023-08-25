using System.Collections.Generic;

namespace QuestNamespace
{
    public class Quest
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public List<QuestStep> Steps { get; set; }
    }
}
