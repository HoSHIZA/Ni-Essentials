namespace NiGames.Essentials.Tabs
{
    public interface ITabContent
    {
        string DisplayName { get; }
        
        void OnSelected();
        void OnDeselected();
    }
}