using System;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Windows.Media;

namespace VideoScreensaver
{
    /// <summary>
    /// Interaction logic for Screensaver.xaml
    /// </summary>
    public partial class ScreensaverWindow : Window
    {
        private const string PositionFile = "video_position.txt";

        private TimeSpan position = TimeSpan.FromSeconds(0);

        public ScreensaverWindow()
        {
            InitializeComponent();

            var options = Config.ReadConfig();
            ScreensaverVideo.Source = new Uri(options.VideoPath);
            ScreensaverVideo.Volume = options.Volume;

            // 加载上次保存的播放位置
            if (File.Exists(PositionFile))
            {
                try
                {
                    var positionText = File.ReadAllText(PositionFile);
                    if (TimeSpan.TryParse(positionText, out var savedPosition))
                    {
                        position = savedPosition;
                    }
                }
                catch
                {
                }
            }
        }
        private void PlayVideo(object sender, RoutedEventArgs e)
        {
            ScreensaverVideo.Play();
            ScreensaverVideo.Position = position;
        }

        private void PlayVideoFromBeginning(object sender, RoutedEventArgs e)
        {
            ScreensaverVideo.Position = new TimeSpan(0, 0, 0);
            ScreensaverVideo.Play();
        }

        private Point mouseLocation;
        private bool InitialMouseSet = true;
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (InitialMouseSet)
            {
                mouseLocation = e.GetPosition(this);
                InitialMouseSet = false;
            }
            else
            {
                if (Math.Abs(mouseLocation.X - e.GetPosition(this).X) > 5 ||
                    Math.Abs(mouseLocation.Y - e.GetPosition(this).Y) > 5)
                {
                    SaveCurrentPosition();
                    Application.Current.Shutdown();
                }

                mouseLocation = e.GetPosition(this);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SaveCurrentPosition();
            Application.Current.Shutdown();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            SaveCurrentPosition();
            Application.Current.Shutdown();
        }

        private void SaveCurrentPosition()
        {
            try
            {
                // 保存当前播放位置
                var currentPosition = ScreensaverVideo.Position;
                File.WriteAllText(PositionFile, currentPosition.ToString());
            }
            catch
            {
                // 忽略保存失败的情况
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            SaveCurrentPosition();
        }
    }
}