namespace SOJIBENTERPRISE.Domain
{
    public class DamageProductReturnDetailViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
        public double UnitofPrice { get; set; }
        public double LossUnitofPrice { get; set; }
        public double Price { get; set; }

        public int FreeQuantity { get; set; }

        public FreeProductOfferDTO FreeProductOffer { get; set; }
    }
}
