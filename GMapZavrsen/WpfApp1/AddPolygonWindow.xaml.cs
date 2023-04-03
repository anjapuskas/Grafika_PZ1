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
    /// Interaction logic for AddPolygonWindow.xaml
    /// </summary>
    public partial class AddPolygonWindow : Window
    {
        public AddPolygonWindow()
        {
            InitializeComponent();
            bojaKontureVrednost.ItemsSource = typeof(Brushes).GetProperties(BindingFlags.Static | BindingFlags.Public).Select(o => o.Name);
            bojaVrednost.ItemsSource = typeof(Brushes).GetProperties(BindingFlags.Static | BindingFlags.Public).Select(o => o.Name);
            bojaTekstaVrednost.ItemsSource = typeof(Brushes).GetProperties(BindingFlags.Static | BindingFlags.Public).Select(o => o.Name);
            if (Izmena == true)
            {
                debljinaVrednost.Text = debljina.ToString();
                providnostVrednost.Text = providnost.ToString();
                bojaVrednost.SelectedItem = imeBoje;
                bojaKontureVrednost.SelectedItem = imeBojeKonture;
                bojaTekstaVrednost.SelectedItem = imeBojeTeksta;
                tekstVrednost.Text = tekst;

                Izmena = false;
            }
        }

        public static int debljina;
        public static SolidColorBrush boja;
        public static string imeBoje;
        public static SolidColorBrush bojaKonture;
        public static string imeBojeKonture;
        public static string tekst;
        public static SolidColorBrush bojaTeksta;
        public static string imeBojeTeksta;
        public static double providnost;
        public static bool Closed;
        public static bool Izmena;

        public void Draw_click(object sender, RoutedEventArgs e)
        {
            if(debljinaVrednost.Text != "" && providnostVrednost.Text != "" )
            {
                debljina = Int32.Parse(debljinaVrednost.Text);
                providnost = Convert.ToDouble(providnostVrednost.Text);
                tekst = tekstVrednost.Text;
                boja = new SolidColorBrush((Color)ColorConverter.ConvertFromString((string)bojaVrednost.SelectedItem));
                bojaKonture = new SolidColorBrush((Color)ColorConverter.ConvertFromString((string)bojaKontureVrednost.SelectedItem));
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

        private void OpacityNumber(object sender, TextCompositionEventArgs e)
        {
            e.Handled = 0 <= Convert.ToDouble(e.Text) && Convert.ToDouble(e.Text) <= 1;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Closed = true;
        }
    }
}
