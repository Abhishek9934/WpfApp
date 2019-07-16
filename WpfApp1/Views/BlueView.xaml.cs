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

        List<Google.Apis.Drive.v3.Data.File> vidList;        //list for storing video files
        List<Google.Apis.Drive.v3.Data.File> imgList;        // list for storing image files
        static string path = @"C:\BioPack\";

        public BlueView()
        {
            InitializeComponent();
        }

        public BlueView(string s1,DriveService ds)
        {
            folderID = s1;
            InitializeComponent();
            service = ds;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            loadView();
        }
   
        private void loadView()                                                                 // Function for loading images ,videos and text in the blue view
        {
            List<Google.Apis.Drive.v3.Data.File> ls = FolderContent(folderID);
            foreach (Google.Apis.Drive.v3.Data.File x in ls)
            {
                if(x.Name == "Images")                                                          // for displaying images
                {
                    imgList = FolderContent(x.Id);
                    downloadImages();
                    Thread thread1 = new Thread(new ThreadStart(downloadImages));
                    thread1.Start();
                    if (imgList.Count > 0)
                    {
                        
                        this.Dispatcher.Invoke(() =>
                        {
                            foreach (Google.Apis.Drive.v3.Data.File f in imgList)
                            {
                                MenuItem menuItem = new MenuItem();
                                menuItem.Header = f.Name;
                                menuItem.Click += filePlayer;
                                imagesMenu.Items.Add(menuItem);
                            }
                        });
                    }
                }
                                    
                if (x.Name == "Text")                                                           // for displaying text
                {
                    List<Google.Apis.Drive.v3.Data.File> descList = FolderContent(x.Id);
                    if(descList.Count>0)desc.Text = getText(descList[0].Id);
                }

                if(x.Name == "Videos")                                                          // for displaying videos
                {
                    vidList = FolderContent(x.Id);
                    Thread thread = new Thread(new ThreadStart(downloadVideos));
                    thread.Start();
                    foreach (Google.Apis.Drive.v3.Data.File f in vidList)
                    {
                        MenuItem menuItem = new MenuItem();
                        menuItem.Header = f.Name;
                        menuItem.Click += filePlayer;
                        vlMenu.Items.Add(menuItem);
                    }
                }
            }
        }
        private void filePlayer(object sender, RoutedEventArgs e)               
        {
            System.Diagnostics.Process.Start(path + (sender as MenuItem).Header);                 // display image

        }
        private void downloadVideos()
        {
            foreach (Google.Apis.Drive.v3.Data.File f in vidList)
            {
                saveFile(f);
            }
        }

        private void downloadImages()                                                           
        {
            foreach (Google.Apis.Drive.v3.Data.File f in imgList)                                     
            {
                saveFile(f);
            }
        }
         

        private void saveFile(Google.Apis.Drive.v3.Data.File file)                                     // save file to  C drive of the computer
        {
            string fileID = file.Id, fileName = file.Name;
            if (System.IO.File.Exists(path+ fileName))
            {
                return;
            }
            var request = service.Files.Get(fileID);
            var stream = new System.IO.MemoryStream();
            request.Download(stream);
            stream.Seek(0, System.IO.SeekOrigin.Begin);
            using (Stream fileStream = System.IO.File.Create(path+fileName))
            {
                stream.CopyTo(fileStream);
            }
            Console.WriteLine(path + fileName);
        }
        

        private string getText(string fileID)                                                           //function to read text file from the drive
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

        private static List<Google.Apis.Drive.v3.Data.File> FolderContent(string folderID)              // To list all the files from the drive 
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
