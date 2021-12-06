using CodingChallenge.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows;
using System.Text.Json;
using System.Linq;

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
            var response = await client.GetAsync
                (@"https://api.json-generator.com/templates/M-rnKhH0Z6uf/data?access_token=edfgbmrjms4yldm77epct3jgyo3q65wxjinddzmc");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var format1 = JsonSerializer.Deserialize<List<List<Format1>>>(json, new JsonSerializerOptions() 
                { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull});

            // Get transactions
            var transactions = format1[1][0].transactions;

            // Get list of all days
            var days = transactions.Select(s => s.createdDateUTC).Distinct().ToList();

            // For each day...
            foreach (var day in days)
            {
                // Get transactions
                var dayTransactions = transactions.Where(dt => dt.createdDateUTC == day);

                // Get transaction types
                var dayTransactionTypes = dayTransactions.Select(dtt => dtt.transactionType);

                // Number of sales
                var saleCount = dayTransactionTypes.Where(dtt => dtt == @"Sale").Count();

                // Number of returns
                var returnCount = dayTransactionTypes.Where(dtt => dtt == @"Return").Count();

                // Number of voided transactions
                var voidCount = dayTransactionTypes.Where(dtt => dtt == @"Void").Count();

                // Number of items sold
                var itemsSold = 0;
                foreach (var dayTransaction in dayTransactions)
                {
                    if (dayTransaction.items.Count > 0)
                    {                        
                        if (dayTransaction.transactionType == "Sale")
                        {
                            itemsSold += dayTransaction.items.Count;
                        }
                        else if (dayTransaction.transactionType == "Return")
                        {
                            itemsSold -= dayTransaction.items.Count;
                        }
                    }
                }

                // Net Revenue (Need to review how to cast int? to double?)
                double? netRevenue = 0.0;
                foreach (var dayTransaction in dayTransactions)
                {
                    foreach (var item in dayTransaction.items)
                    {
                        var revenue = (double?)item.quantity * item.pricePaid;
                        netRevenue += revenue;
                    }
                }

                // Total of Discounts applied
                double? totalDiscounts = 0.0;
                foreach (var dayTransaction in dayTransactions)
                {
                    // Sum transaction discounts list
                    totalDiscounts += dayTransaction.transactionDiscounts.Sum(td => td.amount);

                    // Sum item discounts
                    foreach (var item in dayTransaction.items)
                    {
                        totalDiscounts += item.itemDiscount.Sum(id => id.amount);
                    }
                }

                // Profit for each day (Sale total - cogs)
                double? profit = 0.0;
                foreach (var dayTransaction in dayTransactions)
                {
                    dayTransaction.items.ForEach(i => profit += i.quantity * (i.pricePaid - i.cogs));
                }
            }
        }
    }
}
