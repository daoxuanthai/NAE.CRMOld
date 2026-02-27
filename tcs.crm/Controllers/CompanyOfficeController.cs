
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
    public class CompanyOfficeController : BaseController
    {
        #region Index

        [Permissions("CompanyOffice.Index")]
        public ActionResult Index()
        {
            var total = 0;
            var model = new CompanyOfficeModel();
            InitCompanyOfficeModel(model);

            model.ListCompanyOffice = SearchCompanyOffice(model, ref total);
            if (model.ListCompanyOffice != null && model.ListCompanyOffice.Any())
            {
                model.TotalRecord = total;
                model.TotalString = ViewHelper.GetTotalString(model.PageIndex, model.PageSize, total);
                model.PagerString = ViewHelper.BuildCMSPaging(total, model.PageIndex, 5, model.PageSize);
            }

            return View(model);
        }

        [HttpPost]
        [Permissions("CompanyOffice.Index")]
        public ActionResult Index(CompanyOfficeModel model)
        {
            var total = 0;
            InitCompanyOfficeModel(model);
            model.ListCompanyOffice = SearchCompanyOffice(model, ref total);
            if (model.ListCompanyOffice != null && model.ListCompanyOffice.Any())
            {
                model.TotalRecord = total;
                model.TotalString = ViewHelper.GetTotalString(model.PageIndex, model.PageSize, total);
                model.PagerString = ViewHelper.BuildCMSPaging(total, model.PageIndex, 5, model.PageSize);
            }
            return View(model);
        }

        public List<CompanyOfficeBo> SearchCompanyOffice(CompanyOfficeModel model, ref int totalRecord)
        {
            var query = new CompanyOfficeQuery()
            {
                Company = AccountInfo.CompanyId.ToString(),
                ProvinceId = model.ProvinceId,
                Keyword = model.Keyword,
                From = model.From,
                To = model.To,
                Page = model.PageIndex,
                PageSize = model.PageSize
            };
            var result = CompanyOfficeDb.Instance.Select(query);
            totalRecord = query.TotalRecord;
            return result;
        }

        public void InitCompanyOfficeModel(CompanyOfficeModel model)
        {
            if (model == null)
                model = new CompanyOfficeModel();

            if (!string.IsNullOrEmpty(model.FromDate))
            {
                model.From = model.FromDate.ToDateTime();
            }
            if (!string.IsNullOrEmpty(model.ToDate))
            {
                model.To = model.ToDate.ToDateTime().AddHours(23).AddMinutes(59).AddSeconds(59);
            }

            var lstProvince = ProvinceCompanyDb.Instance.GetByCompany(AccountInfo.CompanyId);
            if(lstProvince != null)
            {
                lstProvince.Insert(0, new ProvinceCompanyBo() { Id = -1, ProvinceName = "Tất cả" });
                model.ListProvince = lstProvince.Select(x => new SelectListItem()
                {
                    Text = x.ProvinceName,
                    Value = x.Id.ToString()
                }).ToList();
            }
        }

        #endregion

        #region Detail

        [Permissions("CompanyOffice.Detail")]
        public ActionResult Detail(int id = 0)
        {
            var model = new CompanyOfficeInsertModel() { Id = id };
            var office = CompanyOfficeDb.Instance.Read(id);
            if (id > 0 && (office == null || !AccountHelper.CheckCompanyPermission(AccountInfo, office.CompanyId)))
                return Error("Bạn không có quyền cập nhật thông tin này");

            model.CompanyId = AccountInfo.CompanyId;
            InitInsertUpdateModel(model, id > 0);

            var html = RenderPartialViewToString("Detail", model);
            return Success("Success", html);
        }

        [Permissions("CompanyOffice.Detail.Post")]
        [HttpPost]
        [ValidateModel]
        public ActionResult Detail(CompanyOfficeInsertModel model)
        {
            var office = CompanyOfficeDb.Instance.Read(model.Id);
            if (model.Id > 0 && (office == null || !AccountHelper.CheckCompanyPermission(AccountInfo, office.CompanyId)))
                return Error("Bạn không có quyền cập nhật thông tin này");

            var lstProvince = ProvinceDb.Instance.Select(new ProvinceQuery());

            if (model.Id > 0)
            {
                #region Cập nhật thông tin văn phòng

                office.ProvinceId = model.ProvinceId;
                office.ProvinceName = lstProvince.FirstOrDefault(p => p.Id == model.ProvinceId)?.ProvinceName;
                office.OfficeName = model.OfficeName;
                office.OfficePhone = !string.IsNullOrEmpty(model.OfficePhone) ? model.OfficePhone.Replace("-","") : string.Empty;
                office.OfficeEmail = model.OfficeEmail;
                office.OfficeAddress = model.OfficeAddress;
                office.Note = model.Note;
                office.Status = model.Status;
                office.DirectorUserId = model.DirectorUserId;
                office.UpdateUserId = WebSecurity.CurrentUserId;
                office.UpdateUserName = WebSecurity.CurrentUserName;

                if (CompanyOfficeDb.Instance.Update(office))
                {
                    return Success("Cập nhật thông tin văn phòng thành công");
                }

                #endregion
            }
            else
            {
                #region Thêm mới thông tin văn phòng

                office = new CompanyOfficeBo
                {
                    ProvinceId = model.ProvinceId,
                    ProvinceName = lstProvince.FirstOrDefault(p => p.Id == model.ProvinceId)?.ProvinceName,
                    CompanyId = AccountInfo.CompanyId,
                    OfficeName = model.OfficeName,
                    OfficePhone = !string.IsNullOrEmpty(model.OfficePhone) ? model.OfficePhone.Replace("-", "") : string.Empty,
                    OfficeEmail = model.OfficeEmail,
                    OfficeAddress = model.OfficeAddress,
                    Note = model.Note,
                    Status = model.Status,
                    CreateUserId = WebSecurity.CurrentUserId,
                    CreateUserName = WebSecurity.CurrentUserName
                };

                var id = CompanyOfficeDb.Instance.Create(office);
                if (id > 0)
                {
                    return Success("Thêm mới thông tin văn phòng thành công");
                }

                #endregion
            }
            return Error("Có lỗi xảy ra vui lòng thử lại sau");
        }

        public void InitInsertUpdateModel(CompanyOfficeInsertModel model, bool loadMoreInfo = false)
        {
            if (model == null)
                model = new CompanyOfficeInsertModel();
            
            if(model.Id > 0)
            {
                var info = CompanyOfficeDb.Instance.Read(model.Id);
                if(info != null)
                {
                    model.OfficeName = info.OfficeName;
                    model.OfficeAddress = info.OfficeAddress;
                    model.OfficePhone = info.OfficePhone;
                    model.OfficeEmail = info.OfficeEmail;
                    model.Note = info.Note;
                    model.ProvinceId = info.ProvinceId;
                    model.Status = info.Status;
                    model.DirectorUserId = info.DirectorUserId;
                }
            }

            model.ListStatus = CompanyOfficeStatus.Instant().GetAll();
            model.ListProvince = ProvinceDb.Instance.Select();

            var lstCompanyTitle = CompanyTitleDb.Instance.GetByCompany(model.CompanyId);
            if (lstCompanyTitle != null)
            {
                lstCompanyTitle = lstCompanyTitle.Where(i => i.UserType == CompanyTitleType.Leader.Key).ToList();
                model.ListCompanyTitle = lstCompanyTitle.Select(i => new SelectListItem()
                {
                    Text = i.Code + " (" + i.UserFullName + ")",
                    Value = i.Id.ToString()
                }).ToList();
            }
        }
        
        #endregion

        #region Delete

        [HttpPost]
        [Permissions("CompanyOffice.Delete")]
        public ActionResult Delete(string ids, int companyId)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var company = CompanyDb.Instance.Read(companyId);

                if (company == null)
                    return Error("Không tìm thấy thông tin");

                if (!AccountHelper.CheckCompanyPermission(AccountInfo, company.Id))
                    return Error("Bạn không có quyền cập nhật thông tin này");

                if (CompanyOfficeDb.Instance.Delete(ids, AccountInfo.CompanyId, WebSecurity.CurrentUserId, WebSecurity.CurrentUserName))
                {
                    return Success();
                }
            }
            return Error();
        }

        #endregion
    }
}