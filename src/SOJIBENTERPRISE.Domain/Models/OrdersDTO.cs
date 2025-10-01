using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SOJIBENTERPRISE.Domain
{
    public class OrdersDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderDateFormate => OrderDate.ToString("dd-MMM-yyyy (ddd)");
        public double TotalPrice { get; set; }
        public double TotalGetAmount { get; set; }
        public double ShopDueAmount { get; set; }
        public double ExpenseAmount { get; set; }
        public string Address { get; set; }
        public bool IsLock { get; set; }
    }
}