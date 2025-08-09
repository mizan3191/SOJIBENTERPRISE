namespace SOJIBENTERPRISE.Domain
{
    public abstract class DSRShopDueDTOBase
    {
        public int Id { get; set; }

        public string CustomerName { get; set; }
        
        public string ShopName { get; set; }
        public double DueAmount { get; set; }
        public DateTime? Date { get; set; }
    }

    public class DSRShopDueDTO : DSRShopDueDTOBase
    {
        public string IssuedBYCustomerName { get; set; }
        public string ShopShopOwner { get; set; }
        public string ShopArea { get; set; }
        public string ShopNumber { get; set; }
        public int? OrderId { get; set; }
        
    }

    public class DSRShopDueForOrderDTO : DSRShopDueDTOBase
    {
        public string DSRCustomerName { get; set; }
        public int? OrderId { get; set; }
    }
}
