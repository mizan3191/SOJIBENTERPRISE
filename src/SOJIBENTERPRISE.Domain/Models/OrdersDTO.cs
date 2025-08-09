namespace SOJIBENTERPRISE.Domain
{
    public class OrdersDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public DateTime OrderDate { get; set; }
        public double TotalPrice { get; set; }
        public string Address { get; set; }
        public bool IsLock { get; set; }
    }
}