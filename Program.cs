using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace ClockScreensaver
{
	public class Program
	{
		[DllImport("user32.dll", SetLastError = true)]
		static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

		[DllImport("user32.dll")]
		static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		const int GWL_STYLE = -16;
		const int WS_CHILD = 0x40000000;

		[STAThread]
		public static void Main(string[] args)
		{
			if (args.Length > 0)
			{
				string arg = args[0].ToLower().Trim();

				if (arg.StartsWith("/c"))
				{
					// No configuration window
					return;
				}
				else if (arg.StartsWith("/p") && args.Length >= 2)
				{
					// Preview mode (inside small monitor window)
					if (int.TryParse(args[1], out int previewHandle))
					{
						IntPtr hwndParent = new IntPtr(previewHandle);

						var previewWindow = new MainWindow();
						previewWindow.WindowStyle = WindowStyle.None;
						previewWindow.ResizeMode = ResizeMode.NoResize;
						previewWindow.ShowInTaskbar = false;

						WindowInteropHelper helper = new WindowInteropHelper(previewWindow);
						helper.Owner = hwndParent;

						previewWindow.SourceInitialized += (s, e) =>
						{
							IntPtr hwnd = helper.Handle;
							SetParent(hwnd, hwndParent);
							int style = GetWindowLong(hwnd, GWL_STYLE);
							SetWindowLong(hwnd, GWL_STYLE, style | WS_CHILD);
						};

						var app = new Application();
						app.Run(previewWindow);
						return;
					}
				}
				else if (arg.StartsWith("/s"))
				{
					// Full screen saver
					var app = new Application();
					app.Run(new MainWindow());
					return;
				}
			}

			// Default: full screen if no arguments
			var defaultApp = new Application();
			defaultApp.Run(new MainWindow());
		}
	}
}
