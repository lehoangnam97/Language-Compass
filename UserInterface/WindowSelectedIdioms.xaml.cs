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
using System.Windows.Shapes;


namespace UserInterface
{
    /// <summary>
    /// Interaction logic for WindowSelectedIdioms.xaml
    /// </summary>
    public partial class WindowSelectedIdioms : Window
    {


        public string selectedItem
        {
            get;
            set;
        }
        public WindowSelectedIdioms()
        {


            InitializeComponent();

        }

        public void SetUpItems(string[] words)
        {

            foreach (string m in words)
            {
                lboxWords.Items.Add(m);
            }

            lboxWords.SelectionChanged += LboxWords_SelectionChanged;
        }


        private void LboxWords_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            selectedItem = lboxWords.SelectedValue.ToString();
            this.DialogResult = true;
        }
    }

}
