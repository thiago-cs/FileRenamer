using System;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;


namespace FileRenamer.Helpers;

internal static class WindowHelper
{
	public static void Resize(Window window, int width, int height)
	{
		//AppWindow appWindow = GetAppWindow(window);
		GetAppWindow(window).Resize(new Windows.Graphics.SizeInt32(width, height));
	}

	private static AppWindow GetAppWindow(Window window)
	{
		IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
		WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
		AppWindow appWindow = AppWindow.GetFromWindowId(windowId);
		return appWindow;
	}
}