using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using tcs.adapter.Elastics;
using tcs.adapter.Helper;
using tcs.adapter.Sql;
using tcs.bo;
using tcs.crm.Models;
using tcs.lib;
using WebMatrix.WebData;

namespace tcs.crm.Controllers
{
    [Authorize]
    public class SeminarRegisterController : BaseController
    {
        #region Index

        [Permissions("SeminarRegister.Index")]
        public ActionResult Index(int id)
        {
            if (id <= 0)
                return null;

            var model = new SeminarRegisterModel();
            model.SeminarId = id;
            model.SeminarPlaceId = -1;
            model.IsAttend = -1;
            InitSeminarRegisterModel(model);

            var total = 0;
            model.ListSeminarRegister = SearchSeminarRegister(model, ref total);
            if (model.ListSeminarRegister != null && model.ListSeminarRegister.Any())
            {
                model.TotalRecord = total;
                model.TotalString = ViewHelper.GetTotalString(model.PageIndex, model.PageSize, total);
                model.PagerString = ViewHelper.BuildCMSPaging(total, model.PageIndex, 5, model.PageSize);
            }

            return View(model);
        }

        [HttpPost]
        [Permissions("SeminarRegister.Index")]
        public ActionResult Index(SeminarRegisterModel model)
        {
            var total = 0;
            InitSeminarRegisterModel(model);
            model.ListSeminarRegister = SearchSeminarRegister(model, ref total);
            if (model.ListSeminarRegister != null && model.ListSeminarRegister.Any())
            {
                model.TotalRecord = total;
                model.TotalString = ViewHelper.GetTotalString(model.PageIndex, model.PageSize, total);
                model.PagerString = ViewHelper.BuildCMSPaging(total, model.PageIndex, 5, model.PageSize);
            }
            return View(model);
        }

        public List<SeminarRegisterBo> SearchSeminarRegister(SeminarRegisterModel model, ref int total)
        {
            var query = new SeminarRegisterQuery()
            {
                Keyword = model.Keyword,
                SeminarId = model.SeminarId,
                SeminarPlaceId = model.SeminarPlaceId,
                IsAttend = model.IsAttend,
                From = model.From,
                To = model.To,
                TitleId = model.TitleId,
                Page = model.PageIndex,
                PageSize = model.PageSize,
                Company = AccountInfo.CompanyId.ToString()
            };
            var lstSeminarRegister = SeminarRegisterDb.Instance.Search(query);
            total = query.TotalRecord;
            return lstSeminarRegister;
        }

        public void InitSeminarRegisterModel(SeminarRegisterModel model)
        {
            if (model == null)
                model = new SeminarRegisterModel();

            if (!string.IsNullOrEmpty(model.FromDate))
            {
                model.From = model.FromDate.ToDateTime();
            }
            if (!string.IsNullOrEmpty(model.ToDate))
            {
                model.To = model.ToDate.ToDateTime().AddHours(23).AddMinutes(59).AddSeconds(59);
            }

            var lstTitle = CompanyTitleDb.Instance.GetByCompany(AccountInfo.CompanyId);
            if (lstTitle != null)
            {
                var tmp = lstTitle.Select(i => new SelectListItem()
                {
                    Text = (i.UserType == CompanyTitleType.Leader.Key ? "" : "---- ") + i.Code,
                    Value = i.Id.ToString()
                }).ToList();
                tmp.Insert(0, new SelectListItem()
                {
                    Text = "Tất cả nhân viên",
                    Value = "-1"
                });
                model.ListTitle = tmp;
            }

            if (model.TitleId <= 0)
                model.TitleId = -1;

            var seminar = SeminarDb.Instance.Read(model.SeminarId);
            model.SeminarName = seminar.Name;
            model.AccountInfo = AccountInfo;
            var lstPlace = SeminarPlaceDb.Instance.GetBySeminar(model.SeminarId);
            if(lstPlace != null)
            {
                var tmp = lstPlace.Select(i => new SelectListItem()
                {
                    Text = i.Place,
                    Value = i.Id.ToString()
                }).ToList();
                tmp.Insert(0, new SelectListItem()
                {
                    Text = "Tất cả địa điểm",
                    Value = "-1"
                });
                model.ListSeminarPlace = tmp;
            }
        }

        #endregion

        #region Detail

        [Permissions("SeminarRegister.Detail")]
        public ActionResult Detail(int id = 0, int seminarId = 0)
        {
            var model = new SeminarRegisterInsertModel();
            model.Id = id;
            model.SeminarId = seminarId;

            var info = SeminarRegisterDb.Instance.Read(id);
            if (info == null)
                info = new SeminarRegisterBo() { SeminarId = seminarId };
            InitInsertUpdateModel(model, info);

            var html = RenderPartialViewToString("Detail", model);
            return Success("Success", html);
        }

        [Permissions("SeminarRegister.Detail.Post")]
        [HttpPost]
        [ValidateModelAttribute]
        public ActionResult Detail(SeminarRegisterInsertModel model)
        {
            var customer = CustomerDb.Instance.Read(model.CustomerId);

            if (model.Id > 0)
            {
                #region Cập nhật thông tin đăng ký

                var info = SeminarRegisterDb.Instance.Read(model.Id);
                if (info == null)
                    return Error("Không tìm thấy thông tin dăng ký");

                info.SeminarPlaceId = model.SeminarPlaceId;
                info.TicketId = model.TicketId;
                info.School1 = model.School1;
                info.School1Time = model.School1Time;
                info.School2 = model.School2;
                info.School2Time = model.School2Time;
                info.School3 = model.School3;
                info.School3Time = model.School3Time;
                info.CustomerNote = model.CustomerNote;
                info.Note = model.Note;
                info.IsAttend = model.IsAttend;
                info.UpdateUserId = WebSecurity.CurrentUserId;
                info.UpdateUserName = WebSecurity.CurrentUserName;

                if (SeminarRegisterDb.Instance.Update(info))
                {
                    var parents = ParentDb.Instance.GetByCustomer(model.CustomerId);
                    var histories = StudyHistoryDb.Instance.GetByCustomer(model.CustomerId);
                    var abroads = StudyAbroadDb.Instance.GetByCustomer(model.CustomerId);
                    var languages = LanguageDb.Instance.GetByCustomer(model.CustomerId);
                    var obj = SeminarRegisterSE.ToSeminarRegisterSE(info, customer, parents, histories, abroads, languages);
                    SeminarRegisterSearch.Instance.Index(obj);

                    return Success("Cập nhật thông tin đăng ký thành công");
                }

                #endregion
            }
            else
            {
                #region Thêm mới thông tin đăng ký

                var info = new SeminarRegisterBo
                {
                    CompanyId = AccountInfo.CompanyId,
                    SeminarId = model.SeminarId,
                    SeminarPlaceId = model.SeminarPlaceId,
                    CustomerId = model.CustomerId,
                    School1 = model.School1,
                    School1Time = model.School1Time,
                    School2 = model.School2,
                    School2Time = model.School2Time,
                    School3 = model.School3,
                    School3Time = model.School3Time,
                    CustomerNote = model.CustomerNote,
                    Note = model.Note,
                    IsAttend = model.IsAttend,
                    CreateUserId = WebSecurity.CurrentUserId,
                    CreateUserName = WebSecurity.CurrentUserName,
                };

                var id = SeminarRegisterDb.Instance.Create(info);
                if (id > 0)
                {
                    info.Id = id;
                    var parents = ParentDb.Instance.GetByCustomer(model.CustomerId);
                    var histories = StudyHistoryDb.Instance.GetByCustomer(model.CustomerId);
                    var abroads = StudyAbroadDb.Instance.GetByCustomer(model.CustomerId);
                    var languages = LanguageDb.Instance.GetByCustomer(model.CustomerId);
                    var obj = SeminarRegisterSE.ToSeminarRegisterSE(info, customer, parents, histories, abroads, languages);
                    SeminarRegisterSearch.Instance.Index(obj);

                    return Success("Thêm mới thông tin đăng ký thành công");
                }

                #endregion
            }
            return Error("Có lỗi xảy ra vui lòng thử lại sau");
        }

        public void InitInsertUpdateModel(SeminarRegisterInsertModel model, SeminarRegisterBo info)
        {
            if (model == null)
                model = new SeminarRegisterInsertModel();

            var seminar = SeminarDb.Instance.Read(info.SeminarId);
            var customer = CustomerDb.Instance.Read(info.CustomerId);
            if (info != null)
            {
                model.SeminarId = info.SeminarId;
                model.SeminarName = seminar?.Name;
                model.SeminarPlaceId = info.SeminarPlaceId;
                model.CustomerId = info.CustomerId;
                model.CustomerName = customer.Fullname;
                model.TicketId = info.TicketId;
                model.School1 = info.School1;
                model.School1Time = info.School1Time;
                model.School2 = info.School2;
                model.School2Time = info.School2Time;
                model.School3 = info.School3;
                model.School3Time = info.School3Time;
                model.CustomerPhone = customer.Phone;
                model.CustomerEmail = customer.Email;
                model.CustomerNote = info.CustomerNote;
                model.Note = info.Note;
                model.IsAttend = info.IsAttend;
            }

            var lstPlace = SeminarPlaceDb.Instance.GetBySeminar(info.SeminarId);
            if (lstPlace != null)
            {
                model.ListSeminarPlace = lstPlace.Select(i => new SelectListItem()
                {
                    Text = i.Place,
                    Value = i.Id.ToString()
                }).ToList();
            }
        }

        #endregion

        #region Delete

        public ActionResult Delete(string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                if (SeminarRegisterDb.Instance.Delete(ids, AccountInfo.CompanyId, WebSecurity.CurrentUserId, WebSecurity.CurrentUserName))
                {
                    return Success();
                }
            }
            return Error();
        }

        #endregion

        [Permissions("SeminarRegister.ExportExcel")]
        public ActionResult ExportExcel(int id, int place, int title)
        {
            try
            {
                var query = new SeminarRegisterQuery()
                {
                    Keyword = "",
                    SeminarId = id,
                    SeminarPlaceId = place,
                    IsAttend = -1,
                    From = DateTime.Now.AddMonths(-1),
                    To = DateTime.Now.AddMonths(1),
                    TitleId = title,
                    Page = 0,
                    PageSize = 200,
                    Company = AccountInfo.CompanyId.ToString()
                };
                var lstSeminarRegister = SeminarRegisterDb.Instance.Search(query);

                //var lstCustomer = SeminarRegisterSearch.Instance.Search(id, place, title, 0, 1000);
                if (lstSeminarRegister != null)
                {
                    var lstInfo = new  List<SeminarRegisterSE>();
                    foreach (var cus in lstSeminarRegister)
                    {
                        var info = SeminarRegisterDb.Instance.Read(cus.Id);
                        var customer = CustomerDb.Instance.Read(cus.CustomerId);
                        var parents = ParentDb.Instance.GetByCustomer(cus.CustomerId);
                        var histories = StudyHistoryDb.Instance.GetByCustomer(cus.CustomerId);
                        var abroads = StudyAbroadDb.Instance.GetByCustomer(cus.CustomerId);
                        var languages = LanguageDb.Instance.GetByCustomer(cus.CustomerId);
                        var obj = SeminarRegisterSE.ToSeminarRegisterSE(info, customer, parents, histories, abroads, languages);

                        lstInfo.Add(obj);
                    }    
                    var lstExcel = lstInfo.Select(c => SeminarRegisterSE.ToSeminarRegisterExcel(c)).ToList();
                    ExportToExcel.Export<SeminarRegisterExcel>(lstExcel, Server.MapPath("~/Files/TemplateExcel/SeminarRegisterList.xls"), "DSKH.xls", "Danh sách khách hàng", "");
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("ExportExcel", ex);
            }

            return View();
        }
    }
}