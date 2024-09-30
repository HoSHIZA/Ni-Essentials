using System;
using NiGames.Essentials.Easing;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace NiGames.Essentials.Editor
{
    [CustomPropertyDrawer(typeof(EaseData))]
    public sealed class EaseDataDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var element = new VisualElement();
            
            var easeField = new VisualElement()
            {
                name = "ease",
                style =
                {
                    flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row),
                    flexGrow = 1,
                },
            };
            
            // ! Ease Type
            var easeTypeField = new EnumField("Ease")
            {
                name = "ease-type",
                style = { flexGrow = 1, },
            };
            easeTypeField.BindPropertyRelative(property, "_easeType");
            easeTypeField.SetInspectorAligned(true);
            
            // ! Ease Power
            var easePower = property.FindPropertyRelative("_easePower");
            var easePowerField = new EnumField(string.Empty)
            {
                name = "ease-power",
                style = { flexGrow = 4, },
            };
            easePowerField.BindProperty(easePower);
            easePowerField.SetInspectorAligned(true);
            
            // ! AnimationCurve
            var curve = property.FindPropertyRelative("_curve");
            var curveField = new CurveField(string.Empty)
            {
                name = "ease-curve",
                style = { flexGrow = 4, },
            };
            curveField.BindProperty(curve);
            curveField.SetInspectorAligned(true);
            
            // ! Callbacks
            
            easeTypeField.RegisterValueChangedCallback(evt =>
            {
                SetEasePowerVisibility(evt.newValue);
            });
            easePowerField.RegisterValueChangedCallback(evt =>
            {
                SetEaseTypeVisibility(evt.newValue);
            });
            
            SetEasePowerVisibility(easeTypeField.value);
            SetEaseTypeVisibility(easePowerField.value);
            
            easeField.Add(easeTypeField);
            easeField.Add(easePowerField);
            easeField.Add(curveField);
            
            element.Add(easeField);
            
            return element;
            
            void SetEasePowerVisibility(Enum value)
            {
                if (value is EaseType.Custom)
                {
                    easePowerField.SetDisplay(false);
                    curveField.SetDisplay(true);
                }
                else
                {
                    easePowerField.SetDisplay(true);
                    curveField.SetDisplay(false);
                }
            }
            
            void SetEaseTypeVisibility(Enum value)
            {
                if (value is EasePower.Linear)
                {
                    easeTypeField.SetDisplay(false);
                    easePowerField.label = "Ease";
                }
                else
                {
                    easeTypeField.SetDisplay(true);
                    easePowerField.label = string.Empty;
                }
            }
        }
    }
}