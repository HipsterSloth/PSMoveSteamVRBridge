using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using PSMoveService;

namespace SystemTrayApp
{
    public partial class TestVideoFeed : UserControl, IAppPanel
    {
        private TrackerVideoFrame TrackerVideo;

        public TestVideoFeed()
        {
            InitializeComponent();

            TrackerVideo= new TrackerVideoFrame();
            TrackerVideo.Size = new System.Drawing.Size(320, 240);

            VideoFrame.Controls.Add(TrackerVideo);
        }

        private void PrevCameraButton_Click(object sender, EventArgs e)
        {
            TrackerVideo.GotoPrevTracker();
        }

        private void NextCameraButton_Click(object sender, EventArgs e)
        {
            TrackerVideo.GotoNextTracker();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            AppWindow.Instance.SetSteamVRPanel(new SteamVRPanel());
        }

        private void OkButton_Click(object sender, EventArgs e)
        {

        }

        public void OnPanelEntered()
        {
        }

        public void OnPanelExited()
        {
            VideoFrame.Controls.Remove(TrackerVideo);
            TrackerVideo.Dispose();
        }
    }
}
