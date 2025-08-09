using System.ComponentModel.DataAnnotations;

namespace SOJIBENTERPRISE.Domain
{
    public class Expense
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ExpenseDate { get; set; } = DateTime.UtcNow;
        
        [Required]
        public int ExpenseTypeId { get; set; }
        public ExpenseType ExpenseType { get; set; } // e.g., Electricity Bill, Salary

        [Required]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } // e.g., Electricity Bill, Salary


        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public double Amount { get; set; }

        [Required]
        public int PaymentMethodId { get; set; } // Cash, Bank, etc.
        public PaymentMethod PaymentMethod { get; set; } // Cash, Bank, etc.

        [StringLength(100)]
        public string ReferenceNumber { get; set; } // Bill or invoice number

        [StringLength(100)]
        public string PaidTo { get; set; } // e.g., staff, landlord, supplier

        public bool IsDeleted { get; set; } = false;
    }
}