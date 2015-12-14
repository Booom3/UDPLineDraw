//Run this in LINQPad or whatever. Change IPs and ports.

Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
ProtocolType.Udp);

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
	string text = rnd.Next(1000).ToString() + "," + rnd.Next(1000).ToString() + "," + (4000 + rnd.Next(1000)).ToString() + "," + rnd.Next(1000).ToString();
	byte[] send_buffer = Encoding.ASCII.GetBytes(text );
	
	foreach (var s in endPoints)
	{
		sock.SendTo(send_buffer , s);
	}
	Thread.Sleep(2000);
}
