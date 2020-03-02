using System;
using System.Net.Mime;
using System.Windows.Forms;
using System.Drawing;
using RemoteService.Properties;

namespace RemoteService
{
    public class TrayController : ApplicationContext
    {
        private NotifyIcon trayIcon;

        public NotifyIcon TrayIcon => trayIcon;

        private MCRemoteService service;
        public TrayController(MCRemoteService srv)
        {
            service = srv;
            // Initialize Tray Icon
            trayIcon = new NotifyIcon()
            {
                Icon = Icon.FromHandle(Resources.AppIcon.GetHicon()),
                ContextMenu = new ContextMenu(new[]
                {
                    new MenuItem("E&xit", Exit),
                    new MenuItem("Run&Game", RunGame),
                    new MenuItem("Open&Settings", OpenSettings),
                    new MenuItem("&ReloadSettings", ReloadSettings),
                }),
                Visible = true,
                Text = "MCRemote"
            };
        }

        private void ReloadSettings(object sender, EventArgs e) => service.ReloadOption();

        void RunGame(object sender, EventArgs e) => service.RunByUI();

        void OpenSettings(object sender, EventArgs e) => service.OpenSettingByUI();

        public void Exit(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            trayIcon.Visible = false;
            service.ShutdownByUI();
        }
        protected override void Dispose( bool disposing )
        {
            // Clean up any components being used.
            if( disposing )
                base.Dispose( disposing );
        }
    }
}