namespace SOJIBENTERPRISE.Domain
{
    public class SRDiscountTempShopSummary
    {
        public int SRDiscountId { get; set; }
        public string SRName { get; set; }
        public DateTime Date { get; set; }
        public double DueAmount { get; set; }
        public double PaidAmount { get; set; }
    }
}