using System.Collections.Generic;

namespace CodingChallenge.Models
{
    public class Format1Input
    {
        public List<StoreInformation> StoreInformation { get; set; } = new List<StoreInformation>();
        public List<Transaction> transactions { get; set; } = new List<Transaction>();

        public Format1Input()
        {

        }
    }

    public class StoreInformation
    {
        public string? Name { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? TimeZone { get; set; }

        public StoreInformation()
        {

        }
    }

    public class Transaction
    {
        public string? receiptId { get; set; }
        public string? createdDateUTC { get; set; }
        public int? budtenderId { get; set; }
        public int? customerId { get; set; }
        public string? transactionType { get; set; }
        public List<Item> items { get; set; } = new List<Item>();
        public List<DiscountInfo> transactionDiscounts { get; set; } = new List<DiscountInfo>();

        public Transaction()
        {

        }
    }

    public class Item
    {
        public int? itemId { get; set; }
        public string? category { get; set; }
        public decimal? cogs { get; set; }
        public int? quantity { get; set; }
        public decimal? basePrice { get; set; }
        public decimal? pricePaid { get; set; }
        public List<DiscountInfo> itemDiscount { get; set; } = new List<DiscountInfo>();

        public Item()
        {

        }
    }

    public class DiscountInfo
    {
        public int? discountId { get; set; }
        public string? name { get; set; }
        public decimal? amount { get; set; }

        public DiscountInfo()
        {

        }
    }
}
