using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;

namespace WpfApp1.Views
{
    /// <summary>
    /// Interaction logic for BlueView.xaml
    /// </summary>
    public partial class BlueView : UserControl
    {
        public BlueView()
        {
            InitializeComponent();
            img2.Source = getImage();
            img1.Source = getImage();
        }

        private BitmapImage getImage()
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://192.168.1.2/Fruit shapes.jpg");
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential("microfluidics lab", "workstation");
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = responseStream;
            bi.EndInit();
            return bi;
        }

        private void uploadFile()
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://192.168.1.2/up.txt");
            request.Method = WebRequestMethods.Ftp.UploadFile;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential("microfluidics lab", "workstation");

            // Copy the contents of the file to the request stream.
            byte[] fileContents;
            using (StreamReader sourceStream = new StreamReader("C:/Users/Microfluidics Lab/Desktop/up.txt"))
            {
                fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
            }

            request.ContentLength = fileContents.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(fileContents, 0, fileContents.Length);
            }
        }

        private string deleteFile()
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://192.168.1.2/up.txt");
            request.Method = WebRequestMethods.Ftp.DeleteFile;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential("microfluidics lab", "workstation");
            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                return response.StatusDescription;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            uploadFile();
        }
    }
}
