using System.Collections.Generic;

namespace CodingChallenge.Models
{
    public class Format1Json
    {
        public Totals? Totals { get; set; }
        public List<Daily>? Daily { get; set; }

        public Format1Json()
        {

        }

    }

    public class Daily
    {
        public string Day { get; set; }
        public int? Sales { get; set; }
        public int? Returns { get; set; }
        public int? Voids { get; set; }
        public double? Revenue { get; set; }
        public double? Discounts { get; set; }
        public double? Profit { get; set; }
        public int? NetItemCount { get; set; }

        public Daily()
        {

        }
    }

    public class Totals
    {
        public double? RevenueTotal { get; set; }
        public double? DiscountTotal { get; set; }
        public double? Profit { get; set; }
        public int? TransCount { get; set; }
        public int? ItemCount { get; set; }
        public int? TotalSales { get; set; }
        public int? TotalReturns { get; set; }
        public int? TotalVoids { get; set; }

        public Totals()
        {

        }
    }
}
