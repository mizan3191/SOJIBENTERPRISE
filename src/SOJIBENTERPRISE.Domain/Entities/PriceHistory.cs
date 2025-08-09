namespace SOJIBENTERPRISE.Domain
{
    public class PriceHistory
    {
        public int Id { get; set; } // Primary Key
        public double BuyingOldPrice { get; set; }
        public double BuyingNewPrice { get; set; }
        public double SellingOldPrice { get; set; }
        public double SellingNewPrice { get; set; }
        public DateTime Date { get; set; }
        public int ProductId { get; set; } // Foreign Key
        public virtual Product Product { get; set; } // Navigation Property
    }
}
