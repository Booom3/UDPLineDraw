using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using ShareX.HelpersLib;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace WindowsFormsApplication1
{
    public partial class MainForm : Form
    {
		private int offsetx, offsety ;
        public MainForm()
        {
			offsetx = Program.arg1;
			offsety = Program.arg2;
            InitializeComponent();
        }

        Thread points;
        Thread udpListen;
        string udpString = "";
        private void canvas1_Load(object sender, EventArgs e)
        {
            canvas1.Start(12);
            points = new Thread(updatePoints);
            //points.Start();
            udpListen = new Thread(listenUdp);
            udpListen.Start();
        }

        private class UdpState
        {
            public UdpClient u;
            public IPEndPoint e;
        }
        bool messageReceived = false;
        private void ReceiveCallback(IAsyncResult ar)
        {
            UdpClient u = (UdpClient)((UdpState)(ar.AsyncState)).u;
            IPEndPoint e = (IPEndPoint)((UdpState)(ar.AsyncState)).e;

            Byte[] receiveBytes = u.EndReceive(ar, ref e);
            string receiveString = Encoding.ASCII.GetString(receiveBytes);

            udpString = receiveString;
            string[] splits = receiveString.Split(',');
			if (splits.Length != 4)
				return;
            List<int> ints = new List<int>();
            foreach (string s in splits)
                ints.Add(Convert.ToInt32(s));
            p1.X = ints[0] - offsetx;
			p1.Y = ints[1] - offsety;
			p2.X = ints[2] - offsetx;
            p2.Y = ints[3] - offsety;
            messageReceived = true;
			BeginReceive ();
        }

		private void BeginReceive()
		{
			u.BeginReceive(new AsyncCallback(ReceiveCallback), s);
		}


		UdpClient u;
		UdpState s = new UdpState();

        private void listenUdp()
        {
            // Receive a message and write it to the console.
			IPEndPoint e = new IPEndPoint(IPAddress.Any, Program.arg3);
			u = new UdpClient (e);

            s.e = e;
            s.u = u;
            
			BeginReceive ();
            // Do some work while we wait for a message. For this example,
            // we'll just sleep
            while (true)
            {
                Thread.Sleep(100);
                if (messageReceived)
                {
                }
            }
        }
        

        Point p1, p2;
        private void updatePoints()
        {
            while (true)
            {
                Rectangle scr = Screen.PrimaryScreen.Bounds;
                p1 = new Point(rnd.Next(0, scr.Right), rnd.Next(0, scr.Bottom));
                Thread.Sleep(2000);
            }
        }

        Random rnd = new Random();

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            points.Abort();
            udpListen.Abort();
        }

        private int fps = 0, redraws = 0;

        private void MainForm_LocationChanged(object sender, EventArgs e)
        {
            canvas1.timer_Tick(sender, e);
        }

        private DateTime nextFrame = DateTime.Now;
        private void canvas1_Draw(Graphics g)
        {
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;
            redraws++;
            if (DateTime.Now > nextFrame)
            {
                fps = redraws;
                redraws = 0;
                nextFrame = nextFrame.AddSeconds(1);
            }
            using (Pen pen = new Pen(Color.Red, 2))
            {
                Point tempp1, tempp2;
                Point tempWinp;
                tempWinp = new Point(this.Left, this.Top);
                tempp1 = Point.Subtract(p1, (Size)tempWinp);
                tempp2 = Point.Subtract(p2, (Size)tempWinp);
                g.DrawLine(pen, tempp1, tempp2);
            }
            using (Brush brush = new SolidBrush(Color.Black))
            using (Font font = new Font("Arial", 16))
            {
                g.DrawString(fps.ToString(), font, brush, new Point(0, 0));
                g.DrawString(udpString, font, brush, new Point(0, 20));
            }
        }
    }
}
