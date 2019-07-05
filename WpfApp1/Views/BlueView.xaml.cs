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


using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.Download;

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
        static string folderID;
        static DriveService service;

        List<Google.Apis.Drive.v3.Data.File> vidList;

        public BlueView()
        {
            InitializeComponent();
        }

        public BlueView(string s1,DriveService ds)
        {
            folderID = s1;
            //Console.WriteLine("FolderId = "+s1);
            InitializeComponent();
            service = ds;
            loadView();
            //webBrowser1.Navigate(new Uri("https://www.youtube.com/"));
        }
   
        private void loadView()
        {
            List<Google.Apis.Drive.v3.Data.File> ls = FolderContent(folderID);
            foreach (Google.Apis.Drive.v3.Data.File x in ls)
            {
                if(x.Name == "Images")
                {
                    List<Google.Apis.Drive.v3.Data.File> imgList = FolderContent(x.Id);
                    if (imgList.Count > 0)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            img1.Source = getImage(imgList[0].Id);
                        });
                    }
                    if (imgList.Count > 1)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            img2.Source = getImage(imgList[1].Id);
                        });
                    }
                    if (imgList.Count > 2)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            img3.Source = getImage(imgList[2].Id);
                        });
                    }
                    if (imgList.Count > 3)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            img4.Source = getImage(imgList[3].Id);
                        });
                    }
                }

                if (x.Name == "Text")
                {
                    List<Google.Apis.Drive.v3.Data.File> descList = FolderContent(x.Id);
                    if(descList.Count>0)desc.Text = getText(descList[0].Id);
                }

                if(x.Name == "Videos")
                {
                    vidList = FolderContent(x.Id);
                    Thread thread = new Thread(new ThreadStart(downloadVideos));
                    thread.Start();
                }
            }
        }
        
        private void downloadVideos()
        {
            foreach (Google.Apis.Drive.v3.Data.File f in vidList)
            {
                    getVideo(f);
            }
        }
        private void getVideo(Google.Apis.Drive.v3.Data.File file)
        {
            string fileID = file.Id, fileName = file.Name;
            var request = service.Files.Get(fileID);
            var stream = new System.IO.MemoryStream();
            request.Download(stream);
            stream.Seek(0, System.IO.SeekOrigin.Begin);
            using (Stream fileStream = System.IO.File.Create(@"F:\"+fileName))
            {
                stream.CopyTo(fileStream);
            }
        }

        private string getText(string fileID)
        {
            var request = service.Files.Get(fileID);

            var stream = new System.IO.MemoryStream();
            request.Download(stream);

            stream.Seek(0, System.IO.SeekOrigin.Begin);
            StreamReader streamReader = new StreamReader(stream);
            return streamReader.ReadToEnd();
        }

        private BitmapImage getImage(string fileID)
        {
            var request = service.Files.Get(fileID);

            var stream = new System.IO.MemoryStream();
            request.Download(stream);

            stream.Seek(0, System.IO.SeekOrigin.Begin);

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            return bitmap;
        }

        private static List<Google.Apis.Drive.v3.Data.File> FolderContent(string folderID)
        {
            // Define parameters of request.
            List<Google.Apis.Drive.v3.Data.File> ans = new List<Google.Apis.Drive.v3.Data.File>();
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.Fields = "nextPageToken, files(id, name, owners, parents,mimeType)";
            string temp = "\'" + folderID + "\' in parents";
            listRequest.Q = temp;

            var request = listRequest.Execute();
            if (request.Files != null && request.Files.Count > 0)
            {
                foreach (var file in request.Files)
                {
                    ans.Add(file);
                }
            }
            else
            {
                Console.WriteLine("No files found.");
            }
            return ans;
        }

        private static void ListFiles(DriveService service)
        {
            // Define parameters of request.
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.Fields = "nextPageToken, files(id, name, owners, parents,mimeType)";
            //listRequest.Q = "mimeType='image/jpeg'";

            // List files.
            var request = listRequest.Execute();


            if (request.Files != null && request.Files.Count > 0)
            {
                foreach (var file in request.Files)
                {
                    Console.WriteLine("{0}", file.Name);

                    if (file.Parents.Count > 0) foreach (string x in file.Parents)
                        {
                            Console.Write(x + " ");
                        }
                    Console.WriteLine();
                }

            }
            else
            {
                Console.WriteLine("No files found.");
            }
        }

        

    }
}

//<MediaElement x:Name ="vid1" Grid.Row="1" Grid.Column="0" Grid.ColumnSpan="1" Margin="25,25,25,25"/>
//            <MediaElement Grid.Row="1" Grid.Column= "1" Grid.ColumnSpan= "1" Margin= "25,25,25,25" />

//private void downloadFile(String fileName)
//{
//    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(path + menuItemName + '/' + subMenuItemName + "/Text/" + fileName);
//    request.Method = WebRequestMethods.Ftp.DownloadFile;
//    request.Credentials = new NetworkCredential("shivam99", "sp99wpfappftp");
//    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
//    Stream responseStream = response.GetResponseStream();
//    StreamReader reader = new StreamReader(responseStream);
//    desc.Text = reader.ReadToEnd();
//}

//private void uploadFile()
//{

//    FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://shivam99:sp99wpfappftp@ftp.drivehq.com/Text/up1.txt");
//    request.Method = WebRequestMethods.Ftp.UploadFile;

//    // This example assumes the FTP site uses anonymous logon.
//    request.Credentials = new NetworkCredential("shivam99", "sp99wpfappftp");

//    // Copy the contents of the file to the request stream.
//    byte[] fileContents;
//    using (StreamReader sourceStream = new StreamReader("C:/Users/Microfluidics Lab/Desktop/up.txt"))
//    {
//        fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
//    }

//    request.ContentLength = fileContents.Length;

//    using (Stream requestStream = request.GetRequestStream())
//    {
//        requestStream.Write(fileContents, 0, fileContents.Length);
//    }

//}

//private string deleteFile()
//{
//    try
//    {
//        FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://shivam99:sp99wpfappftp@ftp.drivehq.com/up1.txt");
//        request.Method = WebRequestMethods.Ftp.DeleteFile;

//        // This example assumes the FTP site uses anonymous logon.
//        request.Credentials = new NetworkCredential("shivam99", "sp99wpfappftp");
//        using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
//        {
//            return response.StatusDescription;
//        }
//    }
//    catch (Exception)
//    {
//        // do nothing
//    }
//    return null;
//}

//private List<string> filesListUtil(string filesList)
//{
//    List<string> ans = new List<string>();
//    string word = "";
//    for (int i = 0; i < filesList.Length; i++)
//    {
//        if (filesList[i] == ' ')
//        {
//            if (word.Length == 5)
//            {
//                if (Char.IsDigit(word[0]) && Char.IsDigit(word[1]) && word[2] == ':')
//                {
//                    word = "";
//                    i++;
//                    while (filesList[i] != '\n')
//                    {
//                        word += filesList[i];
//                        i++;
//                    }
//                    ans.Add(word.Substring(0, word.Length - 1));
//                }
//            }
//            word = "";
//        }
//        else
//        {
//            word += filesList[i];
//        }
//    }
//    return ans;
//}
//private List<string> ListDirectory(string str = "")
//{
//    try
//    {
//        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(path + menuItemName + '/' + subMenuItemName + '/' + str);
//        request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

//        // This example assumes the FTP site uses anonymous logon.
//        request.Credentials = new NetworkCredential("shivam99", "sp99wpfappftp");

//        FtpWebResponse response = (FtpWebResponse)request.GetResponse();

//        Stream responseStream = response.GetResponseStream();
//        StreamReader reader = new StreamReader(responseStream);
//        string filesList = reader.ReadToEnd();
//        List<string> ans = filesListUtil(filesList);
//        Console.WriteLine($"Directory List Complete, status {response.StatusDescription}");
//        reader.Close();
//        response.Close();
//        return ans;
//    }
//    catch (Exception)
//    {
//        return new List<string>();
//    }
//}

//private void readFromDoc()
//{
//    try
//    {
//        string temp = path + menuItemName + "/Coconut/details.docx";
//        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(temp);
//        request.Method = WebRequestMethods.Ftp.DownloadFile;
//        request.Credentials = new NetworkCredential("shivam99", "sp99wpfappftp");
//        FtpWebResponse response = (FtpWebResponse)request.GetResponse();
//        Stream responseStream = response.GetResponseStream();
//        StreamReader reader = new StreamReader(responseStream);
//        desc.Text = reader.ReadToEnd();
//    }
//    catch (Exception)
//    {

//    }
//}

//private void downloadVideo()
//{
//    try
//    {
//        string temp = path + menuItemName + "/1. Coconut/Videos/coconut_video_1.mp4";
//        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(temp);
//        request.Method = WebRequestMethods.Ftp.DownloadFile;
//        request.Credentials = new NetworkCredential("shivam99", "sp99wpfappftp");
//        using (Stream ftpStream = request.GetResponse().GetResponseStream())
//        using (Stream fileStream = System.IO.File.Create(@"D:\coconut_video_1.mp4"))
//        {
//            ftpStream.CopyTo(fileStream);
//        }
//        //vid1.Source = new Uri("D:/coconut_video_1.mp4");

//        Process.Start("wmplayer.exe", "D:/coconut_video_1.mp4");

//    }
//    catch (Exception)
//    {

//    }
//}

//private void playVideo()
//{
//    Thread thread = new Thread(new ThreadStart(downloadVideo));
//    thread.Start();
//    var wait = System.Windows.Forms.MessageBox.Show("Wait for the video to load");
//}
