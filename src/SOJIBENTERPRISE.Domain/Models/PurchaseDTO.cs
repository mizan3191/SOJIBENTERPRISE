using System.Globalization;

namespace SOJIBENTERPRISE.Domain
{
    public class PurchaseDTO
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
        public DateTime OrderDate { get; set; }
        public double TotalPrice { get; set; }

        public string OrderDateFormate => OrderDate.ToString("dd-MMM-yyyy (ddd)", new CultureInfo("bn-BD"));
    }
}