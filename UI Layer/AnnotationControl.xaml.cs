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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Annotations;
using System.Xml;
using eNursePHR.BusinessLayer;

namespace eNursePHR.userInterfaceLayer
{
    /// <summary>
    /// Interaction logic for AnnotationControl.xaml
    /// </summary>
    public partial class AnnotationControl : UserControl
    {
        public AnnotationControl()
        {
            InitializeComponent();
        }

        private void lvAnnotation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


            //Annotation selAnnotation = ((sender as ListBox).SelectedItem) as Annotation;
            //if (selAnnotation == null)
            //    return;

            //// Suggest information/reason for code in CCC framework
            //if (this.getContentType(selAnnotation) == ContentType.Diagnostic)
            //    tbReasonDiagnosis.Text = this.getText(selAnnotation);
            //else

            //    if (this.getContentType(selAnnotation) == ContentType.Interventional)
            //    tbReasonIntervention.Text = this.getText(selAnnotation);


        }

        public string getAnnotationContent(Annotation annotation)
        {
            if (annotation.Anchors[0].Contents.Count == 0)
                return String.Empty;


            XmlDocument xdoc = new XmlDocument();

            xdoc.LoadXml(annotation.Anchors[0].Contents[0].OuterXml);

            XmlNode xe = xdoc.SelectSingleNode("eNursePHR/Statement[1]");
            return xe.Attributes["Content"].Value;

        }


        /// <summary>
        /// This method takes an annotation and reads the extended XML-element
        /// eNursePHR/Statement attribute ContentType
        /// <eNursePHR>
        ///     <Statement ContentType="?"></Statement>
        /// </eNursePHR>
        /// </summary>
        /// <param name="annotation"></param>
        /// <returns></returns>
        public ContentType getContentType(Annotation annotation)
        {

            // Check if annotation contains extended XML-information
            if (annotation.Anchors[0].Contents.Count == 0)
                return ContentType.Null;

            // Load extended XML-info.
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(annotation.Anchors[0].Contents[0].OuterXml);

            // Query for content type
            XmlNode xe = xdoc.SelectSingleNode("eNursePHR/Statement[1]"); // XPath-query
            string contentType = xe.Attributes["ContentType"].Value;

            ContentType cType = new ContentType();
            switch (contentType)
            {
                case "Diagnostic": cType = ContentType.Diagnostic; break;
                case "Interventional": cType = ContentType.Interventional; break;
                case "Disease": cType = ContentType.Disease; break;
                case "Medication": cType = ContentType.Medication; break;
            }

            return cType;
        }


        public string getAquiredDiagnosticInfo()
        {
            Annotation selAnnotation = lvAnnotation.SelectedItem as Annotation;
            if (selAnnotation == null)
            {
                MessageBox.Show("No aquired diagnostic information selected, try selecting a diagnostic statement", "No acquired diagnostic information", MessageBoxButton.OK, MessageBoxImage.Information);
                return null;
            }

            if (getContentType(selAnnotation) == ContentType.Diagnostic)
                return firstCharToUpper(getAnnotationContent(selAnnotation));

            else
            {
                MessageBox.Show("Please select diagnostic information", "Select diagnostic information", MessageBoxButton.OK, MessageBoxImage.Information);
                return null;
            }
        }


        public string getAquiredInterventionalInfo()
        {

            Annotation selAnnotation = lvAnnotation.SelectedItem as Annotation;
            if (selAnnotation == null)
            {
                MessageBox.Show("No aquired interventional or medicational information selected", "No acquired interventional or medicational information", MessageBoxButton.OK, MessageBoxImage.Information);
                return null;
            }

            ContentType contentType = getContentType(selAnnotation);
            if (contentType == ContentType.Interventional || contentType == ContentType.Medication)
                return firstCharToUpper(getAnnotationContent(selAnnotation));
            else
            {
                MessageBox.Show("Please select interventional or medicational information", "Select interventional or medicational information", MessageBoxButton.OK, MessageBoxImage.Information);
                return null;
            }
        }
        /// <summary>
        /// Convert the first character to uppercase
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private string firstCharToUpper(string content)
        {
            string firstChar = content.Substring(0, 1).ToUpperInvariant();
            return content.Remove(0, 1).Insert(0, firstChar);
        }
       


    }
}
