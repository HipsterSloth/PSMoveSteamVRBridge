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

        private DS4ControllerConfig controllerConfig;

        public Dualshock4ControllerPanel(ControllerConfig config)
        {
            InitializeComponent();

            controllerConfig = (DS4ControllerConfig)config;

            ReloadFromConfig();
        }

        private void RumbleSuppressedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            controllerConfig.RumbleSuppressed = RumbleSuppressedCheckBox.Checked;
        }

        private void ZRotate90CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            controllerConfig.ZRotate90Degrees = ZRotate90CheckBox.Checked;
        }

        private void DisableAlignmentGestureCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            controllerConfig.DisableAlignmentGesture = DisableAlignmentGestureCheckBox.Checked;
        }

        private void UseOrientationInHMDAlignmentCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            controllerConfig.UseOrientationInHmdAlignment = UseOrientationInHMDAlignmentCheckBox.Checked;
        }

        private void AddNewMappingButton_Click(object sender, EventArgs e)
        {
            TouchpadMappingsLayoutPanel.Controls.Add(new ButtonMapping(DS4Buttons, ePSMButtonID.PS, eEmulatedTrackpadAction.None));
        }

        private void VelocityExponentTextField_Validated(object sender, System.EventArgs e)
        {
            controllerConfig.LinearVelocityExponent =
                ConfigBase.ParseFloat(VelocityExponentTextField.Text, 1.0f, controllerConfig.LinearVelocityExponent);
        }

        private void VelocityMultiplierTextField_Validated(object sender, System.EventArgs e)
        {
            controllerConfig.LinearVelocityMultiplier =
                ConfigBase.ParseFloat(VelocityMultiplierTextField.Text, 1.0f, controllerConfig.LinearVelocityMultiplier);
        }

        private void ThumbstickDeadzoneTextField_Validated(object sender, System.EventArgs e)
        {
            float ThumbstickDeadzone = 0.0f;
            if (float.TryParse(ThumbstickDeadzoneTextField.Text, out ThumbstickDeadzone)) {
                controllerConfig.ThumbstickDeadzone = ThumbstickDeadzone;
            }
        }

        private void ExtendZTextField_Validated(object sender, System.EventArgs e)
        {
            controllerConfig.ExtendZMeters =
                ConfigBase.ParseFloat(ExtendZTextField.Text, 0.01f, controllerConfig.ExtendZMeters);
        }

        private void ExtendYTextField_Validated(object sender, System.EventArgs e)
        {
            controllerConfig.ExtendYMeters =
                ConfigBase.ParseFloat(ExtendYTextField.Text, 0.01f, controllerConfig.ExtendYMeters);
        }

        public void ReloadFromConfig()
        {
            if (controllerConfig == null)
                return;

            RumbleSuppressedCheckBox.Checked = controllerConfig.RumbleSuppressed;
            ZRotate90CheckBox.Checked = controllerConfig.ZRotate90Degrees;
            DisableAlignmentGestureCheckBox.Checked = controllerConfig.DisableAlignmentGesture;
            UseOrientationInHMDAlignmentCheckBox.Checked = controllerConfig.UseOrientationInHmdAlignment;

            VelocityExponentTextField.Text = controllerConfig.LinearVelocityExponent.ToString();
            VelocityMultiplierTextField.Text = controllerConfig.LinearVelocityMultiplier.ToString();
            ThumbstickDeadzoneTextField.Text = controllerConfig.ThumbstickDeadzone.ToString();
            ExtendYTextField.Text = string.Format("{0}", controllerConfig.ExtendYMeters * 100.0f);
            ExtendZTextField.Text = string.Format("{0}", controllerConfig.ExtendZMeters * 100.0f);

            TouchpadMappingsLayoutPanel.Controls.Clear();
            foreach (ePSMButtonID buttonID in DS4Buttons)
            {
                eEmulatedTrackpadAction trackpadAction = controllerConfig.getTrackpadActionForButton(buttonID);

                if (trackpadAction != eEmulatedTrackpadAction.None)
                {
                    TouchpadMappingsLayoutPanel.Controls.Add(new ButtonMapping(DS4Buttons, buttonID, trackpadAction));
                }
            }
        }

        public void SaveToConfig()
        {
            foreach (ePSMButtonID buttonID in DS4Buttons)
            {
                controllerConfig.setTrackpadActionForButton(buttonID, eEmulatedTrackpadAction.None);
            }

            foreach(ButtonMapping mapping in TouchpadMappingsLayoutPanel.Controls.Cast<ButtonMapping>())
            {
                controllerConfig.setTrackpadActionForButton(mapping.GetButtonID(), mapping.GetTrackpadAction());
            }
        }
    }
}
