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
    public partial class SteamVRHmdAlignTool : UserControl, IAppPanel
    {
        public SteamVRHmdAlignTool()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {

        }

        private void OkButton_Click(object sender, EventArgs e)
        {

        }

        public void OnPanelEntered()
        {
        }

        public void OnPanelExited()
        {
        }
    }
}
