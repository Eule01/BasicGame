#region

using System.Drawing;
using System.Windows.Forms;

#endregion

namespace GameCore.Render
{
    public class RendererGdiBase : Renderer
    {
        public delegate void RefreshControlDel();

        public Graphics TheGraphics;
        private Control theRenderControl;
        private RefreshControlDel myDelegate;

        public RendererGdiBase(GameStatus aGameStatus) : base(aGameStatus)
        {
        }


        public Control TheRenderControl
        {
            get { return theRenderControl; }
            set { theRenderControl = value;
            myDelegate = new RefreshControlDel(TheRenderControl.Refresh);
            }
        }

        protected override void UpdateRender()
        {
            if (TheRenderControl != null && !TheRenderControl.Disposing)
            {
 
                if (TheRenderControl.InvokeRequired)
                {
                    TheRenderControl.Invoke(myDelegate);
                }
                else
                {
                    TheRenderControl.Refresh();
                }
//                Async.UI(delegate { TheRenderControl.Refresh(); }, TheRenderControl, false);
            }
        }
    }
}