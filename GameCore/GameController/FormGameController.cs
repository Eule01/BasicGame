#region

using System;
using System.Windows.Forms;
using GameCore;
using GameCore.UserControls;
using GameCore.Utils;

#endregion

namespace GameController
{
    public partial class FormGameController : Form
    {
        private GameCore.GameCore theGameCore;

        private FormFpsStatus formTempStatus;


        public FormGameController()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            theGameCore = new GameCore.GameCore();
            theGameCore.TheGameEventHandler += TheGameCoreOnTheGameEventHandler;

            Text = "Game Controller";
        }

        protected override void OnLoad(EventArgs e)
        {
            formTempStatus = new FormFpsStatus();
            formTempStatus.Show();

            comboBoxRenderers.Items.AddRange(items: theGameCore.TheRendererManager.TheRenderers.ToArray());
            comboBoxRenderers.SelectedIndex = 2;
            comboBoxRenderers.SelectedIndexChanged += comboBoxRenderers_SelectedIndexChanged;


            FormPositioner.PlaceOnSecondScreenIfPossible(this, FormPositioner.Locations.TopLeft, false);
            FormPositioner.PlaceNextToForm(formTempStatus, this, FormPositioner.Locations.Right);

            theGameCore.Start();

            base.OnLoad(e);
        }

        private void comboBoxRenderers_SelectedIndexChanged(object sender, EventArgs e)
        {
            theGameCore.ChangeRenderer(comboBoxRenderers.SelectedIndex);
        }

        private void TheGameCoreOnTheGameEventHandler(object sender, GameEventArgs args)
        {
            Console.WriteLine(args.ToString());
            switch (args.TheType)
            {
                case GameEventArgs.Types.Status:
                    if (formTempStatus != null && formTempStatus.Visible && args.TheOpStatus != null)
                    {
                        formTempStatus.TheStatusStringDelegate(args.TheOpStatus);
                    }
                    break;
                case GameEventArgs.Types.Message:
                    break;
                case GameEventArgs.Types.MapLoaded:

                    break;
                case GameEventArgs.Types.MapSaved:

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            theGameCore.Close();

            base.OnFormClosing(e);
        }

        private void buttonSaveMap_Click(object sender, EventArgs e)
        {
            theGameCore.SaveMap("testMap2.xml");
        }

        private void buttonLoadMap_Click(object sender, EventArgs e)
        {
            theGameCore.LoadMap("testMap.xml");
        }
    }
}