﻿namespace SOJIBENTERPRISE.DataAccess
{
    public class OrderPaymentHistoryManager : BaseDataManager, IOrderPaymentHistory
    {
        public OrderPaymentHistoryManager(BoniyadiContext model) : base(model)
        {
        }

        public bool UpdateOrderPaymentHistory(OrderPaymentHistory OrderPaymentHistory)
        {
            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                var existingTransaction = _dbContext.TransactionHistories
                  .FirstOrDefault(x => x.OrderPaymentHistoryId == OrderPaymentHistory.Id);

                if (existingTransaction == null)
                    throw new Exception("Transaction history not found.");

                var previousAmountPaid = existingTransaction.BalanceIn ?? 0;
                var currentBalanceBeforeUpdate = existingTransaction.CurrentBalance ?? 0;


                var previousAmount = _dbContext.OrderPaymentHistories
                                            .AsNoTracking()
                                            .FirstOrDefault(x => x.Id == OrderPaymentHistory.Id)
                                            .AmountPaid;

                _dbContext.Update(OrderPaymentHistory);
                _dbContext.SaveChanges();

                var payment = _dbContext.CustomerPaymentHistories
                                 .FirstOrDefault(p => p.OrderId == OrderPaymentHistory.OrderId
                                 && p.OrderPaymentHistoryId == OrderPaymentHistory.Id && !p.IsDeleted
                                 && p.CustomerId == OrderPaymentHistory.DSRCustomerId);


                var UpDownAmount = payment.AmountPaid - OrderPaymentHistory.AmountPaid;

                if (payment != null)
                {
                    // double totalDueBefore = payment.TotalDueAfterPayment;
                    double totalDueAfter = (payment.TotalDueAfterPayment + previousAmount) - OrderPaymentHistory.AmountPaid;

                    payment.PaymentDate = DateTime.Now;
                    payment.AmountPaid = OrderPaymentHistory.AmountPaid;
                    //payment.TotalDueBeforePayment = totalDueBefore;
                    payment.TotalDueAfterPayment = totalDueAfter;
                }

                //AddUpdateEntity(payment);
                _dbContext.Update(payment);
                _dbContext.SaveChanges();


                // Calculate new balance
                double newAmountPaid = OrderPaymentHistory.AmountPaid;
                double difference = newAmountPaid - previousAmountPaid;

                existingTransaction.BalanceIn = newAmountPaid;
                existingTransaction.CurrentBalance = currentBalanceBeforeUpdate + difference;
                existingTransaction.Date = DateTime.Now;

                _dbContext.Update(existingTransaction);
                _dbContext.SaveChanges();

                // Optional: update related transaction history records
                BalanceInTransactionHistories(existingTransaction.Id, difference);

                RecalculateCustomerPaymentsAsync(payment.CustomerId, payment.Id, UpDownAmount);

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
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

        public bool DeleteOrderPaymentHistory(int id)
        {
            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                var orderPaymentHistory = _dbContext.OrderPaymentHistories
                    .FirstOrDefault(x => x.Id == id);

                if (orderPaymentHistory is null)
                {
                    return false;
                }

                orderPaymentHistory.IsDeleted = true;

                _dbContext.Update(orderPaymentHistory);
                _dbContext.SaveChanges();

                if (orderPaymentHistory.OrderId > 0)
                {
                    var customerPaymentHistories = _dbContext.CustomerPaymentHistories
                                    .FirstOrDefault(p => p.CustomerId == orderPaymentHistory.DSRCustomerId
                                    && p.OrderId == orderPaymentHistory.OrderId
                                    && p.OrderPaymentHistoryId == orderPaymentHistory.Id);

                    customerPaymentHistories.IsDeleted = true;

                    _dbContext.Update(customerPaymentHistories);
                    _dbContext.SaveChanges();

                    var amount = customerPaymentHistories.AmountPaid;

                    RecalculateCustomerPaymentsAsync(customerPaymentHistories.CustomerId, customerPaymentHistories.Id, amount);

                }

                var existTransactionHistory = _dbContext.TransactionHistories
                                           .FirstOrDefault(x => x.OrderPaymentHistoryId == id);


                existTransactionHistory.IsDeleted = true;
                _dbContext.Update(existTransactionHistory);
                _dbContext.SaveChanges();

                BalanceOutTransactionHistories(existTransactionHistory.Id, orderPaymentHistory.AmountPaid);

                transaction.Commit();

                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }


        public int CreateOrderPaymentHistory(OrderPaymentHistory OrderPaymentHistory)
        {
            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                AddUpdateEntity(OrderPaymentHistory);


                var lastPayment = _dbContext.CustomerPaymentHistories
                                 .Where(p => p.CustomerId == OrderPaymentHistory.DSRCustomerId && !p.IsDeleted)
                                 .OrderByDescending(p => p.Id)
                                 .FirstOrDefault();

                double totalDueBefore = lastPayment?.TotalDueAfterPayment ?? 0; // If no previous payment, due is 0
                double totalDueAfter = totalDueBefore - OrderPaymentHistory.AmountPaid;

                // Add new entry to Customer Payment History
                var payment = new CustomerPaymentHistory()
                {
                    CustomerId = OrderPaymentHistory.DSRCustomerId,
                    OrderId = OrderPaymentHistory.OrderId,
                    OrderPaymentHistoryId = OrderPaymentHistory.Id,
                    PaymentDate = DateTime.Now,
                    PaymentMethodId = 10,
                    TransactionID = string.Empty,
                    Number = string.Empty,
                    TotalAmountThisOrder = 0,
                    AmountPaid = OrderPaymentHistory.AmountPaid,
                    TotalDueBeforePayment = totalDueBefore,
                    TotalDueAfterPayment = totalDueAfter
                };

                AddUpdateEntity(payment);

                // _dbContext.SaveChanges();

                if (OrderPaymentHistory.AmountPaid > 0)
                {
                    var existCurrentBalance = _dbContext.TransactionHistories
                                           .AsNoTracking()
                                           .OrderByDescending(x => x.Id)
                                           .FirstOrDefault()?.CurrentBalance ?? 0;


                    TransactionHistory transactionHistory = new()
                    {
                        BalanceIn = OrderPaymentHistory.AmountPaid,
                        BalanceOut = 0,
                        CurrentBalance = existCurrentBalance + OrderPaymentHistory.AmountPaid,
                        Date = DateTime.Now,
                        OrderPaymentHistoryId = OrderPaymentHistory.Id,
                        Resone = "DRS Payment",
                    };

                    _dbContext.Add(transactionHistory);

                }

                _dbContext.SaveChanges();

                transaction.Commit();
                return OrderPaymentHistory.Id;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return 0;
            }
        }

        public OrderPaymentHistory GetOrderPaymentHistoryById(int id)
        {
            try
            {
                return _dbContext.OrderPaymentHistories.FirstOrDefault(c => c.Id == id);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public OrderPaymentHistory GetOrderPaymentHistoryByOrderId(int orderId)
        {
            try
            {
                return _dbContext.OrderPaymentHistories.FirstOrDefault(c => c.OrderId == orderId);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public OrderPaymentHistory GetOrderPaymentHistory(int customerId, int orderId)
        {
            try
            {
                return _dbContext.OrderPaymentHistories
                    .FirstOrDefault(c => c.DSRCustomerId == customerId && c.OrderId == orderId);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<IList<OrderPaymentHistory>> GetAllOrderPaymentHistory(int customerId)
        {
            try
            {
                return await _dbContext.OrderPaymentHistories
                    .Include(x => x.DSRCustomer)
                    .Where(x => x.CustomerId == customerId)
                    .OrderByDescending(x => x.Id)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<OrderPaymentHistory>();
            }
        }


        public async Task<IList<OrderPaymentHistory>> GetAllOrderPaymentHistoryByOrderId(int orderId)
        {
            try
            {
                return await _dbContext.OrderPaymentHistories
                    .Include(x => x.Customer)
                    .Include(x => x.DSRCustomer)
                    .Where(x => x.OrderId == orderId && !x.IsDeleted)
                    .OrderByDescending(x => x.Id)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<OrderPaymentHistory>();
            }
        }

        public async Task<IList<OrderPaymentHistory>> GetAllOrderPaymentHistoryList()
        {
            try
            {
                return await _dbContext.OrderPaymentHistories
                    .Include(x => x.Customer)
                    .Include(x => x.Order)
                    .Include(x => x.DSRCustomer)
                    .OrderByDescending(x => x.Id)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<OrderPaymentHistory>();
            }
        }
    }
}
