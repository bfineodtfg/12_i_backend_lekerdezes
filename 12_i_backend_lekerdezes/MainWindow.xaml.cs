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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Http;
using Newtonsoft.Json;

namespace _12_i_backend_lekerdezes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int NumOfKacsa = 0;
        int longestKacsa = 0;
        int shortestKacsa = int.MaxValue;
        int borderHeight = -1;
        int allBorderWidth = 0;
        public MainWindow()
        {
            InitializeComponent();
            CreateTextBlock();
        }

        async void CreateTextBlock()
        {
            kacsak.Children.Clear();
            HttpClient client = new HttpClient();
            string url = "http://127.0.0.1:3000/kacsa";
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                string stringResponse = await response.Content.ReadAsStringAsync();
                List<KacsaClass> kacsaList = JsonConvert.DeserializeObject<List<KacsaClass>>(stringResponse);
                NumOfKacsa = 0;
                longestKacsa = 0;
                shortestKacsa = int.MaxValue;
                foreach (KacsaClass item in kacsaList)
                {
                    NumOfKacsa++;
                    if (shortestKacsa > item.length)
                    {
                        shortestKacsa = item.length;
                    }
                    if (longestKacsa < item.length)
                    {
                        longestKacsa = item.length;
                    }

                    Border oneBorder = new Border();
                    kacsak.Children.Add(oneBorder);
                    Grid oneGrid = new Grid();
                    oneBorder.Child = oneGrid;
                    RowDefinition firstRow = new RowDefinition();
                    RowDefinition secondRow = new RowDefinition();
                    RowDefinition thirdRow = new RowDefinition();
                    oneGrid.RowDefinitions.Add(firstRow);
                    oneGrid.RowDefinitions.Add(secondRow);
                    oneGrid.RowDefinitions.Add(thirdRow);

                    TextBlock NameTextBlock = new TextBlock();
                    TextBlock LengthTextBlock = new TextBlock();
                    Button SellButton = new Button();

                    oneGrid.Children.Add(NameTextBlock);
                    oneGrid.Children.Add(LengthTextBlock);
                    oneGrid.Children.Add(SellButton);

                    Grid.SetRow(LengthTextBlock, 1);
                    Grid.SetRow(SellButton, 2);

                    NameTextBlock.Text = $"Név: {item.name}";
                    LengthTextBlock.Text = $"Hossz: {item.length}";
                    SellButton.Content = "Eladás";

                    SellButton.Click += async (s, e) =>
                    {
                        HttpClient deleteClient = new HttpClient();
                        string deleteUrl = "http://127.1.1.1:3000/kacsa";
                        try
                        {
                            var jsonObject = new
                            {
                                id = item.id
                            };
                            string jsonString = JsonConvert.SerializeObject(jsonObject);
                            HttpRequestMessage request = new HttpRequestMessage();
                            request.Method = HttpMethod.Delete;
                            request.RequestUri = new Uri(deleteUrl);
                            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                            HttpResponseMessage deleteResponse = await client.SendAsync(request);
                            //deleteClient.DeleteAsync(url);
                            deleteResponse.EnsureSuccessStatusCode();
                            kacsak.Children.Remove(oneBorder);
                            kacsaList.Remove(item);
                            NumOfKacsa = kacsaList.Count;
                            shortestKacsa = kacsaList.Min(x => x.length);
                            longestKacsa = kacsaList.Max(x => x.length);

                            KacsaDarab.Text = NumOfKacsa + " darab";
                            KacsaMin.Text = shortestKacsa + " cm";
                            KacsaMax.Text = longestKacsa + " cm";
                        }
                        catch (Exception error)
                        {
                            MessageBox.Show(error.Message);
                        }
                    };

                    oneBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#484848"));
                    oneBorder.Margin = new Thickness(5);
                    oneBorder.CornerRadius = new CornerRadius(10);
                    oneBorder.Padding = new Thickness(10);

                    if (borderHeight < 0)
                    {
                        borderHeight = (int)oneBorder.Height;
                    }
                    allBorderWidth += (int)oneBorder.Width + (int)oneBorder.Margin.Left + (int)oneBorder.Margin.Right;
                    NameTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    LengthTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));

                    //oneBlock.Text = $"Kacsa neve: {item.name}, kacsa hossza: {item.length}";

                }


                //wrapPanel sor szám magasság alapján:
                int row = (int)kacsak.Height / borderHeight;

                //wrapPanel sor szám szélesség alapján:
                int ROW = (int)Math.Ceiling(allBorderWidth / kacsak.Width);
                //kacsak.Height = ROW * borderHeight;

                KacsaDarab.Text = NumOfKacsa + " darab";
                KacsaMin.Text = shortestKacsa + " cm";
                KacsaMax.Text = longestKacsa + " cm";
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }



        }
        async void AddKacsa(object s, EventArgs e)
        {
            HttpClient client = new HttpClient();
            string url = "http://127.1.1.1:3000/kacsa";

            try
            {
                var jsonObject = new
                {
                    name = KacsaNameTextBox.Text,
                    length = KacsaLengthTextBox.Text
                };

                string jsonData = JsonConvert.SerializeObject(jsonObject);
                StringContent data = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(url, data);
                response.EnsureSuccessStatusCode();
                CreateTextBlock();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
            //MessageBox.Show($"Kacsa neve: {nev.Text}, kacsa hossza: {hossz.Text}cm");
        }
    }
}
