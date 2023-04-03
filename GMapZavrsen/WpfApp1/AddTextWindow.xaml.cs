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
    /// Interaction logic for AddTextWindow.xaml
    /// </summary>
    public partial class AddTextWindow : Window
    {
        public AddTextWindow()
        {
            InitializeComponent();
            bojaVrednost.ItemsSource = typeof(Brushes).GetProperties(BindingFlags.Static | BindingFlags.Public).Select(o => o.Name);
            if (Izmena == true)
            {
                tekstVrednost.Text = tekst;
                velicinaVrednost.Text = velicina.ToString();
                providnostVrednost.Text = providnost.ToString();
                bojaVrednost.SelectedItem = imeBoje;


                Izmena = false;
            }
        }

        public static string tekst;
        public static SolidColorBrush boja;
        public static string imeBoje;
        public static int velicina;
        public static double providnost;
        public static bool Closed;
        public static bool Izmena;

        public void Draw_click(object sender, RoutedEventArgs e)
        {
            if (tekstVrednost.Text != "" && velicinaVrednost.Text != "" && providnostVrednost.Text != "")
            {
                tekst = tekstVrednost.Text;
                velicina = Int32.Parse(velicinaVrednost.Text);
                providnost = Convert.ToDouble(providnostVrednost.Text);
                boja = new SolidColorBrush((Color)ColorConverter.ConvertFromString((string)bojaVrednost.SelectedItem));

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
