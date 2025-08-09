namespace SOJIBENTERPRISE.Domain
{
    public class SupplierPaymentHistoryDTO
    {
        public int Id { get; set; }
        public double? TotalAmountThisPurchase { get; set; }
        public double AmountPaid { get; set; }
        public double TotalDueBeforePayment { get; set; }
        public double TotalDueAfterPayment { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionID { get; set; }
        public string Number { get; set; }
        public string Comments { get; set; }
        public bool IsDisabled { get; set; }
    }
}
