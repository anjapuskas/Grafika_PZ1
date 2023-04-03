using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for AddElipseWindow.xaml
    /// </summary>
    public partial class AddElipseWindow : Window
    {

        public static int x;
        public static int y;
        public static int debljina;
        public static SolidColorBrush boja;
        public static string imeBoje;
        public static string tekst;
        public static SolidColorBrush bojaTeksta;
        public static string imeBojeTeksta;
        public static double providnost;
        public static bool Closed;
        public static bool Izmena;
        public AddElipseWindow()
        {
            InitializeComponent();
            bojaTekstaVrednost.ItemsSource = typeof(Brushes).GetProperties(BindingFlags.Static | BindingFlags.Public).Select(o => o.Name);
            bojaVrednost.ItemsSource = typeof(Brushes).GetProperties(BindingFlags.Static | BindingFlags.Public).Select(o => o.Name);
            if (Izmena == true) {
                xVrednost.Text = x.ToString() ;
                yVrednost.Text = y.ToString();
                debljinaVrednost.Text = debljina.ToString();
                tekstVrednost.Text = tekst;
                providnostVrednost.Text = providnost.ToString();
                bojaVrednost.SelectedItem = imeBoje;
                bojaTekstaVrednost.SelectedItem = imeBojeTeksta;

                Izmena = false;
            }
        }

        public void Draw_click(object sender, RoutedEventArgs e)
        {
            if(xVrednost.Text != "" && yVrednost.Text != "" && debljinaVrednost.Text != "" && providnostVrednost.Text != "")
            {
                x = Int32.Parse(xVrednost.Text);
                y = Int32.Parse(yVrednost.Text);
                debljina = Int32.Parse(debljinaVrednost.Text);
                tekst = tekstVrednost.Text;
                providnost = Convert.ToDouble(providnostVrednost.Text);
                boja = new SolidColorBrush((Color)ColorConverter.ConvertFromString((string)bojaVrednost.SelectedItem));
                if (tekst != "")
                {
                    bojaTeksta = new SolidColorBrush((Color)ColorConverter.ConvertFromString((string)bojaTekstaVrednost.SelectedItem));
                }
                this.Close();
                Closed = false;
            }
            
        }

        private void Cancel_click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ValidationNumber(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Closed = true;
        }
    }
}
