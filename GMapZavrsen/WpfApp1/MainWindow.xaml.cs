using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using WpfApp1.Model;
using Brushes = System.Drawing.Brushes;
using Pen = System.Drawing.Pen;
using Point = WpfApp1.Model.Point;
using Size = System.Drawing.Size;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public double noviX, noviY;
        private List<SubstationEntity> substationEntities = new List<SubstationEntity>();
        private List<NodeEntity> nodeEntities = new List<NodeEntity>();
        private List<SwitchEntity> switchEntities = new List<SwitchEntity>();
        private List<LineEntity> lineEntities = new List<LineEntity>();
        public List<PowerEntity> listAllEnitities = new List<PowerEntity>();
        public List<PowerEntity> listAllShownEnitities = new List<PowerEntity>();
        private List<Ellipse> listEllipse = new List<Ellipse>();
        List<System.Windows.Shapes.Line> listDrawnLines = new List<System.Windows.Shapes.Line>();
        private List<UIElement> istorijaUndo = new List<UIElement>();

        private Ellipse animatedFirstEllipse = new Ellipse();
        private Ellipse animatedSecondEllipse = new Ellipse();
        public static Polygon polygon = new Polygon();

        public double minX, maxX, minY, maxY;

        bool drawEllipse = false;
        bool drawPolygon = false;
        bool addText = false;
        bool obrisanoSve = false;


        public static Dictionary<string, List<PowerEntity>> zauzetaMesta = new Dictionary<string, List<PowerEntity>>();
        public static Dictionary<System.Windows.Shapes.Line, LineEntity> iscrtaneLinije = new Dictionary<System.Windows.Shapes.Line, LineEntity>();


        public MainWindow()
        {
            InitializeComponent();

            this.DrawEllipse.Background = new SolidColorBrush(Colors.LightGray);
            this.DrawPolygon.Background = new SolidColorBrush(Colors.LightGray);
            this.AddText.Background = new SolidColorBrush(Colors.LightGray);
            this.Undo.Background = new SolidColorBrush(Colors.LightGray);
            this.Redo.Background = new SolidColorBrush(Colors.LightGray);
            this.Clear.Background = new SolidColorBrush(Colors.LightGray);

            loadXML("Geographic.xml");

            crtajTacke();
            crtajLinije();
        }

        private void loadXML(string xmlName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlName);



            XmlNodeList xmlSubstationList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Substations/SubstationEntity");

            foreach (XmlNode node in xmlSubstationList)
            {
                SubstationEntity substationEntity = new SubstationEntity();
                substationEntity.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                substationEntity.Name = node.SelectSingleNode("Name").InnerText;

                double x = double.Parse(node.SelectSingleNode("X").InnerText);
                double y = double.Parse(node.SelectSingleNode("Y").InnerText);
                ToLatLon(x, y, 34, out noviX, out noviY);


                substationEntity.X = noviX;
                substationEntity.Y = noviY;

                substationEntities.Add(substationEntity);
                listAllEnitities.Add(substationEntity);
            }

            XmlNodeList xmlNodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Nodes/NodeEntity");

            foreach (XmlNode node in xmlNodeList)
            {
                NodeEntity nodeEntity = new NodeEntity();
                nodeEntity.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                nodeEntity.Name = node.SelectSingleNode("Name").InnerText;

                double x = double.Parse(node.SelectSingleNode("X").InnerText);
                double y = double.Parse(node.SelectSingleNode("Y").InnerText);
                ToLatLon(x, y, 34, out noviX, out noviY);


                nodeEntity.X = noviX;
                nodeEntity.Y = noviY;

                nodeEntities.Add(nodeEntity);
                listAllEnitities.Add(nodeEntity);
            }

            XmlNodeList xmlSwitchList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Switches/SwitchEntity");

            foreach (XmlNode node in xmlSwitchList)
            {
                SwitchEntity switchEntity = new SwitchEntity();
                switchEntity.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                switchEntity.Name = node.SelectSingleNode("Name").InnerText;
                switchEntity.Status = node.SelectSingleNode("Status").InnerText;

                double x = double.Parse(node.SelectSingleNode("X").InnerText);
                double y = double.Parse(node.SelectSingleNode("Y").InnerText);
                ToLatLon(x, y, 34, out noviX, out noviY);


                switchEntity.X = noviX;
                switchEntity.Y = noviY;

                switchEntities.Add(switchEntity);
                listAllEnitities.Add(switchEntity);
            }

            XmlNodeList xmlLineList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Lines/LineEntity");

            foreach (XmlNode node in xmlLineList)
            {
                LineEntity lineEntity = new LineEntity();
                lineEntity.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                lineEntity.Name = node.SelectSingleNode("Name").InnerText;
                lineEntity.IsUnderground = node.SelectSingleNode("IsUnderground").InnerText.Equals("true");
                lineEntity.R = float.Parse(node.SelectSingleNode("R").InnerText);
                lineEntity.ConductorMaterial = node.SelectSingleNode("ConductorMaterial").InnerText;
                lineEntity.LineType = node.SelectSingleNode("LineType").InnerText;
                lineEntity.ThermalConstantHeat = long.Parse(node.SelectSingleNode("ThermalConstantHeat").InnerText);
                lineEntity.FirstEnd = long.Parse(node.SelectSingleNode("FirstEnd").InnerText);
                lineEntity.SecondEnd = long.Parse(node.SelectSingleNode("SecondEnd").InnerText);
                lineEntity.Vertices = new List<Point>();

                foreach (XmlNode pointNode in node.ChildNodes[9].ChildNodes)
                {
                    Point point = new Point();

                    double x = double.Parse(pointNode.SelectSingleNode("X").InnerText);
                    double y = double.Parse(pointNode.SelectSingleNode("Y").InnerText);

                    ToLatLon(x, y, 34, out noviX, out noviY);

                    point.X = noviX;
                    point.Y = noviY;
                    lineEntity.Vertices.Add(point);


                }


                lineEntities.Add(lineEntity);
            }

        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void crtajTacke()
        {
            minX = listAllEnitities.Min(entity => entity.X) - 0.01; //trazenje ekstrema, preko njih trazimo gde se nalazi neka tacka
            maxX = listAllEnitities.Max(entity => entity.X) + 0.01;
            minY = listAllEnitities.Min(entity => entity.Y) - 0.01;
            maxY = listAllEnitities.Max(entity => entity.Y) + 0.01;
            
            foreach (var entity in listAllEnitities)
            {
                double positionX = (entity.X - minX) / ((maxX - minX) / 300);
                double positionY = (entity.Y - minY) / ((maxY - minY) / 300);

                int intX = (int)positionX;
                int intY = (int)positionY;

                if (!zauzetaMesta.ContainsKey(intX + "/" + intY))
                {
                    List<PowerEntity> lista = new List<PowerEntity>();
                    lista.Add(entity);
                    zauzetaMesta.Add(intX + "/" + intY, lista);
                } else
                {
                    zauzetaMesta[intX + "/" + intY].Add(entity);
                }
            }

            foreach (KeyValuePair<string, List<PowerEntity>> entry in zauzetaMesta) //kljuc x/y, vrednost lista tacaka
            {
                string[] xypar = entry.Key.Split('/');
                int intX = Convert.ToInt32(xypar[0]);
                int intY = Convert.ToInt32(xypar[1]);
                if(entry.Value.Count == 1)
                {
                    PowerEntity powerEntity = entry.Value[0];
                    Ellipse el = new Ellipse();
                    //el.Stroke = System.Windows.Media.Brushes.Black;
                    el.Width = 5;
                    el.Height = 5;

                    if (powerEntity.GetType().ToString().Contains("SubstationEntity")){
                        el.ToolTip = "Type: SubstationEntity" + "\n" + "Id: " + powerEntity.Id.ToString() + "\n" + "Name: " + powerEntity.Name + "\n";
                        el.Fill = new SolidColorBrush(Colors.Blue);
                    }

                    if (powerEntity.GetType().ToString().Contains("NodeEntity"))
                    {
                        el.ToolTip = "Type: NodeEntity" + "\n" + "Id: " + powerEntity.Id.ToString() + "\n" + "Name: " + powerEntity.Name + "\n";
                        el.Fill = new SolidColorBrush(Colors.Green);
                    }

                    if (powerEntity.GetType().ToString().Contains("SwitchEntity"))
                    {
                        SwitchEntity switchEntity = (SwitchEntity)powerEntity;
                        el.ToolTip = "Type: SwitchEntity" + "\n" + "Id: " + powerEntity.Id.ToString() + "\n" + "Name: " + powerEntity.Name + "\n" + "Status: " + switchEntity.Status;
                        el.Fill = new SolidColorBrush(Colors.Red);
                    }


                    Canvas.SetBottom(el, intX * 6);
                    Canvas.SetLeft(el, intY * 6);
                    PowerEntity newPowerEntity = new PowerEntity();
                    newPowerEntity.Id = powerEntity.Id;
                    newPowerEntity.X = intX;
                    newPowerEntity.Y = intY;
                    el.Tag = powerEntity.Id.ToString();
                    listAllShownEnitities.Add(newPowerEntity);
                    listEllipse.Add(el);
                    Canvass.Children.Add(el);
                }

                else
                {
                    Ellipse el = new Ellipse();
                    el.Stroke = System.Windows.Media.Brushes.Black;
                    el.Width = 5;
                    el.Height = 5;
                    el.Fill = new SolidColorBrush(Colors.Black);

                    string toolTip = "";
                    int index = 1;
                    string id = "";
                    foreach (PowerEntity powerEntity in entry.Value)
                    {
                        if (powerEntity.GetType().ToString().Contains("SubstationEntity"))
                        {
                           toolTip += "Item: " + index + "\nType: SubstationEntity" + "\n" + "Id: " + powerEntity.Id.ToString() + "\n" + "Name: " + powerEntity.Name + "\n\n";
                        }

                        if (powerEntity.GetType().ToString().Contains("NodeEntity"))
                        {
                            toolTip += "Item: " + index + "\nType: NodeEntity" + "\n" + "Id: " + powerEntity.Id.ToString() + "\n" + "Name: " + powerEntity.Name + "\n\n";
                        }

                        if (powerEntity.GetType().ToString().Contains("SwitchEntity"))
                        {
                            SwitchEntity switchEntity = (SwitchEntity)powerEntity;
                            toolTip += "Item: " + index + "\nType: NodeEntity" + "\n" + "Id: " + powerEntity.Id.ToString() + "\n" + "Name: " + powerEntity.Name + "\n" + "Status: " + switchEntity.Status + "\n\n";
                        }
                        index++;
                        id += powerEntity.Id.ToString();
                        PowerEntity newPowerEntity = new PowerEntity();
                        newPowerEntity.Id = powerEntity.Id;
                        newPowerEntity.X = intX;
                        newPowerEntity.Y = intY;
                        listAllShownEnitities.Add(newPowerEntity);
                    }
                    el.Tag = id;
                    el.ToolTip = toolTip;
                    Canvas.SetBottom(el, intX * 6);
                    Canvas.SetLeft(el, intY * 6);
                    listEllipse.Add(el);
                    Canvass.Children.Add(el);
                }
            }

        }

        private void crtajLinije()
        {
            foreach (var lineEntity in lineEntities)  //u lineentity imamo inf sta je first sta je second end
            {
                double pocetnaPozicijaX = -1;
                double pocetnaPozicijaY = -1;
                double krajnjaPozicijaX = -1;
                double krajnjaPozicijaY = -1;
                foreach (var powerEntity in listAllShownEnitities)
                {
                    if (powerEntity.Id == lineEntity.FirstEnd)
                    {
                        pocetnaPozicijaX = powerEntity.X;
                        pocetnaPozicijaY = powerEntity.Y;
                    }
                    if (powerEntity.Id == lineEntity.SecondEnd)
                    {
                        krajnjaPozicijaX = powerEntity.X;
                        krajnjaPozicijaY = powerEntity.Y;
                    }
                }

                if (pocetnaPozicijaX == -1 || pocetnaPozicijaY == 1 || krajnjaPozicijaX == -1 || krajnjaPozicijaY == -1)
                {
                    continue;
                }

                //VERTIKALA
                System.Windows.Shapes.Line linija = new System.Windows.Shapes.Line(); //vertikalna pocinje od krajnje tacke, a x osa je ista

                linija.X1 = krajnjaPozicijaY * 6 + 2.5;
                linija.Y1 = 1800 - pocetnaPozicijaX * 6 - 2.5;
                linija.X2 = krajnjaPozicijaY * 6 + 2.5;
                linija.Y2 = 1800 - krajnjaPozicijaX * 6 - 2.5;
                DrawnLine drawnLine1 = new DrawnLine();
                DrawnLine drawnLine2 = new DrawnLine();

                if (!listDrawnLines.Contains(linija))
                {
                    linija.Stroke = System.Windows.Media.Brushes.DimGray;
                    linija.StrokeThickness = 1;
                    linija.MouseRightButtonUp += Vod_MouseRightButtonUp;
                    linija.ToolTip = "ID: " + lineEntity.Id;
                    drawnLine1.FirstEnd = lineEntity.FirstEnd;  //linije cuvamo u drawnline entity
                    drawnLine1.SecondEnd = lineEntity.SecondEnd;
                    linija.Tag = drawnLine1;                    //objekat drawnline cuvamo u tagu, da svaka linija zna koji su njeni entiteti


                    Canvass.Children.Add(linija);
                    listDrawnLines.Add(linija);
                }
                //HORIZONTALA
                System.Windows.Shapes.Line linija2 = new System.Windows.Shapes.Line();  //horizontalna pocinej od pocetne tacke, y osa je fiksna

                linija2.X1 = pocetnaPozicijaY * 6 + 2.5;
                linija2.Y1 = 1800 - pocetnaPozicijaX * 6 -2.5;
                linija2.X2 = krajnjaPozicijaY * 6 + 2.5;
                linija2.Y2 = 1800 - pocetnaPozicijaX * 6 - 2.5;

                if (!listDrawnLines.Contains(linija2))
                {
                    linija2.Stroke = System.Windows.Media.Brushes.DimGray;
                    linija2.StrokeThickness = 1;
                    linija2.MouseRightButtonUp += Vod_MouseRightButtonUp;
                    linija2.ToolTip = "ID: " + lineEntity.Id;
                    drawnLine2.FirstEnd = lineEntity.FirstEnd;
                    drawnLine2.SecondEnd = lineEntity.SecondEnd;
                    drawnLine2.OtherLine = linija;
                    drawnLine1.OtherLine = linija2;

                    linija.Tag = drawnLine1;
                    linija2.Tag = drawnLine2;


                    Canvass.Children.Add(linija2);
                    listDrawnLines.Add(linija2);
                }
            }
        }

        private void Vod_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            DrawnLine drawnLine = (DrawnLine)(sender as System.Windows.Shapes.Line).Tag;
            string firstEndId = drawnLine.FirstEnd.ToString();
            string secondtEndId = drawnLine.SecondEnd.ToString();

            Ellipse firstEllipse = new Ellipse() ;
            Ellipse secondEllipse = new Ellipse(); 
            foreach (Ellipse el in listEllipse)
            {
                if (el.Tag.ToString().Contains(firstEndId))
                {
                    firstEllipse = el;
                    break;
                }
            }
            foreach (Ellipse el in listEllipse)
            {
                if (el.Tag.ToString().Contains(secondtEndId))
                {
                    secondEllipse = el;
                    break;
                }
            }


            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.AutoReverse = true;
            doubleAnimation.From = 5;
            doubleAnimation.To = 10;
            doubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.5));

            ColorAnimation colorAnimationFirst = new ColorAnimation();
            colorAnimationFirst.AutoReverse = true;
            colorAnimationFirst.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            colorAnimationFirst.From = ((SolidColorBrush)firstEllipse.Fill).Color;
            colorAnimationFirst.To = new SolidColorBrush(Colors.Yellow).Color;

            firstEllipse.Fill.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimationFirst);
            firstEllipse.BeginAnimation(Ellipse.WidthProperty, doubleAnimation);
            firstEllipse.BeginAnimation(Ellipse.HeightProperty, doubleAnimation);

            ColorAnimation colorAnimationSecond = new ColorAnimation();
            colorAnimationSecond.AutoReverse = true;
            colorAnimationSecond.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            colorAnimationSecond.From = ((SolidColorBrush)secondEllipse.Fill).Color;
            colorAnimationSecond.To = new SolidColorBrush(Colors.Yellow).Color;

            secondEllipse.Fill.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimationSecond);
            secondEllipse.BeginAnimation(Ellipse.WidthProperty, doubleAnimation);
            secondEllipse.BeginAnimation(Ellipse.HeightProperty, doubleAnimation);

            DoubleAnimation lineAnimation = new DoubleAnimation();
            lineAnimation.AutoReverse = true;
            lineAnimation.From = 1;
            lineAnimation.To = 2;
            lineAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.5));

            (sender as System.Windows.Shapes.Line).BeginAnimation(System.Windows.Shapes.Line.StrokeThicknessProperty, lineAnimation);
            if (drawnLine.OtherLine != null)
            {
                drawnLine.OtherLine.BeginAnimation(System.Windows.Shapes.Line.StrokeThicknessProperty, lineAnimation);
            }
        }

        public void DrawEllipse_Click(object sender, RoutedEventArgs e)
        {
            if(!drawEllipse)
            {
                drawEllipse = true;
                drawPolygon = false;
                addText = false;
                this.DrawEllipse.Background = new SolidColorBrush(Colors.Green);
                this.DrawPolygon.Background = new SolidColorBrush(Colors.LightGray);
                this.AddText.Background = new SolidColorBrush(Colors.LightGray);
            }
            else
            {
                drawEllipse = false;
                drawPolygon = false;
                addText = false;
                this.DrawEllipse.Background = new SolidColorBrush(Colors.LightGray);
                this.DrawPolygon.Background = new SolidColorBrush(Colors.LightGray);
                this.AddText.Background = new SolidColorBrush(Colors.LightGray);
            }
        }

        public void DrawPolygon_Click(object sender, RoutedEventArgs e)
        {
            if(!drawPolygon)
            {
                drawEllipse = false;
                drawPolygon = true;
                addText = false;
                this.DrawEllipse.Background = new SolidColorBrush(Colors.LightGray);
                this.DrawPolygon.Background = new SolidColorBrush(Colors.Green);
                this.AddText.Background = new SolidColorBrush(Colors.LightGray);
            }
            else
            {
                drawEllipse = false;
                drawPolygon = false;
                addText = false;
                this.DrawEllipse.Background = new SolidColorBrush(Colors.LightGray);
                this.DrawPolygon.Background = new SolidColorBrush(Colors.LightGray);
                this.AddText.Background = new SolidColorBrush(Colors.LightGray);
            }
        }

        public void AddText_Click(object sender, RoutedEventArgs e)
        {
            if(!addText)
            {
                drawEllipse = false;
                drawPolygon = false;
                addText = true;
                this.DrawEllipse.Background = new SolidColorBrush(Colors.LightGray);
                this.DrawPolygon.Background = new SolidColorBrush(Colors.LightGray);
                this.AddText.Background = new SolidColorBrush(Colors.Green);
            }
            else
            {
                drawEllipse = false;
                drawPolygon = false;
                addText = false;
                this.DrawEllipse.Background = new SolidColorBrush(Colors.LightGray);
                this.DrawPolygon.Background = new SolidColorBrush(Colors.LightGray);
                this.AddText.Background = new SolidColorBrush(Colors.LightGray);
            }
        }

        public void Undo_Click(object sender, RoutedEventArgs e)
        {
            this.DrawEllipse.Background = new SolidColorBrush(Colors.LightGray);
            this.DrawPolygon.Background = new SolidColorBrush(Colors.LightGray);
            this.AddText.Background = new SolidColorBrush(Colors.LightGray);
            if (Canvass.Children.Count != 0)
            {
                istorijaUndo.Add(Canvass.Children[Canvass.Children.Count - 1]);   //da mozemo da radimo redo
                Canvass.Children.Remove(Canvass.Children[Canvass.Children.Count - 1]);

            } else
            {
                if (obrisanoSve == true)
                {
                    foreach(UIElement element in istorijaUndo)
                    {
                        Canvass.Children.Add(element);   //vrati sve iz undo
                    }
                    istorijaUndo.Clear();
                    obrisanoSve = false;
                }
            }
        }

        public void Redo_Click(object sender, RoutedEventArgs e)
        {
            this.DrawEllipse.Background = new SolidColorBrush(Colors.LightGray);
            this.DrawPolygon.Background = new SolidColorBrush(Colors.LightGray);
            this.AddText.Background = new SolidColorBrush(Colors.LightGray);
            if (istorijaUndo.Count > 0)
            {
                Canvass.Children.Add(istorijaUndo[istorijaUndo.Count - 1]);
                istorijaUndo.Remove(istorijaUndo[istorijaUndo.Count - 1]);
            }
        }

        public void Clear_Click(object sender, RoutedEventArgs e)
        {
            this.DrawEllipse.Background = new SolidColorBrush(Colors.LightGray);
            this.DrawPolygon.Background = new SolidColorBrush(Colors.LightGray);
            this.AddText.Background = new SolidColorBrush(Colors.LightGray);
            if (obrisanoSve == false)
            {
                foreach (UIElement element in Canvass.Children)
                {
                    istorijaUndo.Add(element);
                }
                Canvass.Children.Clear();
                obrisanoSve = true;
            }
        }

        private void Canvass_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (drawEllipse)
            {
                System.Windows.Point p = e.GetPosition(Canvass);

                AddElipseWindow elipseWindow = new AddElipseWindow();
                elipseWindow.ShowDialog();

                if (!AddElipseWindow.Closed)
                {
                    Ellipse ellipse = new Ellipse();
                    ellipse.Width = AddElipseWindow.x;
                    ellipse.Height = AddElipseWindow.y;
                    ellipse.StrokeThickness = AddElipseWindow.debljina;
                    ellipse.Stroke = new SolidColorBrush(Colors.Black);
                    ellipse.Fill = AddElipseWindow.boja;
                    ellipse.Opacity = AddElipseWindow.providnost;
                    ellipse.MouseLeftButtonUp += EllipseMouseLeftButtonUp;
                    ellipse.Tag = p;

                    Canvas.SetLeft(ellipse, p.X);
                    Canvas.SetTop(ellipse, p.Y);

                    Canvass.Children.Add(ellipse);
                    if (AddElipseWindow.tekst != "")
                    {
                        TextBlock textBlock = new TextBlock();
                        textBlock.Text = AddElipseWindow.tekst;
                        textBlock.Foreground = AddElipseWindow.bojaTeksta;
                        textBlock.Tag = ellipse;
                        textBlock.Opacity = AddElipseWindow.providnost;
                        textBlock.Measure(new System.Windows.Size(Double.PositiveInfinity, Double.PositiveInfinity));

                        Canvas.SetLeft(textBlock, p.X + ellipse.Width / 2 - textBlock.DesiredSize.Width / 2);
                        Canvas.SetTop(textBlock, p.Y + ellipse.Height / 2 - textBlock.DesiredSize.Height / 2);

                        Canvass.Children.Add(textBlock);
                    }
                }
            }
            else if (drawPolygon)
            {
                if (!AddPolygonWindow.Closed)
                {
                    System.Windows.Point newPoint = e.GetPosition(this);
                    polygon.Points.Add(newPoint);
                }
            }

            else if (addText)
            {
                System.Windows.Point p = e.GetPosition(Canvass);

                AddTextWindow addTextWindow = new AddTextWindow();
                addTextWindow.ShowDialog();

                if (!AddTextWindow.Closed)
                {
                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = AddTextWindow.tekst;
                    textBlock.Foreground = AddTextWindow.boja;
                    textBlock.FontSize = AddTextWindow.velicina;
                    textBlock.Opacity = AddTextWindow.providnost;
                    textBlock.MouseLeftButtonUp += TextMouseLeftButtonUp;


                    Canvas.SetLeft(textBlock, p.X);
                    Canvas.SetTop(textBlock, p.Y);

                    Canvass.Children.Add(textBlock);
                }
            }
        }

        private void Canvass_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (drawPolygon && polygon.Points.Count > 2)
            {
                AddPolygonWindow addPolygonWindow = new AddPolygonWindow();
                addPolygonWindow.ShowDialog();

                if (!AddPolygonWindow.Closed)
                {
                    polygon.StrokeThickness = AddPolygonWindow.debljina;
                    polygon.Stroke = AddPolygonWindow.bojaKonture;
                    polygon.Fill = AddPolygonWindow.boja;
                    polygon.Opacity = AddPolygonWindow.providnost;
                    polygon.MouseLeftButtonUp += PolygonMouseLeftButtonUp;

                    System.Windows.Point centar = polygon.Points.Aggregate(new { xSum = 0.0, ySum = 0.0, n = 0 },
                        (acc, pPoint) => new
                        {
                            xSum = acc.xSum + pPoint.X,
                            ySum = acc.ySum + pPoint.Y,
                            n = acc.n + 1
                        },
                        acc => new System.Windows.Point(acc.xSum / acc.n, acc.ySum / acc.n));

                    polygon.Tag = centar;
                    Canvass.Children.Add(polygon);

                    if (AddPolygonWindow.tekst != "")
                    {
                        TextBlock textBlock = new TextBlock();
                        textBlock.Text = AddPolygonWindow.tekst;
                        textBlock.Foreground = AddPolygonWindow.bojaTeksta;
                        textBlock.Tag = polygon;
                        textBlock.Opacity = AddPolygonWindow.providnost;
                        textBlock.Measure(new System.Windows.Size(Double.PositiveInfinity, Double.PositiveInfinity));

                        Canvas.SetLeft(textBlock, centar.X);
                        Canvas.SetTop(textBlock, centar.Y);

                        Canvass.Children.Add(textBlock);
                    }
                }
                polygon = new Polygon();
            }

        }

        private void EllipseMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Ellipse el = (Ellipse)e.OriginalSource;

            if (Canvass.Children.Contains(el))
            {
                int index = Canvass.Children.IndexOf(el);
                bool postojiTekst = false;
                if (Canvass.Children.Count > (index+1) && Canvass.Children[index + 1] != null && Canvass.Children[index + 1] is TextBlock && ((TextBlock)Canvass.Children[index + 1]).Tag == el)
                {
                    postojiTekst = true;   //ako ima nesto posle elipse, ako je to tekstblok i od tacno te elipse
                }

                AddElipseWindow.Izmena = true;
                AddElipseWindow.x = Convert.ToInt32(el.Width);
                AddElipseWindow.y = Convert.ToInt32(el.Height);
                AddElipseWindow.imeBoje = imeBoje((SolidColorBrush)el.Fill);
                AddElipseWindow.debljina = Convert.ToInt32(el.StrokeThickness);
                AddElipseWindow.providnost = Convert.ToDouble(el.Opacity);
                if (postojiTekst)
                {
                    TextBlock textBlock = (TextBlock)Canvass.Children[index + 1];
                    AddElipseWindow.tekst = textBlock.Text;
                    AddElipseWindow.imeBojeTeksta = imeBoje((SolidColorBrush)textBlock.Foreground);
                }
                AddElipseWindow addElipseWindow = new AddElipseWindow();
                addElipseWindow.ShowDialog();
                
                if(!AddElipseWindow.Closed) { 
                    el.Width = AddElipseWindow.x;
                    el.Height = AddElipseWindow.y;
                    el.Fill = AddElipseWindow.boja;
                    el.StrokeThickness = AddElipseWindow.debljina;
                    el.Opacity = AddElipseWindow.providnost;

                    Canvass.Children.RemoveAt(index);
                    Canvass.Children.Insert(index, el);

                    if (AddElipseWindow.tekst != "")
                    {
                        TextBlock textBlock;
                        if(postojiTekst)
                        {
                            textBlock = (TextBlock)Canvass.Children[index + 1];
                        }
                        else
                        {
                            textBlock = new TextBlock();
                        }
                        textBlock.Text = AddElipseWindow.tekst;
                        textBlock.Foreground = AddElipseWindow.bojaTeksta;
                        textBlock.Opacity = AddElipseWindow.providnost;

                        if (postojiTekst)
                        {
                            Canvass.Children.RemoveAt(index + 1);
                            Canvass.Children.Insert(index + 1, textBlock);
                        }
                        else
                        {
                            System.Windows.Point p = (System.Windows.Point)el.Tag;
                            textBlock.Measure(new System.Windows.Size(Double.PositiveInfinity, Double.PositiveInfinity));
                            Canvas.SetLeft(textBlock, p.X + el.Width / 2 - textBlock.DesiredSize.Width / 2);
                            Canvas.SetTop(textBlock, p.Y + el.Height / 2 - textBlock.DesiredSize.Height / 2);
                            Canvass.Children.Add(textBlock);
                        }
                    }
                }
            }
        }

        private void PolygonMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Polygon polygon = (Polygon)e.OriginalSource;

            if (Canvass.Children.Contains(polygon))
            {
                int index = Canvass.Children.IndexOf(polygon);
                bool postojiTekst = false;
                if (Canvass.Children.Count > (index + 1) && Canvass.Children[index + 1] != null && Canvass.Children[index + 1] is TextBlock && ((TextBlock)Canvass.Children[index + 1]).Tag == polygon)
                {
                    postojiTekst = true;
                }

                AddPolygonWindow.Izmena = true;
                
                AddPolygonWindow.imeBoje = imeBoje((SolidColorBrush)polygon.Fill);
                AddPolygonWindow.imeBojeKonture = imeBoje((SolidColorBrush)polygon.Stroke);
                AddPolygonWindow.debljina = Convert.ToInt32(polygon.StrokeThickness);
                AddPolygonWindow.providnost = Convert.ToDouble(polygon.Opacity);
                if (postojiTekst)
                {
                    TextBlock textBlock = (TextBlock)Canvass.Children[index + 1];
                    AddPolygonWindow.tekst = textBlock.Text;
                    AddPolygonWindow.imeBojeTeksta = imeBoje((SolidColorBrush)textBlock.Foreground);
                }

                AddPolygonWindow addPolygonWindow = new AddPolygonWindow(); 
                addPolygonWindow.ShowDialog();

                if (!AddPolygonWindow.Closed)
                {
                    polygon.Fill = AddPolygonWindow.boja;
                    polygon.Stroke = AddPolygonWindow.bojaKonture;
                    polygon.StrokeThickness = AddPolygonWindow.debljina;
                    polygon.Opacity = AddPolygonWindow.providnost;

                    if (AddPolygonWindow.tekst != "")
                    {
                        TextBlock textBlock;
                        if (postojiTekst)
                        {
                            textBlock = (TextBlock)Canvass.Children[index + 1];
                        }
                        else
                        {
                            textBlock = new TextBlock();
                        }
                        textBlock.Text = AddPolygonWindow.tekst;
                        textBlock.Foreground = AddPolygonWindow.bojaTeksta;
                        textBlock.Opacity = AddPolygonWindow.providnost;

                        if (postojiTekst)
                        {
                            Canvass.Children.RemoveAt(index + 1);
                            Canvass.Children.Insert(index + 1, textBlock);
                        }
                        else
                        {
                            System.Windows.Point centar = (System.Windows.Point)polygon.Tag;
                            textBlock.Measure(new System.Windows.Size(Double.PositiveInfinity, Double.PositiveInfinity));
                            Canvas.SetLeft(textBlock, centar.X);
                            Canvas.SetTop(textBlock, centar.Y);
                            Canvass.Children.Add(textBlock);
                        }
                    }
                }
            }
        }

        private void TextMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TextBlock textBlock = (TextBlock)e.OriginalSource;

            if (Canvass.Children.Contains(textBlock))
            {
                int index = Canvass.Children.IndexOf(textBlock);

                AddTextWindow.Izmena = true;
                AddTextWindow.imeBoje = imeBoje((SolidColorBrush)textBlock.Foreground);
                AddTextWindow.tekst = textBlock.Text;
                AddTextWindow.velicina = Convert.ToInt32(textBlock.FontSize);
                AddTextWindow.providnost = Convert.ToDouble(textBlock.Opacity);

                AddTextWindow addTextWindow = new AddTextWindow();
                addTextWindow.ShowDialog();

                if (!AddTextWindow.Closed)
                {
                    textBlock.Foreground = AddTextWindow.boja;
                    textBlock.Text = AddTextWindow.tekst;
                    textBlock.FontSize = AddTextWindow.velicina;
                    textBlock.Opacity = AddTextWindow.providnost;

                    Canvass.Children.RemoveAt(index);
                    Canvass.Children.Insert(index, textBlock);
                }
            }
        }

        public string imeBoje(SolidColorBrush brush)
        {
            System.Windows.Media.Color color = ((SolidColorBrush)brush).Color;
            string selectedcolorname = "";
            foreach (var colorvalue in typeof(Colors).GetRuntimeProperties())
            {
                if ((System.Windows.Media.Color)colorvalue.GetValue(null) == color)
                {
                    selectedcolorname = colorvalue.Name;
                }
            }

            return selectedcolorname;
        }


        //From UTM to Latitude and longitude in decimal
        public static void ToLatLon(double utmX, double utmY, int zoneUTM, out double latitude, out double longitude)
		{
			bool isNorthHemisphere = true;

			var diflat = -0.00066286966871111111111111111111111111;
			var diflon = -0.0003868060578;

			var zone = zoneUTM;
			var c_sa = 6378137.000000;
			var c_sb = 6356752.314245;
			var e2 = Math.Pow((Math.Pow(c_sa, 2) - Math.Pow(c_sb, 2)), 0.5) / c_sb;
			var e2cuadrada = Math.Pow(e2, 2);
			var c = Math.Pow(c_sa, 2) / c_sb;
			var x = utmX - 500000;
			var y = isNorthHemisphere ? utmY : utmY - 10000000;

			var s = ((zone * 6.0) - 183.0);
			var lat = y / (c_sa * 0.9996);
			var v = (c / Math.Pow(1 + (e2cuadrada * Math.Pow(Math.Cos(lat), 2)), 0.5)) * 0.9996;
			var a = x / v;
			var a1 = Math.Sin(2 * lat);
			var a2 = a1 * Math.Pow((Math.Cos(lat)), 2);
			var j2 = lat + (a1 / 2.0);
			var j4 = ((3 * j2) + a2) / 4.0;
			var j6 = ((5 * j4) + Math.Pow(a2 * (Math.Cos(lat)), 2)) / 3.0;
			var alfa = (3.0 / 4.0) * e2cuadrada;
			var beta = (5.0 / 3.0) * Math.Pow(alfa, 2);
			var gama = (35.0 / 27.0) * Math.Pow(alfa, 3);
			var bm = 0.9996 * c * (lat - alfa * j2 + beta * j4 - gama * j6);
			var b = (y - bm) / v;
			var epsi = ((e2cuadrada * Math.Pow(a, 2)) / 2.0) * Math.Pow((Math.Cos(lat)), 2);
			var eps = a * (1 - (epsi / 3.0));
			var nab = (b * (1 - epsi)) + lat;
			var senoheps = (Math.Exp(eps) - Math.Exp(-eps)) / 2.0;
			var delt = Math.Atan(senoheps / (Math.Cos(nab)));
			var tao = Math.Atan(Math.Cos(delt) * Math.Tan(nab));

			longitude = ((delt * (180.0 / Math.PI)) + s) + diflon;
			latitude = ((lat + (1 + e2cuadrada * Math.Pow(Math.Cos(lat), 2) - (3.0 / 2.0) * e2cuadrada * Math.Sin(lat) * Math.Cos(lat) * (tao - lat)) * (tao - lat)) * (180.0 / Math.PI)) + diflat;
		}
	}
}
