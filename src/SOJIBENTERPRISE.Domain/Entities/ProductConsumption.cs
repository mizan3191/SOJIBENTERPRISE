namespace SOJIBENTERPRISE.Domain
{
    public class ProductConsumption
    {
        public int Id { get; set; } // Primary Key     
        public int QuantityConsumed { get; set; }
        public DateTime DateConsumed { get; set; }

        public string Comments { get; set; }
        public bool IsDeleted { get; set; }

        // Navigation Property
        public int ProductId { get; set; } // Foreign Key to Product
        public virtual Product Product { get; set; }

        // Navigation Property
        public int CustomerId { get; set; } // Foreign Key to Product
        public virtual Customer Customer { get; set; }

        public int ReasonofAdjustmentId { get; set; } // Foreign Key to ReasonofAdjustment
        public virtual ReasonofAdjustment ReasonofAdjustment { get; set; }
    }
}
