#region

using System.Drawing;
using System.Windows.Forms;
using CodeToast;

#endregion

namespace GameCore.Render.GDI
{
    public class RendererGdiBase : Renderer
    {
        public delegate void RefreshControlDel();

        public Graphics TheGraphics;
        private Control theRenderControl;
        private RefreshControlDel myDelegate;

        private FormGame theFormGame;


        public RendererGdiBase() : base()
        {
            name = "RendererGdiBase";
        }


        public Control TheRenderControl
        {
            get { return theRenderControl; }
            set
            {
                theRenderControl = value;
                myDelegate = new RefreshControlDel(TheRenderControl.Refresh);
            }
        }

        public override void Start()
        {
            theFormGame = new FormGame(GameCore.TheGameCore);
            theFormGame.Show();
//            Async.Do(delegate { Application.Run(theFormGame); });

            base.Start();
        }

        public override void Close()
        {
            if (theFormGame != null)
            {
                theFormGame.Close();


                //                Async.UI(delegate { theFormGame.Close(); }, theFormGame, true);
                theFormGame = null;
            }
            base.Close();
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