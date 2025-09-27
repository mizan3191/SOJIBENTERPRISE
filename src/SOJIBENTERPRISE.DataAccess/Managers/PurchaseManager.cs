namespace SOJIBENTERPRISE.DataAccess
{
    public class PurchaseManager : BaseDataManager, IPurchase
    {
        public PurchaseManager(BoniyadiContext model) : base(model)
        {
        }

        public bool CreatePurchase(Purchase purchase)
        {
            using var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                _dbContext.Purchases.Add(purchase);
                _dbContext.SaveChanges(); // Save to generate Order ID

                //var lastPayment = _dbContext.SupplierPaymentHistories
                //                 .Where(p => p.SupplierId == purchase.SupplierId && !p.IsDeleted)
                //                 .OrderByDescending(p => p.Id)
                //                 .FirstOrDefault();

                //double totalDueBefore = lastPayment?.TotalDueAfterPayment ?? 0; // If no previous payment, due is 0
                //double totalDueAfter = totalDueBefore + purchase.TotalAmount;

                // Add new entry to SupplierPaymentHistory
                //var payment = new SupplierPaymentHistory()
                //{
                //    SupplierId = purchase.SupplierId,
                //    PurchaseId = purchase.Id,
                //    PaymentDate = purchase.Date,
                //    Comments = purchase.Comments,
                //    TotalAmountThisPurchase = purchase.TotalAmount,
                //    TotalDueBeforePayment = totalDueBefore,
                //    TotalDueAfterPayment = totalDueAfter,
                //};

                //_dbContext.SupplierPaymentHistories.Add(payment);
                //_dbContext.SaveChanges();


                if (purchase.TotalAmount > 0)
                {
                    var existCurrentBalance = _dbContext.TransactionHistories
                                           .AsNoTracking()
                                           .Where(x => !x.IsDeleted)
                                           .OrderByDescending(x => x.Id)
                                           .FirstOrDefault()?.CurrentBalance ?? 0;


                    TransactionHistory transactionHistory = new()
                    {
                        BalanceIn = 0,
                        BalanceOut = purchase.TotalAmount,
                        CurrentBalance = existCurrentBalance - purchase.TotalAmount,
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
            var existingTotalAmount = _dbContext.Purchases
                   .AsNoTracking()
                   .FirstOrDefault(o => o.Id == purchase.Id).TotalAmount;

            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {

                _dbContext.Update(purchase);
                _dbContext.SaveChanges();


                

                //// Get the existing purchase with details (tracked)
                //var existingPurchase = _dbContext.Purchases
                //    .FirstOrDefault(o => o.Id == purchase.Id);

                //if (existingPurchase == null)
                //    return false;

                //// Update supplier payment history
                //var existingPayment = _dbContext.SupplierPaymentHistories
                //    .FirstOrDefault(p => p.PurchaseId == purchase.Id);

                var amountDifference = existingTotalAmount - purchase.TotalAmount;
                //var purchaseDifference = existingTotalAmount - purchase.TotalAmount;

                //if (existingPayment != null)
                //{

                //    existingPayment.PaymentDate = purchase.Date;
                //    existingPayment.Comments = purchase.Comments;
                //    existingPayment.TotalDueAfterPayment = (existingPayment.TotalDueAfterPayment + purchase.TotalAmount) - existingTotalAmount;
                //    existingPayment.TotalAmountThisPurchase = purchase.TotalAmount;

                //    RecalculateSupplierPaymentHistoriesAsync(existingPayment.SupplierId, existingPayment.Id, amountDifference);


                //    _dbContext.SaveChanges();
                //}

                // Update transaction history if payment amount changed
                var existingTransaction = _dbContext.TransactionHistories
                    .FirstOrDefault(x => x.PurchaseId == purchase.Id);

                if (existingTransaction != null && purchase.TotalAmount != existingTotalAmount)
                {
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


                // Get the payment to be soft deleted
                //var supplierPaymentHistoriesEntity = _dbContext.SupplierPaymentHistories
                //    .FirstOrDefault(p => p.PurchaseId == purchasesEntity.Id
                //    && p.SupplierId == purchasesEntity.SupplierId);

                //if (supplierPaymentHistoriesEntity is not null)
                //{
                //    supplierPaymentHistoriesEntity.IsDeleted = true;

                //    var amount = purchasesEntity.TotalAmount - supplierPaymentHistoriesEntity.AmountPaid;
                //    RecalculateSupplierPaymentHistoriesAsync(supplierPaymentHistoriesEntity.SupplierId, supplierPaymentHistoriesEntity.Id, amount);

                //    _dbContext.Update(supplierPaymentHistoriesEntity);
                //    _dbContext.SaveChanges();
                //}

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

                //if (purchasesEntity.TotalAmount > 0)
                //{
                //    var supplierPaymentHistorytransactionHistory = _dbContext.TransactionHistories
                //    .FirstOrDefault(t => t.PurchaseId == purchasesEntity.Id);

                //    if (supplierPaymentHistorytransactionHistory is not null)
                //    {

                //        if (supplierPaymentHistorytransactionHistory.BalanceOut.HasValue
                //           && supplierPaymentHistorytransactionHistory.BalanceOut.Value > 0)
                //        {
                //            BalanceOutTransactionHistories(supplierPaymentHistorytransactionHistory.Id, supplierPaymentHistorytransactionHistory.BalanceOut.Value);
                //        }

                //        supplierPaymentHistorytransactionHistory.IsDeleted = true;
                //        _dbContext.Update(supplierPaymentHistorytransactionHistory);
                //        _dbContext.SaveChanges();
                //    }
                //}

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        //private void RecalculateSupplierPaymentHistoriesAsync(int supplierId, int id, double amount)
        //{
        //    if (amount == 0)
        //        return;

        //    try
        //    {
        //        var payments = _dbContext.SupplierPaymentHistories
        //                           .Where(p => p.SupplierId == supplierId && p.Id > id)
        //                           .OrderBy(p => p.Id)
        //                           .ToList();

        //        if (!payments.Any() || payments.Count() == 0 || payments is null)
        //        {
        //            return;
        //        }

        //        foreach (var payment in payments)
        //        {
        //            payment.TotalDueBeforePayment -= amount;
        //            payment.TotalDueAfterPayment -= amount;
        //        }

        //        _dbContext.SaveChanges();
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        public async Task<Purchase> GetPurchaseById(int purchaseId)
        {
            try
            {
                return await _dbContext.Purchases
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