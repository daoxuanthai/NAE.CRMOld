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
    public class SeminarController : BaseController
    {
        #region Index

        [Permissions("Seminar.Index")]
        public ActionResult Index()
        {
            var model = new SeminarModel();
            InitSeminarModel(model);

            var total = 0;
            model.ListSeminar = SearchSeminar(model, ref total);
            if (model.ListSeminar != null && model.ListSeminar.Any())
            {
                model.TotalRecord = total;
                model.TotalString = ViewHelper.GetTotalString(model.PageIndex, model.PageSize, total);
                model.PagerString = ViewHelper.BuildCMSPaging(total, model.PageIndex, 5, model.PageSize);
            }

            return View(model);
        }

        [HttpPost]
        [Permissions("Seminar.Index")]
        public ActionResult Index(SeminarModel model)
        {
            var total = 0;
            InitSeminarModel(model);
            model.ListSeminar = SearchSeminar(model, ref total);
            if (model.ListSeminar != null && model.ListSeminar.Any())
            {
                model.TotalRecord = total;
                model.TotalString = ViewHelper.GetTotalString(model.PageIndex, model.PageSize, total);
                model.PagerString = ViewHelper.BuildCMSPaging(total, model.PageIndex, 5, model.PageSize);
            }
            return View(model);
        }

        public List<SeminarBo> SearchSeminar(SeminarModel model, ref int total)
        {
            var query = new SeminarQuery()
            {
                Keyword = model.Keyword,
                From = model.From,
                To = model.To,
                Status = model.Status,
                Page = model.PageIndex,
                PageSize = model.PageSize,
                Company = AccountInfo.CompanyId.ToString()
            };
            var lstSeminar = SeminarDb.Instance.Search(query);
            total = query.TotalRecord;
            return lstSeminar;
        }

        public void InitSeminarModel(SeminarModel model)
        {
            if (model == null)
                model = new SeminarModel();

            if (!string.IsNullOrEmpty(model.FromDate))
            {
                model.From = model.FromDate.ToDateTime();
            }
            if (!string.IsNullOrEmpty(model.ToDate))
            {
                model.To = model.ToDate.ToDateTime().AddHours(23).AddMinutes(59).AddSeconds(59);
            }

            model.AccountInfo = AccountInfo;
            model.ListStatus = SeminarStatus.Instant().GetAll(true);
        }

        #endregion

        #region Detail

        [Permissions("Seminar.Detail")]
        public ActionResult Detail(int id = 0)
        {
            var model = new SeminarInsertModel() { Id = id };
            
            var seminar = SeminarDb.Instance.Read(id);

            if (seminar == null)
                seminar = new SeminarBo()
                {
                    CompanyId = AccountInfo.CompanyId
                };

            model.Id = seminar.Id;
            model.SeminarInfo = seminar;
            InitInsertUpdateModel(model, seminar);

            var html = RenderPartialViewToString("Detail", model);
            return Success("Success", html);
        }

        [Permissions("Seminar.Detail.Post")]
        [HttpPost]
        [ValidateModelAttribute]
        public ActionResult Detail(SeminarInsertModel model)
        {
            if (model.Id > 0)
            {
                #region Cập nhật thông tin hội thảo

                var seminar = SeminarDb.Instance.Read(model.Id);
                if (seminar == null)
                    return Error("Không tìm thấy thông tin hội thảo");

                seminar.Name = model.Name;
                seminar.Description = model.Description;
                seminar.Note = model.Note;
                seminar.Link = model.Link;
                seminar.Status = model.Status;
                seminar.UpdateUserId = WebSecurity.CurrentUserId;
                seminar.UpdateUserName = WebSecurity.CurrentUserName;

                if (SeminarDb.Instance.Update(seminar))
                {
                    return Success("Cập nhật thông tin hội thảo thành công");
                }

                #endregion
            }
            else
            {
                #region Thêm mới thông tin hội thảo

                var seminar = new SeminarBo
                {
                    CompanyId = AccountInfo.CompanyId,
                    Name = model.Name,
                    Description = model.Description,
                    Note = model.Note,
                    Link = model.Link,
                    Status = model.Status,
                    CreateUserId = WebSecurity.CurrentUserId,
                    CreateUserName = WebSecurity.CurrentUserName,
                };

                var id = SeminarDb.Instance.Create(seminar);
                if (id > 0)
                {
                    return Success("Thêm mới thông tin hội thảo thành công");
                }

                #endregion
            }
            return Error("Có lỗi xảy ra vui lòng thử lại sau");
        }

        public void InitInsertUpdateModel(SeminarInsertModel model, SeminarBo seminar)
        {
            if (model == null)
                model = new SeminarInsertModel();

            if (seminar != null)
            {
                model.Name = seminar.Name;
                model.Description = seminar.Description;
                model.Note = seminar.Note;
                model.Link = seminar.Link;
                model.Status = seminar.Status;
            }

            model.ListStatus = SeminarStatus.Instant().GetAll();

            #region Thông tin địa điểm tổ chức hội thảo

            var placeModel = new SeminarPlaceInsertModel();
            placeModel.SeminarId = seminar != null ? seminar.Id : 0;
            placeModel.ListSeminarPlace = SeminarPlaceDb.Instance.GetBySeminar(model.Id);
            placeModel.ListStatus = SeminarPlaceStatus.Instant().GetAll();
            var lstProvince = ProvinceDb.Instance.Select();
            if (lstProvince != null)
            {
                placeModel.ProvinceId = lstProvince.FirstOrDefault().Id;
                placeModel.ListProvince = lstProvince.Select(x => new SelectListItem()
                {
                    Text = x.ProvinceName,
                    Value = x.Id.ToString()
                }).ToList();
            }

            model.SeminarPlaceModel = placeModel;

            #endregion
        }

        #endregion

        #region SeminarPlace

        public ActionResult SeminarPlace(int id)
        {
            if (id <= 0)
                return Error("Có lỗi xảy ra vui lòng thử lại sau");

            var info = SeminarPlaceDb.Instance.Read(id);
            if (info == null)
                return Error("Không thể tìm thấy thông tin địa điểm");

            if (info.SeminarDate != DateTime.MinValue)
                info.SeminarDateString = info.SeminarDate.ToDateString();

            return new Extension.JsonResult(HttpStatusCode.OK, info);
        }

        [Permissions("SeminarPlace.Post")]
        [HttpPost]
        [ValidateModelAttribute]
        public ActionResult SeminarPlace(SeminarPlaceInsertModel model)
        {
            if(string.IsNullOrEmpty(model.SeminarDateString))
            {
                var tmp = new Dictionary<string, string>();
                tmp.Add("common", "Vui lòng nhập thông tin ngày hội thảo");
                return tmp.ToJsonResult();
            }

            var provinceName = ProvinceDb.Instance.GetProvinceName(model.ProvinceId);

            if (model.Id > 0)
            {
                #region Cập nhật thông tin địa điểm

                var place = SeminarPlaceDb.Instance.Read(model.Id);
                if (place == null)
                    return Error("Không tìm thấy thông tin địa điểm");

                place.SeminarId = model.SeminarId;
                place.ProvinceId = model.ProvinceId;
                place.ProvinceName = provinceName;
                place.Place = model.Place;
                place.Address = model.Address;
                if(!string.IsNullOrEmpty(model.SeminarDateString))
                {
                    place.SeminarDate = model.SeminarDateString.ToDateTime();
                }
                place.Note = model.Note;
                place.Status = model.Status;
                place.UpdateUserId = WebSecurity.CurrentUserId;
                place.UpdateUserName = WebSecurity.CurrentUserName;

                if (SeminarPlaceDb.Instance.Update(place))
                {
                    var lstPlace = SeminarPlaceDb.Instance.GetBySeminar(model.SeminarId);
                    var html = RenderPartialViewToString("_ListSeminarPlace", lstPlace);

                    return Success("Cập nhật thông tin địa điểm thành công", html);
                }

                #endregion
            }
            else
            {
                #region Thêm mới thông tin địa điểm

                var place = new SeminarPlaceBo
                {
                    CompanyId = AccountInfo.CompanyId,
                    SeminarId = model.SeminarId,
                    ProvinceId = model.ProvinceId,
                    ProvinceName = provinceName,
                    Place = model.Place,
                    Address = model.Address,
                    Note = model.Note,
                    Status = model.Status,
                    CreateUserId = WebSecurity.CurrentUserId,
                    CreateUserName = WebSecurity.CurrentUserName,
                };

                if (!string.IsNullOrEmpty(model.SeminarDateString))
                {
                    place.SeminarDate = model.SeminarDateString.ToDateTime();
                }

                var id = SeminarPlaceDb.Instance.Create(place);
                if (id > 0)
                {
                    var lstPlace = SeminarPlaceDb.Instance.GetBySeminar(model.SeminarId);
                    var html = RenderPartialViewToString("_ListSeminarPlace", lstPlace);

                    return Success("Thêm mới thông tin địa điểm thành công", html);
                }

                #endregion
            }
            return Error("Có lỗi xảy ra vui lòng thử lại sau");
        }

        #endregion

        #region Delete

        public ActionResult DeleteSeminar(string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                if (SeminarDb.Instance.Delete(ids, AccountInfo.CompanyId, WebSecurity.CurrentUserId, WebSecurity.CurrentUserName))
                {
                    return Success();
                }
            }
            return Error();
        }

        #endregion
    }
}