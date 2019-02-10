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
    public partial class FreePIEHmdSlotMapping : UserControl
    {
        public delegate void SlotMappingChanged();
        public event SlotMappingChanged SlotMappingChangedEvent;

        public delegate void SlotMappingDeleted();
        public event SlotMappingDeleted SlotMappingDeletedEvent;

        private static Dictionary<string, eHmdSource> SourceTable = PSMHmdPool.MakeHmdSourceDictionary();
        private static Dictionary<string, eHmdPropertySource> PropertyTable = PSMHmdPool.MakeHmdPropertyDictionary();

        public FreePIEHmdSlotMapping(FreePIEHmdSlotDefinition slotDefinition)
        {
            InitializeComponent();

            SetupComboBoxPair(
                XSourceComboBox, slotDefinition.xProperty.hmdSource,
                XPropertyComboBox, slotDefinition.xProperty.hmdPropertySource);
            SetupComboBoxPair(
                YSourceComboBox, slotDefinition.yProperty.hmdSource,
                YPropertyComboBox, slotDefinition.yProperty.hmdPropertySource);
            SetupComboBoxPair(
                ZSourceComboBox, slotDefinition.zProperty.hmdSource,
                ZPropertyComboBox, slotDefinition.zProperty.hmdPropertySource);
            SetupComboBoxPair(
                PitchSourceComboBox, slotDefinition.pitchProperty.hmdSource,
                PitchPropertyComboBox, slotDefinition.pitchProperty.hmdPropertySource);
            SetupComboBoxPair(
                YawSourceComboBox, slotDefinition.yawProperty.hmdSource,
                YawPropertyComboBox, slotDefinition.yawProperty.hmdPropertySource);
            SetupComboBoxPair(
                RollSourceComboBox, slotDefinition.rollProperty.hmdSource,
                RollPropertyComboBox, slotDefinition.rollProperty.hmdPropertySource);
        }

        public void FetchSlotDefinition(FreePIEHmdSlotDefinition slotDefinition)
        {
            FetchComboBoxValues(
                XSourceComboBox, out slotDefinition.xProperty.hmdSource,
                XPropertyComboBox, out slotDefinition.xProperty.hmdPropertySource);
            FetchComboBoxValues(
                YSourceComboBox, out slotDefinition.yProperty.hmdSource,
                YPropertyComboBox, out slotDefinition.yProperty.hmdPropertySource);
            FetchComboBoxValues(
                ZSourceComboBox, out slotDefinition.zProperty.hmdSource,
                ZPropertyComboBox, out slotDefinition.zProperty.hmdPropertySource);
            FetchComboBoxValues(
                PitchSourceComboBox, out slotDefinition.pitchProperty.hmdSource,
                PitchPropertyComboBox, out slotDefinition.pitchProperty.hmdPropertySource);
            FetchComboBoxValues(
                YawSourceComboBox, out slotDefinition.yawProperty.hmdSource,
                YawPropertyComboBox, out slotDefinition.yawProperty.hmdPropertySource);
            FetchComboBoxValues(
                RollSourceComboBox, out slotDefinition.rollProperty.hmdSource,
                RollPropertyComboBox, out slotDefinition.rollProperty.hmdPropertySource);
        }

        public void SetEnabled(bool bIsEnabled)
        {
            XSourceComboBox.Enabled = bIsEnabled;
            XPropertyComboBox.Enabled = bIsEnabled;
            YSourceComboBox.Enabled = bIsEnabled;
            YPropertyComboBox.Enabled = bIsEnabled;
            ZSourceComboBox.Enabled = bIsEnabled;
            ZPropertyComboBox.Enabled = bIsEnabled;
            PitchSourceComboBox.Enabled = bIsEnabled;
            PitchPropertyComboBox.Enabled = bIsEnabled;
            YawSourceComboBox.Enabled = bIsEnabled;
            YawPropertyComboBox.Enabled = bIsEnabled;
            RollSourceComboBox.Enabled = bIsEnabled;
            RollPropertyComboBox.Enabled = bIsEnabled;
            DownButton.Enabled = bIsEnabled;
            UpButton.Enabled = bIsEnabled;
            CloseButton.Enabled = bIsEnabled;
        }

        private void SetupComboBoxPair(
            ComboBox SourceComboBox, eHmdSource Source,
            ComboBox PropertyComboBox, eHmdPropertySource Property)
        {
            SourceComboBox.DataSource = new BindingSource(SourceTable, null);
            SourceComboBox.DisplayMember = "Key";
            SourceComboBox.ValueMember = "Value";
            SourceComboBox.SelectedValue = Source;
            SourceComboBox.SelectedIndexChanged += SourceComboBox_SelectedIndexChanged;

            PropertyComboBox.DataSource = new BindingSource(PropertyTable, null);
            PropertyComboBox.DisplayMember = "Key";
            PropertyComboBox.ValueMember = "Value";
            PropertyComboBox.SelectedValue = Property;
            PropertyComboBox.SelectedIndexChanged += SourceComboBox_SelectedIndexChanged;
        }

        private void SourceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SlotMappingChangedEvent != null)
            {
                SlotMappingChangedEvent();
            }
        }

        private void FetchComboBoxValues(
            ComboBox SourceComboBox, out eHmdSource Source,
            ComboBox PropertyComboBox, out eHmdPropertySource Property)
        {
            Source = ((KeyValuePair<string, eHmdSource>)SourceComboBox.SelectedItem).Value;
            Property = ((KeyValuePair<string, eHmdPropertySource>)PropertyComboBox.SelectedItem).Value;
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            if (SlotMappingDeletedEvent != null) {
                SlotMappingDeletedEvent();
            }

            this.Parent.Controls.Remove(this);
        }

        private void UpButton_Click(object sender, EventArgs e)
        {
            int thisIndex = this.Parent.Controls.IndexOf(this);

            if (thisIndex > 0) {
                int otherIndex = thisIndex - 1;
                var otherControl = this.Parent.Controls[otherIndex];

                this.Parent.Controls.SetChildIndex(this, otherIndex);
                this.Parent.Controls.SetChildIndex(otherControl, thisIndex);
            }
        }

        private void DownButton_Click(object sender, EventArgs e)
        {
            int thisIndex = this.Parent.Controls.IndexOf(this);

            if (thisIndex < this.Parent.Controls.Count - 1) {
                int otherIndex = thisIndex + 1;
                var otherControl = this.Parent.Controls[otherIndex];

                this.Parent.Controls.SetChildIndex(this, otherIndex);
                this.Parent.Controls.SetChildIndex(otherControl, thisIndex);
            }
        }
    }
}
