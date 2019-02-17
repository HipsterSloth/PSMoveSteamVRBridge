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
    public partial class PSMoveControllerPanel : UserControl, IControllerPanel
    {
        Dictionary<string, eEmulatedTrackpadAction> TouchpadActionTable = Constants.MakeTrackpadActionDictionary();
        private PSMoveControllerConfig controllerConfig;

        public PSMoveControllerPanel(PSMoveControllerConfig config)
        {
            InitializeComponent();

            InitTouchpadComboBox(PSButtonComboBox);
            InitTouchpadComboBox(MoveButtonComboBox);
            InitTouchpadComboBox(TriangleButtonComboBox);
            InitTouchpadComboBox(SquareButtonComboBox);
            InitTouchpadComboBox(CircleButtonComboBox);
            InitTouchpadComboBox(CrossButtonComboBox);
            InitTouchpadComboBox(SelectButtonComboBox);
            InitTouchpadComboBox(StartButtonComboBox);

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

        private void DisableControllerCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            controllerConfig.ControllerDisabled = DisableControllerCheckBox.Checked;
        }

        private void RumbleSuppressedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            controllerConfig.RumbleSuppressed = RumbleSuppressedCheckBox.Checked;
        }

        private void ZRotate90CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            controllerConfig.ZRotate90Degrees = ZRotate90CheckBox.Checked;
        }

        private void TouchpadPressDelayCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            controllerConfig.DelayAfterTouchpadPress = TouchpadPressDelayCheckBox.Checked;
        }

        private void VelocityExponentTextField_Validated(object sender, System.EventArgs e)
        {
            controllerConfig.LinearVelocityExponent=
                ConfigBase.ParseFloat(VelocityExponentTextField.Text, 1.0f, controllerConfig.LinearVelocityExponent);
        }

        private void VelocityMultiplierTextField_Validated(object sender, System.EventArgs e)
        {
            controllerConfig.LinearVelocityMultiplier =
                ConfigBase.ParseFloat(VelocityMultiplierTextField.Text, 1.0f, controllerConfig.LinearVelocityMultiplier);
        }

        private void VirtualTumbstickScaleTextField_Validated(object sender, System.EventArgs e)
        {
            controllerConfig.MetersPerTouchpadAxisUnits =
                ConfigBase.ParseFloat(VirtualTumbstickScaleTextField.Text, 0.01f, controllerConfig.MetersPerTouchpadAxisUnits);
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

        private void StartButtonComboBox_SelectedValueChanged(object sender, System.EventArgs e)
        {
            controllerConfig.setTrackpadActionForButton(ePSMButtonID.Start, GetTouchpadComboBoxValue(StartButtonComboBox));
        }

        private void SelectButtonComboBox_SelectedValueChanged(object sender, System.EventArgs e)
        {
            controllerConfig.setTrackpadActionForButton(ePSMButtonID.Select, GetTouchpadComboBoxValue(SelectButtonComboBox));
        }

        private void CrossButtonComboBox_SelectedValueChanged(object sender, System.EventArgs e)
        {
            controllerConfig.setTrackpadActionForButton(ePSMButtonID.Cross, GetTouchpadComboBoxValue(CrossButtonComboBox));
        }

        private void CircleButtonComboBox_SelectedValueChanged(object sender, System.EventArgs e)
        {
            controllerConfig.setTrackpadActionForButton(ePSMButtonID.Circle, GetTouchpadComboBoxValue(CircleButtonComboBox));
        }

        private void SquareButtonComboBox_SelectedValueChanged(object sender, System.EventArgs e)
        {
            controllerConfig.setTrackpadActionForButton(ePSMButtonID.Square, GetTouchpadComboBoxValue(SquareButtonComboBox));
        }

        private void TriangleButtonComboBox_SelectedValueChanged(object sender, System.EventArgs e)
        {
            controllerConfig.setTrackpadActionForButton(ePSMButtonID.Triangle, GetTouchpadComboBoxValue(TriangleButtonComboBox));
        }

        private void MoveButtonComboBox_SelectedValueChanged(object sender, System.EventArgs e)
        {
            controllerConfig.setTrackpadActionForButton(ePSMButtonID.Move, GetTouchpadComboBoxValue(MoveButtonComboBox));
        }

        private void PSButtonComboBox_SelectedValueChanged(object sender, System.EventArgs e)
        {
            controllerConfig.setTrackpadActionForButton(ePSMButtonID.PS, GetTouchpadComboBoxValue(PSButtonComboBox));
        }

        public void ReloadFromConfig()
        {
            if (controllerConfig == null)
                return;

            RumbleSuppressedCheckBox.Checked = controllerConfig.RumbleSuppressed;
            ZRotate90CheckBox.Checked = controllerConfig.ZRotate90Degrees;
            TouchpadPressDelayCheckBox.Checked = controllerConfig.DelayAfterTouchpadPress;

            VelocityExponentTextField.Text = controllerConfig.LinearVelocityExponent.ToString();
            VelocityMultiplierTextField.Text = controllerConfig.LinearVelocityMultiplier.ToString();
            VirtualTumbstickScaleTextField.Text = string.Format("{0}", controllerConfig.MetersPerTouchpadAxisUnits * 100.0f);
            ExtendYTextField.Text = string.Format("{0}", controllerConfig.ExtendYMeters * 100.0f);
            ExtendZTextField.Text = string.Format("{0}", controllerConfig.ExtendZMeters * 100.0f);

            SetTouchpadComboBoxValue(PSButtonComboBox, controllerConfig.getTrackpadActionForButton(ePSMButtonID.PS));
            SetTouchpadComboBoxValue(MoveButtonComboBox, controllerConfig.getTrackpadActionForButton(ePSMButtonID.Move));
            SetTouchpadComboBoxValue(TriangleButtonComboBox, controllerConfig.getTrackpadActionForButton(ePSMButtonID.Triangle));
            SetTouchpadComboBoxValue(SquareButtonComboBox, controllerConfig.getTrackpadActionForButton(ePSMButtonID.Square));
            SetTouchpadComboBoxValue(CircleButtonComboBox, controllerConfig.getTrackpadActionForButton(ePSMButtonID.Circle));
            SetTouchpadComboBoxValue(CrossButtonComboBox, controllerConfig.getTrackpadActionForButton(ePSMButtonID.Cross));
            SetTouchpadComboBoxValue(SelectButtonComboBox, controllerConfig.getTrackpadActionForButton(ePSMButtonID.Select));
            SetTouchpadComboBoxValue(StartButtonComboBox, controllerConfig.getTrackpadActionForButton(ePSMButtonID.Start));
        }

        public void SaveToConfig()
        {
        }
    }
}
