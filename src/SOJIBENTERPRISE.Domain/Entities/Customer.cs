using System.ComponentModel.DataAnnotations.Schema;

namespace SOJIBENTERPRISE.Domain
{
    public class Customer
    {
        public Customer()
        {
            Orders = new HashSet<Order>();
            CustomerProductReturn = new HashSet<CustomerProductReturn>();
            CustomerPaymentHistories = new HashSet<CustomerPaymentHistory>();
            OrderPaymentHistories = new HashSet<OrderPaymentHistory>();
        }

        public int Id { get; set; }

        public int? CustomerTypeId { get; set; }
        public CustomerType CustomerType { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string HouseName { get; set; }
        public string HouseNumber { get; set; }
        public string Village { get; set; }
        public string Upazilla { get; set; }
        public string District { get; set; }
        public string Phone { get; set; }
        public string OptionalPhone { get; set; }
        public bool IsDisable { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<CustomerProductReturn> CustomerProductReturn { get; set; }
        public virtual ICollection<CustomerPaymentHistory> CustomerPaymentHistories { get; set; }
        public virtual ICollection<OrderPaymentHistory> OrderPaymentHistories { get; set; }

        [NotMapped]
        public string FullAddress
        {
            get
            {
                var addressParts = new List<string>();

                if (!string.IsNullOrEmpty(HouseName))
                    addressParts.Add(HouseName);

                if (!string.IsNullOrEmpty(HouseNumber))
                    addressParts.Add(HouseNumber);

                if (!string.IsNullOrEmpty(Village))
                    addressParts.Add(Village);

                if (!string.IsNullOrEmpty(Upazilla))
                    addressParts.Add(Upazilla);

                if (!string.IsNullOrEmpty(District))
                    addressParts.Add(District);

                // Join the parts with a comma and return
                return string.Join(", ", addressParts);
            }
        }
    }
}