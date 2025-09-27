namespace SOJIBENTERPRISE.Domain
{
    public class Purchase
    {
        public Purchase()
        {
            SupplierPaymentHistories = new HashSet<SupplierPaymentHistory>();
        }

        public int Id { get; set; } // Primary Key

        public int SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }

        public double TotalAmount { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;

        public bool IsDeleted { get; set; }
        public string Comments { get; set; }

        public virtual ICollection<SupplierPaymentHistory> SupplierPaymentHistories { get; set; }

    }    
}