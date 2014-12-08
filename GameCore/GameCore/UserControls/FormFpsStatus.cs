#region

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CodeToast;
using GameCore.Utils.Timers;

#endregion

namespace GameCore.UserControls
{
    public partial class FormFpsStatus : Form
    {
        private readonly List<Label> statusLabel = new List<Label>();


        public FormFpsStatus()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            for (int i = 0; i < 6; i++)
            {
                Label tempLabel = new Label();
                statusLabel.Add(tempLabel);
            }

            SuspendLayout();
            foreach (Label label in statusLabel)
            {
                flowLayoutPanel1.Controls.Add(label);
            }
            ResumeLayout(false);

            UpdateLabels(null);
        }

        protected override void OnLoad(EventArgs e)
        {
            foreach (Label label in statusLabel)
            {
                label.Width = flowLayoutPanel1.Width - 10;
            }
        }

        private void UpdateLabels(OpStatus opStatus)
        {
            if (opStatus == null)
            {
                statusLabel[0].Text = "FPS: " + "-" + "Hz";
                statusLabel[1].Text = "Load: " + "-" + "%";
                statusLabel[2].Text = "Avr. Time: " + "-";
                statusLabel[3].Text = "Missed frames: " + "-";
                statusLabel[4].Text = "Interval max time: " + "-";
                statusLabel[5].Text = "Max time: " + "-";
            }
            else
            {
                statusLabel[0].Text = "FPS: " + opStatus.Ops.ToString("0.0") + "Hz";
                statusLabel[1].Text = "Load: " + opStatus.Load.ToString("0.0") + "%";
                statusLabel[2].Text = "Avr. Time: " + OpStatus.GetNiceTime(opStatus.AvrOpTime);
                statusLabel[3].Text = "Missed frames: " + opStatus.MissedFrames;
                statusLabel[4].Text = "Interval max time: " + OpStatus.GetNiceTime(opStatus.IntervalMaxTime);
                statusLabel[5].Text = "Max time: " + OpStatus.GetNiceTime(opStatus.MaxTime);
            }
            Update();
//            Invalidate();
        }


        public void TheStatusStringDelegate(OpStatus opStatus)
        {
            Async.UI(delegate { UpdateLabels(opStatus); }, this, true);
        }
    }
}