
using System.Collections.Generic;
using System.Linq;
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
    public class CompanyProvinceController : BaseController
    {
        #region Index

        [Permissions("CompanyProvince.Index")]
        public ActionResult Index()
        {
            var total = 0;
            var lstOffice = CompanyOfficeDb.Instance.GetByCompany(AccountInfo.CompanyId);
            var model = new CompanyProvinceModel();
            model.PageSize = 100;
            model.ListOffice = lstOffice;
            model.AccountInfo = AccountInfo;

            if(lstOffice != null && lstOffice.Any())
            {
                model.OfficeSelectList = lstOffice.Select(x => new SelectListItem()
                {
                    Text = x.OfficeName,
                    Value = x.Id.ToString()
                }).ToList();
                model.OfficeSelectList.Insert(0, new SelectListItem() { Text = "Tất cả", Value = "-1" });
            }

            model.ListCompanyProvince = SearchCompanyTitle(model, ref total);

            if (model.ListCompanyProvince != null && model.ListCompanyProvince.Any())
            {
                model.TotalRecord = total;
                model.TotalString = ViewHelper.GetTotalString(model.PageIndex, model.PageSize, total);
            }

            return View(model);
        }

        [HttpPost]
        [Permissions("CompanyProvince.Index")]
        public ActionResult Index(CompanyProvinceModel model)
        {
            var total = 0;
            var lstOffice = CompanyOfficeDb.Instance.GetByCompany(AccountInfo.CompanyId);
            model.PageSize = 100;
            model.ListOffice = lstOffice;
            
            if (lstOffice != null && lstOffice.Any())
            {
                model.OfficeSelectList = lstOffice.Select(x => new SelectListItem()
                {
                    Text = x.OfficeName,
                    Value = x.Id.ToString()
                }).ToList();
                model.OfficeSelectList.Insert(0, new SelectListItem() { Text = "Tất cả", Value = "-1" });
            }

            model.ListCompanyProvince = SearchCompanyTitle(model, ref total);

            if (model.ListCompanyProvince != null && model.ListCompanyProvince.Any())
            {
                model.TotalRecord = total;
                model.TotalString = ViewHelper.GetTotalString(model.PageIndex, model.PageSize, total);
            }

            return View(model);
        }

        public List<ProvinceCompanyBo> SearchCompanyTitle(CompanyProvinceModel model, ref int totalRecord)
        {
            var query = new ProvinceCompanyQuery()
            {
                Company = AccountInfo.CompanyId.ToString(),
                Office = model.OfficeId.ToString(),
                Keyword = model.Keyword,
                From = model.From,
                To = model.To,
                Page = model.PageIndex,
                PageSize = model.PageSize
            };
            var result = ProvinceCompanyDb.Instance.Select(query);
            totalRecord = query.TotalRecord;
            return result;
        }

        #endregion

        [HttpPost]
        [Permissions("CompanyProvince.UpdateProvince")]
        public ActionResult UpdateProvince(int id, int oId)
        {
            if (id <= 0 || oId <= 0)
                return Error("Có lỗi xảy ra vui lòng thử lại sau");

            var companyProvince = ProvinceCompanyDb.Instance.Read(id);
            if (companyProvince == null)
                return Error("Có lỗi xảy ra vui lòng thử lại sau");

            var company = CompanyDb.Instance.Read(companyProvince.CompanyId);
            if (company == null)
                return Error("Không thể tìm thấy thông tin công ty");

            if (!AccountHelper.CheckCompanyPermission(AccountInfo, company.Id))
                return Error("Bạn không có quyền cập nhật thông tin này");

            var lstProvince = ProvinceDb.Instance.Select();
            var lstOffice = CompanyOfficeDb.Instance.GetByCompany(company.Id);

            var office = lstOffice.FirstOrDefault(o => o.Id == oId);

            var obj = new ProvinceCompanyBo()
            {
                Id = id,
                CompanyId = company.Id,
                OfficeId = office.Id,
                OfficeName = office.OfficeName,
                UpdateUserId = WebSecurity.CurrentUserId,
                UpdateUserName = WebSecurity.CurrentUserName
            };

            if (ProvinceCompanyDb.Instance.Update(obj))
                return Success("Cập nhật thông tin thành công");

            return Error("Có lỗi xảy ra vui lòng thử lại sau");
        }

        [HttpPost]
        [Permissions("CompanyProvince.Index")]
        public ActionResult ProvincePermission(string ids, int officeId)
        {
            if(string.IsNullOrEmpty(ids) || officeId <= 0)
                return Error("Có lỗi xảy ra vui lòng thử lại sau");

            var lstId = ids.SplitToIntList();
            foreach (var id in lstId)
            {
                var companyProvince = ProvinceCompanyDb.Instance.Read(id);
                if (companyProvince == null)
                    continue;

                var company = CompanyDb.Instance.Read(companyProvince.CompanyId);
                if (company == null)
                    continue;

                if (!AccountHelper.CheckCompanyPermission(AccountInfo, company.Id))
                    continue;

                var lstProvince = ProvinceDb.Instance.Select();
                var lstOffice = CompanyOfficeDb.Instance.GetByCompany(company.Id);

                var office = lstOffice.FirstOrDefault(o => o.Id == officeId);

                var obj = new ProvinceCompanyBo()
                {
                    Id = id,
                    CompanyId = company.Id,
                    OfficeId = office.Id,
                    OfficeName = office.OfficeName,
                    UpdateUserId = WebSecurity.CurrentUserId,
                    UpdateUserName = WebSecurity.CurrentUserName
                };

                ProvinceCompanyDb.Instance.Update(obj);
            }

            return Success();
        }
    }
}