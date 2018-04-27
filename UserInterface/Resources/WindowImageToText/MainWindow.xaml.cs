using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ImgToTextLib;

namespace WindowImageToText
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string filename = @".\capture.png"; //System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + @"\Tessdata\";
  
        public MainWindow()
        {
            InitializeComponent();
            string strContent= ImgToTextLib.ImageToTextLib.strConvertImageToText(filename);
            txtContent.Text = strContent;
            txtResult.Text = ImgToTextLib.ImageToTextLib.strTranslateGoogle(strContent,"en|vi");
            KeyDown += escape;
        }

        private void escape(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
    }
}
