#region

using System.Collections.Generic;
using System.Windows.Forms;
using GameCore.Utils.Timers;

#endregion

namespace GameCore.UserControls
{
    public partial class UserControlFpsStatus : UserControl
    {
        private readonly List<Label> statusLabel = new List<Label>();

        private string name;

        public UserControlFpsStatus()
        {
            InitializeComponent();
            Init();
        }

        public UserControlFpsStatus(string name)
        {
            this.name = name;
            InitializeComponent();
            groupBox1.Text = name;
            Init();
        }

        private void Init()
        {
            for (int i = 0; i < 6; i++)
            {
                Label tempLabel = new Label()
                    {
                        Margin = new Padding(0),
                        Padding = new Padding(0),
                        Height = 16,
                        AutoSize = true
                    };
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


        public void UpdateLabels(OpStatus opStatus)
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
                statusLabel[1].Text = "Load: " + opStatus.Load.ToString("0.00") + "%";
                statusLabel[2].Text = "Avr. Time: " + OpStatus.GetNiceTime(opStatus.AvrOpTime);
                statusLabel[3].Text = "Missed frames: " + opStatus.MissedFrames;
                statusLabel[4].Text = "Interval max time: " + OpStatus.GetNiceTime(opStatus.IntervalMaxTime);
                statusLabel[5].Text = "Max time: " + OpStatus.GetNiceTime(opStatus.MaxTime);
            }
            Update();
            //            Invalidate();
        }
    }
}