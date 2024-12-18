﻿using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace MonitorAndStart.v2
{
	public class ProcessRunner
	{
		//epic thanks to https://stackoverflow.com/a/58646320
		/// <summary>
		/// We are elevated and should launch the process unelevated. We can't create the
		/// process directly without it becoming elevated. So to workaround this, we have
		/// explorer do the process creation (explorer is typically running unelevated).
		/// </summary>
		internal static void ExecuteProcessUnElevated(string process, string args)
		{
			string currentDirectory = Path.GetDirectoryName(process)!;
			var shellWindows = (IShellWindows)new CShellWindows();

			// Get the desktop window
			object loc = CSIDL_Desktop;
			object unused = new object();
			int hwnd;
			var serviceProvider = (IServiceProvider)shellWindows.FindWindowSW(ref loc, ref unused, SWC_DESKTOP, out hwnd, SWFO_NEEDDISPATCH);

			// Get the shell browser
			var serviceGuid = SID_STopLevelBrowser;
			var interfaceGuid = typeof(IShellBrowser).GUID;
			var shellBrowser = (IShellBrowser)serviceProvider.QueryService(ref serviceGuid, ref interfaceGuid);

			// Get the shell dispatch
			var dispatch = typeof(IDispatch).GUID;
			var folderView = (IShellFolderViewDual)shellBrowser.QueryActiveShellView().GetItemObject(SVGIO_BACKGROUND, ref dispatch);
			var shellDispatch = (IShellDispatch2)folderView.Application;

			// Use the dispatch (which is unelevated) to launch the process for us
			shellDispatch.ShellExecute(process, args, currentDirectory, string.Empty, SW_SHOWNORMAL);
		}

		/// <summary>
		/// Interop definitions
		/// </summary>
		private const int CSIDL_Desktop = 0;
		private const int SWC_DESKTOP = 8;
		private const int SWFO_NEEDDISPATCH = 1;
		private const int SW_SHOWNORMAL = 1;
		private const int SVGIO_BACKGROUND = 0;
		private readonly static Guid SID_STopLevelBrowser = new Guid("4C96BE40-915C-11CF-99D3-00AA004AE837");

		[ComImport]
		[Guid("9BA05972-F6A8-11CF-A442-00A0C90A8F39")]
		[ClassInterfaceAttribute(ClassInterfaceType.None)]
		private class CShellWindows
		{
		}

		[ComImport]
		[Guid("85CB6900-4D95-11CF-960C-0080C7F4EE85")]
		[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
		private interface IShellWindows
		{
			[return: MarshalAs(UnmanagedType.IDispatch)]
			object FindWindowSW([MarshalAs(UnmanagedType.Struct)] ref object pvarloc, [MarshalAs(UnmanagedType.Struct)] ref object pvarlocRoot, int swClass, out int pHWND, int swfwOptions);
		}

		[ComImport]
		[Guid("6d5140c1-7436-11ce-8034-00aa006009fa")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		private interface IServiceProvider
		{
			[return: MarshalAs(UnmanagedType.Interface)]
			object QueryService(ref Guid guidService, ref Guid riid);
		}

		[ComImport]
		[Guid("000214E2-0000-0000-C000-000000000046")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		private interface IShellBrowser
		{
			void VTableGap01(); // GetWindow
			void VTableGap02(); // ContextSensitiveHelp
			void VTableGap03(); // InsertMenusSB
			void VTableGap04(); // SetMenuSB
			void VTableGap05(); // RemoveMenusSB
			void VTableGap06(); // SetStatusTextSB
			void VTableGap07(); // EnableModelessSB
			void VTableGap08(); // TranslateAcceleratorSB
			void VTableGap09(); // BrowseObject
			void VTableGap10(); // GetViewStateStream
			void VTableGap11(); // GetControlWindow
			void VTableGap12(); // SendControlMsg
			IShellView QueryActiveShellView();
		}

		[ComImport]
		[Guid("000214E3-0000-0000-C000-000000000046")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		private interface IShellView
		{
			void VTableGap01(); // GetWindow
			void VTableGap02(); // ContextSensitiveHelp
			void VTableGap03(); // TranslateAcceleratorA
			void VTableGap04(); // EnableModeless
			void VTableGap05(); // UIActivate
			void VTableGap06(); // Refresh
			void VTableGap07(); // CreateViewWindow
			void VTableGap08(); // DestroyViewWindow
			void VTableGap09(); // GetCurrentInfo
			void VTableGap10(); // AddPropertySheetPages
			void VTableGap11(); // SaveViewState
			void VTableGap12(); // SelectItem

			[return: MarshalAs(UnmanagedType.Interface)]
			object GetItemObject(UInt32 aspectOfView, ref Guid riid);
		}

		[ComImport]
		[Guid("00020400-0000-0000-C000-000000000046")]
		[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
		private interface IDispatch
		{
		}

		[ComImport]
		[Guid("E7A1AF80-4D96-11CF-960C-0080C7F4EE85")]
		[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
		private interface IShellFolderViewDual
		{
			object Application { [return: MarshalAs(UnmanagedType.IDispatch)] get; }
		}

		[ComImport]
		[Guid("A4C6892C-3BA9-11D2-9DEA-00C04FB16162")]
		[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
		public interface IShellDispatch2
		{
			void ShellExecute([MarshalAs(UnmanagedType.BStr)] string File, [MarshalAs(UnmanagedType.Struct)] object vArgs, [MarshalAs(UnmanagedType.Struct)] object vDir, [MarshalAs(UnmanagedType.Struct)] object vOperation, [MarshalAs(UnmanagedType.Struct)] object vShow);
		}

		public static void ExecuteProcess(string filename, string parameters)
		{
			Process p = new();
			p.StartInfo.FileName = filename;
			p.StartInfo.Arguments = parameters;
			p.StartInfo.WorkingDirectory = Path.GetDirectoryName(filename);
			p.Start();
		}
	}
}
