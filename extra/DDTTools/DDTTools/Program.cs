using Lsj.Util;
using System;
using System.Windows.Forms;

namespace DDTTools
{
	internal static class Program
	{
		[STAThread]
		private static void Main()
		{
			WinForm.CatchAll();
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
	}
}
