namespace SOJIBENTERPRISE.Domain
{
    public class PurchaseDetail
    {
        public int Id { get; set; }
        public int PurchaseId { get; set; }
        public virtual Purchase Purchase { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double Price { get; set; }
        public double? Discount { get; set; }
    }
}
