using CodingChallenge.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows;
using System.Text.Json;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.IO;

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
            // Get the data
            var uri = @"https://api.json-generator.com/templates/M-rnKhH0Z6uf/data?access_token=edfgbmrjms4yldm77epct3jgyo3q65wxjinddzmc";
            var format1 = await getData(uri);

            // Parse the data
            var format1JSON = parseData(format1);

            // Print to file
            File.WriteAllText(@"C:\Users\Public\challengeOutput.json", format1JSON);            
        }

        private async Task<List<List<Format1Input>>> getData(string uri)
        {
            // Get Format1 JSON data, ignore nulls
            var client = new HttpClient();
            var response = await client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var format1 = JsonSerializer.Deserialize<List<List<Format1Input>>>(json, new JsonSerializerOptions()
            { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });

            return format1;
        }

        private string parseData(List<List<Format1Input>> format1)
        {
            // Initialize output
            var format1JSON = new Format1Output();

            // Initialize total metrics
            var totalRevenue = 0.0;
            var totalDiscount = 0.0;
            var totalProfit = 0.0;
            var totalTransactions = 0;
            var totalItems = 0;
            var totalSales = 0;
            var totalReturns = 0;
            var totalVoids = 0;

            // Get transactions
            var transactions = format1[1][0].transactions;

            // Get list of days 
            var days = transactions?.Select(t => t.createdDateUTC.Split('T')[0]).Distinct().ToList();

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
                var dailyNetRevenue = 0.0;
                var dailyTotalDiscounts = 0.0;
                var dailyProfit = 0.0;
                var dailyNetItemCount = 0;
                foreach (var dayTransaction in dayTransactions)
                {
                    // Items sold
                    if (dayTransaction.transactionType == "Sale")
                    {
                        dailyItemsSold += dayTransaction.items.Count;
                    }
                    else if (dayTransaction.transactionType == "Return")
                    {
                        dailyItemsSold -= dayTransaction.items.Count;
                    }

                    // Discounts (Transaction)
                    dailyTotalDiscounts += dayTransaction.transactionDiscounts.Where(td => td.amount.HasValue).ToList().Sum(td => td.amount.Value);

                    // Item Metrics
                    foreach (var item in dayTransaction.items)
                    {
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
                format1JSON.Daily.Add(
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
            format1JSON.Totals = new Totals(totalRevenue, totalDiscount, totalProfit, 
                totalTransactions, totalItems, totalSales, totalReturns, totalVoids);

            // Serialize to JSON
            return JsonSerializer.Serialize(format1JSON, new JsonSerializerOptions() { WriteIndented = true});
        }
    }
}
