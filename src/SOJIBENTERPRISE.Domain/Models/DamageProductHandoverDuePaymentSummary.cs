﻿namespace SOJIBENTERPRISE.Domain
{
    public class DamageProductHandoverDuePaymentSummary
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public double DueAmount { get; set; }
        public double PaidAmount { get; set; }
        public DateTime Date { get; set; }
    }
}
