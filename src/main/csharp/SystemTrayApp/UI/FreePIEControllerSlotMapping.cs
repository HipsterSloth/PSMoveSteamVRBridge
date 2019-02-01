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
    public partial class FreePIEControllerSlotMapping : UserControl
    {
        public delegate void SlotMappingDeleted();
        public event SlotMappingDeleted SlotMappingDeletedEvent;

        private static Dictionary<string, eControllerSource> SourceTable = PSMDevicePool.MakeControllerSourceDictionary();
        private static Dictionary<string, eControllerPropertySource> PropertyTable = PSMDevicePool.MakeControllerPropertyDictionary();

        public FreePIEControllerSlotMapping(FreePIEControllerSlotDefinition slotDefinition)
        {
            InitializeComponent();

            SetupComboBoxPair(
                XSourceComboBox, slotDefinition.xProperty.controllerSource,
                XPropertyComboBox, slotDefinition.xProperty.controllerPropertySource);
            SetupComboBoxPair(
                YSourceComboBox, slotDefinition.yProperty.controllerSource,
                YPropertyComboBox, slotDefinition.yProperty.controllerPropertySource);
            SetupComboBoxPair(
                ZSourceComboBox, slotDefinition.zProperty.controllerSource,
                ZPropertyComboBox, slotDefinition.zProperty.controllerPropertySource);
            SetupComboBoxPair(
                PitchSourceComboBox, slotDefinition.pitchProperty.controllerSource,
                PitchPropertyComboBox, slotDefinition.pitchProperty.controllerPropertySource);
            SetupComboBoxPair(
                YawSourceComboBox, slotDefinition.yawProperty.controllerSource,
                YawPropertyComboBox, slotDefinition.yawProperty.controllerPropertySource);
            SetupComboBoxPair(
                RollSourceComboBox, slotDefinition.rollProperty.controllerSource,
                RollPropertyComboBox, slotDefinition.rollProperty.controllerPropertySource);
        }

        public void FetchSlotDefinition(FreePIEControllerSlotDefinition slotDefinition)
        {
            FetchComboBoxValues(
                XSourceComboBox, out slotDefinition.xProperty.controllerSource,
                XPropertyComboBox, out slotDefinition.xProperty.controllerPropertySource);
            FetchComboBoxValues(
                YSourceComboBox, out slotDefinition.yProperty.controllerSource,
                YPropertyComboBox, out slotDefinition.yProperty.controllerPropertySource);
            FetchComboBoxValues(
                ZSourceComboBox, out slotDefinition.zProperty.controllerSource,
                ZPropertyComboBox, out slotDefinition.zProperty.controllerPropertySource);
            FetchComboBoxValues(
                PitchSourceComboBox, out slotDefinition.pitchProperty.controllerSource,
                PitchPropertyComboBox, out slotDefinition.pitchProperty.controllerPropertySource);
            FetchComboBoxValues(
                YawSourceComboBox, out slotDefinition.yawProperty.controllerSource,
                YawPropertyComboBox, out slotDefinition.yawProperty.controllerPropertySource);
            FetchComboBoxValues(
                RollSourceComboBox, out slotDefinition.rollProperty.controllerSource,
                RollPropertyComboBox, out slotDefinition.rollProperty.controllerPropertySource);
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
            CloseButton.Enabled = bIsEnabled;
        }

        private void SetupComboBoxPair(
            ComboBox SourceComboBox, eControllerSource Source,
            ComboBox PropertyComboBox, eControllerPropertySource Property)
        {
            SourceComboBox.DataSource = new BindingSource(SourceTable, null);
            SourceComboBox.DisplayMember = "Key";
            SourceComboBox.ValueMember = "Value";
            SourceComboBox.SelectedValue = Source;

            PropertyComboBox.DataSource = new BindingSource(PropertyTable, null);
            PropertyComboBox.DisplayMember = "Key";
            PropertyComboBox.ValueMember = "Value";
            PropertyComboBox.SelectedValue = Property;
        }

        private void FetchComboBoxValues(
            ComboBox SourceComboBox, out eControllerSource Source,
            ComboBox PropertyComboBox, out eControllerPropertySource Property)
        {
            Source= ((KeyValuePair<string, eControllerSource>)SourceComboBox.SelectedItem).Value;
            Property = ((KeyValuePair<string, eControllerPropertySource>)SourceComboBox.SelectedItem).Value;
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            if (SlotMappingDeletedEvent != null)
            {
                SlotMappingDeletedEvent();
            }

            this.Parent.Controls.Remove(this);
        }
    }
}
