using Microsoft.EntityFrameworkCore;

namespace SOJIBENTERPRISE.DataAccess
{
    public class ProductManager : BaseDataManager, IProduct
    {
        public ProductManager(BoniyadiContext model) : base(model)
        {
        }

        public bool UpdateProduct(Product updatedProduct)
        {
            if (updatedProduct == null || updatedProduct.Id <= 0)
            {
                return false; // Invalid product
            }

            var productNoTracking = _dbContext.Products
                .AsNoTracking()
                .FirstOrDefault(p => p.Id == updatedProduct.Id);

            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {

                // Store original prices before any changes
                var originalBuyingPrice = productNoTracking.BuyingPrice;
                var originalSellingPrice = productNoTracking.SellingPrice;

                // Update all properties
                // _dbContext.Entry(existingProduct).CurrentValues.SetValues(updatedProduct);

                _dbContext.Update(updatedProduct);
                _dbContext.SaveChanges();

                // Check if prices changed by comparing original to new values
                if (updatedProduct.BuyingPrice != originalBuyingPrice ||
                    updatedProduct.SellingPrice != originalSellingPrice)
                {
                    _dbContext.PriceHistories.Add(new PriceHistory
                    {
                        ProductId = updatedProduct.Id,
                        BuyingOldPrice = originalBuyingPrice,
                        BuyingNewPrice = updatedProduct.BuyingPrice,
                        SellingOldPrice = originalSellingPrice,
                        SellingNewPrice = updatedProduct.SellingPrice,
                        Date = DateTime.Now
                    });
                }

                _dbContext.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                transaction.Rollback();
                // Log the concurrency exception
                Console.WriteLine($"Concurrency error updating product: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                // Log the general exception
                Console.WriteLine($"Error updating product: {ex.Message}");
                return false;
            }
        }

        public int CreateProduct(Product product)
        {
            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                _dbContext.Products.Add(product);
                _dbContext.SaveChanges();

                var category = _dbContext.ProductCategories.FirstOrDefault(x => x.Id == product.ProductCategoryId);
                var newNumber = category.ProductNo + 1;

                product.ProductNo = newNumber;
                category.ProductNo = newNumber;

                // Save the updated ProductNo
                _dbContext.ProductCategories.Update(category);
                _dbContext.SaveChanges();

                _dbContext.PriceHistories.Add(new PriceHistory
                {
                    ProductId = product.Id,
                    BuyingOldPrice = product.BuyingPrice,
                    BuyingNewPrice = product.BuyingPrice,
                    SellingOldPrice = product.SellingPrice,
                    SellingNewPrice = product.SellingPrice,
                    Date = DateTime.Now
                });

                _dbContext.SaveChanges();

                transaction.Commit();
                return product.Id;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                transaction.Rollback();
                // Log the concurrency exception
                Console.WriteLine($"Concurrency error added product: {ex.Message}");
                return 0;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                // Log the general exception
                Console.WriteLine($"Error added product: {ex.Message}");
                return 0;
            }
        }

        public Product GetProduct(int id)
        {
            try
            {
                return _dbContext.Products
                    .Include(c => c.ProductCategory)
                    .Include(c => c.ProductsSize)
                    .Include(c => c.Packaging)
                    .Include(c => c.Supplier)
                    .FirstOrDefault(c => c.Id == id);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Product GetProductPrice(int id)
        {
            try
            {
                return _dbContext.Products
                    .AsNoTracking()
                    .FirstOrDefault(c => c.Id == id);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<IList<Product>> GetAllProduct()
        {
            try
            {
                return await _dbContext.Products
                .Include(x => x.Supplier)
                .Include(x => x.UnitOfMeasurement)
                .Include(x => x.ProductsSize)
                .Include(x => x.Packaging)
                .OrderBy(x => x.Name)                
                .ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<Product>();
            }
        }

        public async Task<IList<ProductDTO>> GetAllReOrderProducts()
        {
            try
            {
                return await _dbContext.Products
                    .Include(x => x.Supplier)
                    .Include(x => x.ProductsSize)
                    .Where(p => p.StockQty <= p.ReOrderLevel && !p.Supplier.IsDisable) // Filter products with low stock
                    .Select(p => new ProductDTO
                    {
                        Id = p.Id,
                        Name = p.DisplayNameSize,
                        UnitOfMeasurementId = p.UnitOfMeasurementId ?? null,
                        ReOrderLevel = p.ReOrderLevel,
                        SupplierId = p.SupplierId,
                        SupplierName = p.Supplier.Name,
                        StockQty = p.StockQty
                    })
                    .OrderByDescending(c => c.Id)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<ProductDTO>(); // Ensure correct return type
            }
        }

        public bool DeleteProduct(int id)
        {
            return RemoveEntity<Product>(id);
        }


        public bool UpdateProductConsumption(ProductConsumption ProductConsumption)
        {
            try
            {
                var entity = _dbContext.Products.FirstOrDefault(x => x.Id == ProductConsumption.ProductId);
                var Adjustment = _dbContext.ProductConsumptions.AsNoTracking().FirstOrDefault(x => x.Id == ProductConsumption.Id).QuantityConsumed;
                entity.StockQty = (entity.StockQty + Adjustment) - ProductConsumption.QuantityConsumed;

                AddUpdateEntity(ProductConsumption);
                _dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public int CreateProductConsumption(ProductConsumption ProductConsumption)
        {
            try
            {
                _dbContext.ProductConsumptions.Add(ProductConsumption);
                _dbContext.SaveChanges();

                var entity = _dbContext.Products.FirstOrDefault(x => x.Id == ProductConsumption.ProductId);
                entity.StockQty = entity.StockQty - ProductConsumption.QuantityConsumed;
                _dbContext.SaveChanges();

                return ProductConsumption.Id;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }


        public bool DeleteProductConsumption(int id)
        {
            try
            {
                var productConsumption = _dbContext.ProductConsumptions.FirstOrDefault(x => x.Id == id);

                var entity = _dbContext.Products.FirstOrDefault(x => x.Id == productConsumption.ProductId);
                entity.StockQty = entity.StockQty + productConsumption.QuantityConsumed;

                _dbContext.Remove(productConsumption);
                _dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public ProductConsumption GetProductConsumption(int id)
        {
            try
            {
                return _dbContext.ProductConsumptions.SingleOrDefault(c => c.Id == id);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<IList<ProductConsumptionDTO>> GetAllProductConsumption(DateTime? startDate, DateTime? endDate)
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



                return await _dbContext.ProductConsumptions
                    .Include(x => x.Product)
                    .ThenInclude(x => x.ProductsSize)
                    .Include(x => x.Customer)
                    .Include(x => x.ReasonofAdjustment)
                    .Where(x => !x.IsDeleted && x.DateConsumed.Date >= fromDate && x.DateConsumed.Date <= toDate)
                    .Select(x => new ProductConsumptionDTO
                    {
                        Id = x.Id,
                        QuantityConsumed = x.QuantityConsumed,
                        ReasonOfConsumed = x.ReasonofAdjustment.Name,
                        DateConsumed = x.DateConsumed,
                        Person = x.Customer != null ? x.Customer.Name : string.Empty,
                        ProductName = x.Product != null ? x.Product.DisplayNameSize : string.Empty
                    })
                    .OrderByDescending(x => x.Id)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<ProductConsumptionDTO>();
            }
        }

        #region Free Product Offer


        public bool UpdateFreeProductOffer(FreeProductOffer freeProductOffer)
        {
            try
            {
                var entity = _dbContext.Products.FirstOrDefault(x => x.Id == freeProductOffer.ProductId);
                entity.IsFreeProductOffer = freeProductOffer.IsActive;
                _dbContext.SaveChanges();

                AddUpdateEntity(freeProductOffer);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public int CreateFreeProductOffer(FreeProductOffer freeProductOffer)
        {
            try
            {
                if (freeProductOffer.IsActive)
                {
                    var entity = _dbContext.Products.FirstOrDefault(x => x.Id == freeProductOffer.ProductId);
                    entity.IsFreeProductOffer = true;
                    _dbContext.SaveChanges();
                }

                AddUpdateEntity(freeProductOffer);
                return freeProductOffer.Id;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public FreeProductOffer GetFreeProductOffer(int productId)
        {
            try
            {
                return _dbContext.FreeProductOffers.FirstOrDefault(c => c.ProductId == productId);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public FreeProductOfferDTO GetProductForFreeOffer(int productId)
        {
            try
            {
                return _dbContext.FreeProductOffers
                           .Where(c => c.ProductId == productId && c.IsActive)
                           .Select(c => new FreeProductOfferDTO
                           {
                               BuyQuantity = c.BuyQuantity,
                               FreeQuantity = c.FreeQuantity,
                               ProductName = c.GiftType == GiftType.SameProduct
                                   ? c.Product.Name
                                   : c.GiftType == GiftType.DifferentProduct
                                       ? c.FreeProduct.Name
                                       : c.CustomItem
                           })
                           .FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion Free Product Offer
    }
}