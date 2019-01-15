using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace SystemTrayApp
{
    public partial class PSNaviControllerPanel : UserControl, IControllerPanel
    {
        Dictionary<string, eEmulatedTrackpadAction> TouchpadActionTable = Constants.MakeTrackpadActionDictionary();
        private PSNaviControllerConfig controllerConfig;

        public PSNaviControllerPanel(PSNaviControllerConfig config)
        {
            InitializeComponent();

            InitTouchpadComboBox(PSButtonComboBox);
            InitTouchpadComboBox(DPadDownButtonComboBox);
            InitTouchpadComboBox(DPadUpButtonComboBox);
            InitTouchpadComboBox(DPadLeftButtonComboBox);
            InitTouchpadComboBox(DPadRightButtonComboBox);
            InitTouchpadComboBox(CircleButtonComboBox);
            InitTouchpadComboBox(CrossButtonComboBox);
            InitTouchpadComboBox(JoystickButtonComboBox);
            InitTouchpadComboBox(ShoulderButtonComboBox);

            controllerConfig = config;

            ReloadFromConfig();
        }

        private void InitTouchpadComboBox(ComboBox combo_box)
        {
            combo_box.DataSource = new BindingSource(TouchpadActionTable, null);
            combo_box.DisplayMember = "Key";
            combo_box.ValueMember = "Value";
        }

        private void SetTouchpadComboBoxValue(ComboBox combo_box, eEmulatedTrackpadAction action)
        {
            combo_box.SelectedIndex = (int)action;
        }

        private eEmulatedTrackpadAction GetTouchpadComboBoxValue(ComboBox combo_box)
        {
            return ((KeyValuePair<string, eEmulatedTrackpadAction>)combo_box.SelectedItem).Value;
        }

        private void NumericText_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            Regex regex = new Regex(@"^[0-9.-]*$");
            Match match = regex.Match(textBox.Text);
            if (!match.Success) {
                // Cancel the event and select the text to be corrected by the user.
                e.Cancel = true;
                textBox.Select(0, textBox.Text.Length);
            }
        }

        private void ThumbstickDeadzoneTextField_Validated(object sender, System.EventArgs e)
        {
            float ThumbstickDeadzone = 0.0f;
            if (float.TryParse(ThumbstickDeadzoneTextField.Text, out ThumbstickDeadzone)) {
                controllerConfig.ThumbstickDeadzone = ThumbstickDeadzone;
            }
        }

        private void ShoulderButtonComboBox_SelectedValueChanged(object sender, System.EventArgs e)
        {
            controllerConfig.setTrackpadActionForButton(ePSMButtonID.Shoulder, GetTouchpadComboBoxValue(ShoulderButtonComboBox));
        }

        private void JoystickButtonComboBox_SelectedValueChanged(object sender, System.EventArgs e)
        {
            controllerConfig.setTrackpadActionForButton(ePSMButtonID.Joystick, GetTouchpadComboBoxValue(JoystickButtonComboBox));
        }

        private void CrossButtonComboBox_SelectedValueChanged(object sender, System.EventArgs e)
        {
            controllerConfig.setTrackpadActionForButton(ePSMButtonID.Cross, GetTouchpadComboBoxValue(CrossButtonComboBox));
        }

        private void CircleButtonComboBox_SelectedValueChanged(object sender, System.EventArgs e)
        {
            controllerConfig.setTrackpadActionForButton(ePSMButtonID.Circle, GetTouchpadComboBoxValue(CircleButtonComboBox));
        }

        private void DPadDownButtonComboBox_SelectedValueChanged(object sender, System.EventArgs e)
        {
            controllerConfig.setTrackpadActionForButton(ePSMButtonID.DPad_Down, GetTouchpadComboBoxValue(DPadDownButtonComboBox));
        }

        private void DPadRightButtonComboBox_SelectedValueChanged(object sender, System.EventArgs e)
        {
            controllerConfig.setTrackpadActionForButton(ePSMButtonID.DPad_Right, GetTouchpadComboBoxValue(DPadRightButtonComboBox));
        }

        private void DPadUpButtonComboBox_SelectedValueChanged(object sender, System.EventArgs e)
        {
            controllerConfig.setTrackpadActionForButton(ePSMButtonID.DPad_Up, GetTouchpadComboBoxValue(DPadUpButtonComboBox));
        }

        private void DPadLeftButtonComboBox_SelectedValueChanged(object sender, System.EventArgs e)
        {
            controllerConfig.setTrackpadActionForButton(ePSMButtonID.DPad_Left, GetTouchpadComboBoxValue(DPadLeftButtonComboBox));
        }

        private void PSButtonComboBox_SelectedValueChanged(object sender, System.EventArgs e)
        {
            controllerConfig.setTrackpadActionForButton(ePSMButtonID.PS, GetTouchpadComboBoxValue(PSButtonComboBox));
        }

        public void ReloadFromConfig()
        {
            ThumbstickDeadzoneTextField.Text = controllerConfig.ThumbstickDeadzone.ToString();

            SetTouchpadComboBoxValue(PSButtonComboBox, controllerConfig.getTrackpadActionForButton(ePSMButtonID.PS));
            SetTouchpadComboBoxValue(DPadDownButtonComboBox, controllerConfig.getTrackpadActionForButton(ePSMButtonID.DPad_Down));
            SetTouchpadComboBoxValue(DPadUpButtonComboBox, controllerConfig.getTrackpadActionForButton(ePSMButtonID.DPad_Up));
            SetTouchpadComboBoxValue(DPadLeftButtonComboBox, controllerConfig.getTrackpadActionForButton(ePSMButtonID.DPad_Left));
            SetTouchpadComboBoxValue(DPadRightButtonComboBox, controllerConfig.getTrackpadActionForButton(ePSMButtonID.DPad_Right));
            SetTouchpadComboBoxValue(CircleButtonComboBox, controllerConfig.getTrackpadActionForButton(ePSMButtonID.Circle));
            SetTouchpadComboBoxValue(CrossButtonComboBox, controllerConfig.getTrackpadActionForButton(ePSMButtonID.Cross));
            SetTouchpadComboBoxValue(JoystickButtonComboBox, controllerConfig.getTrackpadActionForButton(ePSMButtonID.Joystick));
            SetTouchpadComboBoxValue(ShoulderButtonComboBox, controllerConfig.getTrackpadActionForButton(ePSMButtonID.Shoulder));
        }

        public void SaveToConfig()
        {
        }
    }
}
