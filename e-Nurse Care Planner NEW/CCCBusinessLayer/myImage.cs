using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace CCC.BusinessLayer
{

    public class myImage
    {
        private BitmapImage _img;

        public BitmapMetadata _meta;

        public BitmapImage Img
        {
            get { return this._img; }
            set { this._img = value; }
        }

        private string _fileName;

        public string FileName
        {
            get { return this._fileName; }
            set { this._fileName = value; }
        }

        private string _description;

        public string Description
        {
            get { return this._description; }
            set { this._description = value; }
        }

        public  BitmapMetadata Meta
        {
            get { return this._meta; }
            set { this._meta = value; }
        }


        string _camera;
        public string Camera
        {
            get { return this._camera; }
            set { this._camera = value; }

        }

        string _dateTaken;
        public string DateTaken
        {
            get { return this._dateTaken; }
            set { this._dateTaken = value; }
        }
    }

}
