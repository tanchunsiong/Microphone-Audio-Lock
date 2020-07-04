using System;
using System.Diagnostics;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;
using Herochun_Microsoft_Audio_Lock.Properties;

namespace Herochun_Microsoft_Audio_Lock
{
	/// <summary>
	/// 
	/// </summary>
	class ProcessIcon : IDisposable
	{
		/// <summary>
		/// MouseKey Hook Object
		/// </summary>
		private IKeyboardMouseEvents m_Events;

		private ContextMenus contextmenuinstance;

		/// <summary>
		/// The NotifyIcon object.
		/// </summary>
		NotifyIcon ni;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessIcon"/> class.
		/// </summary>
		public ProcessIcon()
		{
			// Instantiate the NotifyIcon object.
			ni = new NotifyIcon();
			contextmenuinstance = new ContextMenus();
			SubscribeGlobal();
		}

		/// <summary>
		/// Displays the icon in the system tray.
		/// </summary>
		public void Display()
		{
			// Put the icon in the system tray and allow it react to mouse clicks.			
			ni.MouseClick += new MouseEventHandler(ni_MouseClick);
			ni.Icon = Resources.SystemTrayApp;
			ni.Text = "System Tray Utility Application Demonstration Program";
			ni.Visible = true;

			// Attach a context menu.
			ni.ContextMenuStrip =  contextmenuinstance.Create();
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		public void Dispose()
		{
			// When the application closes, this will remove the icon from the system tray immediately.
			ni.Dispose();
		}

		/// <summary>
		/// Handles the MouseClick event of the ni control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
		void ni_MouseClick(object sender, MouseEventArgs e)
		{
			// Handle mouse button clicks.
			if (e.Button == MouseButtons.Left)
			{
				// Start Windows Explorer.
				Process.Start("explorer", null);
			}
		}
		/// <summary>
		/// Helper Class for MouseKey Hook
		/// </summary>
		private void SubscribeGlobal()
		{
			Unsubscribe();
			Subscribe(Hook.GlobalEvents());
		}

		private void Subscribe(IKeyboardMouseEvents events)
		{
			m_Events = events;
			m_Events.KeyDown += OnKeyDown;
			m_Events.KeyUp += OnKeyUp;
			m_Events.KeyPress += HookManager_KeyPress;

			m_Events.MouseUp += OnMouseUp;
			m_Events.MouseClick += OnMouseClick;
			m_Events.MouseDoubleClick += OnMouseDoubleClick;

			//m_Events.MouseMove += HookManager_MouseMove;

			//m_Events.MouseDragStarted += OnMouseDragStarted;
			//m_Events.MouseDragFinished += OnMouseDragFinished;

		
			//m_Events.MouseWheel += HookManager_MouseWheel;

		
			m_Events.MouseDown += OnMouseDown;
		}

		private void Unsubscribe()
		{
			if (m_Events == null) return;
			m_Events.KeyDown -= OnKeyDown;
			m_Events.KeyUp -= OnKeyUp;
			m_Events.KeyPress -= HookManager_KeyPress;

			m_Events.MouseUp -= OnMouseUp;
			m_Events.MouseClick -= OnMouseClick;
			m_Events.MouseDoubleClick -= OnMouseDoubleClick;

			m_Events.MouseMove -= HookManager_MouseMove;

			m_Events.MouseDragStarted -= OnMouseDragStarted;
			m_Events.MouseDragFinished -= OnMouseDragFinished;

		

			m_Events.Dispose();
			m_Events = null;
		}
	
		private void OnKeyDown(object sender, KeyEventArgs e)
		{
		if(e.KeyCode == Keys.F20 || e.KeyCode == Keys.NumPad0) {
				contextmenuinstance.SwapMuteUnMute();
			}
			Log(string.Format("KeyDown  \t\t {0}\n", e.KeyCode));
		}

		private void OnKeyUp(object sender, KeyEventArgs e)
		{
			Log(string.Format("KeyUp  \t\t {0}\n", e.KeyCode));
		}

		private void HookManager_KeyPress(object sender, KeyPressEventArgs e)
		{
			Log(string.Format("KeyPress \t\t {0}\n", e.KeyChar));
		}

		private void HookManager_MouseMove(object sender, MouseEventArgs e)
		{
			Log(string.Format("x={0:0000}; y={1:0000}", e.X, e.Y));
		}

		private void OnMouseDown(object sender, MouseEventArgs e)
		{
			Log(string.Format("MouseDown \t\t {0}\n", e.Button));
		}

		private void OnMouseUp(object sender, MouseEventArgs e)
		{
			Log(string.Format("MouseUp \t\t {0}\n", e.Button));
		}

		private void OnMouseClick(object sender, MouseEventArgs e)
		{
			Log(string.Format("MouseClick \t\t {0}\n", e.Button));
		}

		private void OnMouseDoubleClick(object sender, MouseEventArgs e)
		{
			Log(string.Format("MouseDoubleClick \t\t {0}\n", e.Button));
		}

		private void OnMouseDragStarted(object sender, MouseEventArgs e)
		{
			Log("MouseDragStarted\n");
		}

		private void OnMouseDragFinished(object sender, MouseEventArgs e)
		{
			Log("MouseDragFinished\n");
		}

		private void HookManager_MouseWheel(object sender, MouseEventArgs e)
		{
			Log(string.Format("Wheel={0:000}", e.Delta));
		}

		private void HookManager_MouseWheelExt(object sender, MouseEventExtArgs e)
		{
			Log(string.Format("Wheel={0:000}", e.Delta));
			Log("Mouse Wheel Move Suppressed.\n");
			e.Handled = true;
		}

		private void Log(string text)
		{

			//Console.WriteLine(text);
		}

	}
}