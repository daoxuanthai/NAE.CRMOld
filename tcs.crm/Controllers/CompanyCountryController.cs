
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using tcs.adapter.Helper;
using tcs.adapter.Sql;
using tcs.bo;
using tcs.crm.Models;
using tcs.lib;
using WebMatrix.WebData;

namespace tcs.crm.Controllers
{
    [Authorize]
    public class CompanyCountryController : BaseController
    {
        #region Index

        [Permissions("CompanyCountry.Index")]
        public ActionResult Index()
        {
            var model = new CompanyCountryModel();
            model.PageSize = 100;
            model.ListCompanyCountry = CountryCompanyDb.Instance.GetByCompany(AccountInfo.CompanyId);
            if (model.ListCompanyCountry != null && model.ListCompanyCountry.Any())
            {
                model.TotalRecord = model.ListCompanyCountry.Count;
                model.TotalString = ViewHelper.GetTotalString(model.PageIndex, model.PageSize, model.ListCompanyCountry.Count);
            }

            return View(model);
        }
        
        #endregion

        [HttpPost]
        [Permissions("CompanyCountry.UpdateCountry")]
        public ActionResult UpdateCountry(int id, bool visible)
        {
            if (id <= 0)
                return Error("Có lỗi xảy ra vui lòng thử lại sau");

            var company = CompanyDb.Instance.Read(AccountInfo.CompanyId);
            if (company == null)
                return Error("Không thể tìm thấy thông tin công ty");

            if (!AccountHelper.CheckCompanyPermission(AccountInfo, company.Id))
                return Error("Bạn không có quyền cập nhật thông tin này");

            var obj = new CountryCompanyBo()
            {
                Id = id,
                CompanyId = AccountInfo.CompanyId,
                Visible = visible,
                UpdateUserId = WebSecurity.CurrentUserId,
                UpdateUserName = WebSecurity.CurrentUserName
            };

            if (CountryCompanyDb.Instance.Update(obj))
                return Success("Cập nhật thông tin thành công");

            return Error();
        }
    }
}