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
using CCC.BusinessLayer;

namespace CCC.UI
{
    /// <summary>
    /// Interaction logic for ImageShow.xaml
    /// </summary>
    public partial class ImageShow : Window
    {
        public ImageShow()
        {
            InitializeComponent();
        }

        public void showImage(myImage img)
        {
            
           
          
           //this.WindowStyle = WindowStyle.None;
            

            BitmapImage imgFull = new BitmapImage();
            imgFull.BeginInit();
            imgFull.UriSource = new Uri(img.FileName);
            imgFull.EndInit();
            imgShow.Source = imgFull;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
 scrollImage.MaxWidth =  System.Windows.SystemParameters.PrimaryScreenWidth;
           scrollImage.MaxHeight =  System.Windows.SystemParameters.PrimaryScreenHeight-70;

        }

        private void miSvamp_Click(object sender, RoutedEventArgs e)
        {
            inkImage.EditingMode = InkCanvasEditingMode.EraseByStroke;
        }

        private void miPenn_Click(object sender, RoutedEventArgs e)
        {
            inkImage.EditingMode = InkCanvasEditingMode.Ink;
        }
        
    }
}
