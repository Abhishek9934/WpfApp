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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> toDelete;
        static string path = "ftp://shivam99:sp99wpfappftp@ftp.drivehq.com/";
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new HomeView();
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
        private List<string> ListDirectory(string menuItemName,string subMenuItemName = "")
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(path + menuItemName + subMenuItemName);
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

                // This example assumes the FTP site uses anonymous logon.
                request.Credentials = new NetworkCredential("shivam99", "sp99wpfappftp");

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                string filesList = reader.ReadToEnd();
                Console.WriteLine(filesList);
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



        private void BlueView_Clicked(object sender, RoutedEventArgs e)
        {
            DataContext = new BlueView((string)(sender as MenuItem).Tag,(string)(sender as MenuItem).Header);
        }

        private void HomeView_Clicked(object sender, RoutedEventArgs e)
        {
            DataContext = new HomeView();
        }

        private void Edit_Content(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe","ftp://shivam99:sp99wpfappftp@ftp.drivehq.com/");
        }

        private void MenuItem_Clicked(object sender, RoutedEventArgs e)
        {
            if ((string)(sender as MenuItem).Tag == "unchecked")
            {
                List<string> list1 = ListDirectory((string)(sender as System.Windows.Controls.MenuItem).Header);

                for (int i = 0; i < list1.Count; i++)
                {
                    MenuItem menuItem = new MenuItem();menuItem.Tag = (sender as MenuItem).Header;
                    menuItem.Header = list1[i]; menuItem.Click += BlueView_Clicked;
                    (sender as MenuItem).Items.Add(menuItem);

                }
                (sender as MenuItem).Tag = "checked";
            }
        }
    }
}
/*
<Window.Resources>
        <DataTemplate x:Name="blueViewTemplate" DataType="{x:Type viewmodels:BlueViewModel}">
            <views:BlueView DataContext = "{Binding}" />
        </ DataTemplate >
        < DataTemplate x:Name="homeViewTemplate" DataType="{x:Type viewmodels:HomeViewModel}">
            <views:HomeView DataContext = "{Binding}" />
        </ DataTemplate >
    </ Window.Resources >*/

