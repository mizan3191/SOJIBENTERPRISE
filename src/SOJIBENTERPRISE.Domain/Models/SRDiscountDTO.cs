﻿namespace SOJIBENTERPRISE.Domain
{
    public class SRDiscountDTO
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string SRName { get; set; }
        public string DSRName { get; set; }
        public double DiscountAmount { get; set; }
        public DateTime Date { get; set; }
    }
}
