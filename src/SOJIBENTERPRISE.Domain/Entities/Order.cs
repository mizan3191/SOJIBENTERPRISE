namespace SOJIBENTERPRISE.Domain
{
    public class Order
    {
        public Order() 
        {
            OrderDetails = new HashSet<OrderDetail>();
            CustomerPaymentHistories = new HashSet<CustomerPaymentHistory>();
            OrderPaymentHistories = new HashSet<OrderPaymentHistory>();
            DailyExpenses = new HashSet<DailyExpense>();
            DamageProducts = new HashSet<DamageProduct>();
            DSRShopDues = new HashSet<DSRShopDue>();
            SRDiscounts = new HashSet<SRDiscount>();
            CustomerProductReturns = new HashSet<CustomerProductReturn>();
            //ProductReturns = new HashSet<ProductReturn>();
        }

        public int Id { get; set; }

        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        public int? PaymentMethodId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        //public string TransactionID { get; set; }
        //public string Number { get; set; }

        public double DeliveryCharge { get; set; }
        public string DeliveryLocation { get; set; }
        public double TotalAmount { get; set; }
        public double TotalPay { get; set; }
        public double Discount { get; set; }
        public double TotalDue { get; set; }

        public bool IsDeleted { get; set; }

        public string SelectedRoad { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<CustomerPaymentHistory> CustomerPaymentHistories { get; set; }
        public virtual ICollection<OrderPaymentHistory> OrderPaymentHistories { get; set; }
        public virtual ICollection<DailyExpense> DailyExpenses { get; set; }
        public virtual ICollection<DamageProduct> DamageProducts { get; set; }
        public virtual ICollection<DSRShopDue> DSRShopDues { get; set; }
        public virtual ICollection<CustomerProductReturn> CustomerProductReturns { get; set; }
        public virtual ICollection<SRDiscount> SRDiscounts { get; set; }

    }

    public class OrderInfo
    {
        public int OrderId { get; set; }
        public string Area { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }

    }
}