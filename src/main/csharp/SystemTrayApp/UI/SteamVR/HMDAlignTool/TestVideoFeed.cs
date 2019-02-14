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
            TrackerVideo.SelectedTrackerChangedEvent += OnSelectedTrackerChangedEvent;

            VideoFrame.Controls.Add(TrackerVideo);
            OnSelectedTrackerChangedEvent(TrackerVideo.SelectedTrackerIndex);
        }

        private void OnSelectedTrackerChangedEvent(int trackerId)
        {
            if (trackerId != -1)
            {
                this.VideoFrameLabel.Text = string.Format("Tracker {0}", trackerId);
            }
            else
            {
                this.VideoFrameLabel.Text = "No Trackers";
            }
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
            AppWindow.Instance.SetSteamVRPanel(new SteamVRHmdAlignTool());
        }

        public void OnPanelEntered()
        {
        }

        public void OnPanelExited()
        {
            VideoFrame.Controls.Remove(TrackerVideo);
            TrackerVideo.Dispose();
        }

        private void CalibrationMapLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs evt)
        {
            CalibrationMapLink.LinkVisited = true;

            try
            {
                System.Diagnostics.Process.Start("https://github.com/cboulay/PSMoveService/raw/master/misc/calibration/CalibrationMat.pdf");
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("Unable to open link: {0}", e.Message));
            }
        }
    }
}
