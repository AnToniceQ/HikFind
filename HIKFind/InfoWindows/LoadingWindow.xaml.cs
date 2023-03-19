using HIKFind.Search.Webscraper;
using System;
using System.Windows;
using System.Windows.Threading;

namespace HIKFind.InfoWindows
{
    /// <summary>
    /// Interakční logika pro LoadingWindow.xaml
    /// </summary>
    public partial class LoadingWindow : Window
    {
        public string Text { set { this.InfoText.Text = value; } }

        public LoadingWindow(string header, string startText)
        {
            InitializeComponent();
            this.HeaderText.Text = header;
            this.InfoText.Text = startText;
            this.Show();
        }

        public void LoadingFinished()
        {
            int time = 0;

            DispatcherTimer timer = new DispatcherTimer();
            TimeSpan interval = new TimeSpan(0, 0, 1);
            int count = 3;

            this.CloseButton.Content = "Zavřít";

            timer.Interval = interval;
            timer.Tick += (_, a) =>
            {
                this.Text = "Hotovo! Automatické zavření za: " + time;
                if (count-- == 0)
                {
                    timer.Stop();
                    this.Close();
                }
                else
                    time = count;
            };
            time = count;
            timer.Start();


        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            WebScraper.StopDownload();
            this.Close();
        }
    }
}
