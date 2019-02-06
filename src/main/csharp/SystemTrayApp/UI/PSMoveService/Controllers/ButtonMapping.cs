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
    public partial class ButtonMapping : UserControl
    {
        private static int UniqueID = 0;

        private Dictionary<string, ePSMButtonID> ButtonTable = new Dictionary<string, ePSMButtonID>();
        private Dictionary<string, eEmulatedTrackpadAction> TouchpadActionTable = Constants.MakeTrackpadActionDictionary();

        public ButtonMapping(ePSMButtonID[] buttons, ePSMButtonID currentButtonID, eEmulatedTrackpadAction currentAction)
        {
            InitializeComponent();

            this.Name = string.Format("ButtonMapping_{0}", UniqueID);
            UniqueID++;

            foreach (ePSMButtonID buttonID in buttons) {
                string buttonString = Constants.PSMButtonNames[(int)buttonID];
                ButtonTable.Add(buttonString, buttonID);
            }

            PSButtonComboBox.DataSource = new BindingSource(ButtonTable, null);
            PSButtonComboBox.DisplayMember = "Key";
            PSButtonComboBox.ValueMember = "Value";
            PSButtonComboBox.SelectedValue = currentButtonID;

            TrackpadActionComboBox.DataSource = new BindingSource(TouchpadActionTable, null);
            TrackpadActionComboBox.DisplayMember = "Key";
            TrackpadActionComboBox.ValueMember = "Value";
            TrackpadActionComboBox.SelectedValue = currentAction;
        }

        public ePSMButtonID GetButtonID()
        {
            return ((KeyValuePair<string, ePSMButtonID>)PSButtonComboBox.SelectedItem).Value;
        }

        public eEmulatedTrackpadAction GetTrackpadAction()
        {
            return ((KeyValuePair<string, eEmulatedTrackpadAction>)TrackpadActionComboBox.SelectedItem).Value;
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            this.Parent.Controls.Remove(this);
        }
    }
}
