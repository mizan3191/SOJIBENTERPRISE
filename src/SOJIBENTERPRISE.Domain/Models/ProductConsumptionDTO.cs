namespace SOJIBENTERPRISE.Domain
{
    public class ProductConsumptionDTO
    {
        public int Id { get; set; } // Primary Key     
        public double Amount { get; set; }
        public string ReasonOfConsumed { get; set; }
        public DateTime DateConsumed { get; set; }
        public string Comment { get; set; }
    }
}
