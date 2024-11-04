using System;
using UnityEngine.UIElements;

namespace NiGames.Essentials.Editor.UI
{
    public class ActionCard : Card
    {
        private Button _actionButton;
        private Action _action;

        public string ActionText
        {
            get => _actionButton?.text;
            set
            {
                if (_actionButton != null)
                {
                    _actionButton.text = value;
                }
            }
        }

        public ActionCard() : this(null)
        {
        }
        
        public ActionCard(string title) : base(title)
        {
        }

        public void SetAction(Action action)
        {
            _action = action;
            _actionButton?.SetEnabled(action != null);
        }

        public override VisualElement CreateControl()
        {
            _actionButton = new Button(_action)
            {
                text = "Invoke",
                style =
                {
                    marginTop = 0,
                    marginBottom = 0,
                }
            };
            
            _actionButton.SetEnabled(_action != null);
            return _actionButton;
        }
        
        public new class UxmlFactory : UxmlFactory<ActionCard, UxmlTraits> { }
        public new class UxmlTraits : Card.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _actionText = new() { name = "action-text", defaultValue = "Invoke" };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                ((ActionCard)ve).ActionText = _actionText.GetValueFromBag(bag, cc);
            }
        }
    }
}