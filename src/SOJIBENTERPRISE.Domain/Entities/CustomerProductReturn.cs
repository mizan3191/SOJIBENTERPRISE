using System.ComponentModel.DataAnnotations.Schema;

namespace SOJIBENTERPRISE.Domain
{
    public class CustomerProductReturn
    {
        public CustomerProductReturn()
        {
            CustomerProductReturnDetails = new HashSet<CustomerProductReturnDetails>();
            CustomerPaymentHistories = new HashSet<CustomerPaymentHistory>();
        }

        public int Id { get; set; }
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        [ForeignKey("Order")] // Explicitly map OrderId to Order navigation property
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        public string PaymentMethod { get; set; }
        public double TotalAmount { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; }

        public virtual ICollection<CustomerProductReturnDetails> CustomerProductReturnDetails { get; set; }
        public virtual ICollection<CustomerPaymentHistory> CustomerPaymentHistories { get; set; }

    }
}
