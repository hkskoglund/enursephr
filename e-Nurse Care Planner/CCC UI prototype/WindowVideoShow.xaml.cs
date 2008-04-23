using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CCC.UI
{
    /// <summary>
    /// Interaction logic for WindowVideoShow.xaml
    /// </summary>
    public partial class WindowVideoShow : Window
    {
        public WindowVideoShow()
        {
            InitializeComponent();
        }

        #region Context menu
        private void miPlay_Click(object sender, RoutedEventArgs e)
        {
            mediaShow.Play();
           
        }

        private void miPause_Click(object sender, RoutedEventArgs e)
        {
            mediaShow.Pause();
        }

        private void miStop_Click(object sender, RoutedEventArgs e)
        {
            mediaShow.Stop();
        }

       
        private void miMute_Click(object sender, RoutedEventArgs e)
        {
            if (mediaShow.IsMuted)
            {
                miMute.Header = "Turn on audio";
                mediaShow.IsMuted = true;
            }
            else
            {
                miMute.Header = "Mute";
                mediaShow.IsMuted = false;
            }


        }
#endregion

        #region Media event handling
        private void mediaShow_BufferingStarted(object sender, RoutedEventArgs e)
        {
            tbStatus.Text = "Buffering....";
            tbDuration.Text = mediaShow.NaturalDuration.ToString();
            mediaShow.Height = mediaShow.NaturalVideoHeight;
            mediaShow.Width = mediaShow.NaturalVideoWidth;
        }

        private void mediaShow_MediaOpened(object sender, RoutedEventArgs e)
        {
            tbStatus.Text = "Media opened";
        }

        private void mediaShow_MediaEnded(object sender, RoutedEventArgs e)
        {
            tbStatus.Text = "Media ended";
        }

        private void mediaShow_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            tbStatus.Text = "Media failed";
        }
        #endregion


    }
}
