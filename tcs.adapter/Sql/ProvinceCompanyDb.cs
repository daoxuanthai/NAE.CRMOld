using System.Collections.Generic;
using System.Linq;
using tcs.bo;
using tcs.dao;

namespace tcs.adapter.Sql
{
    public class ProvinceCompanyDb
    {
        private static ProvinceCompanyDb _instance;

        public static ProvinceCompanyDb Instance => _instance ?? (_instance = new ProvinceCompanyDb());

        public ProvinceCompanyBo Read(int id)
        {
            return ProvinceCompanySql.Instance.Read(id);
        }

        public List<ProvinceCompanyBo> Select(ProvinceCompanyQuery query)
        {
            return ProvinceCompanySql.Instance.Select(query);
        }

        public bool Update(ProvinceCompanyBo obj)
        {
            return ProvinceCompanySql.Instance.Update(obj);
        }

        /// <summary>
        /// Lấy ra danh sách tỉnh thành theo công ty
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="clearCache"></param>
        /// <returns></returns>
        public List<ProvinceCompanyBo> GetByCompany(int companyId)
        {
            return ProvinceCompanySql.Instance.GetByCompany(companyId);
        }

        /// <summary>
        /// Init thông tin tỉnh thành cho từng công ty
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public bool InitProvinceByCompany(int companyId, int userId, string userName)
        {
            var lstProvince = ProvinceDb.Instance.Select();
            if (lstProvince != null && lstProvince.Any())
            {
                foreach (var prov in lstProvince)
                {
                    var obj = new ProvinceCompanyBo()
                    {
                        CompanyId = companyId,
                        ProvinceId = prov.Id,
                        ProvinceName = prov.ProvinceName,
                        OfficeId = 0,
                        OfficeName = string.Empty,
                        CreateUserId = userId,
                        CreateUserName = userName
                    };
                    ProvinceCompanySql.Instance.Create(obj);
                }
                return true;
            }
            return false;
        }

        public bool DeleteByCompany(int companyId)
        {
            return ProvinceCompanySql.Instance.DeleteByCompany(companyId);
        }
    }
}
