using CodingChallenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Text.Json;

namespace CodingChallenge
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Get the data
            getData();
        }

        private async void getData()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(@"https://api.json-generator.com/templates/M-rnKhH0Z6uf/data?access_token=edfgbmrjms4yldm77epct3jgyo3q65wxjinddzmc");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var format1 = JsonSerializer.Deserialize<List<List<Format1>>>(json, new JsonSerializerOptions() { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull});
        }
    }
}
