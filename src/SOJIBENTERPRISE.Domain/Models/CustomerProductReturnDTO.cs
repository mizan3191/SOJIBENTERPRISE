namespace SOJIBENTERPRISE.Domain
{
    public class CustomerProductReturnDTO
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public string Products { get; set; }
        public DateTime OrderDate { get; set; }
        public double TotalPrice { get; set; }
    }
}