using System.Collections.Generic;
using System.Linq;
using tcs.bo;
using tcs.dao;

namespace tcs.adapter.Sql
{
    public class CountryCompanyDb
    {
        private static CountryCompanyDb _instance;

        public static CountryCompanyDb Instance => _instance ?? (_instance = new CountryCompanyDb());
        
        public bool Update(CountryCompanyBo obj)
        {
            return CountryCompanySql.Instance.Update(obj);
        }

        /// <summary>
        /// Lấy ra danh sách quốc gia theo công ty
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public List<CountryCompanyBo> GetByCompany(int companyId, bool isVisible = false)
        {
            var lstCountry = CountryCompanySql.Instance.GetByCompany(companyId, isVisible);
            if (isVisible)
                lstCountry = lstCountry.Where(c => c.Visible).ToList();
            return lstCountry;
        }

        /// <summary>
        /// Init thông tin quốc gia cho từng công ty
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public bool InitCountryByCompany(int companyId, int userId, string userName)
        {
            var lstCountry = CountryDb.Instance.GetListCountry();
            if (lstCountry != null && lstCountry.Any())
            {
                foreach (var country in lstCountry)
                {
                    var obj = new CountryCompanyBo()
                    {
                        CompanyId = companyId,
                        CountryId = country.Id,
                        CountryName = country.CountryName,
                        Visible = false,
                        CreateUserId = userId,
                        CreateUserName = userName
                    };
                    CountryCompanySql.Instance.Create(obj);
                }
                return true;
            }
            return false;
        }

        public bool DeleteByCompany(int companyId)
        {
            return CountryCompanySql.Instance.DeleteByCompany(companyId);
        }
    }
}
