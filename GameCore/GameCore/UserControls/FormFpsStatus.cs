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

        private readonly Dictionary<string, UserControlFpsStatus> userControlFpsStatuses =
            new Dictionary<string, UserControlFpsStatus>();

        public FormFpsStatus()
        {
            InitializeComponent();
        }


        protected override void OnLoad(EventArgs e)
        {
            foreach (Label label in statusLabel)
            {
                label.Width = flowLayoutPanel1.Width - 10;
            }
        }


        public void TheStatusStringDelegate(OpStatus opStatus)
        {
            Async.UI(delegate { StatusReceived(opStatus); }, this, true);
        }

        private void StatusReceived(OpStatus opStatus)
        {
            string tempName = opStatus.Name;
            if (!userControlFpsStatuses.ContainsKey(tempName))
            {
                userControlFpsStatuses.Add(tempName, CreateUserControlFpsStatus(tempName));
            }
            UserControlFpsStatus tempFpsStatus = userControlFpsStatuses[tempName];

            tempFpsStatus.UpdateLabels(opStatus);
        }

        private UserControlFpsStatus CreateUserControlFpsStatus(string tempName)
        {
            UserControlFpsStatus tempFpsStatus = new UserControlFpsStatus(tempName)
                {
                    Width = flowLayoutPanel1.Width - 5,
                    Height = 130
                };
            flowLayoutPanel1.Controls.Add(tempFpsStatus);

            return tempFpsStatus;
        }
    }
}