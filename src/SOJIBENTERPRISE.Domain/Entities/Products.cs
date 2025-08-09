namespace SOJIBENTERPRISE.Domain
{
    public class Product
    {
        public Product()
        {
            //FreeProductOffers = new HashSet<FreeProductOffer>();
            OrderDetails = new HashSet<OrderDetail>();
            CustomerProductReturnDetails = new HashSet<CustomerProductReturnDetails>();
            PriceHistories = new HashSet<PriceHistory>();
            PurchaseDetails = new HashSet<PurchaseDetail>();
            ProductConsumptions = new HashSet<ProductConsumption>();
            DamageProductHandoverDetails = new HashSet<DamageProductHandoverDetails>();
        }

        public int Id { get; set; }
        public int? ProductNo { get; set; }
        public string Name { get; set; }
       
        public double SellingPrice { get; set; }
        public double BuyingPrice { get; set; }
        public int ReOrderLevel { get; set; }
        public int StockQty { get; set; }
        public bool IsFreeProductOffer { get; set; }
        public string IsFreeProductOfferFormatted => IsFreeProductOffer? "Yes": "No" ;

        public double StockPrice => StockQty* BuyingPrice;

        public int SupplierId { get; set; } 
        public virtual Supplier Supplier { get; set; } 

        public int? UnitOfMeasurementId { get; set; } 
        public virtual UnitOfMeasurement UnitOfMeasurement { get; set; } 
        
        public int? ProductCategoryId { get; set; } 
        public virtual ProductCategory ProductCategory { get; set; }

        public int? PackagingId { get; set; }
        public Packaging Packaging { get; set; }

        public int? ProductsSizeId { get; set; }
        public ProductsSize ProductsSize { get; set; }

       // public bool IsCartun { get; set; } = false;
        public int? CartunToPiece { get; set; }
        //public bool IsBox { get; set; } = false;
        public int? BoxToPiece { get; set; }
        public int? Piece { get; set; } //Piece

        public string DisplayText => $"{ProductNo} - {Name} ({ProductsSize?.Name})";
        public string DisplayNameSize => $"{Name} ({ProductsSize?.Name})";

        //public virtual ICollection<FreeProductOffer> FreeProductOffers { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<CustomerProductReturnDetails> CustomerProductReturnDetails { get; set; }
        public virtual ICollection<PriceHistory> PriceHistories { get; set; }
        public virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; }
        public virtual ICollection<ProductConsumption> ProductConsumptions { get; set; }
        public virtual ICollection<DamageProductHandoverDetails> DamageProductHandoverDetails { get; set; }
    }
}
