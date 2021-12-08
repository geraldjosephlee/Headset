using CodingChallenge.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows;
using System.Text.Json;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.IO;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Media;

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

            // Main method
            main();
        }

        private async void main()
        {
            // Define endpoint
            var uri = @"https://api.json-generator.com/templates/M-rnKhH0Z6uf/data?access_token=edfgbmrjms4yldm77epct3jgyo3q65wxjinddzmc";

            // Get json data
            var json = await getJson(uri);

            // Deserialize json data
            var input = getInput(json);

            // Parse the data
            var output = parseData(input);

            // Create charts
            createCharts(output);

            // Export to file
            exportJson(output);
        }

        private async Task<string> getJson(string uri)
        {
            // Get Format1 JSON data, ignore nulls
            var client = new HttpClient();
            var response = await client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            return json;
        }

        private List<List<Format1Input>> getInput(string json)
        {
            // Deserialize
            var format1 = JsonSerializer.Deserialize<List<List<Format1Input>>>(json, new JsonSerializerOptions()
            { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });

            return format1;
        }

        private Format1Output parseData(List<List<Format1Input>> format1)
        {
            // Initialize output
            var output = new Format1Output();

            // Initialize total metrics
            var totalRevenue = 0m;
            var totalDiscount = 0m;
            var totalProfit = 0m;
            var totalTransactions = 0;
            var totalItems = 0;
            var totalSales = 0;
            var totalReturns = 0;
            var totalVoids = 0;

            // Get transactions
            var transactions = format1[1][0].transactions;

            // Get list of days 
            var days = transactions.Select(t => t.createdDateUTC.Split('T')[0]).Distinct().ToList();

            // For each day...
            foreach (var day in days)
            {
                // Get transactions
                var dayTransactions = transactions.Where(dt => dt.createdDateUTC.Split('T')[0] == day);

                // Get transaction types
                var dayTransactionTypes = dayTransactions.Select(dtt => dtt.transactionType);

                // Number of sales
                var dailySaleCount = dayTransactionTypes.Where(dtt => dtt == @"Sale").Count();

                // Number of returns
                var dailyReturnCount = dayTransactionTypes.Where(dtt => dtt == @"Return").Count();

                // Number of voided transactions
                var dailyVoidCount = dayTransactionTypes.Where(dtt => dtt == @"Void").Count();

                // Tally daily metics in one loop
                var dailyItemsSold = 0;
                var dailyNetRevenue = 0m;
                var dailyTotalDiscounts = 0m;
                var dailyProfit = 0m;
                var dailyNetItemCount = 0;
                foreach (var dayTransaction in dayTransactions)
                {
                    // Discounts (Transaction)
                    dailyTotalDiscounts += dayTransaction.transactionDiscounts.Where(td => td.amount.HasValue).ToList().Sum(td => td.amount.Value);

                    // Item Metrics
                    foreach (var item in dayTransaction.items)
                    {
                        // Items sold
                        if (item.quantity.HasValue)
                        {
                            var tt = dayTransaction.transactionType;
                            if (tt == "Sale")
                            {
                                dailyItemsSold += item.quantity.Value;
                            }
                            else if (tt == "Return")
                            {
                                dailyItemsSold -= item.quantity.Value;
                            }
                        }

                        // Discounts (Item)
                        dailyTotalDiscounts += item.itemDiscount.Where(id => id.amount.HasValue).ToList().Sum(id => id.amount.Value);

                        // Check for nulls
                        if (item.quantity.HasValue && item.pricePaid.HasValue)
                        {
                            // Revenue
                            dailyNetRevenue += item.quantity.Value * item.pricePaid.Value;

                            // Check for null
                            if (item.cogs.HasValue)
                            {
                                // Profit
                                dailyProfit += item.quantity.Value * (item.pricePaid.Value - item.cogs.Value);
                            }
                        }

                        // Item count
                        dailyNetItemCount++;
                    }
                }

                // Add to daily collection
                output.Daily.Add(
                    new Daily(day, dailySaleCount, dailyReturnCount,
                            dailyVoidCount, dailyNetRevenue, dailyTotalDiscounts,
                            dailyProfit, dailyNetItemCount));

                // Tally totals
                totalRevenue += dailyNetRevenue;
                totalDiscount += dailyTotalDiscounts;
                totalProfit += dailyProfit;
                totalTransactions += dayTransactions.Count();
                totalItems += dailyNetItemCount;
                totalSales += dailySaleCount;
                totalReturns += dailyReturnCount;
                totalVoids += dailyVoidCount;
            }

            // Append totals
            output.Totals = new Totals(totalRevenue, totalDiscount, totalProfit,
                totalTransactions, totalItems, totalSales, totalReturns, totalVoids);

            // Sort output daily ascending order
            output.Daily = output.Daily.OrderBy(d => DateTime.Parse(d.Day)).ToList();

            // Return output object
            return output;
        }

        private void createCharts(Format1Output output)
        {
            GridTotal.DataContext = new TotalContext(output);
            GridDaily.DataContext = new DailyContext(output);
        }

        public class TotalContext
        {
            public SeriesCollection SeriesCollection { get; set; }
            public string[] Labels { get; set; }
            public Func<decimal, string> Formatter { get; set; }

            public TotalContext(Format1Output output)
            {
                // Lables
                Labels = new[] { "Revenue", "Discount", "Profit", "Transactions", "Items", "Sale", "Return", "Void" };

                // Show values
                var values = new ChartValues<decimal>();
                values.Add(output.Totals.RevenueTotal);
                values.Add(output.Totals.DiscountTotal);
                values.Add(output.Totals.Profit);
                values.Add(output.Totals.TransCount);
                values.Add(output.Totals.ItemCount);
                values.Add(output.Totals.TotalSales);
                values.Add(output.Totals.TotalReturns);
                values.Add(output.Totals.TotalVoids);

                // Series Collection
                SeriesCollection = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Totals",
                        Values = values
                    }
                };

                Formatter = value => value.ToString("N");
            }
        }

        public class DailyContext
        {
            public SeriesCollection SeriesCollection { get; set; }
            public string[] Labels { get; set; }
            public Func<decimal, string> YFormatter { get; set; }

            public DailyContext(Format1Output output)
            {
                // Get Values
                var salesValues = new ChartValues<int>();
                var returnValues = new ChartValues<int>();
                var voidValues = new ChartValues<int>();
                var revenueValues = new ChartValues<decimal>();
                var discountValues = new ChartValues<decimal>();
                var profitValues = new ChartValues<decimal>();
                var netItemCountValues = new ChartValues<int>();

                foreach (var daily in output.Daily)
                {
                    salesValues.Add(daily.Sales);
                    returnValues.Add(daily.Returns);
                    voidValues.Add(daily.Voids);
                    revenueValues.Add(daily.Revenue);
                    discountValues.Add(daily.Discounts);
                    profitValues.Add(daily.Profit);
                    netItemCountValues.Add(daily.NetItemCount);
                }

                SeriesCollection = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Sales",
                        Values = salesValues,
                    },
                    new LineSeries
                    {
                        Title = "Returns",
                        Values = returnValues,
                    },
                    new LineSeries
                    {
                        Title = "Voids",
                        Values = voidValues,
                    },
                    new LineSeries
                    {
                        Title = "Revenue",
                        Values = revenueValues,
                    },
                    new LineSeries
                    {
                        Title = "Discounts",
                        Values = discountValues,
                    },
                    new LineSeries
                    {
                        Title = "Profit",
                        Values = profitValues,
                    },
                    new LineSeries
                    {
                        Title = "Items",
                        Values = netItemCountValues,
                    },
                };

                Labels = output.Daily.Select(d => d.Day).ToArray();
                YFormatter = value => value.ToString("N");
            }
        }

        private void exportJson(Format1Output format1Ouput)
        {
            // Serialize
            var format1OuputString = JsonSerializer.Serialize(format1Ouput, new JsonSerializerOptions() { WriteIndented = true });

            //Print to file
            File.WriteAllText(@"C:\Users\Public\challengeOutput.json", format1OuputString);
        }

        private void ShowJson_Click(object sender, RoutedEventArgs e)
        {
            // Show in file explorer
            System.Diagnostics.Process.Start("explorer.exe", @"C:\Users\Public");
        }
    }
}
