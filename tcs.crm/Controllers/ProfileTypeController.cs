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
    public class ProfileTypeController : BaseController
    {
        #region Index

        [Permissions("ProfileType.Index")]
        public ActionResult Index()
        {
            var model = new ProfileTypeModel();
            InitProfileModel(model);

            var total = 0;
            model.CountryId = "-1";
            model.ListProfileType = SearchProfile(model, ref total);
            if (model.ListProfileType != null && model.ListProfileType.Any())
            {
                model.TotalRecord = total;
                model.TotalString = ViewHelper.GetTotalString(model.PageIndex, model.PageSize, total);
                model.PagerString = ViewHelper.BuildCMSPaging(total, model.PageIndex, 5, model.PageSize);
            }

            return View(model);
        }

        [HttpPost]
        [Permissions("ProfileType.Index")]
        public ActionResult Index(ProfileTypeModel model)
        {
            var total = 0;
            InitProfileModel(model);
            model.ListProfileType = SearchProfile(model, ref total);
            if (model.ListProfileType != null && model.ListProfileType.Any())
            {
                model.TotalRecord = total;
                model.TotalString = ViewHelper.GetTotalString(model.PageIndex, model.PageSize, total);
                model.PagerString = ViewHelper.BuildCMSPaging(total, model.PageIndex, 5, model.PageSize);
            }
            return View(model);
        }

        public List<ProfileTypeBo> SearchProfile(ProfileTypeModel model, ref int total)
        {
            var query = new ProfileTypeQuery()
            {
                Keyword = model.Keyword,
                From = model.From,
                To = model.To,
                Country = model.CountryId,
                Page = model.PageIndex,
                PageSize = model.PageSize,
                Company = AccountInfo.CompanyId.ToString()
            };
            var lstProfile = ProfileTypeDb.Instance.Select(query);
            total = query.TotalRecord;
            return lstProfile;
        }

        public void InitProfileModel(ProfileTypeModel model)
        {
            if (model == null)
                model = new ProfileTypeModel();

            if (!string.IsNullOrEmpty(model.FromDate))
            {
                model.From = model.FromDate.ToDateTime();
            }
            if (!string.IsNullOrEmpty(model.ToDate))
            {
                model.To = model.ToDate.ToDateTime().AddHours(23).AddMinutes(59).AddSeconds(59);
            }

            model.AccountInfo = AccountInfo;
            
            var lstCountry = CountryCompanyDb.Instance.GetByCompany(AccountInfo.CompanyId, true);
            if(lstCountry != null)
            {
                model.ListCountry = lstCountry.Select(i => new SelectListItem()
                {
                    Text = i.CountryName,
                    Value = i.Id.ToString()
                }).ToList();
                model.ListCountry.Insert(0, new SelectListItem()
                {
                    Text = "Tất cả",
                    Value = "-1"
                });
            }
        }

        #endregion

        #region Detail

        [Permissions("ProfileType.Detail")]
        public ActionResult Detail(int id = 0)
        {
            var model = new ProfileTypeInsertModel() { Id = id };
            var profile = ProfileTypeDb.Instance.Read(id);
            if (id > 0 && (Profile == null || !AccountHelper.CheckCompanyPermission(AccountInfo, profile.CompanyId)))
                return Error("Bạn không có quyền cập nhật thông tin này");

            model.CompanyId = AccountInfo.CompanyId;
            InitInsertUpdateModel(model, id > 0);

            var html = RenderPartialViewToString("Detail", model);
            return Success("Success", html);
        }

        [Permissions("ProfileType.Detail.Post")]
        [HttpPost]
        [ValidateModel]
        public ActionResult Detail(ProfileTypeInsertModel model)
        {
            var profile = ProfileTypeDb.Instance.Read(model.Id);
            if (model.Id > 0 && (Profile == null || !AccountHelper.CheckCompanyPermission(AccountInfo, profile.CompanyId)))
                return Error("Bạn không có quyền cập nhật thông tin này");

            var countryName = CountryDb.Instance.GetCountryName(model.Country);

            if (model.Id > 0)
            {
                #region Cập nhật thông tin loại profile

                profile.TypeName = model.TypeName;
                profile.CountryId = model.Country;
                profile.CountryName = countryName;
                profile.Note = model.Note;
                profile.UpdateUserId = WebSecurity.CurrentUserId;
                profile.UpdateUserName = WebSecurity.CurrentUserName;

                if (ProfileTypeDb.Instance.Update(profile))
                {
                    return Success("Cập nhật thông tin thành công");
                }

                #endregion
            }
            else
            {
                #region Thêm mới thông tin trường

                profile = new ProfileTypeBo
                {
                    CompanyId = AccountInfo.CompanyId,
                    TypeName = model.TypeName,
                    CountryId = model.Country,
                    CountryName = countryName,
                    Note = model.Note,
                    CreateUserId = WebSecurity.CurrentUserId,
                    CreateUserName = WebSecurity.CurrentUserName
                };

                var id = ProfileTypeDb.Instance.Create(profile);
                if (id > 0)
                {
                    return Success("Thêm mới thông tin thành công");
                }

                #endregion
            }
            return Error("Có lỗi xảy ra vui lòng thử lại sau");
        }

        public void InitInsertUpdateModel(ProfileTypeInsertModel model, bool loadMoreInfo = false)
        {
            if (model == null)
                model = new ProfileTypeInsertModel();

            if (model.Id > 0)
            {
                var info = ProfileTypeDb.Instance.Read(model.Id);
                if (info != null)
                {
                    model.Country = info.CountryId;
                    model.TypeName = info.TypeName;
                    model.Note = info.Note;
                }
            }

            var lstCountry = CountryDb.Instance.GetListCountry();
            if (lstCountry != null)
            {
                model.ListCountry = lstCountry.Select(i => new SelectListItem()
                {
                    Text = i.CountryName,
                    Value = i.Id.ToString()
                }).ToList();
            }

            model.ListProfileStep = ProfileStepDb.Instance.Select(new ProfileStepQuery() { ProfileTypeId = model.Id });
            model.ListProfileDocument = ProfileDocumentDb.Instance.Select(new ProfileDocumentQuery() { ProfileTypeId = model.Id });
        }

        #endregion

        #region Delete

        [HttpPost]
        [Permissions("ProfileType.Delete")]
        public ActionResult Delete(string ids, int companyId)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                if (ProfileTypeDb.Instance.Delete(ids, AccountInfo.CompanyId, WebSecurity.CurrentUserId, WebSecurity.CurrentUserName))
                {
                    return Success();
                }
            }
            return Error();
        }

        #endregion

    }
}