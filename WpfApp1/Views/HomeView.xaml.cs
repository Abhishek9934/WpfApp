using System;
using System.Collections.Generic;
using System.Linq;
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

using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Threading;
using Google.Apis.Download;
using System.IO;

namespace WpfApp1.Views
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    /// 
    
    public partial class HomeView : UserControl
    {
        static string[] Scopes = { DriveService.Scope.Drive };                           //Scopes for use with the Google Drive API
        static string ApplicationName = "Drive API .NET SummerProject";
        static DriveService service;
        static string path = @"C:\BioPack\";


        public HomeView()
        {
            InitializeComponent();
        }

        private void open_View(object sender, RoutedEventArgs e)
        {
           

        }

        private static string getID(string str)
        { // Define parameters of request.
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.Fields = "nextPageToken, files(id, name, owners, parents,mimeType)";
            string temp = "name = \'" + str + "\'";
            Console.WriteLine(temp);
            listRequest.Q = temp;
            var request = listRequest.Execute();
            if (request.Files[0].MimeType == "application/vnd.google-apps.folder")
            {
                Console.WriteLine("reached here");
                return null;
            }
            return request.Files[0].Id;
        }

        private void saveFile(object sender, RoutedEventArgs e)                                     // save file to  C drive of the computer
        {
            string fileID = getID((string)(sender as MenuItem).Header);
            if(fileID == null)
            {
                return;
            }
            string fileName = (string)(sender as MenuItem).Header;
            if (System.IO.File.Exists(path + fileName))
            {
                System.Diagnostics.Process.Start(path + (sender as MenuItem).Header);
                return;
            }

            var request = service.Files.Get(fileID);
            var stream = new System.IO.MemoryStream();
            request.Download(stream);
            stream.Seek(0, System.IO.SeekOrigin.Begin);
            using (Stream fileStream = System.IO.File.Create(path + fileName))
            {
                stream.CopyTo(fileStream);
            }
            Console.WriteLine(path + fileName);
            System.Diagnostics.Process.Start(path + (sender as MenuItem).Header);
        }

        private static UserCredential GetCredentials()                                                                 //function for getting credentials
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

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Search_Click(this, new RoutedEventArgs());
            }
        }

        private void Search_Click(object sender, RoutedEventArgs e)                          // function to initiate search on clicking the search button
        {
            string[] Scopes = { DriveService.Scope.Drive };
            string ApplicationName = "Drive API .NET SummerProject";
            UserCredential credential;
            // here is where we Request the user to give us access 
            credential = GetCredentials();
            service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            searchMenu.Items.Clear(); 
            searchQuery(textBox.Text);                                             //calling the function for search by passing the text entered in the search box 
        }

        private void searchQuery(string str)                                 
        {
            FilesResource.ListRequest listRequest = service.Files.List();                   //Constructs a new List request
            listRequest.Fields = "nextPageToken, files(id, name, owners, parents,mimeType)";
            string temp = "fullText contains \'" + str + "\'";                                              
            listRequest.Q = temp;                                                           //search  query            
            var request = listRequest.Execute();
            if (request.Files != null && request.Files.Count > 0) 
            {
                foreach (var file in request.Files)
                {
                    Console.WriteLine("{0}", file.Name);
                    MenuItem menuItem = new MenuItem();
                    menuItem.Header = file.Name;
                    menuItem.Height = 20;
                    menuItem.Click += saveFile;
                    searchMenu.Items.Add(menuItem);                                        // Add items to menu
                }

            }
            else
            {
                searchMenu.Items.Add("No files found.");
            }
        }
    }
}
