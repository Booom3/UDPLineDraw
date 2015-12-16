using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    static class Program
    {
		public static int WinOffsetX = 0, WinOffsetY = 0, Port = 13337;
        public static bool isServer = false;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
		static void Main(string[] args)
        {
            ParseArgs(args);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        private static void ParseArgs(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-winoffsetx":
                        WinOffsetX = Int32.Parse(args.ElementAtOrDefault(i + 1) ?? "0");
                        break;
                    case "-winoffsety":
                        WinOffsetY = Int32.Parse(args.ElementAtOrDefault(i + 1) ?? "0");
                        break;
                    case "-port":
                        Port = Int32.Parse(args.ElementAtOrDefault(i + 1) ?? "13337");
                        break;
                    case "-server":
                        isServer = bool.Parse(args.ElementAtOrDefault(i + 1) ?? "false");
                        break;
                }
            }
        }
    }
}
