namespace SOJIBENTERPRISE.DataAccess
{
    public class CompanyManager : BaseDataManager, ICompany
    {
        public CompanyManager(BoniyadiContext model) : base(model)
        {
        }

        public bool UpdateCompanyInfo(CompanyInfo CompanyInfo)
        {
            return AddUpdateEntity(CompanyInfo);
        }

        public int CreateCompanyInfo(CompanyInfo CompanyInfo)
        {
            AddUpdateEntity(CompanyInfo);
            return CompanyInfo.Id;
        }

        public CompanyInfo GetCompanyInfo()
        {
            try
            {
                return _dbContext.CompanyInfos.FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}