#region

using System.Windows.Forms;
using GameCore.Render;

#endregion

namespace GameTestForm
{
    public partial class UserControlMainGame : UserControl
    {
        private GameCore.GameCore theGameCore;
        private RendererGdi theRenderer;

        public UserControlMainGame()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        public UserControlMainGame(GameCore.GameCore aGameCore)
        {
            theGameCore = aGameCore;
            theRenderer = (RendererGdi) aGameCore.TheRenderer;
            theRenderer.TheRenderControl = this;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            theRenderer.TheGraphics = e.Graphics;
            theRenderer.DrawGame();
//            Invalidate();
        }
    }
}