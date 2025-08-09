namespace SOJIBENTERPRISE.Domain
{
    public class SRPaymentHistory
    {
        public int Id { get; set; }
        public double AmountPaid { get; set; }  
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        public int? PaymentMethodId { get; set; }
        public virtual PaymentMethod PaymentMethod { get; set; }

        public bool IsDeleted { get; set; }
    }
}