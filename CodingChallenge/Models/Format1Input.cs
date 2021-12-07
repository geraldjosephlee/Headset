using System.Collections.Generic;

namespace CodingChallenge.Models
{
    public class Format1Input
    {
        public List<StoreInformation>? StoreInformation { get; set; }
        public List<Transaction>? transactions { get; set; }

        public Format1Input()
        {

        }
    }

    public class StoreInformation
    {
        public string Name { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string TimeZone { get; set; }

        public StoreInformation()
        {

        }
    }

    public class Transaction
    {
        public string receiptId { get; set; }
        public string createdDateUTC { get; set; }
        public int? budtenderId { get; set; }
        public int? customerId { get; set; }
        public string transactionType { get; set; }
        public List<Item>? items { get; set; }
        public List<DiscountInfo>? transactionDiscounts { get; set; }

        public Transaction()
        {

        }
    }

    public class Item
    {
        public int? itemId { get; set; }
        public string category { get; set; }
        public double? cogs { get; set; }
        public int? quantity { get; set; }
        public double? basePrice { get; set; }
        public double? pricePaid { get; set; }
        public List<DiscountInfo>? itemDiscount { get; set; }

        public Item()
        {

        }
    }

    public class DiscountInfo
    {
        public int? discountId { get; set; }
        public string name { get; set; }
        public double? amount { get; set; }

        public DiscountInfo()
        {

        }
    }
}
