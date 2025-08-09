namespace SOJIBENTERPRISE.Domain
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public int Quantity { get; set; }
        public int FreeQuantity { get; set; }
        public int? ReturnQuantity { get; set; }
        public double UnitPrice { get; set; }
        public double BuyingPrice { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }
    }
}