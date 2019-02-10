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
    public partial class TrackerVideoFrame : UserControl
    {
        private PSMVideoFrameBuffer _psmVideoFrame;
        private PSMTrackerPool _psmTrackerPool;
        private eTrackerSource _currentTackerSource;

        private BufferedGraphicsContext _graphicsContext;
        private BufferedGraphics _graphicsBuffer;
        private Bitmap _videoBitmap;

        private Timer _pollTimer;

        public TrackerVideoFrame() : base()
        {
            InitializeComponent();

            this.Resize += new EventHandler(this.OnResize);
            this.SetStyle(
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint,
                true);

            _pollTimer = new Timer();
            _pollTimer.Tick += OnPollVideo;

            _graphicsContext = BufferedGraphicsManager.Current;
            _graphicsContext.MaximumBuffer = new Size(this.Width + 1, this.Height + 1);
            _graphicsBuffer = 
                _graphicsContext.Allocate(
                    this.CreateGraphics(), 
                    new Rectangle(0, 0, this.Width, this.Height));
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            _psmVideoFrame = new PSMVideoFrameBuffer();
            _psmTrackerPool = new PSMTrackerPool();

            _psmTrackerPool.Init(PSMoveServiceContext.Instance.TrackerInfoList);
            _currentTackerSource = _psmTrackerPool.GetFirstValidTrackerSource(eTrackerSource.NONE);

            HandleTrackerChanged();

            PSMoveServiceContext.Instance.TrackerListUpdatedEvent += OnTrackerListUpdatedEvent;
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);

            _psmTrackerPool.Cleanup();

            PSMoveServiceContext.Instance.TrackerListUpdatedEvent -= OnTrackerListUpdatedEvent;
        }

        private void OnPollVideo(object sender, EventArgs e)
        {
            if (_psmTrackerPool.GetVideoFrameBuffer(_currentTackerSource, _psmVideoFrame))
            {
                RenderPSMVideoFrameToBitmap(_graphicsBuffer.Graphics);
                this.Refresh();
            }
        }

        private void OnTrackerListUpdatedEvent()
        {
            TrackerInfo[] trackerInfoList = PSMoveServiceContext.Instance.TrackerInfoList;

            SynchronizedInvoke.Invoke(this, () => HandleTrackerListUpdatedEvent(trackerInfoList));
        }

        private void HandleTrackerListUpdatedEvent(TrackerInfo[] trackerInfoList)
        {
            _psmTrackerPool.RefreshTrackerList(trackerInfoList);

            eTrackerSource oldTrackerSource= _currentTackerSource;
            _currentTackerSource = _psmTrackerPool.GetFirstValidTrackerSource(oldTrackerSource);

            if (oldTrackerSource != _currentTackerSource)
            {
                HandleTrackerChanged();
            }
        }

        public void GotoNextTracker()
        {
            _currentTackerSource = _psmTrackerPool.GetNextValidTrackerSource(_currentTackerSource);
            HandleTrackerChanged();
        }

        public void GotoPrevTracker()
        {
            _currentTackerSource = _psmTrackerPool.GetPrevValidTrackerSource(_currentTackerSource);
            HandleTrackerChanged();
        }

        private void HandleTrackerChanged()
        {
            int timerDelay= _psmTrackerPool.GetTrackerRefreshRateMilliseconds(_currentTackerSource);
            
            if (timerDelay != 0)
            {
                _pollTimer.Interval = timerDelay;

                if (!_pollTimer.Enabled)
                {
                    _pollTimer.Start();
                }
            }
            else
            {
                _pollTimer.Stop();
            }
        }

        private void OnResize(object sender, EventArgs e)
        {
            // Re-create the graphics buffer for a new window size.
            _graphicsContext.MaximumBuffer = new Size(this.Width + 1, this.Height + 1);

            if (_graphicsBuffer != null)
            {
                _graphicsBuffer.Dispose();
                _graphicsBuffer = null;
            }

            _graphicsBuffer = 
                _graphicsContext.Allocate(
                    this.CreateGraphics(),
                    new Rectangle(0, 0, this.Width, this.Height));

            // Cause the background to be cleared and redraw.
            RenderPSMVideoFrameToBitmap(_graphicsBuffer.Graphics);
            this.Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            _graphicsBuffer.Render(e.Graphics);
        }

        private void RenderPSMVideoFrameToBitmap(Graphics g)
        {
            if (_videoBitmap == null ||
                _videoBitmap.Width != _psmVideoFrame.width ||
                _videoBitmap.Height != _psmVideoFrame.height)
            {
                _videoBitmap = new Bitmap((int)_psmVideoFrame.width, (int)_psmVideoFrame.height, PixelFormat.Format24bppRgb);
            }

            BitmapData bData = 
                _videoBitmap.LockBits(
                    new Rectangle(0, 0, _videoBitmap.Width, _videoBitmap.Height),
                    ImageLockMode.WriteOnly, 
                    PixelFormat.Format24bppRgb);

            unsafe
            {
                Buffer.MemoryCopy(
                    _psmVideoFrame.rgb_buffer.ToPointer(),
                    bData.Scan0.ToPointer(),
                    Math.Abs(bData.Stride) * bData.Height,
                    (long)_psmVideoFrame.buffer_size_bytes);
            }

            _videoBitmap.UnlockBits(bData);

            g.DrawImage(_videoBitmap, 0, 0, this.Width, this.Height);
        }
    }
}
