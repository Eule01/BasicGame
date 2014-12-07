#region

using System.Drawing;
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
        }

        public UserControlMainGame(GameCore.GameCore aGameCore)
        {
            theGameCore = aGameCore;
            theRenderer = (RendererGdi) aGameCore.TheRenderer;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            theRenderer.TheGraphics = e.Graphics;
            theRenderer.DrawMap();
        }
    }
}