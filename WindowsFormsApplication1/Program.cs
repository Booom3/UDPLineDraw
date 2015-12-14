using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    static class Program
    {
		public static int arg1, arg2, arg3;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
		static void Main(string[] args)
        {
			arg1 = Int32.Parse(args.ElementAtOrDefault (0) ?? "0");
			arg2 = Int32.Parse(args.ElementAtOrDefault (1) ?? "0");
			arg3 = Int32.Parse (args.ElementAtOrDefault (2) ?? "13337");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
