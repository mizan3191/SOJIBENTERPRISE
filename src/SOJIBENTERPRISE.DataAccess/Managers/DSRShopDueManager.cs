namespace SOJIBENTERPRISE.DataAccess
{
    public class DSRShopDueManager : BaseDataManager, IDSRShopDue
    {
        public DSRShopDueManager(BoniyadiContext model) : base(model)
        {
        }

        public int CreateDSRShopDue(DSRShopDue DSRShopDue)
        {
            AddUpdateEntity(DSRShopDue);

            if (DSRShopDue.OrderId > 0)
            {
                var lastPayment = _dbContext.CustomerPaymentHistories
                                .Where(p => p.CustomerId == DSRShopDue.DSRCustomerId)
                                .OrderByDescending(p => p.Id)
                                .FirstOrDefault();

                double totalDueBefore = lastPayment?.TotalDueAfterPayment ?? 0; // If no previous payment, due is 0
                double totalDueAfter = totalDueBefore - DSRShopDue.DueAmount;

                // Add new entry to Customer Payment History
                var payment = new CustomerPaymentHistory()
                {
                    CustomerId = DSRShopDue.DSRCustomerId.Value,
                    OrderId = DSRShopDue.OrderId,
                    DSRShopDueId = DSRShopDue.Id,
                    PaymentDate = DateTime.Now,
                    PaymentMethodId = 14,
                    TransactionID = string.Empty,
                    Number = string.Empty,
                    TotalAmountThisOrder = 0,
                    AmountPaid = DSRShopDue.DueAmount,
                    TotalDueBeforePayment = totalDueBefore,
                    TotalDueAfterPayment = totalDueAfter
                };

                _dbContext.CustomerPaymentHistories.Add(payment);
                _dbContext.SaveChanges();
            }

            return DSRShopDue.Id;
        }

        public bool UpdateDSRShopDue(DSRShopDue DSRShopDue)
        {
            var previousAmount = _dbContext.DSRShopDues.AsNoTracking().FirstOrDefault(x => x.Id == DSRShopDue.Id).DueAmount;

            using var transaction = _dbContext.Database.BeginTransaction();
            _dbContext.Update(DSRShopDue);
            _dbContext.SaveChanges();

            if (DSRShopDue.OrderId > 0 && DSRShopDue.OrderId is not null)
            {
                var payment = _dbContext.CustomerPaymentHistories
                                 .FirstOrDefault(p => p.DSRShopDueId == DSRShopDue.Id
                                 && p.CustomerId == DSRShopDue.DSRCustomerId);

                var UpDownAmount = payment.AmountPaid - DSRShopDue.DueAmount;

                if (payment != null)
                {
                    double totalDueAfter = (payment.TotalDueAfterPayment + previousAmount) - DSRShopDue.DueAmount;

                    payment.PaymentDate = DateTime.Now;
                    payment.AmountPaid = DSRShopDue.DueAmount;
                    payment.TotalDueAfterPayment = totalDueAfter;
                }

                _dbContext.Update(payment);
                _dbContext.SaveChanges();

                RecalculateCustomerPaymentsAsync(payment.CustomerId, payment.Id, UpDownAmount);
            }

            transaction.Commit();
            return true;
        }

        public bool DeleteShopDue(int id)
        {
            try
            {
                using var transaction = _dbContext.Database.BeginTransaction();

                var ShopDues = _dbContext.DSRShopDues.FirstOrDefault(c => c.Id == id);

                if (ShopDues == null)
                    return false;

                // Update main return entity
                ShopDues.Date = DateTime.UtcNow;
                ShopDues.IsDeleted = true;

                _dbContext.Update(ShopDues);
                _dbContext.SaveChanges();

                if (ShopDues.OrderId > 0 && ShopDues.OrderId is not null)
                {
                    var entity = _dbContext.CustomerPaymentHistories
                                 .FirstOrDefault(p => p.CustomerId == ShopDues.DSRCustomerId
                                 && p.OrderId == ShopDues.OrderId
                                 && p.DSRShopDueId == ShopDues.Id);


                    entity.IsDeleted = true;
                    _dbContext.Update(entity);
                    _dbContext.SaveChanges();


                    RecalculateCustomerPaymentsAsync(entity.CustomerId, entity.Id, entity.AmountPaid);

                }

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void RecalculateCustomerPaymentsAsync(int customerId, int id, double amount)
        {
            if (amount == 0)
                return;

            try
            {
                var payments = _dbContext.CustomerPaymentHistories
                                   .Where(p => p.CustomerId == customerId && p.Id > id)
                                   .OrderBy(p => p.Id)
                                   .ToList();

                if (!payments.Any() || payments.Count() == 0 || payments is null)
                {
                    return;
                }

                foreach (var payment in payments)
                {
                    payment.TotalDueBeforePayment += amount;
                    payment.TotalDueAfterPayment += amount;
                }

                _dbContext.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public IList<DSRShopDueForOrderDTO> GetAllDSRShopDueByOrderId(int orderId)
        {
            try
            {
                return _dbContext.DSRShopDues
                    .Include(x => x.Customer)
                    .Include(x => x.DSRCustomer)
                    .Include(x => x.Shop)
                    .Where(x => x.OrderId == orderId && !x.IsDeleted && !x.Shop.IsDeleted)
                    .OrderByDescending(x => x.Id)
                    .Select(x => new DSRShopDueForOrderDTO
                    {
                        Id = x.Id,
                        CustomerName = x.Customer.Name,
                        DSRCustomerName = x.DSRCustomer.Name,
                        ShopName = x.Shop.Name,
                        OrderId = x.OrderId,
                        DueAmount = x.DueAmount,
                        Date = x.Date
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                // Optional: Log the error if needed
                return new List<DSRShopDueForOrderDTO>();
            }
        }

        public DSRShopDue GetDSRShopDue(int id)
        {
            try
            {
                return _dbContext.DSRShopDues
                    .FirstOrDefault(c => c.Id == id);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<IList<Lov>> GetAllShopDueCustomerList(int shopId)
        {
            try
            {
                var list = await _dbContext.DSRShopDues
                            .Include(x => x.Customer)
                            .Where(x => !x.IsDeleted
                                && !x.Shop.IsDeleted
                                && !x.Customer.IsDisable
                                && x.ShopId == shopId)
                            .Select(x => new Lov
                            {
                                Id = x.CustomerId.Value,
                                Name = x.Customer.Name,
                            })
                            .Distinct()
                            .OrderBy(x => x.Name)
                            .ToListAsync();

                return list;
            }
            catch (Exception ex)
            {
                return new List<Lov>();
            }
        }

        public IList<DSRShopDue> GetAllDSRShopDue()
        {
            try
            {
                return _dbContext.DSRShopDues
                    .Include(x => x.Customer)
                    .Include(x => x.DSRCustomer)
                    .Include(x => x.Shop)
                    .Where(x => !x.IsDeleted && !x.Shop.IsDeleted)
                    .OrderByDescending(x => x.Id)
                    .ToList();
            }
            catch (Exception ex)
            {
                return new List<DSRShopDue>();
            }
        }

        public IList<DSRShopDueDTO> GetAllDSRShopDueList(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                // Default to last 3 days if no date is provided
                DateTime fromDate = startDate.HasValue
                    ? new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 0, 0, 0)
                    : DateTime.Today.AddDays(-30); // default to last 30 days

                DateTime toDate = endDate.HasValue
                    ? new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 23, 59, 59)
                    : DateTime.Today.AddDays(1).AddSeconds(-1); // today till 23:59:59


                return _dbContext.DSRShopDues
                    .Include(x => x.Customer)
                    .Include(x => x.Shop)
                    .Where(x => !x.IsDeleted && !x.Shop.IsDeleted)
                    .OrderByDescending(x => x.Id)
                    .Select(x => new DSRShopDueDTO
                    {
                        Id = x.Id,
                        CustomerName = x.Customer.Name,
                        ShopName = x.Shop.Name,
                        OrderId = x.OrderId,
                        DueAmount = x.DueAmount,
                        ShopArea = x.Shop.Area,
                        ShopNumber = x.Shop.Number,
                        ShopShopOwner = x.Shop.ShopOwner,
                        Date = x.Date
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                // Optional: Log the error if needed
                return new List<DSRShopDueDTO>();
            }
        }

        public IList<DSRShopDueDTO> GetAllDSRShopDueList(int shopId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                // Default to last 3 days if no date is provided
                DateTime fromDate = startDate.HasValue
                    ? new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 0, 0, 0)
                    : DateTime.Today.AddDays(-30); // default to last 7 days

                DateTime toDate = endDate.HasValue
                    ? new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 23, 59, 59)
                    : DateTime.Today.AddDays(1).AddSeconds(-1); // today till 23:59:59


                return _dbContext.DSRShopDues
                    .Include(x => x.Customer)
                    .Include(x => x.DSRCustomer)
                    .Include(x => x.Shop)
                    .Where(x => x.Date.Date >= fromDate && x.Date.Date <= toDate && !x.IsDeleted && x.ShopId == shopId && !x.Shop.IsDeleted)
                    .OrderByDescending(x => x.Id)
                    .Select(x => new DSRShopDueDTO
                    {
                        Id = x.Id,
                        CustomerName = x.Customer.Name,
                        IssuedBYCustomerName = x.DSRCustomer.Name,
                        ShopName = x.Shop.Name,
                        OrderId = x.OrderId,
                        DueAmount = x.DueAmount,
                        ShopArea = x.Shop.Area,
                        ShopNumber = x.Shop.Number,
                        ShopShopOwner = x.Shop.ShopOwner,
                        Date = x.Date
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                // Optional: Log the error if needed
                return new List<DSRShopDueDTO>();
            }
        }

        public async Task<IList<DSRShopDue>> GetAllDSRShopDue(int orderId)
        {
            try
            {
                return await _dbContext.DSRShopDues
                    .Include(x => x.Customer)
                    .Include(x => x.DSRCustomer)
                    .Include(x => x.Shop)
                    .Where(x => x.OrderId == orderId && !x.IsDeleted && !x.Shop.IsDeleted)
                    .OrderByDescending(x => x.Id)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<DSRShopDue>();
            }
        }

    }
}