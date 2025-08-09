namespace SOJIBENTERPRISE.DataAccess
{
    public interface IProduct
    {
        #region Product
        bool UpdateProduct(Product product);
        bool DeleteProduct(int id);
        int CreateProduct(Product product);
        Product GetProduct(int id);
        Product GetProductPrice(int id);
        Task<IList<Product>> GetAllProduct();
        Task<IList<ProductDTO>> GetAllReOrderProducts();
        #endregion Product

        #region Product Consumption
        bool UpdateProductConsumption(ProductConsumption ProductConsumption);
        bool DeleteProductConsumption(int id);
        int CreateProductConsumption(ProductConsumption ProductConsumption);
        ProductConsumption GetProductConsumption(int id);
        Task<IList<ProductConsumptionDTO>> GetAllProductConsumption(DateTime? startDate, DateTime? endDate);
        #endregion Product Consumption

        #region Free Product Offer
        bool UpdateFreeProductOffer(FreeProductOffer freeProductOffer);
        int CreateFreeProductOffer(FreeProductOffer freeProductOffer);
        FreeProductOffer GetFreeProductOffer(int productId);
        FreeProductOfferDTO GetProductForFreeOffer(int productId);
        #endregion Free Product Offer
    }
}