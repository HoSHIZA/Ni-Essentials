using UnityEngine;
using UnityEngine.UI;

namespace NiGames.Essentials.Components
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasRenderer))]
    [AddComponentMenu(Constants.Menu.UI + "Non Drawing Graphic")]
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
