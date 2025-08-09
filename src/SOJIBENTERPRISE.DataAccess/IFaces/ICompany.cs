namespace SOJIBENTERPRISE.DataAccess
{
    public interface ICompany
    {
        bool UpdateCompanyInfo(CompanyInfo companyInfo);
        int CreateCompanyInfo(CompanyInfo companyInfo);
        CompanyInfo GetCompanyInfo();
    }
}