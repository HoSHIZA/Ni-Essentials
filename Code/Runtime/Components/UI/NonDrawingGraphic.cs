using UnityEngine;
using UnityEngine.UI;

namespace NiGames.Essentials.Components
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasRenderer))]
    [AddComponentMenu(MenuPath.COMPONENT_MENU_ROOT_UI + "Non Drawing Graphic")]
    public class NonDrawingGraphic : MaskableGraphic
    {
        public override void SetMaterialDirty()
        {
        }

        public override void SetVerticesDirty()
        {
        }

        protected override void OnPopulateMesh(VertexHelper vh) => vh.Clear();
    }
}
