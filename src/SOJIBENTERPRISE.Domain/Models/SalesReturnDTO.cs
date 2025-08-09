namespace SOJIBENTERPRISE.Domain.Models
{
    public class SalesReturnDTO
    {
        public int OrderId { get; set; }
        public int SupplierId { get; set; }
        public int SellQuentity { get; set; }
        public int ReturnQuentity { get; set; }
        public double ReturnAmount { get; set; }
        public double Amount { get; set; }
        public int Quentity { get; set; }
        public DateTime Date { get; set; }
        public string DateFormated
        {
            get
            {
                return Date.ToString("MMM-dd-yy");
            }
        }

        public double TotalAmount
        {
            get
            {
                return (Amount + ReturnAmount);
            }
        }
        public string Area { get; set; }
        public string SupplierName { get; set; }
    }

    public class SalesHistoryDTO
    {
        public string SupplierName { get; set; }
        public string ProductName { get; set; }
        public int Quentity { get; set; }
        public int SellQuentity { get; set; }
        public int ReturnQuentity { get; set; }
        public double TotalAmount => Amount + ReturnAmount;
        public double Amount { get; set; }
        public double ReturnAmount { get; set; }
        
        public DateTime Date { get; set; }
        public string DateFormated
        {
            get
            {
                return Date.ToString("MMM-dd-yy");
            }
        }
    }
}