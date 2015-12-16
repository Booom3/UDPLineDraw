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
        private List<Line> clientLines = new List<Line>();
        int offsetx, offsety;
        public MainForm()
        {
			offsetx = Program.WinOffsetX;
			offsety = Program.WinOffsetY;
            InitializeComponent();
        }

        private void RunServer()
        {
            List<NetworkObject> newObjects = new List<NetworkObject>();

            int netObjectCount = 0;
            List<NetworkObject> serverObjects = new List<NetworkObject>();
            for (int i = 0; i < 10; i++)
            {
                Line l = new Line(netObjectCount);
                serverObjects.Add(l);
                newObjects.Add(l);
                netObjectCount += 1;
            }
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            //TODO: Have clients call the server with their IP and port
            Tuple<string, int>[] addressAndPorts = {
                Tuple.Create("127.0.0.1", 13337),
                Tuple.Create("192.168.2.10", 13337),
                Tuple.Create("192.168.2.10", 13338)
            };

            var endPoints = new List<IPEndPoint>();
            foreach (var t in addressAndPorts)
            {
                endPoints.Add(new IPEndPoint(IPAddress.Parse(t.Item1), t.Item2));
            }

            Random rnd = new Random();
            while (true)
            {
                List<byte[]> send_buffer = new List<byte[]>();
                //TODO: Wait until new objects are acknowledged by clients
                if (newObjects.Count > 0)
                {
                    foreach (NetworkObject n in newObjects)
                    {
                        string newObjectString = "New," + Line.networkNameIdentifier + "," + n.NetworkID;
                        send_buffer.Add(Encoding.ASCII.GetBytes(newObjectString));
                    }
                    newObjects.Clear();
                }
                else
                {
                    foreach (Line l in serverObjects)
                    {
                        l.SetPoints(new int[] { rnd.Next(1000), rnd.Next(1000),
                    (4000 + rnd.Next(1000)), rnd.Next(1000)});

                        string text = Line.networkNameIdentifier + "," + l.NetworkID + "," + l.GetCSVString();

                        send_buffer.Add(Encoding.ASCII.GetBytes(text));


                    }
                }
                foreach (var s in endPoints)
                {
                    foreach (byte[] b in send_buffer)
                    {
                        sock.SendTo(b, s);
                    }
                }
                Thread.Sleep(2000);
            }
        }

        private void ClientNewNetworkObject(string fullNetString)
        {
            string[] args = fullNetString.Split(',');
            switch (args[1])
            {
                case "Line":
                    clientLines.Add(new Line(int.Parse(args[2])));
                    break;
            }
        }
        
        Thread udpListen;
        Thread server;
        List<Thread> threads = new List<Thread>();
        string udpString = "";
        private void canvas1_Load(object sender, EventArgs e)
        {
            canvas1.Start(12);
            udpListen = new Thread(listenUdp);
            udpListen.Start();
            threads.Add(udpListen);
            if (Program.isServer)
            {
                server = new Thread(RunServer);
                server.Start();
                threads.Add(server);
            }
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
            if (splits[0] == "New")
            {
                ClientNewNetworkObject(receiveString);
            }
            if (splits[0] == Line.networkNameIdentifier)
            {
                int id = int.Parse(splits[1]);
                foreach (Line l in clientLines)
                {
                    if (l.NetworkID == id)
                    {
                        int[] ints = new int[4];
                        for (int i = 0; i < 3; i++)
                            ints[i] = Convert.ToInt32(splits[i + 2]) - (i % 2 == 0 ? offsetx : offsety);
                        l.SetPoints(ints);
                    }
                }
            }

            messageReceived = true;
            BeginReceive();
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
			IPEndPoint e = new IPEndPoint(IPAddress.Any, Program.Port);
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

        Random rnd = new Random();

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach (Thread t in threads)
                t.Abort();
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
            foreach (Line l in clientLines)
            {
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    Point tempp1, tempp2;
                    Point tempWinp;
                    tempWinp = new Point(this.Left, this.Top);
                    tempp1 = Point.Subtract(new Point(l.StartX, l.StartY), (Size)tempWinp);
                    tempp2 = Point.Subtract(new Point(l.EndX, l.EndY), (Size)tempWinp);
                    g.DrawLine(pen, tempp1, tempp2);
                }
            }
            using (Brush brush = new SolidBrush(Color.Black))
            using (Font font = new Font("Arial", 16))
            {
                g.DrawString(fps.ToString() + " Objs: " + clientLines.Count, font, brush, new Point(0, 0));
                g.DrawString(udpString, font, brush, new Point(0, 20));
            }
        }
    }
}
