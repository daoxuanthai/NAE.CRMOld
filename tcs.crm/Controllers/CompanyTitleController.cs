
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
    public class CompanyTitleController : BaseController
    {
        #region Index

        [Permissions("CompanyTitle.Index")]
        public ActionResult Index()
        {
            var total = 0;
            var model = new CompanyTitleModel();
            InitCompanyTitleModel(model);

            model.ListCompanyTitle = SearchCompanyTitle(model, ref total);
            if (model.ListCompanyTitle != null && model.ListCompanyTitle.Any())
            {
                model.TotalRecord = total;
                model.TotalString = ViewHelper.GetTotalString(model.PageIndex, model.PageSize, total);
                model.PagerString = ViewHelper.BuildCMSPaging(total, model.PageIndex, 5, model.PageSize);
            }

            return View(model);
        }

        [HttpPost]
        [Permissions("CompanyTitle.Index")]
        public ActionResult Index(CompanyTitleModel model)
        {
            var total = 0;
            InitCompanyTitleModel(model);
            model.ListCompanyTitle = SearchCompanyTitle(model, ref total);
            if (model.ListCompanyTitle != null && model.ListCompanyTitle.Any())
            if (model.ListCompanyTitle != null && model.ListCompanyTitle.Any())
            {
                model.TotalRecord = total;
                model.TotalString = ViewHelper.GetTotalString(model.PageIndex, model.PageSize, total);
                model.PagerString = ViewHelper.BuildCMSPaging(total, model.PageIndex, 5, model.PageSize);
            }
            return View(model);
        }

        public List<CompanyTitleBo> SearchCompanyTitle(CompanyTitleModel model, ref int totalRecord)
        {
            var query = new CompanyTitleQuery()
            {
                Company = AccountInfo.CompanyId.ToString(),
                Office = model.OfficeId.ToString(),
                Keyword = model.Keyword,
                From = model.From,
                To = model.To,
                Page = model.PageIndex,
                PageSize = model.PageSize
            };
            var result = CompanyTitleDb.Instance.Select(query);
            totalRecord = query.TotalRecord;
            return result;
        }

        public void InitCompanyTitleModel(CompanyTitleModel model)
        {
            if (model == null)
                model = new CompanyTitleModel();

            if (!string.IsNullOrEmpty(model.FromDate))
            {
                model.From = model.FromDate.ToDateTime();
            }
            if (!string.IsNullOrEmpty(model.ToDate))
            {
                model.To = model.ToDate.ToDateTime().AddHours(23).AddMinutes(59).AddSeconds(59);
            }

            model.AccountInfo = AccountInfo;
            model.ListUserType = CompanyTitleType.Instant().GetAll(true);
            var lstOffice = CompanyOfficeDb.Instance.GetByCompany(AccountInfo.CompanyId);
            if (lstOffice != null && lstOffice.Any())
            {
                lstOffice.Insert(0, new CompanyOfficeBo() { Id = -1, OfficeName = "Tất cả" });
                model.ListOffice = lstOffice.Select(x => new SelectListItem()
                {
                    Text = x.OfficeName,
                    Value = x.Id.ToString()
                }).ToList();
            }
        }

        #endregion

        #region Detail

        [Permissions("CompanyTitle.Detail")]
        public ActionResult Detail(int id = 0)
        {
            var model = new CompanyTitleInsertModel() { Id = id };
            var title = CompanyTitleDb.Instance.Read(id);
            if (id > 0 && (title == null || !AccountHelper.CheckCompanyPermission(AccountInfo, title.CompanyId)))
                return Error("Bạn không có quyền cập nhật thông tin này");

            InitInsertUpdateModel(model, id > 0);

            var html = RenderPartialViewToString("Detail", model);
            return Success("Success", html);
        }

        [Permissions("CompanyTitle.Detail.Post")]
        [HttpPost]
        [ValidateModel]
        public ActionResult Detail(CompanyTitleInsertModel model)
        {
            var title = CompanyTitleDb.Instance.Read(model.Id);
            if (model.Id > 0 && (title == null || !AccountHelper.CheckCompanyPermission(AccountInfo, title.CompanyId)))
                return Error("Bạn không có quyền cập nhật thông tin này");

            // nếu là tài khoản ko phải admin, giám đốc và chưa chọn văn phòng thì bắt buộc chọn
            if(model.UserType != CompanyTitleType.Admin.Key && model.UserType != CompanyTitleType.Director.Key)
            {
                if(model.OfficeId == -1)
                    return Error("Vui lòng chọn văn phòng");
            }

            var user = UserDb.Instance.Read(model.UserId);
            var userName = user != null ? user.UserName : "";
            var userFullName = user != null ? user.FullName : "Không phân quyền";

            if (model.Id > 0)
            {
                #region Cập nhật thông tin vị trí

                title.OfficeId = model.OfficeId;
                title.Title = model.Title;
                title.Code = model.Code.ToUpper();
                title.UserId = model.UserId;
                title.UserName = userName;
                title.UserFullName = userFullName;
                title.Note = model.Note;
                title.UserType = model.UserType;
                title.IsLock = model.IsLock;
                title.IsViewAll = model.IsViewAll;
                title.UpdateUserId = WebSecurity.CurrentUserId;
                title.UpdateUserName = WebSecurity.CurrentUserName;

                if (CompanyTitleDb.Instance.Update(title))
                {
                    // cập nhật phân quyền cho user

                    return Success("Cập nhật thông tin thành công");
                }

                #endregion
            }
            else
            {
                #region Thêm mới thông tin vị trí

                title = new CompanyTitleBo
                {
                    CompanyId = AccountInfo.CompanyId,
                    OfficeId = model.OfficeId,
                    Title = model.Title,
                    Code = model.Code.ToUpper(),
                    UserId = model.UserId,
                    UserName = userName,
                    UserFullName = userFullName,
                    Note = model.Note,
                    UserType = model.UserType,
                    IsLock = model.IsLock,
                    IsViewAll = model.IsViewAll,
                    CreateUserId = WebSecurity.CurrentUserId,
                    CreateUserName = WebSecurity.CurrentUserName
                };

                var id = CompanyTitleDb.Instance.Create(title);
                if (id > 0)
                {
                    return Success("Thêm mới thông tin thành công");
                }

                #endregion
            }
            return Error("Có lỗi xảy ra vui lòng thử lại sau");
        }

        public void InitInsertUpdateModel(CompanyTitleInsertModel model, bool loadMoreInfo = false)
        {
            if (model == null)
                model = new CompanyTitleInsertModel();
            
            if(model.Id > 0)
            {
                var info = CompanyTitleDb.Instance.Read(model.Id);
                if(info != null)
                {
                    model.Title = info.Title;
                    model.OfficeId = info.OfficeId;
                    model.Code = info.Code;
                    model.Note = info.Note;
                    model.UserId = info.UserId;
                    model.IsLock = info.IsLock;
                    model.IsViewAll = info.IsViewAll;
                    model.UserType = info.UserType;
                }
            }

            model.CompanyId = model.Id;
            model.ListUserType = CompanyTitleType.Instant().GetAll();
            model.ListUser = AccountHelper.GetSelectListUser();
            var lstOffice = CompanyOfficeDb.Instance.GetByCompany(AccountInfo.CompanyId);
            if (lstOffice != null)
                lstOffice.Insert(0, new CompanyOfficeBo() { Id = -1, OfficeName = "-- Chọn văn phòng --" });
            model.ListOffice = lstOffice;
        }
        
        #endregion

        #region Delete

        [HttpPost]
        [Permissions("CompanyTitle.Delete")]
        public ActionResult Delete(string ids, int companyId)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var company = CompanyDb.Instance.Read(companyId);

                if (company == null)
                    return Error("Không tìm thấy thông tin");

                if (!AccountHelper.CheckCompanyPermission(AccountInfo, company.Id))
                    return Error("Bạn không có quyền cập nhật thông tin này");

                if (CompanyTitleDb.Instance.Delete(ids, AccountInfo.CompanyId, WebSecurity.CurrentUserId, WebSecurity.CurrentUserName))
                {
                    return Success();
                }
            }
            return Error();
        }

        #endregion

        [Permissions("CompanyTitle.LockCompanyTitle")]
        public ActionResult LockCompanyTitle(int id, int cId, bool isLock)
        {
            if (id <= 0)
                return Error();

            var company = CompanyDb.Instance.Read(cId);
            if (company == null)
                return Error("Không tìm thấy thông tin");

            var companyTitle = CompanyTitleDb.Instance.Read(id);
            if (companyTitle == null)
                return Error("Không tìm thấy thông tin");

            companyTitle.IsLock = isLock;
            companyTitle.UpdateUserId = WebSecurity.CurrentUserId;
            companyTitle.UpdateUserName = WebSecurity.CurrentUserName;
            if (CompanyTitleDb.Instance.Update(companyTitle))
            {
                return Success("Cập nhật thông tin thành công");
            }

            return Error();
        }
    }
}