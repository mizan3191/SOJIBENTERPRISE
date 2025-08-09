namespace SOJIBENTERPRISE.Domain
{
    public class DSRShopDue
    {
        public int Id { get; set; }

        public int? CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        public int? DSRCustomerId { get; set; }
        public virtual Customer DSRCustomer { get; set; }

        public int ShopId { get; set; }
        public virtual Shop Shop { get; set; }

        public int? OrderId { get; set; }
        public virtual Order Order { get; set; }

        public double DueAmount { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;

        public string DueHistory
        {
            get
            {
                string shopName = Shop?.Name ?? "";
                string area = Shop?.Area ?? "";
                string customerName = Customer?.Name ?? "";

                return $"{shopName}({area})({customerName})";
            }
        }
    }
}
