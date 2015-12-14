using System;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public class Canvas : UserControl
    {
        public delegate void DrawEventHandler(Graphics g);

        public event DrawEventHandler Draw;

        public int Interval { get; set; }

        private Timer timer;
        private bool needPaint;

        public Canvas()
        {
            Interval = 100;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        public void Start()
        {
            if (timer == null || !timer.Enabled)
            {
                Stop();

                timer = new Timer();
                timer.Interval = Interval;
                timer.Tick += timer_Tick;
                timer.Start();
            }
        }

        public void Start(int interval)
        {
            Interval = interval;
            Start();
        }

        public void Stop()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
            }
        }

        protected override void Dispose(bool disposing)
        {
            Stop();
            base.Dispose(disposing);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            needPaint = true;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (needPaint)
            {
                OnDraw(e.Graphics);
                needPaint = false;
            }
        }

        protected void OnDraw(Graphics g)
        {
            if (Draw != null)
            {
                Draw(g);
            }
        }
    }
}