namespace SOJIBENTERPRISE.Domain
{
    public class ReturnOrderDetailViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Product { get; set; }
        public int Quantity { get; set; } = 1;
        public int ReturnQuantity { get; set; }
        public double Price { get; set; }
        public double ReturnPrice { get; set; }
        public double UnitofPrice { get; set; }
        public double Discount { get; set; }
        public double DiscountAmount { get; set; }

        public int FreeQuantity { get; set; }

        public int? CartunToPiece { get; set; }
        public int? BoxToPiece { get; set; }
        public int? Piece { get; set; }

        public int? BoxUnit { get; set; }
        public int? CartunUnit { get; set; }

        public FreeProductOfferDTO FreeProductOffer { get; set; }
    }
}