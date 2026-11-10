using System.IO;
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
using Microsoft.Win32;

namespace CodeCleaner
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TemaTextBox.Focus();
        }


        private void TallozasButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "C# fájlok (*.cs)|*.cs";

            if (dialog.ShowDialog() == true)
            {
                string fajlUt = dialog.FileName;

                string kod = File.ReadAllText(fajlUt);

                KodTextBox.Text = kod;
            }
        }

        private void KodTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TemaTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Bekuld_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}