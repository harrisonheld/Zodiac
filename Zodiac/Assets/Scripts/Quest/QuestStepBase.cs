namespace QuestNamespace
{
    public abstract class QuestStepBase
    {
        public string Description { get; set; } = "[Default description]";
        public bool IsComplete { get; set; } = false;
    }

    public class QuestStepSpeakToNpc : QuestStepBase
    {
        public string TargetNpcBlueprint { get; set; } = "[Default NPC blueprint]";
    }
}