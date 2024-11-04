using UnityEngine.UIElements;

namespace NiGames.Essentials.Editor.UI
{
    public class TabButton : VisualElement
    {
        public TabButton()
        {
        }
        
        #region UXML Factory
        
        public new class UxmlFactory : UxmlFactory<Tab, UxmlTraits> { }
        
        #endregion
    }
}