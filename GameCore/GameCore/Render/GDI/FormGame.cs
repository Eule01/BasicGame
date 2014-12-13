#region

using System;
using System.ComponentModel;
using System.Windows.Forms;
using GameCore.Interface;
using GameCore.UserControls;

#endregion

namespace GameCore.Render.GDI
{
    public partial class FormGame : Form
    {
        private UserControlMainGame theControlMainGame;
        private GameCore theGameCore;
        private UserInput theUserInput;

        public FormGame()
        {
            InitializeComponent();
            Init();
        }

        public FormGame(GameCore theGameCore)
        {
            this.theGameCore = theGameCore;
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

//            theGameCore = new GameCore();
//            theGameCore.TheRenderer = new RendererGdi {TheGameStatus = theGameCore.TheGameStatus};
//            theGameCore.TheRenderer.Start();
            theUserInput = theGameCore.TheUserInput;


            theControlMainGame = new UserControlMainGame(theGameCore) {Dock = DockStyle.Fill};
            Controls.Add(theControlMainGame);
        }

      

        protected override void OnLoad(EventArgs e)
        {

//            theGameCore.Start();


            base.OnLoad(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
//            theGameCore.Close();

//            e.Cancel = true;
            base.OnFormClosing(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
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