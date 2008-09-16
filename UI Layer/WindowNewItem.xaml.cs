using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.IO;
using System.Data;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Win32;
using System.Windows.Markup;
using System.Data.EntityClient;

using eNursePHR.BusinessLayer;
using eNursePHR.BusinessLayer.PHR;

namespace eNursePHR.userInterfaceLayer
{
    /// <summary>
    /// Interaction logic for WindowNewItem.xaml
    /// </summary>
    public partial class WindowNewItem : Window
    {
        private Item newItem;
        private History newHistory;
        private bool itemSaved = false; // Allows multiple click on save, first creation, then update

        private bool _updateItem;
        public bool UpdateItem
        {
            get { return _updateItem; }
            set {
                if (value==true) {

                    this.Title="Update care plan item";
                _updateItem = true;
              
                } else
                    _updateItem = false;
            }
        }

        public Item CurrentItem
        {
            get { return newItem; }
            set { newItem = value; }
        }

        public WindowNewItem()
        {
            InitializeComponent();

            
        }

       
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (UpdateItem == false)
            {
               
                newItem = new Item();
                newHistory = new History();

                newItem.Id = Guid.NewGuid();
                CurrentItem = newItem;
                //spNewItemUpdate.Visibility = Visibility.Collapsed;
            }
            else
            {
                TextRange doc = new TextRange(rtbDescription.Document.ContentStart, rtbDescription.Document.ContentEnd);
                if (newItem.Description != null)
                {
                   // MemoryStream stream = new MemoryStream(ConvertHelper.convertToByteArray(newItem.Description));

                  
                    MemoryStream stream = new MemoryStream(newItem.Description);
                    
                    doc.Load(stream, DataFormats.XamlPackage);
                    //doc.Load(stream, DataFormats.Xaml);
                }
              
            }
          
            spNewItem.DataContext = CurrentItem;
         
        }

       

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            WindowMain wndMain = (WindowMain)App.Current.MainWindow;
               
            // Validate title
            BindingExpression be = tbTitle.GetBindingExpression(TextBox.TextProperty);
            be.UpdateSource();
            if (be.HasError)
            {
                tbTitle.ToolTip = be.ValidationError.ErrorContent.ToString();
                return;
            }

            TextRange doc = new TextRange(rtbDescription.Document.ContentStart,rtbDescription.Document.ContentEnd);
            MemoryStream stream = new MemoryStream();
           
            doc.Save(stream, DataFormats.XamlPackage);
          //  doc.Save(stream, DataFormats.Xaml);

//            newItem.Description = ConvertHelper.convertToString(stream.ToArray());

            newItem.Description = stream.ToArray();
       
           


            
              
            
                if (!itemSaved && UpdateItem == false)
                {
                    newHistory = History.CreateHistory(Guid.NewGuid(), DateTime.Now, System.Environment.UserName);
                    newItem.History = newHistory;
                    // Prevent Item_Association event handling - do not want to update yet
                    App.s_carePlan.ActiveCarePlan.Item.AssociationChanged -= new CollectionChangeEventHandler(wndMain.Item_AssociationChanged);
                    newItem.CarePlan = App.s_carePlan.ActiveCarePlan; // Will trigger Item_Association-changed event
                    //spNewHistory.Visibility = Visibility.Visible;
                    App.s_carePlan.ActiveCarePlan.Item.AssociationChanged += new CollectionChangeEventHandler(wndMain.Item_AssociationChanged);


                    //EntityCommand ecmd = (EntityCommand)App.carePlan.DB.Connection.CreateCommand();
                    //ecmd.Connection.Open();
                    //ecmd.CommandText = "INSERT INTO Item (Id,Title,Description) VALUES (@Id,@Title,@Description)";
                    //ecmd.Parameters.AddWithValue("Id", newItem.Id);
                    //ecmd.Parameters.AddWithValue("Title", newItem.Title);
                    //EntityParameter descriptionParam = ecmd.Parameters.Add("Description", DbType.String);
                    //descriptionParam.Value = ConvertHelper.convertToString(stream.ToArray());
                    //MessageBox.Show(ecmd.ToTraceString());
                    //ecmd.ExecuteNonQuery();
 
                    App.s_carePlan.DB.AddToItem(newItem);
                    App.s_carePlan.DB.AddToHistory(newHistory);
                }
                else
                {
                    newItem.History.UpdatedBy = System.Environment.UserName;
                    newItem.History.UpdatedDate = DateTime.Now;
                    //spNewItemUpdate.Visibility = Visibility.Visible;
                }

                if (((WindowMain)(App.Current.MainWindow)).SaveCarePlan() != -1)
                {

                        //spNewHistory.DataContext = CurrentItem.History;
                        sbInformation.Text = "Item saved.";
                        itemSaved = true;
                        if (!UpdateItem)
                           newItem.Tag.AssociationChanged += new CollectionChangeEventHandler(((WindowMain)App.Current.MainWindow).Tag_AssociationChanged);
                        wndMain.Item_AssociationChanged(this, null); // Manually refresh item-collection      
                }
           }

        //private void lbAttachments_Drop(object sender, DragEventArgs e)
        //{
        //    IDataObject data = e.Data;

        //        string[] filenames;

        //        string[] formats = data.GetFormats(false);

        //        lbAttachments.Items.Add("http://wolterskluwer.httpsvc.vitalstreamcdn.com/wolterskluwer_vitalstream_com/AJN/TRYTHIS_EP5_FULL_FINAL.wmv");

        //    if (data.GetDataPresent(DataFormats.UnicodeText))
        //       lbAttachments.Items.Add((string)data.GetData(DataFormats.UnicodeText));
        //    else if (data.GetDataPresent(DataFormats.FileDrop))
        //    {
        //        filenames = (string[]) data.GetData(DataFormats.FileDrop);
        //        foreach (string filename in filenames)
        //            lbAttachments.Items.Add(filename);
        //    }

         

        //    e.Handled = true;
        //}

        //private void lbAttachments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
         
        //    string source = (string)(sender as ListBox).SelectedItem;


        //    //// Try to show image
        //    //try
        //    //{
        //    //    BitmapImage img = new BitmapImage();
        //    //    BitmapSource imgsource = BitmapFrame.Create(new Uri(source));
        //    //    BitmapMetadata imgmeta = (BitmapMetadata)imgsource.Metadata;

        //    //    img.BeginInit();
        //    //    img.UriSource = new Uri(source);
        //    //    img.DecodePixelHeight = 64; // Tip; only choose width or height to conserve aspect ratio
        //    //    img.EndInit();
        //    //    imgShow.Source = img;
        //    //    imgShow.Visibility = Visibility.Visible;
        //    //    return;
        //    //}
        //    //catch (Exception ee)
        //    //{
        //    //}


        //    // Try to play video/audio

            
        //    try
        //    {
        //        imgShow.Visibility = Visibility.Collapsed;
        //        WindowVideoShow wVideoShow = new WindowVideoShow();

        //        wVideoShow.mediaShow.Source = new Uri(source);
        //        wVideoShow.mediaShow.Stretch = Stretch.Uniform;
        //        wVideoShow.mediaShow.Play();
        //        wVideoShow.Show();

        //    }
        //    catch (Exception ee)
        //    {
        //    }

            
        //}

        private void tbTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            updateSave();
          }

        private void rtbDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            updateSave();
         }

        private void updateSave()
        {
            
            sbInformation.Text = String.Empty;
        }

        private void btnInsertHyperlink_Click(object sender, RoutedEventArgs e)
        {
            TextPointer tp = rtbDescription.CaretPosition;
            
            WindowInsertHyperlink wndInsertHyperlink = new WindowInsertHyperlink();
            wndInsertHyperlink.ShowDialog();

            if (wndInsertHyperlink.WebTitle == null || wndInsertHyperlink.URL == null)
                return;

            Hyperlink hyperTest = new Hyperlink();
           
            try
            {
                hyperTest.NavigateUri = new Uri(wndInsertHyperlink.URL);
            }
            catch (System.UriFormatException)
            {
                MessageBox.Show("Invalid web-address, please enter a valid address");
                return;
            }

            
            Hyperlink hyper = new Hyperlink(new Run(wndInsertHyperlink.WebTitle), tp);
            hyper.NavigateUri = new Uri(wndInsertHyperlink.URL);
            hyper.ToolTip = wndInsertHyperlink.URL;


            //hyper.RequestNavigate += new System.Windows.Navigation.RequestNavigateEventHandler(hyper_RequestNavigate);
        }

        private void btnInsertImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fld = new OpenFileDialog();
            fld.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            fld.Multiselect = false; // Allow only one image
            fld.Title = "Choose a image to insert";

            bool? result = fld.ShowDialog();

            if (result == null || (bool) result == false)
                return;

            TextPointer tp = rtbDescription.CaretPosition;

            // Based on 
            // http://msdn2.microsoft.com/en-us/library/ms748873.aspx
            // accessed : 5 april 2008

            // Create Image Element
            Image myImage = new Image();
           
            // Create source
            BitmapImage myBitmapImage = new BitmapImage();

            // BitmapImage.UriSource must be in a BeginInit/EndInit block
            myBitmapImage.BeginInit();
            myBitmapImage.UriSource = new Uri(fld.FileName);
            //myBitmapImage.DecodePixelWidth = 200;
            myBitmapImage.EndInit();
            //set image source
            myImage.Source = myBitmapImage;

            BlockUIContainer bui = new BlockUIContainer(myImage);
            Floater floater = new Floater(bui, tp);
            floater.Width = 200;
           
        }

        //void hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        //{
        //    Process.Start(e.Uri.ToString());

        //    e.Handled = true;
        //}
    }
}













