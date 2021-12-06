using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace CodingChallenge.Models
{
    public class Format1
    {
        public List<StoreInformation> StoreInformation { get; set; } = new List<StoreInformation>();
        public List<Transaction> transactions { get; set; } = new List<Transaction>();

        public Format1()
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
        public int budtenderId { get; set; }
        public int customerId { get; set; }
        public string transactionType { get; set; }
        public List<Item> items { get; set; } = new List<Item>();
        public List<DiscountInfo> transactionDiscounts { get; set; } = new List<DiscountInfo>();

        public Transaction()
        {

        }
    }

    public class Item
    {
        public int itemId { get; set; }
        public string category { get; set; }
        public double cogs { get; set; }
        public int quantity { get; set; }
        public double basePrice { get; set; }
        public double pricePaid { get; set; }
        public List<DiscountInfo> itemDiscount { get; set; }

        public Item()
        {

        }
    }

    public class DiscountInfo
    {
        public int discountId { get; set; }
        public string name { get; set; }
        public double amount { get; set; }

        public DiscountInfo()
        {

        }
    }
}
