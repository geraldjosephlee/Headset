using System.Collections.Generic;

namespace CodingChallenge.Models
{
    public class Format1Output
    {
        public Totals Totals { get; set; } = new Totals();
        public List<Daily> Daily { get; set; } = new List<Daily>();

        public Format1Output()
        {

        }
    }

    public class Daily
    {
        public string Day { get; set; } = string.Empty;
        public int Sales { get; set; }
        public int Returns { get; set; }
        public int Voids { get; set; }
        public double Revenue { get; set; }
        public double Discounts { get; set; }
        public double Profit { get; set; }
        public int NetItemCount { get; set; }

        public Daily()
        {

        }

        public Daily(string day, int sales, int returns, 
            int voids, double revenue, double discounts, 
            double profit, int netItemCount)
        {
            Day = day;
            Sales = sales;
            Returns = returns;
            Voids = voids;
            Revenue = revenue;
            Discounts = discounts;
            Profit = profit;
            NetItemCount = netItemCount;
        }
    }

    public class Totals
    {
        public double RevenueTotal { get; set; }
        public double DiscountTotal { get; set; }
        public double Profit { get; set; }
        public int TransCount { get; set; }
        public int ItemCount { get; set; }
        public int TotalSales { get; set; }
        public int TotalReturns { get; set; }
        public int TotalVoids { get; set; }

        public Totals()
        {

        }

        public Totals(double revenueTotal, double discountTotal, 
            double profit, int transCount, int itemCount, 
            int totalSales, int totalReturns, int totalVoids)
        {
            RevenueTotal = revenueTotal;
            DiscountTotal = discountTotal;
            Profit = profit;
            TransCount = transCount;
            ItemCount = itemCount;
            TotalSales = totalSales;
            TotalReturns = totalReturns;
            TotalVoids = totalVoids;
        }
    }
}
