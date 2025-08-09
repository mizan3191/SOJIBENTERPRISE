namespace SOJIBENTERPRISE.DataAccess
{
    public interface ICustomer
    {
        bool UpdateCustomer(Customer customer);
        bool DeleteCustomer(int id);
        int CreateCustomer(Customer customer);
        Customer GetCustomer(int id);
        Task<IList<Customer>> GetAllCustomer();

        Task<IList<Lov>> GetAllClients();
        int AddCustomerPayment(CustomerPaymentHistory customerPayment);
        bool UpdateCustomerPayment(CustomerPaymentHistory customerPayment);
        CustomerPaymentHistory GetCustomerPaymentHistory(int id);
        bool DeleteCustomerPayment(int id);
    }
}