using System.Drawing;
using System.Windows.Forms;
using CodeToast;

namespace GameCore.Render
{
    public class RendererGdiBase : Renderer
    {
        public Graphics TheGraphics;
        public Control TheRenderControl;

        public RendererGdiBase(GameStatus aGameStatus) : base(aGameStatus)
        {
        }

        protected override void UpdateRender()
        {
            if (TheRenderControl != null)
            {
                Async.UI(delegate { TheRenderControl.Refresh(); }, TheRenderControl, false);
            }
        }
    }
}