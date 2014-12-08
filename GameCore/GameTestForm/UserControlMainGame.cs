#region

using System.Drawing;
using System.Windows.Forms;
using GameCore.Interface;
using GameCore.Render;

#endregion

namespace GameTestForm
{
    public partial class UserControlMainGame : UserControl
    {
        private GameCore.GameCore theGameCore;
        private RendererGdiBase theRenderer;
        private UserInput theUserInput;

        public UserControlMainGame()
        {
            InitializeComponent();
            Init();
        }

        public UserControlMainGame(GameCore.GameCore aGameCore)
        {
            theGameCore = aGameCore;
            theUserInput = theGameCore.TheUserInput;
            theRenderer = (RendererGdiBase) aGameCore.TheRenderer;
            theRenderer.TheRenderControl = this;
            Init();
        }


        private void Init()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
//            DoubleBuffered = true;
            Paint += new PaintEventHandler(pictureBoxBackGround_Paint);
            MouseMove += UserControlMainGame_MouseMove;
            MouseLeave += UserControlMainGame_MouseLeave;
//            MousePosition
        }

        void UserControlMainGame_MouseLeave(object sender, System.EventArgs e)
        {
            if (theUserInput != null)
            {
                theUserInput.MousePosition = Point.Empty;
            }
        }

        private void UserControlMainGame_MouseMove(object sender, MouseEventArgs e)
        {
            if (theUserInput != null)
            {
                theUserInput.MousePosition = e.Location;
            }
        }

        private void pictureBoxBackGround_Paint(object sender, PaintEventArgs e)
        {
            theRenderer.TheGraphics = e.Graphics;
            theRenderer.DrawGame();
            Invalidate();
        }

        //        protected override void OnPaint(PaintEventArgs e)
//        {
//            theRenderer.TheGraphics = e.Graphics;
//            theRenderer.DrawGame();
////            Invalidate();
//        }
    }
}