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
using System.Net;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        byte[] ConvertImageToBinary(System.Drawing.Image img)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        System.Drawing.Image ConvertBinaryToImage(byte[] data)
        {
            using(MemoryStream ms = new MemoryStream(data))
            {
                return System.Drawing.Image.FromStream(ms);
            }
        }


        private void BlueView_Clicked(object sender, RoutedEventArgs e)
        {
            DataContext = new BlueViewModel();
        }

    }
}
