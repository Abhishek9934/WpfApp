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
    /// Interaction logic for AboutView.xaml
    /// </summary>
    public partial class AboutView : UserControl
    {
        static string folderID;
        static DriveService service;


        static string path = @"C:\BioPack\";
       
        public AboutView(string s1, DriveService ds)
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
                if (x.Name == "About")                                                        
                {
                    List<Google.Apis.Drive.v3.Data.File> descList = FolderContent(x.Id);
                    if (descList.Count > 0) desc.Text = getText(descList[0].Id);
                }

            }
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
    }
}
