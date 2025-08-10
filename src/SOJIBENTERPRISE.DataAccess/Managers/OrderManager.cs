namespace SOJIBENTERPRISE.DataAccess
{
    public class OrderManager : BaseDataManager, IOrder
    {
        public OrderManager(BoniyadiContext model) : base(model)
        {
        }

        public bool CreateOrder(Order order)
        {
            if (order == null || order.OrderDetails == null || !order.OrderDetails.Any())
            {
                return false;
            }

            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                _dbContext.Orders.Add(order);
                _dbContext.SaveChanges(); // Save to generate Order ID

                foreach (var item in order.OrderDetails)
                {
                    var entity = _dbContext.Products.FirstOrDefault(x => x.Id == item.ProductId);
                    if (entity != null)
                    {
                        entity.StockQty = entity.StockQty - item.Quantity;
                        _dbContext.SaveChanges();
                    }
                }


                var lastPayment = _dbContext.CustomerPaymentHistories
                                 .Where(p => p.CustomerId == order.CustomerId && p.IsDeleted == false)
                                 .OrderByDescending(p => p.Id)
                                 .FirstOrDefault();

                double totalDueBefore = lastPayment?.TotalDueAfterPayment ?? 0; // If no previous payment, due is 0
                double totalDueAfter = totalDueBefore + order.TotalAmount;

                // Add new entry to CustomerPaymentHistory
                var payment = new CustomerPaymentHistory()
                {
                    CustomerId = order.CustomerId,
                    OrderId = order.Id,
                    PaymentDate = order.Date,
                    PaymentMethodId = order.PaymentMethodId,
                    //TransactionID = order.TransactionID,
                    //Number = order.Number,
                    TotalAmountThisOrder = order.TotalAmount,
                    AmountPaid = order.TotalPay,
                    TotalDueBeforePayment = totalDueBefore,
                    TotalDueAfterPayment = totalDueAfter - order.TotalPay
                };

                _dbContext.CustomerPaymentHistories.Add(payment);
                _dbContext.SaveChanges();


                if (order.TotalPay > 0)
                {
                    var existCurrentBalance = _dbContext.TransactionHistories
                                            .Where(x => !x.IsDeleted)
                                            .AsNoTracking()
                                            .OrderByDescending(x => x.Id)
                                            .FirstOrDefault()?.CurrentBalance ?? 0;

                    TransactionHistory transactionHistory = new()
                    {
                        BalanceIn = order.TotalPay,
                        BalanceOut = 0,
                        CurrentBalance = existCurrentBalance + order.TotalPay,
                        Date = order.Date,
                        OrderId = order.Id,
                        Resone = $"Sales Product",
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

        public bool DeleteOrder(int id)
        {
            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                var existingOrder = _dbContext.Orders
                                        .Include(o => o.OrderDetails)
                                        .FirstOrDefault(o => o.Id == id);

                if (existingOrder != null)
                {
                    existingOrder.IsDeleted = true;

                    _dbContext.Update(existingOrder);
                    _dbContext.SaveChanges();



                    foreach (var item in existingOrder.OrderDetails)
                    {
                        var entity = _dbContext.Products.FirstOrDefault(x => x.Id == item.ProductId);

                        if (entity != null)
                        {
                            entity.StockQty = entity.StockQty + item.Quantity;
                            _dbContext.Update(entity);
                            _dbContext.SaveChanges();
                        }
                    }

                    var existingPaymentDue = _dbContext.CustomerPaymentHistories
                                           .Where(p => p.OrderId == id).ToList();

                    if (existingPaymentDue != null && existingPaymentDue.Count() > 0)
                    {
                        foreach (var payent in existingPaymentDue)
                        {
                            payent.IsDeleted = true;
                            _dbContext.Update(payent);
                            _dbContext.SaveChanges();

                            DeleteRecalculateCustomerPaymentsAsync(payent.CustomerId, payent.Id, payent.AmountPaid);

                        }
                    }

                    var transactionHistory = _dbContext.TransactionHistories
                                           .FirstOrDefault(p => p.OrderId == id);

                    if (transactionHistory != null)
                    {
                        transactionHistory.IsDeleted = true;
                        _dbContext.Update(transactionHistory);
                        _dbContext.SaveChanges();
                    }


                    transaction.Commit();
                    return true;
                }

                transaction.Rollback();
                return false;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return false;
            }
        }

        public bool UpdateOrder(Order order)
        {

            var orderDetailsList = _dbContext.OrderDetails
                                 .Where(o => o.OrderId == order.Id)
                                 .AsNoTracking()
                                 .ToList();

            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                var existingOrder = _dbContext.Orders
                                        .Include(o => o.OrderDetails)
                                        .FirstOrDefault(o => o.Id == order.Id);

                if (existingOrder != null)
                {
                    existingOrder.PaymentMethodId = order.PaymentMethodId;
                    existingOrder.CustomerId = order.CustomerId;
                    existingOrder.DeliveryLocation = order.DeliveryLocation;
                    existingOrder.DeliveryCharge = order.DeliveryCharge;
                    existingOrder.TotalAmount = order.TotalAmount;
                    existingOrder.TotalPay = order.TotalPay;
                    existingOrder.TotalDue = order.TotalDue;
                    existingOrder.Date = order.Date;

                    existingOrder.OrderDetails = order.OrderDetails;

                    _dbContext.Update(existingOrder);
                    _dbContext.SaveChanges();

                    var existingPaymentDue = _dbContext.CustomerPaymentHistories
                                            .AsNoTracking()
                                            .FirstOrDefault(p => p.OrderId == order.Id
                                            );

                    var existingPayment = _dbContext.CustomerPaymentHistories
                                           .FirstOrDefault(p => p.OrderId == order.Id
                                           && p.CustomerId == order.CustomerId);

                    var allProductIds = orderDetailsList.Select(p => p.ProductId)
                                .Union(order.OrderDetails.Select(p => p.ProductId))
                                .Distinct();

                    var oldDetails = orderDetailsList ?? new List<OrderDetail>();
                    var newDetails = order.OrderDetails ?? new List<OrderDetail>();

                    var differences = allProductIds.Select(productId =>
                    {
                        var oldQty = oldDetails.FirstOrDefault(p => p.ProductId == productId)?.Quantity ?? 0;
                        var newQty = newDetails.FirstOrDefault(p => p.ProductId == productId)?.Quantity ?? 0;

                        return new ProductQuantityDifference
                        {
                            ProductId = productId,
                            QuantityDifference = oldQty - newQty,
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

                    if (existingPayment != null)
                    {
                        existingPayment.CustomerId = order.CustomerId;
                        existingPayment.PaymentMethodId = order.PaymentMethodId;
                        existingPayment.PaymentDate = order.Date;
                        //existingPayment.TransactionID = order.TransactionID;
                        existingPayment.TotalAmountThisOrder = order.TotalAmount;
                        existingPayment.AmountPaid = order.TotalPay;
                        existingPayment.TotalDueBeforePayment = order.TotalPay;
                        existingPayment.TotalDueAfterPayment = (existingPayment.TotalDueBeforePayment + order.TotalAmount) - order.TotalPay;

                        var UpDownAmount = existingPaymentDue.TotalDueAfterPayment - existingPayment.TotalDueAfterPayment;

                        _dbContext.Update(existingPayment);
                        _dbContext.SaveChanges();

                        RecalculateCustomerPaymentsAsync(existingPayment.CustomerId, existingPayment.Id, UpDownAmount);


                    }
                    else
                    {
                        var existingCustomerPaymentHistories = _dbContext.CustomerPaymentHistories
                                            .FirstOrDefault(p => p.OrderId == order.Id);

                        existingCustomerPaymentHistories.IsDeleted = true;
                        _dbContext.Update(existingCustomerPaymentHistories);
                        _dbContext.SaveChanges();


                        var lastPayment = _dbContext.CustomerPaymentHistories
                                 .Where(p => p.CustomerId == order.CustomerId)
                                 .OrderByDescending(p => p.Id)
                                 .FirstOrDefault();

                        double totalDueBefore = lastPayment?.TotalDueAfterPayment ?? 0; // If no previous payment, due is 0
                        double totalDueAfter = totalDueBefore + order.TotalAmount;

                        // Add new entry to CustomerPaymentHistory
                        var payment = new CustomerPaymentHistory()
                        {
                            CustomerId = order.CustomerId,
                            OrderId = order.Id,
                            PaymentDate = order.Date,
                            PaymentMethodId = order.PaymentMethodId,
                            //TransactionID = order.TransactionID,
                            // Number = order.Number,
                            TotalAmountThisOrder = order.TotalAmount,
                            AmountPaid = order.TotalPay,
                            TotalDueBeforePayment = totalDueBefore,
                            TotalDueAfterPayment = totalDueAfter - order.TotalPay
                        };

                        _dbContext.CustomerPaymentHistories.Add(payment);
                        _dbContext.SaveChanges();

                    }


                    transaction.Commit();
                    return true;
                }

                transaction.Rollback();
                return false;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return false;
            }
        }


        private void DeleteRecalculateCustomerPaymentsAsync(int customerId, int id, double amount)
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

        public async Task<Order> GetOrderById(int orderId)
        {
            try
            {
                return await _dbContext.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == orderId && !o.IsDeleted);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<OrderInfo> GetOrderInfoById(int orderId)
        {
            try
            {
                return await _dbContext.Orders
                    .AsNoTracking()
                    .Where(o => o.Id == orderId)
                    .Select(o => new OrderInfo
                    {
                        OrderId = o.Id,
                        Area = o.SelectedRoad, // Assuming Area maps to DeliveryLocation
                        CustomerName = o.Customer.Name, // Ensure Customer navigation property is included
                        OrderDate = o.Date
                    })
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                // Optional: log the exception
                return null;
            }
        }

        public PersonInfoDTO GetPersonInfo(int personId)
        {
            try
            {
                return _dbContext.Customers
                  .Where(o => o.Id == personId)
                      .Select(o => new PersonInfoDTO
                      {
                          Id = o.Id,
                          Name = o.Name,
                          Email = o.Email,
                          Phone = o.Phone,
                          OptionalPhone = o.OptionalPhone,
                          HouseName = o.HouseName,
                          HouseNumber = o.HouseNumber,
                          Village = o.Village,
                          Upazilla = o.Upazilla,
                          District = o.District,
                      })
                  .FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public IEnumerable<OrdersByPersonDTO> GetAllOrdersByPerson(int personId)
        {
            try
            {
                var orders = _dbContext.Orders
                   .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                        .ThenInclude(p => p.ProductsSize)
                   .Where(o => o.CustomerId == personId)
                   .Select(o => new OrdersByPersonDTO
                   {
                       OrderId = o.Id,
                       ProductsName = string.Join(", ", o.OrderDetails.Select(od => od.Product.DisplayNameSize)), // Comma-separated product names
                       OrderDate = o.Date,
                       Area = o.SelectedRoad,
                   })
                   .OrderByDescending(o => o.OrderId)
                    .ToList();

                return orders;
            }
            catch (Exception ex)
            {
                return Enumerable.Empty<OrdersByPersonDTO>();
            }
        }

        public IEnumerable<CustomerDueDTO> GetCustomerDueHistory()
        {
            try
            {
                var query = from customer in _dbContext.Customers
                            join payment in _dbContext.CustomerPaymentHistories.Where(p => !p.IsDeleted)
                                on customer.Id equals payment.CustomerId into paymentsGroup
                            select new CustomerDueDTO
                            {
                                Id = customer.Id,
                                Name = customer.Name,
                                Phone = customer.Phone,
                                HouseName = customer.HouseName,
                                HouseNumber = customer.HouseNumber,
                                Village = customer.Village,
                                Upazilla = customer.Upazilla,
                                District = customer.District,
                                TotalDue = paymentsGroup.OrderByDescending(p => p.Id)
                                                        .Select(p => (double?)p.TotalDueAfterPayment)
                                                        .FirstOrDefault() ?? 0
                            };

                return query.Where(c => c.TotalDue != 0)
                            .OrderByDescending(c => c.TotalDue)
                            .ToList();
            }
            catch (Exception ex)
            {
                return Enumerable.Empty<CustomerDueDTO>();
            }
        }

        public async Task<IEnumerable<OrdersDTO>> GetAllOrders(DateTime? startDate, DateTime? endDate)
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


                var orders = await _dbContext.Orders
                    .Where(x => !x.IsDeleted && x.Date.Date >= fromDate && x.Date.Date <= toDate)
                    .Include(o => o.Customer)
                    .Include(o => o.OrderDetails)
                    .Include(o => o.CustomerPaymentHistories)
                    .Include(o => o.OrderPaymentHistories)
                    .Include(o => o.DailyExpenses)
                    .Include(o => o.DamageProducts)
                    .Include(o => o.DSRShopDues)
                    .Include(o => o.CustomerProductReturns)

                    .Select(o => new OrdersDTO
                    {
                        Id = o.Id,
                        Name = o.Customer.Name,
                        CustomerId = o.Customer.Id,
                        Address = o.SelectedRoad,
                        OrderDate = o.Date,
                        TotalPrice = o.TotalAmount,

                        // Lock if any related table has data
                        IsLock = o.OrderPaymentHistories.Any(x => !x.IsDeleted) ||
                                 o.DailyExpenses.Any(x => !x.IsDeleted) ||
                                 o.DamageProducts.Any() ||
                                 o.SRDiscounts.Any(x => !x.IsDeleted) ||
                                 o.DSRShopDues.Any(x => !x.IsDeleted) ||
                                 o.CustomerProductReturns.Any(r =>
                                    r.CustomerProductReturnDetails.Any(d => d.ReturnQuantity > 0))
                    })
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();

                return orders;
            }
            catch (Exception ex)
            {
                return Enumerable.Empty<OrdersDTO>();
            }
        }

        public IEnumerable<OrderDetailsDTO> GetOrderDetailsByOrderId(int orderId)
        {
            try
            {
                return _dbContext.OrderDetails
                .Include(od => od.Product)
                .Where(od => od.OrderId == orderId)
                .Select(od => new OrderDetailsDTO
                {
                    ProductName = od.Product.Name,
                    Quantity = od.Quantity,
                    ProductPrice = od.Product.SellingPrice,
                    Discount = od.Discount,
                    TotalPrice = od.Price
                })
                .ToList();
            }
            catch (Exception ex)
            {
                return Enumerable.Empty<OrderDetailsDTO>();
            }
        }

        public async Task<IEnumerable<OrderDetailsDTO>> GetOrderDetailsByOrderAsync(int orderId)
        {
            try
            {
                var orderDetails = await _dbContext.OrderDetails
                    .Include(od => od.Product)
                    .ThenInclude(od => od.ProductsSize)
                    .Where(od => od.OrderId == orderId)
                    .Select(od => new OrderDetailsDTO
                    {
                        ProductName = od.Product.DisplayNameSize,
                        Quantity = od.Quantity,
                        ReturnQuantity = od.ReturnQuantity ?? 0, // Handle null with default value
                        SellingQuantity = od.Quantity - (od.ReturnQuantity ?? 0),
                        ProductPrice = od.UnitPrice,
                        TotalProductPrice = od.Quantity * od.UnitPrice,
                        ReturnPrice = (od.ReturnQuantity ?? 0) * od.UnitPrice,
                        Discount = od.Discount,
                        TotalPrice = (od.Quantity - (od.ReturnQuantity ?? 0)) * od.UnitPrice - od.Discount
                    })
                    .ToListAsync();

                return orderDetails;
            }
            catch (Exception ex)
            {
                // Consider logging the exception here
                // _logger.LogError(ex, "Error getting order details for order {OrderId}", orderId);
                return Enumerable.Empty<OrderDetailsDTO>();
            }
        }

        public async Task<IEnumerable<OrderDetailsDTO>> GetOrderDetailsByOrderAsync2(int orderId)
        {
            try
            {
                // First get the order to access its date
                var order = await _dbContext.Orders
                    .Where(o => o.Id == orderId)
                    .FirstOrDefaultAsync();

                if (order == null)
                    return Enumerable.Empty<OrderDetailsDTO>();

                var orderDate = order.Date; // Assuming there's a Date property on Order

                var orderDetails = await _dbContext.OrderDetails
                    .Include(od => od.Product)
                    .ThenInclude(od => od.ProductsSize)
                    .Where(od => od.OrderId == orderId)
                    .ToListAsync();

                var returnDetails = await _dbContext.CustomerProductReturnDetails
                    .Include(rd => rd.CustomerProductReturn)
                    .Where(rd => rd.CustomerProductReturn.OrderId == orderId && !rd.CustomerProductReturn.IsDeleted)
                    .ToListAsync();

                // Get all product IDs from the order
                var productIds = orderDetails.Select(od => od.ProductId).Distinct().ToList();

                // Get the most recent price history for each product before or on the order date
                var priceHistories = await _dbContext.PriceHistories
                    .Where(ph => productIds.Contains(ph.ProductId) && ph.Date <= orderDate)
                    .GroupBy(ph => ph.ProductId)
                    .Select(g => g.OrderByDescending(ph => ph.Date).FirstOrDefault())
                    .ToListAsync();

                var result = orderDetails
                    .GroupJoin(
                        returnDetails,
                        od => od.ProductId,
                        rd => rd.ProductId,
                        (od, rdGroup) => new { od, rdGroup }
                    )
                    .Select(group =>
                    {
                        var returnQuantity = group.rdGroup.Sum(r => r.ReturnQuantity);
                        var sellingQuantity = group.od.Quantity - returnQuantity;

                        // Find the price history for this product
                        var priceHistory = priceHistories.FirstOrDefault(ph => ph.ProductId == group.od.ProductId);

                        // Use the historical price if available, otherwise fall back to current price
                        var productPrice = priceHistory?.SellingNewPrice ?? group.od.Product.SellingPrice;
                        var discount = group.od.Discount;

                        return new OrderDetailsDTO
                        {
                            ProductName = group.od.Product.DisplayNameSize,
                            Quantity = group.od.Quantity,
                            ReturnQuantity = returnQuantity,
                            SellingQuantity = sellingQuantity,
                            ProductPrice = productPrice,
                            TotalProductPrice = group.od.Quantity * productPrice,
                            ReturnPrice = returnQuantity * productPrice,
                            Discount = discount,
                            TotalPrice = (sellingQuantity * productPrice) - discount
                        };
                    })
                    .ToList();

                return result;
            }
            catch (Exception ex)
            {
                // Log if necessary
                return Enumerable.Empty<OrderDetailsDTO>();
            }
        }

        public async Task<IEnumerable<DamageProductDetailsDTO>> GetDamageProductDetailsByOrderAsync(int orderId)
        {
            try
            {
                return await _dbContext.DamageProductReturnDetails
                .Include(od => od.Product)
                .ThenInclude(od => od.ProductsSize)
                .Include(rd => rd.DamageProductReturn)
                 .Where(rd => rd.DamageProductReturn.OrderId == orderId) // Fixed line
                .Select(od => new DamageProductDetailsDTO
                {
                    ProductName = od.Product.DisplayNameSize,
                    Quantity = od.Quantity,
                    ProductPrice = od.UnitPrice,

                    TotalPrice = od.Price
                })
                .ToListAsync();
            }
            catch (Exception ex)
            {
                return Enumerable.Empty<DamageProductDetailsDTO>();
            }
        }

        public OrderInfoDTO OrderInfoById(int orderId)
        {
            try
            {
                OrderInfoDTO invoice = _dbContext.Orders
               .Include(o => o.Customer)
               .Where(o => o.Id == orderId)
               .Select(o => new OrderInfoDTO
               {
                   Id = o.Id,
                   Name = o.Customer.Name,
                   Email = o.Customer.Email,
                   Phone = o.Customer.Phone,
                   OptionalPhone = o.Customer.OptionalPhone,
                   HouseName = o.Customer.HouseName,
                   Village = o.Customer.Village,
                   Upozilla = o.Customer.Upazilla,
                   District = o.Customer.District,
                   HouseNumber = o.Customer.HouseNumber,

                   OrderDate = o.Date,
                   OrderId = o.Id,
                   PaymentMethod = o.PaymentMethod.Name,
                   DeliveryLocation = o.DeliveryLocation,
                   ShippingMethod = " ",
                   //ShippingMethod = o.ShippingMethod.Name,
               }).FirstOrDefault();

                return invoice;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public InvoiceDTO GetOrdersById(int orderId)
        {
            try
            {
                InvoiceDTO invoice = _dbContext.Orders
               .Include(o => o.Customer)
               .Where(o => o.Id == orderId)
               .Select(o => new InvoiceDTO
               {
                   Id = o.Id,
                   Name = o.Customer.Name,
                   Email = o.Customer.Email,
                   Phone = o.Customer.Phone,
                   OptionalPhone = o.Customer.OptionalPhone,
                   HouseName = o.Customer.HouseName,
                   Village = o.Customer.Village,
                   Upozilla = o.Customer.Upazilla,
                   District = o.Customer.District,
                   HouseNumber = o.Customer.HouseNumber,
               }).FirstOrDefault();

                return invoice;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<IList<CustomerPaymentHistoryDTO>> GetCustomerPaymentHistoryById(int customerId)
        {
            try
            {
                var customerPaymentHistory = await _dbContext.CustomerPaymentHistories
                  .Where(x => x.CustomerId == customerId && !x.IsDeleted)
                  .Select(x => new CustomerPaymentHistoryDTO
                  {
                      Id = x.Id,
                      TotalAmountThisOrder = x.TotalAmountThisOrder,
                      AmountPaid = x.AmountPaid,
                      TotalDueBeforePayment = x.TotalDueBeforePayment,
                      TotalDueAfterPayment = x.TotalDueAfterPayment,
                      PaymentDate = x.PaymentDate,
                      PaymentMethod = x.PaymentMethod.Name,
                      TransactionID = x.TransactionID,
                      Number = x.Number,
                      IsDisabled = x.OrderId.HasValue ||
                            x.CustomerProductReturnId.HasValue ||
                            x.DamageProductReturnId.HasValue ||
                            x.DSRShopDueId.HasValue ||
                            x.DailyExpenseId.HasValue ||
                            x.SRDiscountId.HasValue ||
                            x.OrderPaymentHistoryId.HasValue

                  })
                  .OrderByDescending(x => x.Id)
                  .ToListAsync();

                return customerPaymentHistory;
            }
            catch (Exception ex)
            {
                return new List<CustomerPaymentHistoryDTO>();
            }
        }

        public List<(string name, double value)> GetPayment(int id)
        {
            var order = _dbContext.Orders.FirstOrDefault(o => o.Id == id);
            if (order is not null)
            {
                return new List<(string Name, double Value)>
                {
                    ("Delivery Charge", order.DeliveryCharge),
                    ("Grand Total", order.TotalAmount),
                    ("Discount", order.Discount),
                    ("Total Pay", order.TotalPay),
                    ("Total Due", order.TotalDue)
                };
            }
            else
            {
                return new List<(string Name, double Value)>();
            }
        }

        public CustomerPaymentHistoryDTO DuePayment(int id)
        {
            try
            {
                CustomerPaymentHistoryDTO customerPaymentHistory = _dbContext.CustomerPaymentHistories
                .Where(x => x.Id == id)
                  .Select(x => new CustomerPaymentHistoryDTO
                  {
                      Id = x.Id,
                      TotalAmountThisOrder = x.TotalAmountThisOrder,
                      AmountPaid = x.AmountPaid,
                      TotalDueBeforePayment = x.TotalDueBeforePayment,
                      TotalDueAfterPayment = x.TotalDueAfterPayment,
                      PaymentDate = x.PaymentDate,
                      PaymentMethod = x.PaymentMethod.Name,
                      TransactionID = x.TransactionID,
                      Number = x.Number,
                  })
                  .FirstOrDefault();

                return customerPaymentHistory;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}