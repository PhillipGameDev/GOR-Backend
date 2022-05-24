namespace GameOfRevenge.Common.Models.Quest.Template
{
    public interface IBaseQuestTemplateData
    {
        QuestType QuestType { get; }
        string Name { get; }
        void SetData(string template);
    }
}
