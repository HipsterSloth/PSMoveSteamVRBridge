using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SystemTrayApp
{
    public partial class Dualshock4ControllerPanel : UserControl, IControllerPanel
    {
        private static ePSMButtonID[] DS4Buttons = new ePSMButtonID[] {
            ePSMButtonID.PS,
            ePSMButtonID.Triangle,
            ePSMButtonID.Circle,
            ePSMButtonID.Cross,
            ePSMButtonID.Square,
            ePSMButtonID.DPad_Left,
            ePSMButtonID.DPad_Up,
            ePSMButtonID.DPad_Right,
            ePSMButtonID.DPad_Down,
            ePSMButtonID.Options,
            ePSMButtonID.Share,
            ePSMButtonID.Touchpad,
            ePSMButtonID.LeftJoystick,
            ePSMButtonID.RightJoystick,
            ePSMButtonID.LeftShoulder,
            ePSMButtonID.RightShoulder
        };

        private ControllerConfig AssignedConfig;

        public Dualshock4ControllerPanel(ControllerConfig config)
        {
            InitializeComponent();

            AssignedConfig = config;
        }

        private void AddNewMappingButton_Click(object sender, EventArgs e)
        {
            TouchpadMappingsLayoutPanel.Controls.Add(new ButtonMapping(DS4Buttons, ePSMButtonID.PS, eEmulatedTrackpadAction.None));
        }

        public void Reload()
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }
    }
}
