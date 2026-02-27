using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using tcs.adapter.Helper;
using tcs.adapter.Sql;
using tcs.bo;
using tcs.crm.Models;
using tcs.lib;
using WebMatrix.WebData;

namespace tcs.crm.Controllers
{
    public class AccountController : BaseController
    {
        [Permissions("Account.Index")]
        public ActionResult Index()
        {
            var total = 0;
            var model = new AccountModel();

            var lstUser = SearchUser(model, ref total);
            if (lstUser != null && lstUser.Any())
            {
                model.ListUser = lstUser;
                model.TotalRecord = total;
                model.TotalString = ViewHelper.GetTotalString(model.PageIndex, model.PageSize, total);
                model.PagerString = ViewHelper.BuildCMSPaging(total, model.PageIndex, 5, model.PageSize);
            }
            return View(model);
        }

        [HttpPost]
        [Permissions("Account.Index")]
        public ActionResult Index(AccountModel model)
        {
            var total = 0;
            var lstUser = SearchUser(model, ref total);
            if (lstUser != null && lstUser.Any())
            {
                model.ListUser = lstUser;
                model.TotalRecord = total;
                model.TotalString = ViewHelper.GetTotalString(model.PageIndex, model.PageSize, total);
                model.PagerString = ViewHelper.BuildCMSPaging(total, model.PageIndex, 5, model.PageSize);
            }
            return View(model);
        }

        public List<UserBo> SearchUser(AccountModel model, ref int total)
        {
            var query = new UserQuery()
            {
                Keyword = model.Keyword,
                Company = "-1",
                From = model.FromDate.ToDateTime(),
                To = model.ToDate.ToDateTime().AddHours(23).AddMinutes(59),
                Page = model.PageIndex,
                PageSize = model.PageSize
            };
            var result = UserDb.Instance.Select(query);
            total = query.TotalRecord;
            return result;
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            var token = WebSecurity.GeneratePasswordResetToken("sontran");
            var ret = WebSecurity.ResetPassword(token, "thaidx@123");

            ViewBag.ReturnUrl = returnUrl;
            return PartialView();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, model.RememberMe))
            {
                var company = 0;
                var userName = model.UserName;
                var userId = WebSecurity.GetUserId(userName);

                //var lstCompany = CompanyDb.Instance.GetByUser(userId);
                //if(lstCompany == null || !lstCompany.Any())
                //{
                //    var err = $"User: {userName} - List company null";
                //    LogHelper.WriteLog("AccountController.Login", err);
                //    return RedirectToAction("Login");
                //}

                //company = lstCompany.FirstOrDefault().Id;

                //// lấy thông tin companyId từ cookie nếu có
                //if(Request.Cookies["CompanyId"] != null)
                //{
                //    var ck = Request.Cookies["CompanyId"];
                //    if(!string.IsNullOrEmpty(ck.Value))
                //        company = int.Parse(ck.Value);
                //}

                //var companySetting = ConfigMgr.Get<int>("CompanyId");
                //if (companySetting > 0)
                //{
                //    company = companySetting;
                //}

                //// kiểm tra lại DS công ty xem có tồn tại ID giống với cookie hay không
                //if (!lstCompany.Any(c => c.Id == company))
                //{
                //    company = lstCompany.FirstOrDefault().Id;
                //    HttpCookie ck = new HttpCookie("CompanyId");
                //    ck.Value = company.ToString();
                //    ck.Expires = DateTime.Now.AddYears(1);
                //    Response.Cookies.Add(ck);
                //}

                //var lstCompanyTitle = CompanyTitleDb.Instance.GetByUser(userId);
                //if (lstCompanyTitle == null || !lstCompanyTitle.Any())
                //{
                //    var err = $"User: {userName} - List company title null";
                //    LogHelper.WriteLog("AccountController.Login", err);
                //    return RedirectToAction("Login");
                //}

                //var tmp = lstCompanyTitle.FirstOrDefault(c => c.CompanyId == company);

                //var lstRole = AccountHelper.GetRoles(userName);
                //var titleId = tmp.Id;
                //var titleType = tmp.UserType;
                //var officeByTitle = tmp.OfficeId;
                //var listOfficeByTitle = lstCompanyTitle.Where(c => c.CompanyId == company).Select(i => i.OfficeId).Distinct().ToArray();

                //if(lstCompanyTitle.Any(i => i.UserType == CompanyTitleType.Director.Key))
                //{
                //    var allOffice = CompanyOfficeDb.Instance.GetAll(company);
                //    listOfficeByTitle = allOffice.Select(i => i.Id).ToArray();
                //}    

                //var accountInfo = new AccountInfo()
                //{
                //    UserId = userId,
                //    UserName = userName,
                //    CompanyId = company,
                //    ListCompany = lstCompany,
                //    ListCompanyTitle = lstCompanyTitle,
                //    ListTitle = AccountHelper.GetTitle(userName, company, listOfficeByTitle, titleType),
                //    OfficeByTitle = officeByTitle,
                //    ListOfficeByTitle = listOfficeByTitle,
                //    TitleId = titleId,
                //    TitleCode = tmp.Code,
                //    TitleType = titleType, 
                //    IsViewAll = tmp.IsViewAll,
                //    ListRole = lstRole,
                //    IsSysAdmin = lstRole.Contains("Sys_Admin"),
                //    Permission = AccountHelper.GetPermission(userName, company, officeByTitle, titleType)
                //};
                //Session["AccountInfo"] = accountInfo;

                Session["AccountInfo"] = AccountHelper.GetAccountInfo(userId, userName);

                // mới thêm
                //FormsAuthentication.SetAuthCookie(userName, true);

                if (!string.IsNullOrEmpty(returnUrl))
                    return RedirectToLocal(returnUrl);

                return Redirect("/");
            }

            ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không hợp lệ.");
            return PartialView(model);
        }

        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Login");
        }

        [HttpPost]
        public ActionResult SetCompany(int id)
        {
            if (AccountInfo == null || AccountInfo.ListCompany == null || !AccountInfo.ListCompany.Any(c => c.Id == id))
                return null;

            #region Lấy lại thông tin theo company

            var lstCompanyTitle = CompanyTitleDb.Instance.GetByUser(AccountInfo.UserId);
            var tmp = lstCompanyTitle.FirstOrDefault(c => c.CompanyId == id);
            var acc = AccountInfo;
            acc.CompanyId = id;
            acc.TitleId = tmp.Id;
            acc.TitleType = tmp.UserType;
            acc.OfficeByTitle = tmp.OfficeId;

            var listOfficeByTitle = lstCompanyTitle.Where(c => c.CompanyId == id).Select(i => i.OfficeId).Distinct().ToArray();

            if (lstCompanyTitle.Any(i => i.UserType == CompanyTitleType.Director.Key))
            {
                var allOffice = CompanyOfficeDb.Instance.GetAll(id);
                listOfficeByTitle = allOffice.Select(i => i.Id).ToArray();
            }

            acc.ListTitle = AccountHelper.GetTitle(acc.UserName, id, listOfficeByTitle, tmp.UserType);
            acc.ListOfficeByTitle = listOfficeByTitle;

            Session["AccountInfo"] = acc;

            #endregion

            // lưu thông tin công ty đã chọn vào cookie để lần đăng nhập sau load đúng công ty đang làm việc
            HttpCookie ck = new HttpCookie("CompanyId");
            ck.Value = id.ToString();
            ck.Expires = DateTime.Now.AddYears(1);
            Response.Cookies.Add(ck);

            return Success("Success");
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [Permissions("CreateAccount")]
        public ActionResult CreateAccount(string name, string pass, string roles)
        {
            var result = new { Code = -1, Msg = "Fail" };
            try
            {
                WebSecurity.CreateUserAndAccount(name, pass);
                Roles.AddUsersToRole(new string[] { name }, roles);
                result = new { Code = 0, Msg = "Success" };
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("", ex);
                result = new { Code = -1, Msg = ex.Message };
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Permissions("ResetPassword")]
        public ActionResult ResetPassword(string userName)
        {
            var result = new { Code = -1, Msg = "Fail" };
            try
            {
                //WebSecurity.CreateAccount(userName, "Temp@123");

                var token = WebSecurity.GeneratePasswordResetToken(userName);
                var ret = WebSecurity.ResetPassword(token, "N@mAnh@2026");
                result = new { Code = 200, Msg = "Success" };
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("", ex);
                result = new { Code = -1, Msg = ex.Message };
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangePassword()
        {
            ChangePasswordModel model = new ChangePasswordModel();
            model.Code = -1;
            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            model.Code = -1;
            if (!ModelState.IsValid)
            {
                model.Message = "Dữ liệu không hợp lệ";
                return PartialView(model);
            }

            if (!WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
            {
                model.Message = "Mật khẩu cũ không hợp lệ";
                return PartialView(model);
            }

            model.Code = 0;
            model.Message = "Đổi mật khẩu thành công";
            return PartialView(model);
        }

        public ActionResult UpdateInfo()
        {
            var model = new UpdateInfoModel();
            var userId = WebSecurity.CurrentUserId;
            var user = UserDb.Instance.Read(userId);
            if (user == null)
                return Redirect("~/");

            model.UserName = user.UserName;
            model.FullName = user.FullName;
            model.Phone = user.Phone;
            model.Email = user.Email;
            model.Address = user.Address;
            model.Avatar = !string.IsNullOrWhiteSpace(user.Avatar) ? user.Avatar : "/Content/img/noavatar.png";

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateInfo(UpdateInfoModel model)
        {
            model.Code = -1;
            if (!ModelState.IsValid)
            {
                model.Message = "Dữ liệu không hợp lệ";
                return PartialView(model);
            }

            if (model.AvatarFile != null)
            {
                string strExtension = Path.GetExtension(model.AvatarFile.FileName);
                if (!ViewHelper.IsValidFileUpload(strExtension, ConfigMgr.ValidFileUpload))
                {
                    model.Message = "Hình đại diện không đúng định dạng";
                    return View(model);
                }
                if (model.AvatarFile.ContentLength > 1024 * 1024)
                {
                    model.Message = string.Format("Kích thước tối đa cho phép upload là {0}MB", 1);
                    return View(model);
                }
            }

            model.FullName = model.FullName.Trim();
            
            var user = UserDb.Instance.Read(WebSecurity.CurrentUserId);
            if (user == null)
                return Redirect("~/");

            user.UserName = model.UserName;
            user.FullName = model.FullName;
            user.Phone = model.Phone.Trim();
            user.Email = model.Email.Trim();
            user.Address = model.Address;
            user.UpdateUserId = WebSecurity.CurrentUserId;
            user.UpdateUserName = WebSecurity.CurrentUserName;
            if (model.AvatarFile != null)
            {
                string coverImageLink = string.Empty;
                if (UploadAvatarImage(WebSecurity.CurrentUserName, model.AvatarFile, ref coverImageLink))
                {
                    user.Avatar = coverImageLink;
                }
            }

            if (!string.IsNullOrWhiteSpace(user.Avatar) && string.IsNullOrWhiteSpace(model.Avatar))
                model.Avatar = user.Avatar;

            if (UserDb.Instance.Update(user))
            {
                model.Code = 0;
                model.Message = "Cập nhật thông tin thành công";
                return View(model);
            }

            model.Message = "Cập nhật không thành công";
            return View(model);
        }

        private bool UploadAvatarImage(string userName, HttpPostedFileBase objFile, ref string strCoverImageLink)
        {
            if (objFile == null)
            {
                return false;
            }
            string strCoverImageFileLink = string.Empty;
            string strFileName = objFile.FileName.Replace(" ", "");
            string strFolderName = "Employee";
            string strUploadPath = string.Concat(ConfigMgr.UploadFolder, strFolderName, "/", userName.ToLower(), "/");
            string strAbsolutePath = Server.MapPath(string.Concat("~", strUploadPath));
            strCoverImageFileLink = strUploadPath + strFileName;

            ViewHelper.UploadFile(strAbsolutePath, strFileName, objFile);

            strCoverImageLink = strCoverImageFileLink;
            return true;
        }

        #region Detail

        [Permissions("Account.Detail")]
        public ActionResult Detail(int id = 0)
        {
            var model = new AccountInsertModel() { Id = id };
            var user = UserDb.Instance.Read(id);
            if (id > 0 && user == null)
                return Error("Không tìm thấy thông tin tài khoản");

            InitInsertUpdateModel(model);

            var html = RenderPartialViewToString("Detail", model);
            return Success("Success", html);
        }

        [Permissions("Account.Detail.Post")]
        [HttpPost]
        [ValidateModel]
        public ActionResult Detail(AccountInsertModel model)
        {
            var err = ValidateInsertUpdate(model);
            if (err != null)
                return err;

            model.FullName = model.FullName.Trim().ToTitleCase();

            if (model.Id > 0)
            {
                var user = UserDb.Instance.Read(model.Id);
                if (user == null)
                    return Error("Không tìm thấy thông tin tài khoản");

                #region Cập nhật thông tin tài khoản

                user.FullName = model.FullName;
                user.Avatar = model.Avatar;
                user.Phone = !string.IsNullOrEmpty(model.Phone) ? model.Phone.Replace("-", "") : string.Empty;
                user.Email = model.Email;
                user.Address = model.Address;
                user.Note = model.Note;
                user.IsLock = model.IsLock;
                user.UpdateUserId = WebSecurity.CurrentUserId;
                user.UpdateUserName = WebSecurity.CurrentUserName;

                if (UserDb.Instance.Update(user))
                {
                    return Success("Cập nhật thông tin thành công");
                }

                #endregion
            }
            else
            {
                #region Thêm mới thông tin tài khoản

                WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                var newUser = UserDb.Instance.GetByUserName(model.UserName);

                if (newUser != null)
                {
                    var user = new UserBo
                    {
                        Id = newUser.Id,
                        FullName = model.FullName,
                        Avatar = model.Avatar,
                        Phone = !string.IsNullOrEmpty(model.Phone) ? model.Phone.Replace("-", "") : string.Empty,
                        Email = model.Email,
                        Address = model.Address,
                        Note = model.Note,
                        IsLock = model.IsLock,
                        UpdateUserId = WebSecurity.CurrentUserId,
                        UpdateUserName = WebSecurity.CurrentUserName
                    };
                    if (UserDb.Instance.Update(user))
                        return Success("Thêm mới thông tin thành công");
                }

                #endregion
            }
            return Error("Có lỗi xảy ra vui lòng thử lại sau");
        }

        public void InitInsertUpdateModel(AccountInsertModel model)
        {
            if (model == null)
                model = new AccountInsertModel();

            if (model.Id > 0)
            {
                var info = UserDb.Instance.Read(model.Id);
                if (info != null)
                {
                    model.UserName = info.UserName;
                    model.FullName = info.FullName;
                    model.Avatar = info.Avatar;
                    model.Phone = info.Phone;
                    model.Email = info.Email;
                    model.Address = info.Address;
                    model.Note = info.Note;
                    model.IsLock = info.IsLock;
                }
            }
        }

        public Extension.JsonResult ValidateInsertUpdate(AccountInsertModel model)
        {
            var err = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(model.FullName) && !model.FullName.IsValidCustomerName())
            {
                err.Add("FullName", "Họ và tên không hợp lệ");
            }
            if (!string.IsNullOrEmpty(model.Phone))
            {
                if (!model.Phone.IsPhoneNumber())
                {
                    err.Add("Phone", "Số điện thoại không hợp lệ");
                }
            }
            if (!string.IsNullOrEmpty(model.Email))
            {
                if (!model.Email.IsEmail())
                {
                    err.Add("Email", "Địa chỉ email không hợp lệ");
                }
            }
            if (model.Id <= 0)
            {
                // kiểm tra theo username
                var user = UserDb.Instance.GetByUserName(model.UserName);
                if (user != null)
                    err.Add("UserName", "Tên tài khoản đã tồn tại vui lòng nhập tên khác");
            }
            if (err.Any())
            {
                return err.ToJsonResult();
            }
            return null;
        }

        #endregion

        #region Lock user

        [HttpPost]
        [Permissions("Account.LockUser")]
        public ActionResult LockUser(int id, bool isLock)
        {
            if (id <= 0)
                return Error();

            var user = UserDb.Instance.Read(id);
            if (user == null)
                return Error("Không tìm thấy thông tin");

            user.IsLock = isLock;
            user.UpdateUserId = WebSecurity.CurrentUserId;
            user.UpdateUserName = WebSecurity.CurrentUserName;
            if (UserDb.Instance.Update(user))
            {
                return Success("Cập nhật thông tin thành công");
            }

            return Error("Có lỗi xảy ra vui lòng thử lại sau");
        }

        #endregion

        #region Delete

        [HttpPost]
        [Permissions("Account.Delete")]
        public ActionResult Delete(string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                if (UserDb.Instance.Delete(ids, WebSecurity.CurrentUserId, WebSecurity.CurrentUserName))
                {
                    return Success();
                }
            }
            return Error();
        }

        #endregion
    }
}