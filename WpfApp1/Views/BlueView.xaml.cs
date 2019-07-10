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
                            foreach (Google.Apis.Drive.v3.Data.File f in imgList)
                            {
                                MenuItem menuItem = new MenuItem();
                                menuItem.Header = f.Name;
                                menuItem.Click += imagePlayer;
                                imagesMenu.Items.Add(menuItem);
                            }
                           // img1.Source = getImage(imgList[0].Id); txt1.Text=service.Files.Get(imgList[0].Id).Execute().Name;
                        });
                    }
                /*    if (imgList.Count > 1)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            img2.Source = getImage(imgList[1].Id); txt2.Text = service.Files.Get(imgList[1].Id).Execute().Name;
                        });
                    }
                    if (imgList.Count > 2)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            img3.Source = getImage(imgList[2].Id); txt3.Text = service.Files.Get(imgList[2].Id).Execute().Name;
                        });
                    }
                    if (imgList.Count > 3)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            img4.Source = getImage(imgList[3].Id); txt4.Text = service.Files.Get(imgList[3].Id).Execute().Name;
                        });
                    }*/
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
                    foreach (Google.Apis.Drive.v3.Data.File f in vidList)
                    {
                        MenuItem menuItem = new MenuItem();
                        menuItem.Header = f.Name;
                        menuItem.Click += videoPlayer;
                        vlMenu.Items.Add(menuItem);
                    }
                }
            }
        }
        private void imagePlayer(object sender, RoutedEventArgs e)
        {
            Process.Start("photoviewer.exe", "F:\\" + (sender as MenuItem).Header);

        }
        private void downloadVideos()
        {
            foreach (Google.Apis.Drive.v3.Data.File f in vidList)
            {
                getVideo(f);
            }
        }

        private void videoPlayer(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", "F:\\" + (sender as MenuItem).Header);
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
            Console.WriteLine("F:/" + fileName);
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.Fields = "nextPageToken, files(id, name, owners, parents,mimeType)";
            listRequest.Q = "fullText contains 'husk'";
            var request = listRequest.Execute();
            if (request.Files != null && request.Files.Count > 0)
            {
                foreach (var file in request.Files)
                {
                    Console.WriteLine("{0}", file.Name);
                }

            }
            else
            {
                Console.WriteLine("No files found.");
            }
        }
       
    }
}
