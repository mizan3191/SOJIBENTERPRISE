namespace SOJIBENTERPRISE.Domain
{
    public class CustomerProductReturnDetails
    {
        public int Id { get; set; }
        public int CustomerProductReturnId { get; set; }
        public virtual CustomerProductReturn CustomerProductReturn { get; set; }

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public int FreeQuantity { get; set; }
        public int Quantity { get; set; }
        public int ReturnQuantity { get; set; }
        public double UnitPrice { get; set; }
        public double Price { get; set; }
        public double ReturnPrice { get; set; }
        public double Discount { get; set; }
    }
}
