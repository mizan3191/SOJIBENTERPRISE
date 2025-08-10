namespace SOJIBENTERPRISE.DataAccess
{
    public class PurchaseManager : BaseDataManager, IPurchase
    {
        public PurchaseManager(BoniyadiContext model) : base(model)
        {
        }

        public bool CreatePurchase(Purchase purchase)
        {
            if (purchase == null || purchase.PurchaseDetails == null || !purchase.PurchaseDetails.Any())
            {
                return false;
            }

            using var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                _dbContext.Purchases.Add(purchase);
                _dbContext.SaveChanges(); // Save to generate Order ID

                foreach (var item in purchase.PurchaseDetails)
                {
                    var entity = _dbContext.Products.FirstOrDefault(x => x.Id == item.ProductId);
                    if (entity != null)
                    {
                        entity.StockQty = entity.StockQty + item.Quantity;
                        _dbContext.SaveChanges();
                    }
                }


                var lastPayment = _dbContext.SupplierPaymentHistories
                                 .Where(p => p.SupplierId == purchase.SupplierId && !p.IsDeleted)
                                 .OrderByDescending(p => p.Id)
                                 .FirstOrDefault();

                double totalDueBefore = lastPayment?.TotalDueAfterPayment ?? 0; // If no previous payment, due is 0
                double totalDueAfter = totalDueBefore + purchase.TotalAmount;

                // Add new entry to SupplierPaymentHistory
                var payment = new SupplierPaymentHistory()
                {
                    SupplierId = purchase.SupplierId,
                    PurchaseId = purchase.Id,
                    PaymentDate = purchase.Date,
                    Comments = purchase.Comments,
                    PaymentMethod = purchase.PaymentMethod,
                    TransactionID = purchase.TransactionID,
                    Number = purchase.Number,
                    TotalAmountThisPurchase = purchase.TotalAmount,
                    AmountPaid = purchase.TotalPay,
                    TotalDueBeforePayment = totalDueBefore,
                    TotalDueAfterPayment = totalDueAfter - purchase.TotalPay
                };

                _dbContext.SupplierPaymentHistories.Add(payment);
                _dbContext.SaveChanges();

                if (purchase.DamageProductDueAdjustment > 0)
                {
                    DamageProductHandoverPaymentHistory paymentHistory = new()
                    {
                        SupplierId = purchase.SupplierId,
                        PurchaseId = purchase.Id,
                        Date = purchase.Date,
                        AmountPaid = purchase.DamageProductDueAdjustment,
                    };

                    _dbContext.DamageProductHandoverPaymentHistories.Add(paymentHistory);
                    _dbContext.SaveChanges();
                }

                if (purchase.TotalPay > 0)
                {
                    var existCurrentBalance = _dbContext.TransactionHistories
                                           .AsNoTracking()
                                           .Where(x => !x.IsDeleted)
                                           .OrderByDescending(x => x.Id)
                                           .FirstOrDefault()?.CurrentBalance ?? 0;


                    TransactionHistory transactionHistory = new()
                    {
                        BalanceIn = 0,
                        BalanceOut = purchase.TotalPay,
                        CurrentBalance = existCurrentBalance - purchase.TotalPay,
                        Date = purchase.Date,
                        PurchaseId = purchase.Id,
                        Resone = purchase.Supplier?.Name != null ? $"Purchase from {purchase.Supplier.Name}." : $"Purchase Payment",
                    };

                    _dbContext.Add(transactionHistory);
                    _dbContext.SaveChanges();
                }

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return false;
            }
        }



        public bool UpdatePurchase(Purchase purchase)
        {
            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                var existingPurchaseData = _dbContext.Purchases
                   .Include(o => o.PurchaseDetails)
                   .AsNoTracking()
                   .FirstOrDefault(o => o.Id == purchase.Id);

                // Get the existing purchase with details (tracked)
                var existingPurchase = _dbContext.Purchases
                    .Include(o => o.PurchaseDetails)
                    .FirstOrDefault(o => o.Id == purchase.Id);

                if (existingPurchase == null)
                    return false;


                // Update purchase header
                existingPurchase.PaymentMethod = purchase.PaymentMethod;
                existingPurchase.ShippingMethod = purchase.ShippingMethod;
                existingPurchase.DeliveryCharge = purchase.DeliveryCharge;
                existingPurchase.TotalAmount = purchase.TotalAmount;
                existingPurchase.TotalPay = purchase.TotalPay;
                existingPurchase.Comments = purchase.Comments;
                existingPurchase.TotalDue = purchase.TotalDue;
                existingPurchase.Date = purchase.Date;
                existingPurchase.PurchaseDetails = purchase.PurchaseDetails;

                var oldDetails = existingPurchaseData?.PurchaseDetails ?? new List<PurchaseDetail>();
                var newDetails = existingPurchase?.PurchaseDetails ?? new List<PurchaseDetail>();


                var allProductIds = existingPurchaseData?.PurchaseDetails.Select(p => p.ProductId)
                            .Union(existingPurchase?.PurchaseDetails.Select(p => p.ProductId))
                            .Distinct();

                var differences = allProductIds.Select(productId =>
                {
                    var oldQty = oldDetails.FirstOrDefault(p => p.ProductId == productId)?.Quantity ?? 0;
                    var newQty = newDetails.FirstOrDefault(p => p.ProductId == productId)?.Quantity ?? 0;

                    return new ProductQuantityDifference
                    {
                        ProductId = productId,
                        QuantityDifference = newQty - oldQty
                    };
                }).ToList();

                foreach (var detail in differences)
                {
                    if (detail.QuantityDifference != 0)
                    {
                        var product = _dbContext.Products.Find(detail.ProductId);
                        if (product != null)
                        {
                            product.StockQty += detail.QuantityDifference;
                        }
                    }
                }
                _dbContext.SaveChanges();


                // Update supplier payment history
                var existingPayment = _dbContext.SupplierPaymentHistories
                    .FirstOrDefault(p => p.PurchaseId == purchase.Id);

                var amountDifference = existingPurchaseData.TotalPay - purchase.TotalPay;
                var purchaseDifference = existingPurchaseData.TotalAmount - purchase.TotalAmount;

                if (existingPayment != null)
                {

                    existingPayment.PaymentMethod = purchase.PaymentMethod;
                    existingPayment.Number = purchase.Number;
                    existingPayment.PaymentDate = purchase.Date;
                    existingPayment.Comments = purchase.Comments;
                    existingPayment.TransactionID = purchase.TransactionID;
                    existingPayment.AmountPaid = purchase.TotalPay;
                    existingPayment.TotalDueAfterPayment = (existingPayment.TotalDueAfterPayment + amountDifference) - purchaseDifference;
                    existingPayment.TotalAmountThisPurchase = purchase.TotalAmount;

                    RecalculateSupplierPaymentHistoriesAsync(existingPayment.SupplierId, existingPayment.Id, purchaseDifference);


                    _dbContext.SaveChanges();
                }

                // Update damage product history if exists
                var damageProductHistory = _dbContext.DamageProductHandoverPaymentHistories
                    .FirstOrDefault(x => x.PurchaseId == purchase.Id);

                if (damageProductHistory != null)
                {
                    damageProductHistory.AmountPaid = existingPurchase.DamageProductDueAdjustment;
                }

                // Update transaction history if payment amount changed
                var existingTransaction = _dbContext.TransactionHistories
                    .FirstOrDefault(x => x.PurchaseId == purchase.Id);

                if (existingTransaction != null && purchase.TotalPay != existingPurchaseData.TotalPay)
                {
                    //var amountDifference = existingPurchaseData.TotalPay - purchase.TotalPay;

                    existingTransaction.BalanceOut -= amountDifference;
                    existingTransaction.CurrentBalance += amountDifference;

                    if (amountDifference != 0)
                    {
                        BalanceInTransactionHistories(existingTransaction.Id, amountDifference);
                    }
                }

                _dbContext.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                // Log the exception here
                return false;
            }
        }


        public bool DeletePurchase(int id)
        {
            using var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                var purchasesEntity = _dbContext.Purchases.FirstOrDefault(x => x.Id == id);

                if (purchasesEntity == null)
                {
                    return false;
                }

                purchasesEntity.IsDeleted = true;
                _dbContext.Update(purchasesEntity);
                _dbContext.SaveChanges();

                var purchasesDetailsEntity = _dbContext.PurchaseDetails.Where(x => x.PurchaseId == id).ToList();

                foreach (var product in purchasesDetailsEntity)
                {
                    var productEntity = _dbContext.Products.FirstOrDefault(x => x.Id == product.ProductId);
                    productEntity.StockQty = productEntity.StockQty - product.Quantity;

                    _dbContext.Update(productEntity);
                    _dbContext.SaveChanges();
                }

                // Get the payment to be soft deleted
                var supplierPaymentHistoriesEntity = _dbContext.SupplierPaymentHistories
                    .FirstOrDefault(p => p.PurchaseId == purchasesEntity.Id
                    && p.SupplierId == purchasesEntity.SupplierId);

                if (supplierPaymentHistoriesEntity is not null)
                {
                    supplierPaymentHistoriesEntity.IsDeleted = true;

                    var amount = purchasesEntity.TotalAmount - supplierPaymentHistoriesEntity.AmountPaid;
                    RecalculateSupplierPaymentHistoriesAsync(supplierPaymentHistoriesEntity.SupplierId, supplierPaymentHistoriesEntity.Id, amount);

                    _dbContext.Update(supplierPaymentHistoriesEntity);
                    _dbContext.SaveChanges();
                }

                // Get the Purchase related transaction history
                var purchasestransactionHistory = _dbContext.TransactionHistories
                    .FirstOrDefault(t => t.PurchaseId == purchasesEntity.Id);

                if (purchasestransactionHistory is not null)
                {
                    if (purchasestransactionHistory.BalanceOut.HasValue
                        && purchasestransactionHistory.BalanceOut.Value > 0)
                    {
                        BalanceInTransactionHistories(purchasestransactionHistory.Id, purchasestransactionHistory.BalanceOut.Value);
                    }

                    purchasestransactionHistory.IsDeleted = true;
                    _dbContext.Update(purchasestransactionHistory);
                    _dbContext.SaveChanges();
                }

                // Get the Supplier Payment History related transaction history

                if (supplierPaymentHistoriesEntity is not null)
                {
                    var supplierPaymentHistorytransactionHistory = _dbContext.TransactionHistories
                    .FirstOrDefault(t => t.SupplierPaymentHistoryId == supplierPaymentHistoriesEntity.Id);



                    if (supplierPaymentHistorytransactionHistory is not null)
                    {

                        if (supplierPaymentHistorytransactionHistory.BalanceOut.HasValue
                           && supplierPaymentHistorytransactionHistory.BalanceOut.Value > 0)
                        {
                            BalanceOutTransactionHistories(supplierPaymentHistorytransactionHistory.Id, supplierPaymentHistorytransactionHistory.BalanceOut.Value);
                        }

                        supplierPaymentHistorytransactionHistory.IsDeleted = true;
                        _dbContext.Update(supplierPaymentHistorytransactionHistory);
                        _dbContext.SaveChanges();
                    }
                }
                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private void RecalculateSupplierPaymentHistoriesAsync(int supplierId, int id, double amount)
        {
            if (amount == 0)
                return;

            try
            {
                var payments = _dbContext.SupplierPaymentHistories
                                   .Where(p => p.SupplierId == supplierId && p.Id > id)
                                   .OrderBy(p => p.Id)
                                   .ToList();

                if (!payments.Any() || payments.Count() == 0 || payments is null)
                {
                    return;
                }

                foreach (var payment in payments)
                {
                    payment.TotalDueBeforePayment -= amount;
                    payment.TotalDueAfterPayment -= amount;
                }

                _dbContext.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public async Task<Purchase> GetPurchaseById(int purchaseId)
        {
            try
            {
                return await _dbContext.Purchases
                .Include(o => o.PurchaseDetails)
                .FirstOrDefaultAsync(o => o.Id == purchaseId && !o.IsDeleted);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<IEnumerable<PurchasesDetailsDTO>> GetPurchasesDetailsByOrderAsync(int purchaseId)
        {
            try
            {
                var purchaseDetails = await _dbContext.PurchaseDetails
                    .Include(pd => pd.Product)
                        .ThenInclude(p => p.ProductsSize)
                    .Include(pd => pd.Product)
                        .ThenInclude(p => p.Supplier)
                    .Where(pd => pd.PurchaseId == purchaseId)
                    .Select(pd => new PurchasesDetailsDTO
                    {
                        ProductName = pd.Product.DisplayNameSize,
                        SupplierName = pd.Product.Supplier.Name,
                        Quantity = pd.Quantity,
                        ProductPrice = pd.UnitPrice, // Using current buying price
                        Discount = pd.Discount,
                        TotalPrice = (double)((pd.Quantity * pd.UnitPrice) - pd.Discount)
                    })
                    .ToListAsync();

                return purchaseDetails;
            }
            catch (Exception ex)
            {
                // Consider logging the exception here
                // _logger.LogError(ex, "Error getting purchase details for purchase {PurchaseId}", purchaseId);
                return Enumerable.Empty<PurchasesDetailsDTO>();
            }
        }
    }

    public class ProductQuantityDifference
    {
        public int ProductId { get; set; }
        public int QuantityDifference { get; set; }
    }
}