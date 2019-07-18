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
namespace WpfApp1.Views
{
    /// <summary>
    /// Interaction logic for ListView.xaml
    /// </summary>
    public partial class ListView : UserControl
    {
        static string[] Scopes = { DriveService.Scope.Drive };                                //Scopes for use with the Google Drive API

        static DriveService service;
        public ListView(DriveService ds)
        {
            UserCredential credential;

            credential = GetCredentials();
            InitializeComponent();

           
#if DEBUG
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
#endif

            service = ds;
            ListFiles();
        }
    
        private static UserCredential GetCredentials()                                      //function for getting credentials
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

        private static string getID(string str)
        { // Define parameters of request.
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.Fields = "nextPageToken, files(id, name, owners, parents,mimeType)";
            string temp = "name = \'" + str + "\'";
           // Console.WriteLine(temp);
            listRequest.Q = temp;
            var request = listRequest.Execute();
            return request.Files[0].Id;
        }

        private void FolderContent(string folderID,MenuItem parent)              // To list all the files from the drive 
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
                    MenuItem menuItem = new MenuItem();
                    menuItem.Header = '\t' + file.Name;
                    menuItem.Visibility = Visibility.Visible;
                    Menulist.Items.Add(menuItem);
                }
            }
            else
            {

            }
        }

        private void ListFiles()
        {
            // Define parameters of request.
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.Fields = "nextPageToken, files(id, name, owners, parents)";
            listRequest.Q = "mimeType='application/vnd.google-apps.folder'";

            // List files.
            var request = listRequest.Execute();


            if (request.Files != null && request.Files.Count > 0)
            {
                foreach (var file in request.Files)
                {
                    if (file.Name == "Plants" || file.Name == "Animals" || file.Name == "Fungi" || file.Name == "Microorganisms" || file.Name == "On Land" || file.Name == "In Air" || file.Name == "In Water")
                    {
                        if (file.Name != "Text" && file.Name != "Images" && file.Name != "Videos")
                        {
                            MenuItem menuItem = new MenuItem();
                            menuItem.Header = file.Name;
                            Menulist.Items.Add(menuItem);
                            menuItem.Visibility = Visibility.Visible;
                            FolderContent(file.Id,menuItem);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("No files found.");
            }
        }
    }
}
