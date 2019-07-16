using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Controls;
using System.Windows.Shapes;
using WpfApp1.ViewModels;
using WpfApp1.Views;
using System.Net;
using System.Diagnostics;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Threading;
using Google.Apis.Download;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
      
        static string[] Scopes = { DriveService.Scope.Drive };
        static string ApplicationName = "Drive API .NET SummerProject";
        static DriveService service;

        public string MyApplicationName { get; private set; }

        public MainWindow()
        {
            UserCredential credential;

            credential = GetCredentials();
            InitializeComponent();
          
                DataContext = new HomeView();
#if DEBUG
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
#endif

            //Main1();
            service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        private List<string> filesListUtil(string filesList)
        {
            List<string> ans = new List<string>();
            string word = "";
            for (int i = 0; i < filesList.Length; i++)
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
                            ans.Add(word.Substring(0, word.Length - 1));
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

        private void BlueView_Clicked(object sender, RoutedEventArgs e)
        {
            DataContext = new BlueView(getID((string)(sender as MenuItem).Header),service);
        }

        private void Print_Clicked(object sender, RoutedEventArgs e)
        {
           
        }

        private void HomeView_Clicked(object sender, RoutedEventArgs e)
        {
            DataContext = new HomeView();                                                                //load the home view
        }

        private void Edit_Content(object sender, RoutedEventArgs e)
        {
            var wait = System.Windows.Forms.MessageBox.Show("Login to upload/edit");
            System.Diagnostics.Process.Start("https://drive.google.com/drive/my-drive");                  //open the drive       
        }
        private void Open_Content(object sender, RoutedEventArgs e){                  
            var wait = System.Windows.Forms.MessageBox.Show("Login to open file");
            System.Diagnostics.Process.Start("https://drive.google.com/drive/my-drive");                 //open the drive       
        }
        private void Feedback_content(object sender, RoutedEventArgs e)                                
        {
            System.Diagnostics.Process.Start("https://docs.google.com/forms/d/e/1FAIpQLSdUPYRPHhvGDXGasUk1Xb1ztOe2kz1yH72lklx3Bkt-QJTaYw/viewform?usp=sf_link");                //load the feedback form

        }

        private void MenuItem_Clicked(object sender, RoutedEventArgs e)
        {
            if ((string)(sender as MenuItem).Tag == "unchecked")
            {
                string folderID = getID((string)(sender as System.Windows.Controls.MenuItem).Header);
                Console.WriteLine(folderID);
                List<string> list1 = FolderContent(service,folderID);
                list1.Sort();
                for (int i = 0; i < list1.Count; i++)
                {
                    MenuItem menuItem = new MenuItem(); menuItem.Tag = (sender as MenuItem).Header;
                    menuItem.Header = list1[i]; menuItem.Click += BlueView_Clicked;
                    (sender as MenuItem).Items.Add(menuItem);

                }
                (sender as MenuItem).Tag = "checked";
                MenuItem_Clicked(sender, e);
            }
        }

        private static string getID(string str)
        { // Define parameters of request.
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.Fields = "nextPageToken, files(id, name, owners, parents,mimeType)";
            string temp = "name = \'" + str + "\'";
            Console.WriteLine(temp);
            listRequest.Q = temp;
            var request = listRequest.Execute();
            return request.Files[0].Id;
        }

        private static List<string> FolderContent(DriveService service,string folderID)
        {
            // Define parameters of request.
            List<string> ans = new List<string>();
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.Fields = "nextPageToken, files(id, name, owners, parents,mimeType)";
            string temp = "\'" + folderID + "\' in parents";
            listRequest.Q = temp;

            var request = listRequest.Execute();
            if (request.Files != null && request.Files.Count > 0)
            {
                foreach (var file in request.Files)
                {
                    Console.WriteLine("{0}", file.Name);
                    Console.WriteLine("{0}", file.Id);
                    ans.Add(file.Name);
                }
            }
            else
            {
                Console.WriteLine("No files found.");
            }
            return ans;
        }


        private void ListFiles(DriveService service)
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
                }
              
            }
            else
            {
                Console.WriteLine("No files found.");

            }
       
        }

        private static void UploadFile(string path, DriveService service, string mType)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File();
            fileMetadata.Name = System.IO.Path.GetFileName(path);
            fileMetadata.MimeType = mType; ;
            FilesResource.CreateMediaUpload request;
            using (var stream = new System.IO.FileStream(path, System.IO.FileMode.Open))
            {
                request = service.Files.Create(fileMetadata, stream, mType);
                request.Fields = "id";
                request.Upload();
            }

            var file = request.ResponseBody;

            Console.WriteLine("File ID: " + file.Id);

        }

        private static UserCredential GetCredentials()                         // to get credentials
        {
            UserCredential credential;

            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

                credPath = System.IO.Path.Combine(credPath, ".credentials/drive-dotnet-quickstart.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                // Console.WriteLine("Credential file saved to: " + credPath);
            }

            return credential;
        }

        private static void CreateFolder(string folderName, DriveService service)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = folderName,
                MimeType = "application/vnd.google-apps.folder"
            };
            var request = service.Files.Create(fileMetadata);
            request.Fields = "id";
            var file = request.Execute();
            Console.WriteLine("Folder ID: " + file.Id);

        }

        private static void DownloadFile(DriveService service,string fileID)
        {
            var request = service.Files.Get(fileID);
           
            var stream = new System.IO.MemoryStream();
            request.MediaDownloader.ProgressChanged +=
                (IDownloadProgress progress) =>
                {
                    switch (progress.Status)
                    {
                        case DownloadStatus.Downloading:
                            {
                                Console.WriteLine(progress.BytesDownloaded);
                                break;
                            }
                        case DownloadStatus.Completed:
                            {
                                Console.WriteLine("Download complete.");
                                break;
                            }
                        case DownloadStatus.Failed:
                            {
                                Console.WriteLine("Download failed.");
                                break;
                            }
                    }
                };
            request.Download(stream);
            using (FileStream file = new FileStream("F:/img.jpg", FileMode.Create, FileAccess.Write))
            {
                stream.WriteTo(file);
                Console.WriteLine("File Downloaded successfully.");
            }
        }

        private void Close_Clicked(object sender, RoutedEventArgs e)                    // close app
        {
            this.Close();
        }

        private void list_Clicked(object sender, RoutedEventArgs e)  
        {
            DataContext = new WpfApp1.Views.ListView(service);                                // open the list view
        
        }

        private void About_Clicked(object sender, RoutedEventArgs e)
        {
            DataContext = new WpfApp1.Views.AboutView(getID((string)(sender as MenuItem).Header), service);

        }

    }
    
}

//pass = rNd8L7YV

// MimeTypes: "images/jpeg", "text/plain", "application/pdf", "application/vnd.google-apps.folder", "video/mp4", "audio/mpeg" , "image/png"
