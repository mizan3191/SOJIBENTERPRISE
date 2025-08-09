namespace SOJIBENTERPRISE.Domain
{
    public class DamageProductHandoverTempDuePaymentSummary
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public DateTime Date { get; set; }
        public double DueAmount { get; set; }
        public double PaidAmount { get; set; }
    }
}
