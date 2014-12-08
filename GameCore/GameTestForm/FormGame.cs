#region

using System;
using System.ComponentModel;
using System.Windows.Forms;
using GameCore;
using GameCore.Interface;
using GameCore.Render;

#endregion

namespace GameTestForm
{
    public partial class FormGame : Form
    {
        private UserControlMainGame theControlMainGame;
        private GameCore.GameCore theGameCore;
        private UserInput theUserInput;

        public FormGame()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            Text = "Game Test Form";
            KeyPreview = true;
//            DoubleBuffered = true;
//
//            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
//            SetStyle(ControlStyles.UserPaint, true);
//            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            theGameCore = new GameCore.GameCore();
            theGameCore.TheRenderer = new RendererGdi(theGameCore.TheGameStatus);
            theUserInput = theGameCore.TheUserInput;
            theGameCore.TheGameEventHandler += TheGameCoreOnTheGameEventHandler;


            theControlMainGame = new UserControlMainGame(theGameCore) {Dock = DockStyle.Fill};
            Controls.Add(theControlMainGame);
        }

        private void TheGameCoreOnTheGameEventHandler(object sender, GameEventArgs args)
        {
            Console.WriteLine(args.ToString());
        }

        protected override void OnLoad(EventArgs e)
        {
            theGameCore.Start();
            base.OnLoad(e);
        }


        protected override void OnClosing(CancelEventArgs e)
        {
            theGameCore.Close();
        }

        private const Keys KEY_FORWARD = Keys.W;
        private const Keys KEY_BACKWARD = Keys.S;


        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == KEY_FORWARD)
            {
                theUserInput.Forward = true;
                e.Handled = true;
            }
            if (e.KeyCode == KEY_BACKWARD)
            {
                theUserInput.Backward = true;
                e.Handled = true;
            }
            base.OnKeyDown(e);
        }


        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == KEY_FORWARD)
            {
                theUserInput.Forward = false;
                e.Handled = true;
            }
            if (e.KeyCode == KEY_BACKWARD)
            {
                theUserInput.Backward = false;
                e.Handled = true;
            }
            base.OnKeyUp(e);
        }


        protected override bool IsInputKey(Keys keyData)
        {
//            switch (keyData)
            switch (keyData & Keys.KeyCode)
            {
                case KEY_FORWARD:
                    return true;
                case KEY_BACKWARD:
                    return true;
                default:
                    return base.IsInputKey(keyData);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override bool ProcessKeyPreview(ref Message m)
        {
            //            //for debugging only
            //            MsgCodes MsgCode = (MsgCodes)m.Msg;
            //            Keys Key = (Keys)((int)(m.WParam)) | Control.ModifierKeys;
            //            if (Array.IndexOf(ExaminedKeyActions, MsgCode) >= 0)
            //            {
            //                Debug.WriteLine(string.Concat(MsgCode, " ", Key));
            //            }
            //
            //            if ((m.Msg == 0x100) || (m.Msg == 0x104))
            //            {
            //                KeyEventArgs e = new KeyEventArgs(((Keys)((int)(m.WParam))) | Control.ModifierKeys);
            //                TrappedKeyDown(e);
            //                if (e.Handled)
            //                {
            //                    Debug.WriteLine("Trapped");
            //                    return true;
            //                }
            //            }

            return base.ProcessKeyPreview(ref m);
        }

    }
}