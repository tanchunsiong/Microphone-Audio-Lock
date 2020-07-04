using System;
using System.Diagnostics;
using System.Windows.Forms;
using Herochun_Microsoft_Audio_Lock.Properties;
using System.Drawing;
using NAudio.CoreAudioApi;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Herochun_Microsoft_Audio_Lock
{
    /// <summary>
    /// 
    /// </summary>
    class ContextMenus
    {

        CancellationTokenSource _cancelationTokenSource;
        MMDeviceEnumerator enumerator;
        List<KeyValuePair<float, string>> MicrophoneLevels;
        List<KeyValuePair<float, string>> MicrophoneLevelsCopy;
        List<KeyValuePair<float, string>> MicrophoneLevelsMute;
        
        bool ifIsFirstRun = true;

     

        /// <summary>
        /// Is the About box displayed?
        /// </summary>
        bool isAboutLoaded = false;

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>ContextMenuStrip</returns>
        public ContextMenuStrip Create()
        {
            _cancelationTokenSource = new CancellationTokenSource();
            enumerator = new MMDeviceEnumerator();

            MicrophoneLevels = new List<KeyValuePair<float, string>>();
            MicrophoneLevelsMute = new List<KeyValuePair<float, string>>();
            MicrophoneLevelsCopy = new List<KeyValuePair<float, string>>();
           
            // Add the default menu options.
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem item;
            ToolStripSeparator sep;
            ToolStripButton btn;

            // Lock Unlock
            btn = new ToolStripButton();
            btn.Text = "Lock/Unlock Volume";
            btn.Click += new EventHandler(LockUnlock_Click);
            btn.CheckOnClick = true;
            btn.Image = Resources.Explorer;
            menu.Items.Add(btn);

            // Windows Explorer.
            item = new ToolStripMenuItem();
            item.Text = "Explorer";
            item.Click += new EventHandler(Explorer_Click);
            item.Image = Resources.Explorer;
            menu.Items.Add(item);

            // About.
            item = new ToolStripMenuItem();
            item.Text = "About";
            item.Click += new EventHandler(About_Click);
            item.Image = Resources.About;
            menu.Items.Add(item);

            // Separator.
            sep = new ToolStripSeparator();
            menu.Items.Add(sep);

            // Exit.
            item = new ToolStripMenuItem();
            item.Text = "Exit";
            item.Click += new System.EventHandler(Exit_Click);
            item.Image = Resources.Exit;
            menu.Items.Add(item);

            return menu;
        }
        public void SwapMuteUnMute() { 
            //not running
           
              MicrophoneLevels= MicrophoneLevelsMute;
              MicrophoneLevelsMute = MicrophoneLevelsCopy;
              MicrophoneLevelsCopy = MicrophoneLevels;
 

        }

        private void LockUnlock_Click(object sender, EventArgs e)
        {
            ToolStripButton btn = (ToolStripButton)sender;
            if (btn.Checked)
            {
                _cancelationTokenSource = new CancellationTokenSource();
                new Task(() => InfiniteLoop(), _cancelationTokenSource.Token, TaskCreationOptions.LongRunning).Start();
            }

            else if (btn.Checked == false)
            {
                _cancelationTokenSource.Cancel();

            }
            GC.Collect();
        }

        public async void InfiniteLoop()
        {
            MMDeviceCollection mmdc = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);

            if (ifIsFirstRun) {

                //add to the list
                foreach (MMDevice device in mmdc)
                {

                    string devicename = device.DeviceFriendlyName.Replace(" ", "").Replace("-", "").ToLower();
                    KeyValuePair<float, string> kvp = new KeyValuePair<float, string>(device.AudioEndpointVolume.MasterVolumeLevelScalar, devicename);
                    KeyValuePair<float, string> kvpmute = new KeyValuePair<float, string>(0, devicename);

                    MicrophoneLevels.Add(kvp);
                    MicrophoneLevelsMute.Add(kvpmute);
                    //}
                    //catch (Exception ex) { }
                }

                MicrophoneLevelsCopy = MicrophoneLevels;
                ifIsFirstRun = false;
            }


            while (!_cancelationTokenSource.Token.IsCancellationRequested)
            {
                int count = 0;
                foreach (MMDevice device in mmdc)
                {
                    //try
                    //{
                    //Debug.WriteLine("{0}, {1}, {2}", device.FriendlyName, device.State, device.AudioEndpointVolume.VolumeRange);
                    //Debug.WriteLine("level is {0}", device.AudioEndpointVolume.MasterVolumeLevelScalar);
                    await Task.Run(() =>
                    {
                        device.AudioEndpointVolume.MasterVolumeLevelScalar = MicrophoneLevels[count].Key;
                    });
                    //}
                    //catch (Exception ex) { }
                    count++;
                }
                GC.Collect();
                Thread.Sleep(100);
            }
        }



        /// <summary>
        /// Handles the Click event of the Explorer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Explorer_Click(object sender, EventArgs e)
        {
            Process.Start("explorer", null);
        }

        /// <summary>
        /// Handles the Click event of the About control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void About_Click(object sender, EventArgs e)
        {
            if (!isAboutLoaded)
            {
                isAboutLoaded = true;
                new AboutBox().ShowDialog();
                isAboutLoaded = false;
            }
        }

        /// <summary>
        /// Processes a menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Exit_Click(object sender, EventArgs e)
        {
            // Quit without further ado.
            Application.Exit();
        }
    }
}