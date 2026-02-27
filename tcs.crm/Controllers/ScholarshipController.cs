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
    public class ScholarshipController : BaseController
    {
        #region Index

        [Permissions("Scholarship.Index")]
        public ActionResult Index()
        {
            var model = new ScholarshipModel();
            InitScholarshipModel(model);

            var schoolId = -1;
            if(Request["sid"] != null)
            {
                int.TryParse(Request["sid"], out schoolId);
            }

            var total = 0;
            model.CountryId = -1;
            model.ScholarshipType = -1;
            model.SchoolId = schoolId;
            model.ListScholarship = SearchScholarship(model, ref total);
            if (model.ListScholarship != null && model.ListScholarship.Any())
            {
                model.TotalRecord = total;
                model.TotalString = ViewHelper.GetTotalString(model.PageIndex, model.PageSize, total);
                model.PagerString = ViewHelper.BuildCMSPaging(total, model.PageIndex, 5, model.PageSize);
            }

            return View(model);
        }

        [HttpPost]
        [Permissions("Scholarship.Index")]
        public ActionResult Index(ScholarshipModel model)
        {
            var total = 0;
            InitScholarshipModel(model);
            model.ListScholarship = SearchScholarship(model, ref total);
            if (model.ListScholarship != null && model.ListScholarship.Any())
            {
                model.TotalRecord = total;
                model.TotalString = ViewHelper.GetTotalString(model.PageIndex, model.PageSize, total);
                model.PagerString = ViewHelper.BuildCMSPaging(total, model.PageIndex, 5, model.PageSize);
            }
            return View(model);
        }

        public List<ScholarshipBo> SearchScholarship(ScholarshipModel model, ref int total)
        {
            var query = new ScholarshipQuery()
            {
                Keyword = model.Keyword,
                From = model.From,
                To = model.To,
                ScholarshipType = model.ScholarshipType,
                CountryId = model.CountryId,
                SchoolId = model.SchoolId,
                Page = model.PageIndex,
                PageSize = model.PageSize,
                Company = AccountInfo.CompanyId.ToString()
            };
            var lstScholarship = ScholarshipDb.Instance.Select(query);
            total = query.TotalRecord;
            return lstScholarship;
        }

        public void InitScholarshipModel(ScholarshipModel model)
        {
            if (model == null)
                model = new ScholarshipModel();

            if (!string.IsNullOrEmpty(model.FromDate))
            {
                model.From = model.FromDate.ToDateTime();
            }
            if (!string.IsNullOrEmpty(model.ToDate))
            {
                model.To = model.ToDate.ToDateTime().AddHours(23).AddMinutes(59).AddSeconds(59);
            }

            model.AccountInfo = AccountInfo;
            model.ListScholarshipType = ScholarshipType.Instant().GetAll(true);

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

            var lstSchool = SchoolDb.Instance.GetAll(AccountInfo.CompanyId);
            if (lstSchool != null)
            {
                model.ListSchool = lstSchool.Select(i => new SelectListItem()
                {
                    Text = i.SchoolName,
                    Value = i.Id.ToString()
                }).ToList();
                model.ListSchool.Insert(0, new SelectListItem()
                {
                    Text = "Tất cả",
                    Value = "-1"
                });
            }
        }

        #endregion

        #region Detail

        [Permissions("Scholarship.Detail")]
        public ActionResult Detail(int id = 0, int school = -1)
        {
            var model = new ScholarshipInsertModel() { Id = id };
            var Scholarship = ScholarshipDb.Instance.Read(id);
            if (id > 0 && (Scholarship == null || !AccountHelper.CheckCompanyPermission(AccountInfo, Scholarship.CompanyId)))
                return Error("Bạn không có quyền cập nhật thông tin này");

            model.CompanyId = AccountInfo.CompanyId;
            InitInsertUpdateModel(model, id > 0);
            if(school > 0)
            {
                model.School = school;
            }
            
            var html = RenderPartialViewToString("Detail", model);
            return Success("Success", html);
        }

        [Permissions("Scholarship.Detail.Post")]
        [HttpPost]
        [ValidateModel]
        public ActionResult Detail(ScholarshipInsertModel model)
        {
            var scholarship = ScholarshipDb.Instance.Read(model.Id);

            var countryName = CountryDb.Instance.GetCountryName(model.Country);
            var schoolName = string.Empty;
            if (model.School > 0)
            {
                var school = SchoolDb.Instance.Read(model.School);
                schoolName = school != null ? school.SchoolName : string.Empty;
            }
            if (model.Id > 0)
            {
                #region Cập nhật thông tin học bông

                scholarship.ScholarshipName = model.ScholarshipName;
                scholarship.ShortName = model.ShortName;
                scholarship.CountryId = model.Country;
                scholarship.CountryName = countryName;
                scholarship.SchoolId = model.School;
                scholarship.SchoolName = schoolName;
                scholarship.Require = model.Require;
                scholarship.ScholarshipType = model.ScholarshipType;
                scholarship.Amount = model.Amount;
                scholarship.Quantity = model.Quantity;
                scholarship.TotalRegister = model.TotalRegister;
                scholarship.Note = model.Note;
                scholarship.UpdateUserId = WebSecurity.CurrentUserId;
                scholarship.UpdateUserName = WebSecurity.CurrentUserName;

                if (!string.IsNullOrEmpty(model.ExpiredDateString))
                    scholarship.ExpiredDate = model.ExpiredDateString.ToDateTime();

                if (ScholarshipDb.Instance.Update(scholarship))
                {
                    return Success("Cập nhật thông tin học bổng thành công");
                }

                #endregion
            }
            else
            {
                #region Thêm mới thông tin học bổng

                scholarship = new ScholarshipBo
                {
                    CompanyId = AccountInfo.CompanyId,
                    ScholarshipName = model.ScholarshipName,
                    ShortName = model.ShortName,
                    CountryId = model.Country,
                    CountryName = countryName,
                    SchoolId = model.School,
                    SchoolName = schoolName,
                    Require = model.Require,
                    ScholarshipType = model.ScholarshipType,
                    Amount = model.Amount,
                    Quantity = model.Quantity,
                    TotalRegister = model.TotalRegister,
                    Note = model.Note,
                    CreateUserId = WebSecurity.CurrentUserId,
                    CreateUserName = WebSecurity.CurrentUserName
                };

                if (!string.IsNullOrEmpty(model.ExpiredDateString))
                    scholarship.ExpiredDate = model.ExpiredDateString.ToDateTime();

                var id = ScholarshipDb.Instance.Create(scholarship);
                if (id > 0)
                {
                    return Success("Thêm mới thông tin học bổng thành công");
                }

                #endregion
            }
            return Error("Có lỗi xảy ra vui lòng thử lại sau");
        }

        public void InitInsertUpdateModel(ScholarshipInsertModel model, bool loadMoreInfo = false)
        {
            if (model == null)
                model = new ScholarshipInsertModel();

            if (model.Id > 0)
            {
                var info = ScholarshipDb.Instance.Read(model.Id);
                if (info != null)
                {
                    model.ScholarshipName = info.ScholarshipName;
                    model.ShortName = info.ShortName;
                    model.Country = info.CountryId;
                    model.CountryName = info.CountryName;
                    model.School = info.SchoolId;
                    model.SchoolName = info.SchoolName;
                    model.Require = info.Require;
                    model.Amount = info.Amount;
                    model.Quantity = info.Quantity;
                    model.TotalRegister = info.TotalRegister;
                    model.TotalRemain = info.Quantity - info.TotalRegister;
                    model.ScholarshipType = info.ScholarshipType;
                    if(info.ExpiredDate.HasValue)
                        model.ExpiredDateString = info.ExpiredDate.Value.ToDateString();
                    model.Note = info.Note;
                }
            }

            model.ListScholarshipType = ScholarshipType.Instant().GetAll();

            var lstCountry = CountryDb.Instance.GetListCountry();
            if (lstCountry != null)
            {
                model.ListCountry = lstCountry.Select(i => new SelectListItem()
                {
                    Text = i.CountryName,
                    Value = i.Id.ToString()
                }).ToList();
            }

            var lstSchool = SchoolDb.Instance.GetAll(AccountInfo.CompanyId);
            if (lstSchool != null)
            {
                model.ListSchool = lstSchool.Select(i => new SelectListItem()
                {
                    Text = i.SchoolName,
                    Value = i.Id.ToString()
                }).ToList();
                model.ListSchool.Insert(0, new SelectListItem()
                {
                    Text = "Tất cả",
                    Value = "-1"
                });
            }
        }

        #endregion

        #region Delete

        [HttpPost]
        [Permissions("Scholarship.Delete")]
        public ActionResult Delete(string ids, int companyId)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                if (ScholarshipDb.Instance.Delete(ids, AccountInfo.CompanyId, WebSecurity.CurrentUserId, WebSecurity.CurrentUserName))
                {
                    return Success();
                }
            }
            return Error();
        }

        #endregion

        public ActionResult LoadScholarship(int id)
        {
            var lstScholarship = ScholarshipDb.Instance.GetBySchool(id);
            if (lstScholarship != null)
                lstScholarship = lstScholarship.Where(i => i.Quantity - i.TotalRegister > 0).ToList();
            var html = RenderPartialViewToString("LoadScholarship", lstScholarship);
            return Success("Success", html);
        }
    }
}