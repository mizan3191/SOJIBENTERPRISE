namespace SOJIBENTERPRISE.Domain
{
    public class CustomerPaymentHistoryDTO
    {
        public int Id { get; set; }
        public double? TotalAmountThisOrder { get; set; }
        public double AmountPaid { get; set; }
        public double TotalDueBeforePayment { get; set; }
        public double TotalDueAfterPayment { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionID { get; set; }
        public string Number { get; set; }
        public bool IsDisabled { get; set; }
    }
}
