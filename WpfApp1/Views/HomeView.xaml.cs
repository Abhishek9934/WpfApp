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
        static string[] Scopes = { DriveService.Scope.Drive };
        static string ApplicationName = "Drive API .NET SummerProject";

        public Action<object, RoutedEventArgs> AcceptButton { get; }

        public HomeView()
        {
            InitializeComponent(); 
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private static UserCredential GetCredentials()
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
 
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string[] Scopes = { DriveService.Scope.Drive };
            string ApplicationName = "Drive API .NET SummerProject";
            UserCredential credential;

            credential = GetCredentials();
            DriveService service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            searchMenu.Items.Clear(); 
            searchQuery(service, textBox.Text);
        }

        private void searchQuery(DriveService service,string str)
        {
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.Fields = "nextPageToken, files(id, name, owners, parents,mimeType)";
            string temp = "fullText contains \'" + str + "\'";
            listRequest.Q = temp;
            var request = listRequest.Execute();
            if (request.Files != null && request.Files.Count > 0)
            {
                foreach (var file in request.Files)
                {
                    Console.WriteLine("{0}", file.Name);
                    MenuItem menuItem = new MenuItem();
                    menuItem.Header = file.Name;
                    menuItem.Height = 20;
                    //menuItem.Click += videoPlayer;
                    searchMenu.Items.Add(menuItem);
                }

            }
            else
            {
                searchMenu.Items.Add("No files found.");
            }
        }
    }
}
