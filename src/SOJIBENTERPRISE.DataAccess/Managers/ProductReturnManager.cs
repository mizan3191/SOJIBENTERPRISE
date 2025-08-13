namespace SOJIBENTERPRISE.DataAccess
{
    public class ProductReturnManager : BaseDataManager, IProductReturn
    {
        public ProductReturnManager(BoniyadiContext model) : base(model)
        {
        }

        public bool CreateCustomerProductReturn(CustomerProductReturn customerProductReturn)
        {
            if (customerProductReturn == null || customerProductReturn.CustomerProductReturnDetails == null || !customerProductReturn.CustomerProductReturnDetails.Any())
            {
                return false;
            }

            using var transaction = _dbContext.Database.BeginTransaction();


            try
            {

                var details = customerProductReturn.CustomerProductReturnDetails;
                customerProductReturn.CustomerProductReturnDetails = null;

                _dbContext.CustomerProductReturns.Add(customerProductReturn);

                _dbContext.SaveChanges(); // Save to generate Customer Product Return ID

                var returnId = customerProductReturn.Id;
                foreach (var item in details)
                {
                    if (item != null)
                    {
                        CustomerProductReturnDetails customerProductReturnDetails = new();

                        customerProductReturnDetails = item;
                        customerProductReturnDetails.Id = 0;
                        customerProductReturnDetails.CustomerProductReturnId = returnId;
                        _dbContext.CustomerProductReturnDetails.Add(item);
                        _dbContext.SaveChanges();
                    }

                    var orderEntity = _dbContext.OrderDetails
                      .FirstOrDefault(p => p.OrderId == customerProductReturn.OrderId
                      && p.ProductId == item.ProductId
                      );

                    orderEntity.ReturnQuantity = item.ReturnQuantity;

                }

                foreach (var item in details)
                {
                    var entity = _dbContext.Products.FirstOrDefault(x => x.Id == item.ProductId);
                    if (entity != null)
                    {
                        entity.StockQty += item.ReturnQuantity;
                        _dbContext.SaveChanges();
                    }
                }

                var lastPayment = _dbContext.CustomerPaymentHistories
                                 .Where(p => p.CustomerId == customerProductReturn.CustomerId)
                                 .OrderByDescending(p => p.Id)
                                 .FirstOrDefault();

                double totalDueBefore = lastPayment?.TotalDueAfterPayment ?? 0; // If no previous payment, due is 0
                double totalDueAfter = totalDueBefore - customerProductReturn.TotalAmount;

                // Add new entry to Customer Payment History
                var payment = new CustomerPaymentHistory()
                {
                    CustomerId = customerProductReturn.CustomerId,
                    OrderId = customerProductReturn.OrderId,
                    CustomerProductReturnId = returnId,
                    PaymentDate = customerProductReturn.Date,
                    PaymentMethodId = 11,
                    TransactionID = string.Empty,
                    Number = string.Empty,
                    TotalAmountThisOrder = 0,
                    AmountPaid = customerProductReturn.TotalAmount,
                    TotalDueBeforePayment = totalDueBefore,
                    TotalDueAfterPayment = totalDueAfter
                };

                _dbContext.CustomerPaymentHistories.Add(payment);
                _dbContext.SaveChanges();

                transaction.Commit();

                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return false;
            }
        }

        public bool UpdateCustomerProductReturn(CustomerProductReturn customerProductReturn)
        {
            if (customerProductReturn == null || customerProductReturn.CustomerProductReturnDetails == null || !customerProductReturn.CustomerProductReturnDetails.Any())
            {
                return false;
            }

            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {

                // Reverse stock changes from previous return
                foreach (var details in customerProductReturn.CustomerProductReturnDetails)
                {
                    var newQuentity = details.ReturnQuantity;
                    var entity = _dbContext.CustomerProductReturnDetails.FirstOrDefault(p => p.Id == details.Id);

                    var oldQuentity = entity.ReturnQuantity;

                    var product = _dbContext.Products.FirstOrDefault(p => p.Id == details.ProductId);
                    if (product is not null)
                    {
                        product.StockQty = (product.StockQty - oldQuentity) + newQuentity;
                    }

                    if (entity is not null)
                    {
                        entity.ReturnPrice = details.ReturnPrice;
                        entity.Price = details.Price;
                        entity.Quantity = details.Quantity;
                        entity.ReturnQuantity = details.ReturnQuantity;
                        entity.Discount = details.Discount;
                    }

                    var orderEntity = _dbContext.OrderDetails
                        .FirstOrDefault(p => p.OrderId == customerProductReturn.OrderId
                        && p.ProductId == details.ProductId
                        );

                    orderEntity.ReturnQuantity = entity.ReturnQuantity;
                }

                var existingReturn = _dbContext.CustomerProductReturns
                    .Include(r => r.CustomerProductReturnDetails)
                    .FirstOrDefault(r => r.Id == customerProductReturn.Id);

                if (existingReturn == null)
                    return false;

                // Update main return entity
                existingReturn.Date = customerProductReturn.Date;
                existingReturn.TotalAmount = customerProductReturn.TotalAmount;
                existingReturn.PaymentMethod = customerProductReturn.PaymentMethod;

                // Update payment history for this return
                var payment = _dbContext.CustomerPaymentHistories
                    .FirstOrDefault(p => p.CustomerProductReturnId == customerProductReturn.Id);

                var UpDownAmount = payment.AmountPaid - customerProductReturn.TotalAmount;


                if (payment != null)
                {
                    double totalDueBefore = payment.TotalDueBeforePayment;
                    double totalDueAfter = totalDueBefore - customerProductReturn.TotalAmount;

                    payment.PaymentDate = customerProductReturn.Date;
                    payment.AmountPaid = customerProductReturn.TotalAmount;
                    payment.TotalDueAfterPayment = totalDueAfter;
                }

                _dbContext.SaveChanges();
                transaction.Commit();

                RecalculateCustomerPaymentsAsync(payment.CustomerId, payment.Id, UpDownAmount);

                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();

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

        public bool DeleteCustomerProductReturn(int customerProductReturnId)
        {
            try
            {
                using var transaction = _dbContext.Database.BeginTransaction();
                var customerProductReturn = _dbContext.CustomerProductReturns
                    .Include(x => x.CustomerProductReturnDetails)
                    .FirstOrDefault(x => x.Id == customerProductReturnId);

                if (customerProductReturn == null)
                    return false;

                // Reverse stock changes from previous return
                foreach (var details in customerProductReturn.CustomerProductReturnDetails)
                {
                    var entity = _dbContext.CustomerProductReturnDetails.FirstOrDefault(p => p.Id == details.Id);
                    var product = _dbContext.Products.FirstOrDefault(p => p.Id == details.ProductId);

                    if (product is not null)
                    {
                        product.StockQty = product.StockQty - entity.ReturnQuantity;
                    }
                }

                // Update main return entity
                customerProductReturn.Date = DateTime.UtcNow;
                customerProductReturn.IsDeleted = true;

                var lastPayment = _dbContext.CustomerPaymentHistories
                                 .Where(p => p.CustomerId == customerProductReturn.CustomerId)
                                 .OrderByDescending(p => p.Id)
                                 .FirstOrDefault();

                double totalDueBefore = lastPayment?.TotalDueAfterPayment ?? 0; // If no previous payment, due is 0
                double totalDueAfter = totalDueBefore + customerProductReturn.TotalAmount;

                // Add new entry to Customer Payment History
                var payment = new CustomerPaymentHistory()
                {
                    CustomerId = customerProductReturn.CustomerId,
                    OrderId = customerProductReturn.OrderId,
                    CustomerProductReturnId = customerProductReturn.Id,
                    PaymentDate = DateTime.Now,
                    PaymentMethodId = 11,
                    TransactionID = string.Empty,
                    Number = string.Empty,
                    TotalAmountThisOrder = 0,
                    AmountPaid = customerProductReturn.TotalAmount,
                    TotalDueBeforePayment = totalDueBefore,
                    TotalDueAfterPayment = totalDueAfter
                };

                _dbContext.CustomerPaymentHistories.Add(payment);

                _dbContext.SaveChanges();
                transaction.Commit();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<CustomerProductReturn> GetCustomerProductReturnByOrderId(int orderId)
        {
            try
            {
                // Get the order with details
                Order order = await _dbContext.Orders
                    .Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                    return null;

                // Check for existing returns for this order
                var existingReturn = await _dbContext.CustomerProductReturns
                    .Include(r => r.CustomerProductReturnDetails)
                    .FirstOrDefaultAsync(r => r.OrderId == orderId);

                if (existingReturn == null)
                {
                    // No existing return - create new one with all products
                    CustomerProductReturn customerProductReturn = new()
                    {
                        OrderId = order.Id,
                        CustomerId = order.CustomerId,
                        Date = DateTime.UtcNow,
                        PaymentMethod = "Product Return",
                        TotalAmount = order.TotalAmount,
                        CustomerProductReturnDetails = order.OrderDetails.Select(od => new CustomerProductReturnDetails
                        {
                            ProductId = od.ProductId,
                            UnitPrice = od.UnitPrice,
                            Quantity = od.Quantity,
                            ReturnQuantity = 0, // Initialize with 0 return quantity
                            Price = od.Price,
                            ReturnPrice = 0,
                            Discount = od.Discount
                        }).ToList()
                    };
                    return customerProductReturn;
                }
                else
                {
                    // Existing return found - update it with current order details
                    existingReturn.Date = DateTime.UtcNow;
                    existingReturn.TotalAmount = order.TotalAmount;

                    // Get list of product IDs in current order
                    var currentOrderProductIds = order.OrderDetails.Select(od => od.ProductId).ToList();

                    // Find return details to remove (products no longer in the order)
                    var detailsToRemove = existingReturn.CustomerProductReturnDetails
                        .Where(rd => !currentOrderProductIds.Contains(rd.ProductId))
                        .ToList();

                    // Remove them one by one
                    foreach (var detail in detailsToRemove)
                    {
                        existingReturn.CustomerProductReturnDetails.Remove(detail);
                    }

                    // Process each order detail to update return details
                    foreach (var orderDetail in order.OrderDetails)
                    {
                        var existingReturnDetail = existingReturn.CustomerProductReturnDetails
                            .FirstOrDefault(rd => rd.ProductId == orderDetail.ProductId);

                        if (existingReturnDetail == null)
                        {
                            // New product in order - add to return details
                            existingReturn.CustomerProductReturnDetails.Add(new CustomerProductReturnDetails
                            {
                                ProductId = orderDetail.ProductId,
                                UnitPrice = orderDetail.UnitPrice,
                                Quantity = orderDetail.Quantity,
                                ReturnQuantity = 0, // Initialize with 0 return quantity
                                Price = orderDetail.Price,
                                ReturnPrice = 0,
                                Discount = orderDetail.Discount
                            });
                        }
                        else
                        {
                            // Existing product - update quantities and prices
                            existingReturnDetail.Quantity = orderDetail.Quantity;
                            existingReturnDetail.UnitPrice = orderDetail.UnitPrice;
                            existingReturnDetail.Price = orderDetail.Price;
                            existingReturnDetail.Discount = orderDetail.Discount;

                            // Don't modify ReturnQuantity as it might have previous returns
                        }
                    }

                    return existingReturn;
                }
            }
            catch (Exception ex)
            {
                // Optionally log the exception
                return null;
            }
        }

        public async Task<CustomerProductReturn> GetCustomerProductReturnById(int id)
        {
            try
            {
                CustomerProductReturn customerProductReturn = await _dbContext.CustomerProductReturns
                    .Include(o => o.CustomerProductReturnDetails)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (customerProductReturn == null)
                    return null;

                return customerProductReturn;
            }
            catch (Exception ex)
            {
                // Optionally log the exception
                return null;
            }
        }

        public async Task<CustomerProductReturn> GetExistingCustomerProductReturnByOrderId(int orderId)
        {
            try
            {
                CustomerProductReturn customerProductReturn = await _dbContext.CustomerProductReturns
                    .Include(o => o.CustomerProductReturnDetails)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (customerProductReturn == null)
                    return null;

                return customerProductReturn;
            }
            catch (Exception ex)
            {
                // Optionally log the exception
                return null;
            }
        }

        public async Task<IEnumerable<CustomerProductReturnDTO>> GetAllCustomerReturnProducts()
        {
            try
            {
                var orders = await _dbContext.CustomerProductReturns
                        .Include(o => o.Customer)
                        .Include(o => o.CustomerProductReturnDetails)
                            .ThenInclude(d => d.Product)
                        .Where(o => o.IsDeleted == false)
                        .OrderByDescending(o => o.Id)
                        .Select(o => new CustomerProductReturnDTO
                        {
                            Id = o.Id,
                            OrderId = o.OrderId,
                            CustomerName = o.Customer.Name,
                            Products = string.Join(", ",
                                o.CustomerProductReturnDetails.Select(d =>
                                    d.Product.Name + "(" + d.ReturnQuantity + ")")),
                            OrderDate = o.Date,
                            TotalPrice = o.TotalAmount
                        })
                        .ToListAsync();

                return orders;
            }
            catch (Exception ex)
            {
                return Enumerable.Empty<CustomerProductReturnDTO>();
            }
        }

    }
}