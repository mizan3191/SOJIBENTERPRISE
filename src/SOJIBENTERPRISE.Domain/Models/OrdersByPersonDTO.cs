using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SOJIBENTERPRISE.Domain
{
    public class OrdersByPersonDTO
    {
        public int OrderId { get; set; }
        public string ProductsName { get; set; }
        public string Area { get; set; }
        public DateTime OrderDate { get; set; }
        public string DateFormate => OrderDate.ToString("dd-MMM-yyyy (ddd)");
    }
}
