using Newtonsoft.Json;
using System;
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
    public class SchoolController : BaseController
    {
        #region Index

        [Permissions("School.Index")]
        public ActionResult Index()
        {
            var model = new SchoolModel();
            InitSchoolModel(model);

            var total = 0;
            model.CountryId = -1;
            model.SchoolType = -1;
            model.ListSchool = SearchSchool(model, ref total);
            if (model.ListSchool != null && model.ListSchool.Any())
            {
                model.TotalRecord = total;
                model.TotalString = ViewHelper.GetTotalString(model.PageIndex, model.PageSize, total);
                model.PagerString = ViewHelper.BuildCMSPaging(total, model.PageIndex, 5, model.PageSize);
            }

            return View(model);
        }

        [HttpPost]
        [Permissions("School.Index")]
        public ActionResult Index(SchoolModel model)
        {
            var total = 0;
            InitSchoolModel(model);
            model.ListSchool = SearchSchool(model, ref total);
            if (model.ListSchool != null && model.ListSchool.Any())
            {
                model.TotalRecord = total;
                model.TotalString = ViewHelper.GetTotalString(model.PageIndex, model.PageSize, total);
                model.PagerString = ViewHelper.BuildCMSPaging(total, model.PageIndex, 5, model.PageSize);
            }
            return View(model);
        }

        public List<SchoolBo> SearchSchool(SchoolModel model, ref int total)
        {
            var query = new SchoolQuery()
            {
                Keyword = model.Keyword,
                From = model.From,
                To = model.To,
                SchoolType = model.SchoolType,
                Country = model.CountryId,
                Page = model.PageIndex,
                PageSize = model.PageSize,
                Company = AccountInfo.CompanyId.ToString()
            };
            var lstSchool = SchoolDb.Instance.Select(query);
            total = query.TotalRecord;
            return lstSchool;
        }

        public void InitSchoolModel(SchoolModel model)
        {
            if (model == null)
                model = new SchoolModel();

            if (!string.IsNullOrEmpty(model.FromDate))
            {
                model.From = model.FromDate.ToDateTime();
            }
            if (!string.IsNullOrEmpty(model.ToDate))
            {
                model.To = model.ToDate.ToDateTime().AddHours(23).AddMinutes(59).AddSeconds(59);
            }

            model.AccountInfo = AccountInfo;
            model.ListSchoolType = SchoolType.Instant().GetAll(true);
            
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

        [Permissions("School.Detail")]
        public ActionResult Detail(int id = 0)
        {
            var model = new SchoolInsertModel() { Id = id };
            var school = SchoolDb.Instance.Read(id);
            if (id > 0 && (school == null || !AccountHelper.CheckCompanyPermission(AccountInfo, school.CompanyId)))
                return Error("Bạn không có quyền cập nhật thông tin này");

            model.CompanyId = AccountInfo.CompanyId;
            InitInsertUpdateModel(model, id > 0);

            var html = RenderPartialViewToString("Detail", model);
            return Success("Success", html);
        }

        [Permissions("School.Detail.Post")]
        [HttpPost]
        [ValidateModel]
        public ActionResult Detail(SchoolInsertModel model)
        {
            var school = SchoolDb.Instance.Read(model.Id);
            if (model.Id > 0 && (school == null || !AccountHelper.CheckCompanyPermission(AccountInfo, school.CompanyId)))
                return Error("Bạn không có quyền cập nhật thông tin này");

            var countryName = CountryDb.Instance.GetCountryName(model.Country);

            if (model.Id > 0)
            {
                #region Cập nhật thông tin trường

                school.SchoolName = model.SchoolName;
                school.ShortName = model.ShortName;
                school.Country = model.Country;
                school.CountryName = countryName;
                school.City = model.City;
                school.Association = model.Association;
                school.Address = model.Address;
                school.VnAddress = model.VnAddress;
                school.ContactName = model.ContactName;
                school.ContactPhone = model.ContactPhone;
                school.ContactEmail = model.ContactEmail;
                school.HotProgram = model.HotProgram;
                school.EducationLevel = model.EducationLevel;
                school.CouseOpenTime = model.CouseOpenTime;
                school.SchoolType = model.SchoolType;
                school.IsStrategy = model.IsStrategy;
                school.Note = model.Note;
                school.UpdateUserId = WebSecurity.CurrentUserId;
                school.UpdateUserName = WebSecurity.CurrentUserName;

                if (SchoolDb.Instance.Update(school))
                {
                    return Success("Cập nhật thông tin trường thành công");
                }

                #endregion
            }
            else
            {
                #region Thêm mới thông tin trường

                school = new SchoolBo
                {
                    CompanyId = AccountInfo.CompanyId,
                    SchoolName = model.SchoolName,
                    ShortName = model.ShortName,
                    Country = model.Country,
                    CountryName = countryName,
                    City = model.City,
                    Association = model.Association,
                    Address = model.Address,
                    VnAddress = model.VnAddress,
                    ContactName = model.ContactName,
                    ContactPhone = model.ContactPhone,
                    ContactEmail = model.ContactEmail,
                    HotProgram = model.HotProgram,
                    EducationLevel = model.EducationLevel,
                    CouseOpenTime = model.CouseOpenTime,
                    SchoolType = model.SchoolType,
                    IsStrategy = model.IsStrategy,
                    Note = model.Note,
                    CreateUserId = WebSecurity.CurrentUserId,
                    CreateUserName = WebSecurity.CurrentUserName
                };

                var id = SchoolDb.Instance.Create(school);
                if (id > 0)
                {
                    return Success("Thêm mới thông tin trường thành công");
                }

                #endregion
            }
            return Error("Có lỗi xảy ra vui lòng thử lại sau");
        }

        public void InitInsertUpdateModel(SchoolInsertModel model, bool loadMoreInfo = false)
        {
            if (model == null)
                model = new SchoolInsertModel();

            if (model.Id > 0)
            {
                var info = SchoolDb.Instance.Read(model.Id);
                if (info != null)
                {
                    model.SchoolName = info.SchoolName;
                    model.ShortName = info.ShortName;
                    model.Country = info.Country;
                    model.CountryName = info.CountryName;
                    model.City = info.City;
                    model.Association = info.Association;
                    model.Address = info.Address;
                    model.VnAddress = info.VnAddress;
                    model.ContactName = info.ContactName;
                    model.ContactPhone = info.ContactPhone;
                    model.ContactEmail = info.ContactEmail;
                    model.HotProgram = info.HotProgram;
                    model.EducationLevel = info.EducationLevel;
                    model.CouseOpenTime = info.CouseOpenTime;
                    model.SchoolType = info.SchoolType;
                    model.IsStrategy = info.IsStrategy;
                    model.Note = info.Note;
                }
            }

            model.ListSchoolType = SchoolType.Instant().GetAll();

            var lstCountry = CountryCompanyDb.Instance.GetByCompany(AccountInfo.CompanyId, true);
            if (lstCountry != null)
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

        #region Delete

        [HttpPost]
        [Permissions("School.Delete")]
        public ActionResult Delete(string ids, int companyId)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                if (SchoolDb.Instance.Delete(ids, AccountInfo.CompanyId, WebSecurity.CurrentUserId, WebSecurity.CurrentUserName))
                {
                    return Success();
                }
            }
            return Error();
        }

        #endregion

    }
}