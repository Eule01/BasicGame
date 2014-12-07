#region

using System;
using System.ComponentModel;
using System.Windows.Forms;
using GameCore;
using GameCore.Render;

#endregion

namespace GameTestForm
{
    public partial class FormGame : Form
    {
        private UserControlMainGame theControlMainGame;
        private GameCore.GameCore theGameCore; 

        public FormGame()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            this.Text = "Game Test Form";

            theGameCore = new GameCore.GameCore();
            theGameCore.TheRenderer = new RendererGdi(theGameCore.TheGameStatus);
            theGameCore.TheGameEventHandler +=TheGameCoreOnTheGameEventHandler;



            theControlMainGame = new UserControlMainGame(theGameCore) {Dock = DockStyle.Fill};
            Controls.Add(theControlMainGame);
        }

        private void TheGameCoreOnTheGameEventHandler(object sender, GameEventArgs args)
        {
            Console.WriteLine(args.ToString());
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            theGameCore.Close();
        }
    }
}