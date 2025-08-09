namespace SOJIBENTERPRISE.DataAccess
{
    public interface IDSRShopDue
    {
        int CreateDSRShopDue(DSRShopDue DSRShopDue);
        bool UpdateDSRShopDue(DSRShopDue DSRShopDue);
        bool DeleteShopDue(int id);
        
        DSRShopDue GetDSRShopDue(int id);
        IList<DSRShopDue> GetAllDSRShopDue();
        Task<IList<Lov>> GetAllShopDueCustomerList(int shopId);
        IList<DSRShopDueDTO> GetAllDSRShopDueList(DateTime? startDate, DateTime? endDate);
        IList<DSRShopDueDTO> GetAllDSRShopDueList(int shopId, DateTime? startDate, DateTime? endDate);
        Task<IList<DSRShopDue>> GetAllDSRShopDue(int orderId);


        //int CreateDSRShopDueInOrderTime(DSRShopDue DSRShopDue);
        //bool UpdateDSRShopDueInOrderTime(DSRShopDue DSRShopDue);
        //bool DeleteShopDueInOrderTime(int id);
        IList<DSRShopDueForOrderDTO> GetAllDSRShopDueByOrderId(int orderId);
    }
}
