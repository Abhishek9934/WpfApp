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
//using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using UserControl = System.Windows.Controls.UserControl;
using Path = System.IO.Path;
using System.Threading;
using System.Reflection;

namespace WpfApp1.Views
{
    /// <summary>
    /// Interaction logic for BlueView.xaml
    /// </summary>
    public partial class BlueView : UserControl
    {
        string menuItemName;
        string subMenuItemName;
        static string path = "ftp://shivam99:sp99wpfappftp@ftp.drivehq.com/";
        public List<string> toDelete;
        public BlueView(string s1,string s2)
        {
            menuItemName = s1;
            subMenuItemName = s2;
            toDelete = new List<string>();
            InitializeComponent();
            Console.WriteLine(menuItemName);
            Console.WriteLine(subMenuItemName);
            loadView();
           // downloadFile("Details.txt");
          
        }
   
        private void loadView()
        {
            List<string> imgList = ListDirectory("Images");
            foreach(string x in imgList)
            {
                Console.WriteLine(x);
            }
            //string str = "img";
            //for(int i = 0; i < 4; i++)
            //{
            //    char c = (char)(i + 49);
            //    GetValue(str + c);
            //}
            if(imgList.Count>0) img1.Source = getImage(imgList[0]);
            if (imgList.Count > 1) img2.Source = getImage(imgList[1]);
            if (imgList.Count > 2) img3.Source = getImage(imgList[2]);
            if (imgList.Count > 3) img4.Source = getImage(imgList[3]);
      
        }
        public BlueView()
        {
            InitializeComponent();
        }

        private BitmapImage getImage(string fileName)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(path + menuItemName + '/' + subMenuItemName + "/Images/"+fileName);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential("shivam99", "sp99wpfappftp");
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = responseStream;
            bi.EndInit();
            return bi;
        }

        private void downloadFile(String fileName)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(path + menuItemName + '/' + subMenuItemName + "/Text/"+fileName );
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential("shivam99", "sp99wpfappftp");
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            desc.Text = reader.ReadToEnd();
        }

        private void uploadFile()
        {
            
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://shivam99:sp99wpfappftp@ftp.drivehq.com/Text/up1.txt");
                request.Method = WebRequestMethods.Ftp.UploadFile;

                // This example assumes the FTP site uses anonymous logon.
                request.Credentials = new NetworkCredential("shivam99", "sp99wpfappftp");

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
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://shivam99:sp99wpfappftp@ftp.drivehq.com/up1.txt");
                request.Method = WebRequestMethods.Ftp.DeleteFile;

                // This example assumes the FTP site uses anonymous logon.
                request.Credentials = new NetworkCredential("shivam99", "sp99wpfappftp");
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    return response.StatusDescription;
                }
            }catch(Exception)
            {
                // do nothing
            }
            return null;
        }

        private List<string> filesListUtil(string filesList)
        {
            List<string> ans = new List<string>();
            string word="";
            for(int i = 0; i < filesList.Length; i++)
            {
                if (filesList[i] == ' ')
                {
                    if (word.Length == 5)
                    {
                        if (Char.IsDigit(word[0]) && Char.IsDigit(word[1]) && word[2] == ':')
                        {
                            word = "";
                            i++;
                            while (filesList[i] != '\n')
                            {
                                word += filesList[i];
                                i++;
                            }
                            ans.Add(word.Substring(0,word.Length-1));
                        }
                    }
                    word = "";
                }
                else
                {
                    word += filesList[i];
                }
            }
            return ans;
        }
        private List<string> ListDirectory(string str="")
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(path + menuItemName + '/' + subMenuItemName +'/' + str);
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

                // This example assumes the FTP site uses anonymous logon.
                request.Credentials = new NetworkCredential("shivam99", "sp99wpfappftp");

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                string filesList = reader.ReadToEnd();
                List<string> ans = filesListUtil(filesList);
                Console.WriteLine($"Directory List Complete, status {response.StatusDescription}");
                reader.Close();
                response.Close();
                return ans;
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        private void readFromDoc()
        {
            try
            {
                string temp = path + menuItemName + "/Coconut/details.docx";
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(temp);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential("shivam99", "sp99wpfappftp");
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                desc.Text = reader.ReadToEnd();
            }
            catch (Exception)
            {

            }
        }

        private void downloadVideo()
        {
            try
            {
                string temp = path + menuItemName + "/1. Coconut/Videos/coconut_video_1.mp4";
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(temp);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential("shivam99", "sp99wpfappftp");
                using (Stream ftpStream = request.GetResponse().GetResponseStream())
                using (Stream fileStream = File.Create(@"D:\coconut_video_1.mp4"))
                {
                    ftpStream.CopyTo(fileStream);
                }
                //vid1.Source = new Uri("D:/coconut_video_1.mp4");
                
                Process.Start("wmplayer.exe", "D:/coconut_video_1.mp4");

            }
            catch (Exception)
            {

            }
        }

        private void playVideo()
        {
            Thread thread = new Thread(new ThreadStart(downloadVideo));
            thread.Start();
            var wait = System.Windows.Forms.MessageBox.Show("Wait for the video to load");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
       //downloadFile();
          //listfiles();
            ListDirectory("/1. Coconut");
            //readFromDoc();
            /*uploadFile();
            //deleteFile();
            var myValue = (sender as Button).Content;
            Console.WriteLine(myValue);*/
        }

    }
}
