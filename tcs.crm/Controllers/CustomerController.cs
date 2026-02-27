using Newtonsoft.Json;
using NPOI.HSSF.Model;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
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
    public class CustomerController : BaseController
    {
        #region Index

        [Permissions("Customer.Index")]
        public ActionResult Index()
        {
            var total = 0;
            var model = new CustomerModel();
            InitCustomerModel(model);

            model.ListCustomer = SearchCustomer(model, ref total);
            if (model.ListCustomer != null && model.ListCustomer.Any())
            {
                model.TotalRecord = total;
                model.TotalString = ViewHelper.GetTotalString(model.PageIndex, model.PageSize, total);
                model.PagerString = ViewHelper.BuildCMSPaging(total, model.PageIndex, 5, model.PageSize);
            }

            return View(model);
        }

        [HttpPost]
        [Permissions("Customer.Index")]
        public ActionResult Index(CustomerModel model)
        {
            var total = 0;
            InitCustomerModel(model);
            model.ListCustomer = SearchCustomer(model, ref total);
            if (model.ListCustomer != null && model.ListCustomer.Any())
            {
                model.TotalRecord = total;
                model.TotalString = ViewHelper.GetTotalString(model.PageIndex, model.PageSize, total);
                model.PagerString = ViewHelper.BuildCMSPaging(total, model.PageIndex, 5, model.PageSize);
            }
            return View(model);
        }

        public List<CustomerBo> SearchCustomer(CustomerModel model, ref int total)
        {
            var employee = string.Empty;
            if (model.Employee != "-1" && !string.IsNullOrEmpty(model.Employee))
            {
                employee = model.Employee;
            }
            else if (AccountInfo.IsAdminGroup())
            {
                employee = "ADMIN";
            }
            else if (AccountInfo.ListTitle != null)
            {
                var lstEmployee = AccountInfo.ListTitle.Where(t => t.OfficeId == AccountInfo.OfficeByTitle);
                employee = string.Join(",", lstEmployee.Select(i => i.Id).ToArray());
            }
            else
            {
                employee = AccountInfo.TitleId.ToString();
            }
            var isAgency = false;
            var agency = "-1";
            if(AccountInfo.TitleType == CompanyTitleType.Agency.Key)
            {
                // nếu là tài khoản đại lý thì hiển thị những khách hàng thuộc đại lý này
                agency = AccountInfo.TitleId.ToString();
                isAgency = true;
            }
            var query = new CustomerQuery()
            {
                Keyword = model.Keyword,
                From = model.From,
                To = model.To,
                Status = model.Status,
                Country = model.Country,
                Employee = employee,
                Company = AccountInfo.CompanyId.ToString(),
                Office = model.Office,
                Source = model.CustomerSource,
                SourceType = model.SourceType,
                EducationLevel = model.EducationLevel,
                Agency = agency,
                IsAgency = isAgency,
                Page = model.PageIndex,
                PageSize = model.PageSize,
                Sort = model.Sort
            };

            var result = CustomerDb.Instance.Select(query);
            total = query.TotalRecord;
            return result;
        }

        public void InitCustomerModel(CustomerModel model)
        {
            if (model == null)
                model = new CustomerModel();

            if (!string.IsNullOrEmpty(model.FromDate))
            {
                model.From = model.FromDate.ToDateTime();
            }
            if (!string.IsNullOrEmpty(model.ToDate))
            {
                model.To = model.ToDate.ToDateTime().AddHours(23).AddMinutes(59).AddSeconds(59);
            }

            model.AccountInfo = AccountInfo;
            model.ListStatus = CustomerStatus.Instant().GetAll(true);
            model.ListCustomerSource = CustomerSource.Instant().GetAll(true);
            model.ListCustomerSourceType = CustomerSourceType.Instant().GetAll(true);
            model.ListEducationLevel = EducationLevel.Instant().GetAll(true);
            model.ListCustomerSort = CustomerSort.Instant().GetAll();

            var lstCountry = CountryCompanyDb.Instance.GetByCompany(AccountInfo.CompanyId);
            var lstCompanyTitle = AccountHelper.GetListEmployee(AccountInfo);
            var lstOffice = CompanyOfficeDb.Instance.GetByCompany(AccountInfo.CompanyId);

            if (lstCountry != null)
            {
                model.ListCountry = lstCountry.Select(i => new SelectListItem()
                {
                    Text = i.CountryName,
                    Value = i.CountryId.ToString()
                }).ToList();
                model.ListCountry.Insert(0, new SelectListItem()
                {
                    Text = "Tất cả",
                    Value = "-1"
                });
            }
            if (lstCompanyTitle != null)
            {
                model.ListCompanyTitle = lstCompanyTitle.Select(i => new SelectListItem()
                {
                    Text = (i.UserType == CompanyTitleType.Leader.Key ? "" : "---- ") + i.Code,
                    Value = i.Id.ToString()
                }).ToList();
                model.ListCompanyTitle.Insert(0, new SelectListItem()
                {
                    Text = "Chưa phân quyền",
                    Value = "0"
                });
                model.ListCompanyTitle.Insert(0, new SelectListItem()
                {
                    Text = "Tất cả",
                    Value = "-1"
                });
            }
            if (lstOffice != null)
            {
                model.ListOffice = lstOffice.Select(i => new SelectListItem()
                {
                    Text = i.ProvinceName,
                    Value = i.Id.ToString()
                }).ToList();
                model.ListOffice.Insert(0, new SelectListItem()
                {
                    Text = "Tất cả",
                    Value = "-1"
                });
            }
        }

        #endregion

        #region Detail

        [Permissions("Customer.Detail")]
        public ActionResult Detail(int id = 0)
        {
            var model = new CustomerInsertModel() { Id = id };
            var customer = CustomerDb.Instance.Read(id);
            if (id > 0 && customer == null)
            {
                var tmp = new Dictionary<string, string>();
                tmp.Add("common", "Bạn không có quyền cập nhật thông tin khách hàng");
                return tmp.ToJsonResult();
            }

            if (!AccountHelper.CheckCustomerPermission(AccountInfo, id))
            {
                var tmp = new Dictionary<string, string>();
                tmp.Add("common", "Bạn không có quyền cập nhật thông tin khách hàng");
                return tmp.ToJsonResult();
            }

            InitInsertUpdateModel(model, customer, id > 0);

            var html = RenderPartialViewToString("Detail", model);
            return Success("Success", html);
        }

        [Permissions("Customer.Detail.Post")]
        [HttpPost]
        [ValidateModel]
        public ActionResult Detail(CustomerInfoModel model)
        {
            var err = ValidateInsertUpdate(model);
            if (err != null)
                return err;

            // nếu là tài khoản admin thì ko được quyền tạo hoặc cập nhật thông tin KH
            if (AccountInfo.TitleType == CompanyTitleType.Admission.Key)
            {
                var tmp = new Dictionary<string, string>();
                tmp.Add("common", "Bạn không có quyền cập nhật thông tin khách hàng");
                return tmp.ToJsonResult();
            }

            var lstTitle = CompanyTitleDb.Instance.GetByCompany(AccountInfo.CompanyId);
            var titleName = string.Empty;
            var officeId = 0;
            if (lstTitle != null && lstTitle.Any(t => t.Id == model.EmployeeId))
            {
                titleName = lstTitle.FirstOrDefault(t => t.Id == model.EmployeeId).Code;
                officeId = lstTitle.FirstOrDefault(t => t.Id == model.EmployeeId).OfficeId;
            }

            var agencyName = string.Empty;
            var lstAgency = CompanyTitleDb.Instance.GetListAgency(AccountInfo.CompanyId);
            if (lstAgency != null && lstAgency.Any(t => t.Id == model.AgencyId))
            {
                agencyName = lstAgency.FirstOrDefault(t => t.Id == model.AgencyId).Code;
            }

            if (model.Id > 0)
            {
                #region Cập nhật thông tin khách hàng

                var cus = CustomerDb.Instance.Read(model.Id);
                if (cus == null || !AccountHelper.CheckCustomerPermission(AccountInfo, model.Id))
                    return null;

                var beforeStatus = cus.Status;
                var beforeEmployeeId = cus.EmployeeId;

                cus.OfficeId = officeId;
                cus.Fullname = model.Fullname;
                cus.Email = model.Email;
                cus.Phone = !string.IsNullOrEmpty(model.Phone) ? model.Phone.Replace("-", "") : string.Empty;
                cus.ProvinceId = model.ProvinceId;
                cus.Gender = model.Gender;
                cus.Address = model.Address;
                cus.Source = model.Source;
                cus.Status = model.Status;
                cus.CustomerNote = model.CustomerNote;
                cus.EmployeeNote = model.EmployeeNote;
                cus.EmployeeId = model.EmployeeId;
                cus.EmployeeName = titleName;
                cus.AgencyId = model.AgencyId;
                cus.AgencyName = agencyName;
                cus.Desire = model.Desire;
                cus.UpdateUserId = WebSecurity.CurrentUserId;
                cus.UpdateUserName = WebSecurity.CurrentUserName;

                if (!string.IsNullOrEmpty(model.BirthdayString))
                {
                    cus.Birthday = model.BirthdayString.ToDateTime();
                }
                else
                {
                    cus.Birthday = null;
                }
                if (AccountInfo.TitleType == CompanyTitleType.Agency.Key)
                {
                    cus.AgencyId = AccountInfo.TitleId;
                    cus.AgencyName = AccountInfo.TitleCode;
                } 

                if (CustomerDb.Instance.Update(cus))
                {
                    #region Nếu trạng thái là đã ký hợp đồng thì tạo hợp đồng nếu chưa có

                    if (model.Status == CustomerStatus.Contracted.Key && beforeStatus != CustomerStatus.Contracted.Key)
                    {
                        var tmp = ContractDb.Instance.GetByCustomer(cus.Id);
                        if (tmp == null)
                        {
                            var contract = new ContractBo()
                            {
                                CustomerId = cus.Id,
                                CompanyId = AccountInfo.CompanyId,
                                Status = ContractStatus.New.Key,
                                CreateUserId = WebSecurity.CurrentUserId,
                                CreateUserName = WebSecurity.CurrentUserName
                            };
                            ContractDb.Instance.Create(contract);
                        }
                    }

                    #endregion

                    if(cus.EmployeeId != beforeEmployeeId && cus.EmployeeId != AccountInfo.TitleId)
                    {
                        var title = $"{AccountInfo.UserName} phân quyền bạn chăm sóc KH {cus.Fullname}";
                        var content = $"<b>{AccountInfo.UserName}</b> phân quyền bạn chăm sóc KH <b>{cus.Fullname}</b>";
                        NotifyDb.Instance.CreateNotify(AccountInfo.TitleId, title, content, NotifyType.Customer.Key, NotifyType.Customer.Value,
                                                            cus.Id, createUserId: AccountInfo.TitleId, createUserName: AccountInfo.UserName);
                    }

                    //LogHelper.Warning($"Cập nhật thông tin khách hàng {cus.Id}-{cus.Fullname} thành công", sendNotify: true);
                    return Success("Cập nhật thông tin khách hàng thành công");
                }

                #endregion
            }
            else
            {
                #region Thêm mới thông tin khách hàng

                var cus = new CustomerBo
                {
                    CompanyId = AccountInfo.CompanyId,
                    OfficeId = officeId,
                    Fullname = model.Fullname,
                    Email = model.Email,
                    Phone = !string.IsNullOrEmpty(model.Phone) ? model.Phone.Replace("-", "") : string.Empty,
                    ProvinceId = model.ProvinceId,
                    Gender = model.Gender,
                    Address = model.Address,
                    Source = model.Source,
                    Status = model.Status,
                    CustomerNote = model.CustomerNote,
                    EmployeeNote = model.EmployeeNote,
                    EmployeeId = model.EmployeeId,
                    EmployeeName = titleName,
                    AgencyId = model.AgencyId,
                    AgencyName = agencyName,
                    Desire = model.Desire,
                    CreateUserId = WebSecurity.CurrentUserId,
                    CreateUserName = WebSecurity.CurrentUserName
                };
                if (!string.IsNullOrEmpty(model.BirthdayString))
                {
                    cus.Birthday = model.BirthdayString.ToDateTime();
                }

                // nếu là tài khoản đại lý thì phân quyền về cho TVP của đại lý đó
                if(AccountInfo.TitleType == CompanyTitleType.Agency.Key)
                {
                    cus.AgencyId = AccountInfo.TitleId;
                    cus.AgencyName = AccountInfo.TitleCode;
                    var office = CompanyOfficeDb.Instance.Read(AccountInfo.OfficeByTitle);
                    if(office != null && office.DirectorUserId > 0)
                    {
                        cus.EmployeeId = office.DirectorUserId;
                        if(lstTitle != null && lstTitle.Any(t => t.Id == office.DirectorUserId))
                        {
                            cus.EmployeeName = lstTitle.FirstOrDefault(t => t.Id == office.DirectorUserId)?.Code;
                        }
                    }
                }

                var id = CustomerDb.Instance.Create(cus);
                if (id > 0)
                {
                    cus.Id = id;
                    if (cus.EmployeeId != AccountInfo.TitleId)
                    {
                        var title = $"{AccountInfo.UserName} phân quyền bạn chăm sóc KH {cus.Fullname}";
                        var content = $"<b>{AccountInfo.UserName}</b> phân quyền bạn chăm sóc KH <b>{cus.Fullname}</b>";
                        NotifyDb.Instance.CreateNotify(AccountInfo.TitleId, title, content, NotifyType.Customer.Key, NotifyType.Customer.Value,
                                                            cus.Id, createUserId: AccountInfo.TitleId, createUserName: AccountInfo.UserName);
                    }

                    //LogHelper.Warning($"Thêm mới thông tin khách hàng {cus.Id}-{cus.Fullname} thành công", sendNotify: true);
                    return Success("Thêm mới thông tin khách hàng thành công", id: id);
                }

                #endregion
            }
            return Error("Có lỗi xảy ra vui lòng thử lại sau");
        }

        public void InitInsertUpdateModel(CustomerInsertModel model, CustomerBo customer, bool loadMoreInfo = false)
        {
            if (model == null)
                model = new CustomerInsertModel()
                {
                    CustomerInfo = new CustomerInfoModel(),
                    CustomerParent = new CustomerParentModel(),
                    CustomerCare = new CustomerCareModel(),
                    StudyHistory = new StudyHistoryModel(),
                    StudyAbroad = new StudyAbroadModel(),
                    ListRegisterHistory = new List<RegisterHistoryBo>(),
                    AccountInfo = AccountInfo
                };

            model.CustomerInfo.AccountInfo = AccountInfo;

            #region Load thông tin khách hàng

            if (customer != null)
            {
                model.CustomerInfo.Id = model.Id;
                model.CustomerInfo.Fullname = customer.Fullname;
                model.CustomerInfo.Email = customer.Email;
                model.CustomerInfo.Phone = customer.Phone;
                model.CustomerInfo.ProvinceId = customer.ProvinceId;
                model.CustomerInfo.Gender = customer.Gender;
                if (customer.Birthday.HasValue && customer.Birthday.Value != DateTime.MinValue)
                {
                    model.CustomerInfo.BirthdayString = customer.Birthday.Value.ToDateString();
                }
                model.CustomerInfo.Address = customer.Address;
                model.CustomerInfo.NewsUrlRef = customer.NewsUrlRef;
                model.CustomerInfo.Source = customer.Source;
                model.CustomerInfo.Status = customer.Status;
                model.CustomerInfo.CustomerNote = customer.CustomerNote;
                model.CustomerInfo.EmployeeNote = customer.EmployeeNote;
                model.CustomerInfo.EmployeeId = customer.EmployeeId;
                model.CustomerInfo.EmployeeName = customer.EmployeeName;
                model.CustomerInfo.AgencyId = customer.AgencyId;
                model.CustomerInfo.AgencyName = customer.AgencyName;
                model.CustomerInfo.Desire = customer.Desire;
            }
            else
            {
                model.CustomerInfo.EmployeeId = AccountInfo.TitleId;
                if(AccountInfo.TitleType == CompanyTitleType.Agency.Key)
                {
                    model.CustomerInfo.AgencyId = AccountInfo.TitleId;
                }
            }

            #endregion

            #region Load thông tin chi tiết khác

            if (model.Id > 0 && loadMoreInfo)
            {
                #region Load thông tin phụ huynh

                var parentModel = new CustomerParentModel
                {
                    CustomerId = model.Id,
                    Id = 0,
                    ListParent = ParentDb.Instance.GetByCustomer(model.Id)
                };
                model.CustomerParent = parentModel;

                #endregion

                #region Load thông tin lịch sử chăm sóc

                var customerCareModel = new CustomerCareModel
                {
                    CustomerId = model.Id,
                    Id = 0,
                    ListCustomerCare = CustomerCareDb.Instance.GetByCustomer(model.Id)
                };
                model.CustomerCare = customerCareModel;

                #endregion

                #region Thông tin lịch sử học tập

                var studyHistoryModel = new StudyHistoryModel()
                {
                    CustomerId = model.Id,
                    Id = 0,
                    ListStudyHistory = StudyHistoryDb.Instance.GetByCustomer(model.Id)
                };
                model.StudyHistory = studyHistoryModel;

                #endregion

                #region Dự định du học
                
                var studyAbroadModel = new StudyAbroadModel()
                {
                    CustomerId = model.Id,
                    Id = 0,
                    ListStudyAbroad = StudyAbroadDb.Instance.GetByCustomer(model.Id),
                    ListStudyLevel = StudyLevel.Instant().GetAll(),
                    ListCountryCompany = CountryCompanyDb.Instance.GetByCompany(AccountInfo.CompanyId)
                };
                model.StudyAbroad = studyAbroadModel;

                #endregion

                #region Ngoại ngữ

                var languageModel = new LanguageModel()
                {
                    CustomerId = model.Id,
                    Id = 0,
                    ListLanguage = LanguageDb.Instance.GetByCustomer(model.Id),
                    ListCertificate = LanguageCertificate.Instant().GetAll()
                };
                model.Language = languageModel;

                #endregion

                #region Thân nhân

                var lstRelatives = RelativesDb.Instance.GetByCustomer(model.Id);
                var relativesModel = new CustomerRelativesModel()
                {
                    CustomerId = model.Id,
                    Id = 0,
                    ListRelatives = lstRelatives
                };
                model.Relatives = relativesModel;

                #endregion

                #region Bảo lãnh

                var guaranteeModel = new CustomerGuaranteeModel()
                {
                    CustomerId = model.Id,
                    Id = 0,
                    ListGuarantee = GuaranteeDb.Instance.GetByCustomer(model.Id),
                    ListRelatives = lstRelatives
                };
                model.Guarantee = guaranteeModel;

                #endregion

                #region Thông tin đăng ký hội thảo

                var lstSeminar = SeminarDb.Instance.GetByCompany(AccountInfo.CompanyId);
                var listSeminarItem = new List<SelectListItem>();
                var listPlaceItem = new List<SelectListItem>();
                if (lstSeminar != null && lstSeminar.Any())
                {
                    listSeminarItem = lstSeminar.Select(i => new SelectListItem()
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    }).ToList();
                    var lstPlace = SeminarPlaceDb.Instance.GetBySeminar(lstSeminar.FirstOrDefault().Id);
                    if(lstPlace != null && lstPlace.Any())
                    {
                        listPlaceItem = lstPlace.Select(i => new SelectListItem()
                        {
                            Text = i.Place,
                            Value = i.Id.ToString()
                        }).ToList();
                    }
                }

                var seminarRegisterModel = new SeminarRegisterInsertModel()
                {
                    CustomerId = model.Id,
                    ListSeminar = listSeminarItem,
                    ListSeminarPlace = listPlaceItem,
                    ListSeminarRegister = SeminarRegisterDb.Instance.GetByCustomer(model.Id, -1)
                };
                model.SeminarRegister = seminarRegisterModel;

                #endregion
            }

            #endregion

            #region Load thông tin như tỉnh thành, phân quyền

            var lstCompanyTitle = AccountInfo.ListTitle;
            var lstProvince = ProvinceDb.Instance.Select(new ProvinceQuery());
            var lstAgency = CompanyTitleDb.Instance.GetListAgency(AccountInfo.CompanyId);

            if (lstProvince != null)
            {
                model.CustomerInfo.ListProvince = lstProvince.Select(i => new SelectListItem()
                {
                    Text = i.ProvinceName,
                    Value = i.Id.ToString()
                }).ToList();

                model.CustomerParent.ListProvince = lstProvince.Select(i => new SelectListItem()
                {
                    Text = i.ProvinceName,
                    Value = i.Id.ToString()
                }).ToList();
            }

            if (lstCompanyTitle != null)
            {
                model.CustomerInfo.ListEmployee = lstCompanyTitle.Select(i => new SelectListItem()
                {
                    Text = (i.UserType == CompanyTitleType.Leader.Key ? "" : "---- ") + i.Code + " (" + i.UserFullName + ")",
                    Value = i.Id.ToString()
                }).ToList();
                model.CustomerInfo.ListEmployee.Insert(0, new SelectListItem()
                {
                    Text = "Chưa phân quyền",
                    Value = "0"
                });
            }

            if (lstAgency != null)
            {
                model.CustomerInfo.ListAgency = lstAgency.Select(i => new SelectListItem()
                {
                    Text = "Đại lý: " + i.Code + " (" + i.UserFullName + ")",
                    Value = i.Id.ToString()
                }).ToList();
                model.CustomerInfo.ListAgency.Insert(0, new SelectListItem()
                {
                    Text = "Chọn đại lý",
                    Value = "0"
                });
            }

            model.CustomerInfo.ListStatus = CustomerStatus.Instant().GetAll();
            model.CustomerInfo.ListSource = CustomerSource.Instant().GetAll();
            model.CustomerInfo.ListDesire = CustomerDesire.Instant().GetAll();
            model.CustomerParent.ListDesire = CustomerDesire.Instant().GetAll();

            #endregion

            #region load lich su tai khoan

            if(customer != null)
            {
                RegisterHistoryQuery query = new RegisterHistoryQuery()
                {
                    CustomerIds = new List<int> { customer.Id },
                    CompanyIds = customer.CompanyId > 0 ? new List<int> { customer.CompanyId } : null
                };
                var lstRegister = RegisterHistoryDb.Instance.GetList(query);
                if (lstRegister != null)
                    model.ListRegisterHistory = lstRegister;
            }
            
            #endregion
        }

        /// <summary>
        /// Kiểm tra một số thông tin nếu có nhập như Email, năm sinh
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Extension.JsonResult ValidateInsertUpdate(CustomerInfoModel model)
        {
            var err = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(model.Fullname) && !model.Fullname.IsValidCustomerName())
            {
                err.Add("Fullname", "Họ và tên không hợp lệ");
            }
            if (!string.IsNullOrEmpty(model.Email))
            {
                if (!model.Email.IsEmail())
                {
                    err.Add("Email", "Địa chỉ email không hợp lệ");
                }
                else
                {
                    // kiểm tra email xem có trùng thông tin hay không
                    var user = CustomerDb.Instance.GetByEmail(model.Email, AccountInfo.CompanyId);
                    if (user != null && user.Id != model.Id)
                    {
                        err.Add("Email", "Địa chỉ email đã được sử dụng");
                    }
                }
            }
            if (!string.IsNullOrEmpty(model.Phone))
            {
                if (!model.Phone.IsPhoneNumber())
                {
                    err.Add("Phone", "Số điện thoại không hợp lệ");
                }
                else
                {
                    // kiểm tra số điện thoại có trùng hay không
                    var user = CustomerDb.Instance.GetByPhone(model.Phone, AccountInfo.CompanyId);
                    if (user != null && user.Id != model.Id)
                    {
                        err.Add("Phone", "Số điện thoại đã được sử dụng");
                    }
                }
            }
            if (!string.IsNullOrEmpty(model.BirthdayString))
            {
                if (!model.BirthdayString.IsValidDate())
                {
                    err.Add("BirthdayString", "Ngày sinh không hợp lệ");
                }
            }
            if (err.Any())
            {
                return err.ToJsonResult();
            }
            return null;
        }

        #endregion

        #region CustomerParent

        [HttpPost]
        [ValidateModelAttribute]
        [Permissions("Customer.CustomerParent.Post")]
        public ActionResult CustomerParent(CustomerParentModel model)
        {
            var err = ValidateInsertUpdateParent(model);
            if (err != null)
                return err;

            if (model.Id > 0)
            {
                #region Cập nhật thông tin phụ huynh

                var parent = ParentDb.Instance.Read(model.Id);
                if (parent != null)
                {
                    parent.Name = model.Name;
                    parent.Phone = !string.IsNullOrEmpty(model.Phone) ? model.Phone.Replace("-", "") : string.Empty;
                    parent.Email = model.Email;
                    parent.Gender = model.Gender;
                    parent.Note = model.Note;
                    parent.Desire = model.Desire;
                    parent.JobName = model.JobName;
                    parent.PositionName = model.PositionName;
                    parent.CompanyName = model.CompanyName;
                    parent.Income = model.Income;
                    parent.OtherIncome = model.OtherIncome;
                    if (!string.IsNullOrEmpty(model.BirthdayString))
                        parent.Birthday = model.BirthdayString.ToDateTime();
                    parent.UpdateUserId = WebSecurity.CurrentUserId;
                    parent.UpdateUserName = WebSecurity.CurrentUserName;

                    if (ParentDb.Instance.Update(parent))
                    {
                        var lstParent = ParentDb.Instance.GetByCustomer(model.CustomerId);
                        var html = RenderPartialViewToString("_ListParent", lstParent);

                        // cập nhật thông tin phụ huynh để search
                        CustomerDb.Instance.SEIndex(parent.CustomerId);

                        return Success("Cập nhật thông tin phụ huynh thành công", html);
                    }
                }

                #endregion
            }
            else
            {
                #region Thêm mới thông tin phụ huynh

                var parent = new ParentBo
                {
                    CustomerId = model.CustomerId,
                    Name = model.Name,
                    Phone = !string.IsNullOrEmpty(model.Phone) ? model.Phone.Replace("-", "") : string.Empty,
                    Email = model.Email,
                    Gender = model.Gender,
                    Note = model.Note,
                    Desire = model.Desire,
                    JobName = model.JobName,
                    PositionName = model.PositionName,
                    CompanyName = model.CompanyName,
                    Income = model.Income,
                    OtherIncome = model.OtherIncome,
                    CompanyId = AccountInfo.CompanyId,
                    CreateUserId = WebSecurity.CurrentUserId,
                    CreateUserName = WebSecurity.CurrentUserName
                };
                if (!string.IsNullOrEmpty(model.BirthdayString))
                    parent.Birthday = model.BirthdayString.ToDateTime();

                var id = ParentDb.Instance.Create(parent);
                if (id > 0)
                {
                    var lstParent = ParentDb.Instance.GetByCustomer(model.CustomerId);

                    // cập nhật thông tin phụ huynh để search
                    CustomerDb.Instance.SEIndex(parent.CustomerId);

                    var html = RenderPartialViewToString("_ListParent", lstParent);
                    return Success("Thêm mới thông tin phụ huynh thành công", html);
                }

                #endregion
            }
            return Error("Có lỗi xảy ra vui lòng thử lại sau");
        }

        public Extension.JsonResult ValidateInsertUpdateParent(CustomerParentModel model)
        {
            var err = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(model.Name) && !model.Name.IsValidCustomerName())
            {
                err.Add("Name", "Họ và tên không hợp lệ");
            }
            if (!string.IsNullOrEmpty(model.Email))
            {
                if (!model.Email.IsEmail())
                {
                    err.Add("Email", "Địa chỉ email không hợp lệ");
                }
            }
            if (!string.IsNullOrWhiteSpace(model.Phone))
            {
                if (!model.Phone.IsPhoneNumber())
                {
                    err.Add("Phone", "Số điện thoại không hợp lệ");
                }
            }
            if (!string.IsNullOrWhiteSpace(model.BirthdayString) && model.BirthdayString.IsValidDate())
            {
                err.Add("BirthdayString", "Ngày sinh không hợp lệ");
            }
            if (err.Any())
            {
                return new Extension.JsonResult(HttpStatusCode.BadRequest, new
                {
                    Error = err.Select(x => new
                    {
                        key = x.Key,
                        msg = x.Value
                    })
                });
            }
            return null;
        }

        public ActionResult ParentInfo(int id)
        {
            if (id <= 0)
                return Error("Có lỗi xảy ra vui lòng thử lại sau");

            var parent = ParentDb.Instance.Read(id);
            if (parent == null)
                return Error("Không tìm thấy thông tin phụ huynh");

            return new Extension.JsonResult(HttpStatusCode.OK, parent);
        }

        #endregion

        #region CustomerCare

        [HttpPost]
        [ValidateModelAttribute]
        [Permissions("Customer.CustomerCare.Post")]
        public ActionResult CustomerCare(CustomerCareModel model)
        {
            var err = ValidateInsertUpdateCustomerCare(model);
            if (err != null)
                return err;

            if (model.CustomerId <= 0)
                return Error("Thông tin khách hàng không hợp lệ");

            // lấy thông tin và kiểm tra phân quyền theo customer
            var customer = CustomerDb.Instance.Read(model.CustomerId);
            if (customer == null || !AccountHelper.CheckCustomerPermission(AccountInfo, model.CustomerId))
                return Error("Bạn không có quyền cập nhật thông tin này");

            if (model.Id > 0)
            {
                #region Cập nhật thông tin chăm sóc

                var info = CustomerCareDb.Instance.Read(model.Id);
                if (info != null)
                {
                    info.Advisory = model.Advisory;
                    info.IsAlarm = model.IsAlarm;
                    info.UpdateUserId = WebSecurity.CurrentUserId;
                    info.UpdateUserName = WebSecurity.CurrentUserName;

                    if (model.IsAlarm)
                    {
                        info.AlarmTime = model.AlarmTimeString.ToDateTime();
                        CustomerDb.Instance.UpdateAlarmTime(customer.Id, model.IsAlarm, model.AlarmTimeString.ToDateTime(),
                            AccountInfo.UserId, AccountInfo.UserName);
                    }
                    else
                    {
                        info.AlarmTime = ConfigMgr.DefaultDate;
                        CustomerDb.Instance.UpdateAlarmTime(customer.Id, model.IsAlarm, ConfigMgr.DefaultAlarmDate,
                            AccountInfo.UserId, AccountInfo.UserName);
                    }

                    if (CustomerCareDb.Instance.Update(info))
                    {
                        var lstCustomerCare = CustomerCareDb.Instance.GetByCustomer(model.CustomerId);
                        var html = RenderPartialViewToString("_ListCustomerCare", lstCustomerCare);
                        return Success("Cập nhật thông tin chăm sóc thành công", html);
                    }
                }

                #endregion
            }
            else
            {
                #region Thêm mới thông tin chăm sóc

                var info = new CustomerCareBo
                {
                    CustomerId = model.CustomerId,
                    Advisory = model.Advisory,
                    IsAlarm = model.IsAlarm,
                    CompanyId = AccountInfo.CompanyId,
                    CreateUserId = WebSecurity.CurrentUserId,
                    CreateUserName = WebSecurity.CurrentUserName
                };

                if (model.IsAlarm)
                {
                    info.AlarmTime = model.AlarmTimeString.ToDateTime();
                    CustomerDb.Instance.UpdateAlarmTime(customer.Id, model.IsAlarm, model.AlarmTimeString.ToDateTime(),
                        AccountInfo.UserId, AccountInfo.UserName);
                }
                else
                {
                    info.AlarmTime = ConfigMgr.DefaultDate;
                    CustomerDb.Instance.UpdateAlarmTime(customer.Id, model.IsAlarm, ConfigMgr.DefaultDate,
                        AccountInfo.UserId, AccountInfo.UserName);
                }

                var id = CustomerCareDb.Instance.Create(info);
                if (id > 0)
                {
                    CustomerDb.Instance.UpdateLastCare(model.CustomerId, DateTime.Now);

                    var lstCustomerCare = CustomerCareDb.Instance.GetByCustomer(model.CustomerId);
                    var html = RenderPartialViewToString("_ListCustomerCare", lstCustomerCare);
                    return Success("Thêm mới thông tin chăm sóc thành công", html);
                }

                #endregion
            }
            return Error("Có lỗi xảy ra vui lòng thử lại sau");
        }

        public Extension.JsonResult ValidateInsertUpdateCustomerCare(CustomerCareModel model)
        {
            var err = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(model.AlarmTimeString) && !model.AlarmTimeString.IsValidDate())
            {
                err.Add("AlarmTimeString", "Ngày tư vấn không hợp lệ");
            }
            if (err.Any())
            {
                return new Extension.JsonResult(HttpStatusCode.BadRequest, new
                {
                    Error = err.Select(x => new
                    {
                        key = x.Key,
                        msg = x.Value
                    })
                });
            }
            return null;
        }

        public ActionResult CustomerCareInfo(int id)
        {
            if (id <= 0)
                return Error("Có lỗi xảy ra vui lòng thử lại sau");

            var info = CustomerCareDb.Instance.Read(id);
            if (info == null)
                return Error("Không thể tìm thấy thông tin chăm sóc");

            // lấy thông tin và kiểm tra phân quyền theo customer
            var customer = CustomerDb.Instance.Read(info.CustomerId);
            if (customer == null || !AccountHelper.CheckCustomerPermission(AccountInfo, info.CustomerId))
                return Error("Bạn không có quyền cập nhật thông tin này");

            if (info.AlarmTime != DateTime.MinValue && info.AlarmTime != ConfigMgr.DefaultDate)
                info.AlarmTimeString = info.AlarmTime.ToString(ConfigMgr.DefaultDateFormat);

            return new Extension.JsonResult(HttpStatusCode.OK, info);
        }

        #endregion

        #region StudyHistory

        [HttpPost]
        [ValidateModelAttribute]
        [Permissions("Customer.StudyHistory.Post")]
        public ActionResult StudyHistory(StudyHistoryModel model)
        {
            var err = ValidateInsertUpdateStudyHistory(model);
            if (err != null)
                return err;

            if (model.CustomerId <= 0)
                return Error("Thông tin khách hàng không hợp lệ");

            // lấy thông tin và kiểm tra phân quyền theo customer
            var customer = CustomerDb.Instance.Read(model.CustomerId);
            if (customer == null || !AccountHelper.CheckCustomerPermission(AccountInfo, model.CustomerId))
                return Error("Bạn không có quyền cập nhật thông tin này");
            
            if (model.Id > 0)
            {
                #region Cập nhật thông tin lịch sử học tập

                var info = StudyHistoryDb.Instance.Read(model.Id);
                if (info != null)
                {
                    info.School = model.School;
                    info.Major = model.Major;
                    info.Score = model.Score;
                    info.Class = model.Class;
                    info.Note = model.Note;
                    info.UpdateUserId = WebSecurity.CurrentUserId;
                    info.UpdateUserName = WebSecurity.CurrentUserName;

                    if (!string.IsNullOrWhiteSpace(model.GraduateDateString))
                    {
                        info.GraduateDate = new DateTime(Convert.ToInt32(model.GraduateDateString), 1, 1);
                    }
                    
                    if (StudyHistoryDb.Instance.Update(info))
                    {
                        var lstStudyHistory = StudyHistoryDb.Instance.GetByCustomer(model.CustomerId);
                        var html = RenderPartialViewToString("_ListStudyHistory", lstStudyHistory);
                        return Success("Cập nhật thông tin lịch sử học tập thành công", html);
                    }
                }

                #endregion
            }
            else
            {
                #region Thêm mới thông tin chăm sóc

                var info = new StudyHistoryBo()
                {
                    CustomerId = model.CustomerId,
                    CompanyId = AccountInfo.CompanyId,
                    School = model.School,
                    Major = model.Major,
                    Score = model.Score,
                    Class = model.Class,
                    Note = model.Note,
                    CreateUserId = WebSecurity.CurrentUserId,
                    CreateUserName = WebSecurity.CurrentUserName
                };

                if (!string.IsNullOrWhiteSpace(model.GraduateDateString))
                {
                    info.GraduateDate = new DateTime(Convert.ToInt32(model.GraduateDateString), 1, 1);
                }

                var id = StudyHistoryDb.Instance.Create(info);
                if (id > 0)
                {
                    var lstStudyHistory = StudyHistoryDb.Instance.GetByCustomer(model.CustomerId);
                    var html = RenderPartialViewToString("_ListStudyHistory", lstStudyHistory);
                    return Success("Thêm mới thông tin lịch sử học tập thành công", html);
                }

                #endregion
            }
            return Error("Có lỗi xảy ra vui lòng thử lại sau");
        }

        public Extension.JsonResult ValidateInsertUpdateStudyHistory(StudyHistoryModel model)
        {
            var err = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(model.GraduateDateString) && !model.GraduateDateString.IsNumber())
            {
                err.Add("GraduateDateString", "Năm tốt nghiệp không hợp lệ");
            }
            if (err.Any())
            {
                return new Extension.JsonResult(HttpStatusCode.BadRequest, new
                {
                    Error = err.Select(x => new
                    {
                        key = x.Key,
                        msg = x.Value
                    })
                });
            }
            return null;
        }

        public ActionResult StudyHistoryInfo(int id)
        {
            if (id <= 0)
                return Error("Có lỗi xảy ra vui lòng thử lại sau");

            var info = StudyHistoryDb.Instance.Read(id);
            if (info == null)
                return Error("Không thể tìm thấy thông tin lịch sử học tập");

            // lấy thông tin và kiểm tra phân quyền theo customer
            var customer = CustomerDb.Instance.Read(info.CustomerId);
            if (customer == null || !AccountHelper.CheckCustomerPermission(AccountInfo, info.CustomerId))
                return Error("Bạn không có quyền cập nhật thông tin này");

            if (info.GraduateDate.HasValue && info.GraduateDate != DateTime.MinValue && info.GraduateDate != ConfigMgr.DefaultDate)
                info.GraduateDateString = info.GraduateDate.Value.Year.ToString();

            return new Extension.JsonResult(HttpStatusCode.OK, info);
        }

        #endregion

        #region StudyAbroad

        [HttpPost]
        [ValidateModelAttribute]
        [Permissions("Customer.StudyAbroad.Post")]
        public ActionResult StudyAbroad(StudyAbroadModel model)
        {
            if (model.CustomerId <= 0)
                return Error("Thông tin khách hàng không hợp lệ");

            // lấy thông tin và kiểm tra phân quyền theo customer
            var customer = CustomerDb.Instance.Read(model.CustomerId);
            if (customer == null || !AccountHelper.CheckCustomerPermission(AccountInfo, model.CustomerId))
                return Error("Bạn không có quyền cập nhật thông tin này");

            var countryName = string.Empty;
            var lstCountry = CountryCompanyDb.Instance.GetByCompany(AccountInfo.CompanyId);
            if (lstCountry != null && lstCountry.Any(c => c.Id == model.CountryId))
            {
                countryName = lstCountry.FirstOrDefault(c => c.Id == model.CountryId)?.CountryName;
            }

            if (model.Id > 0)
            {
                #region Cập nhật thông tin dự định du học

                var info = StudyAbroadDb.Instance.Read(model.Id);
                if (info != null)
                {
                    info.School = model.School;
                    info.Major = model.Major;
                    info.Note = model.Note;
                    info.Year = model.Year;
                    info.Time = model.Time;
                    info.CountryId = model.CountryId;
                    info.CountryName = countryName;
                    info.Level = model.Level;
                    info.UpdateUserId = WebSecurity.CurrentUserId;
                    info.UpdateUserName = WebSecurity.CurrentUserName;

                    if (StudyAbroadDb.Instance.Update(info))
                    {
                        var lstStudyAbroad = StudyAbroadDb.Instance.GetByCustomer(model.CustomerId);

                        // cập nhật lại thông tin quốc gia và bậc học trong customer
                        if(lstStudyAbroad != null && lstStudyAbroad.Any())
                        {
                            var countries = lstStudyAbroad.Select(s => s.CountryId).ToArray();
                            var levels = lstStudyAbroad.Select(s => s.Level).ToArray();
                            var times = lstStudyAbroad.Select(s => s.Year).ToArray();
                            var countryNames = lstStudyAbroad.Select(s => s.CountryName).ToArray();
                            CustomerDb.Instance.UpdateCountryLevel(model.CustomerId, string.Join(",", countries), string.Join(",", levels), 
                                string.Join(",", countryNames), string.Join(",", times));
                        }

                        var html = RenderPartialViewToString("_ListStudyAbroad", lstStudyAbroad);
                        return Success("Cập nhật thông tin dự định du học thành công", html);
                    }
                }

                #endregion
            }
            else
            {
                #region Thêm mới thông tin dự định du học

                var info = new StudyAbroadBo()
                {
                    CustomerId = model.CustomerId,
                    CompanyId = AccountInfo.CompanyId,
                    School = model.School,
                    Major = model.Major,
                    Year = model.Year,
                    Time = model.Time,
                    CountryId = model.CountryId,
                    CountryName = countryName,
                    Level = model.Level,
                    Note = model.Note,
                    CreateUserId = WebSecurity.CurrentUserId,
                    CreateUserName = WebSecurity.CurrentUserName
                };

                var id = StudyAbroadDb.Instance.Create(info);
                if (id > 0)
                {
                    var lstStudyAbroad = StudyAbroadDb.Instance.GetByCustomer(model.CustomerId);

                    // cập nhật lại thông tin quốc gia và bậc học trong customer
                    if (lstStudyAbroad != null && lstStudyAbroad.Any())
                    {
                        var countries = lstStudyAbroad.Select(s => s.CountryId).ToArray();
                        var levels = lstStudyAbroad.Select(s => s.Level).ToArray();
                        var times = lstStudyAbroad.Select(s => s.Year).ToArray();
                        var countryNames = lstStudyAbroad.Select(s => s.CountryName).ToArray();
                        CustomerDb.Instance.UpdateCountryLevel(model.CustomerId, string.Join(",", countries), string.Join(",", levels),
                            string.Join(",", countryNames), string.Join(",", times));
                    }

                    var html = RenderPartialViewToString("_ListStudyAbroad", lstStudyAbroad);
                    return Success("Thêm mới thông tin dự định du học thành công", html);
                }

                #endregion
            }
            return Error("Có lỗi xảy ra vui lòng thử lại sau");
        }
        
        public ActionResult StudyAbroadInfo(int id)
        {
            if (id <= 0)
                return Error("Có lỗi xảy ra vui lòng thử lại sau");

            var info = StudyAbroadDb.Instance.Read(id);
            if (info == null)
                return Error("Không thể tìm thấy thông tin lịch sử học tập");

            // lấy thông tin và kiểm tra phân quyền theo customer
            var customer = CustomerDb.Instance.Read(info.CustomerId);
            if (customer == null || !AccountHelper.CheckCustomerPermission(AccountInfo, info.CustomerId))
                return Error("Bạn không có quyền cập nhật thông tin này");
            
            return new Extension.JsonResult(HttpStatusCode.OK, info);
        }

        #endregion

        #region Language

        [HttpPost]
        [ValidateModelAttribute]
        [Permissions("Customer.Language.Post")]
        public ActionResult Language(LanguageModel model)
        {
            var err = ValidateInsertUpdateLanguage(model);
            if (err != null)
                return err;

            if (model.CustomerId <= 0)
                return Error("Thông tin khách hàng không hợp lệ");

            // lấy thông tin và kiểm tra phân quyền theo customer
            var customer = CustomerDb.Instance.Read(model.CustomerId);
            if (customer == null || !AccountHelper.CheckCustomerPermission(AccountInfo, model.CustomerId))
                return Error("Bạn không có quyền cập nhật thông tin này");

            if (model.Id > 0)
            {
                #region Cập nhật thông tin ngoại ngữ

                var info = LanguageDb.Instance.Read(model.Id);
                if (info != null)
                {
                    info.Language = model.Language;
                    info.Certificate = model.Certificate;
                    info.CertificateName = LanguageCertificate.Instant().GetValueByKey(model.Certificate);
                    info.Score = model.Score;
                    info.Note = model.Note;
                    info.UpdateUserId = WebSecurity.CurrentUserId;
                    info.UpdateUserName = WebSecurity.CurrentUserName;

                    if (!string.IsNullOrWhiteSpace(model.RetestDateString))
                        info.RetestDate = model.RetestDateString.ToDateTime();

                    if (LanguageDb.Instance.Update(info))
                    {
                        var lstLanguage = LanguageDb.Instance.GetByCustomer(model.CustomerId);
                        var html = RenderPartialViewToString("_ListLanguage", lstLanguage);
                        return Success("Cập nhật thông tin ngoại ngữ thành công", html);
                    }
                }

                #endregion
            }
            else
            {
                #region Thêm mới thông tin chăm sóc

                var info = new LanguageBo()
                {
                    CustomerId = model.CustomerId,
                    CompanyId = AccountInfo.CompanyId,
                    Language = model.Language,
                    Certificate = model.Certificate,
                    CertificateName = LanguageCertificate.Instant().GetValueByKey(model.Certificate),
                    Score = model.Score,
                    Note = model.Note,
                    CreateUserId = WebSecurity.CurrentUserId,
                    CreateUserName = WebSecurity.CurrentUserName
                };

                if (!string.IsNullOrWhiteSpace(model.RetestDateString))
                    info.RetestDate = model.RetestDateString.ToDateTime();

                var id = LanguageDb.Instance.Create(info);
                if (id > 0)
                {
                    var lstLanguage = LanguageDb.Instance.GetByCustomer(model.CustomerId);
                    var html = RenderPartialViewToString("_ListLanguage", lstLanguage);
                    return Success("Thêm mới thông tin ngoại ngữ thành công", html);
                }

                #endregion
            }
            return Error("Có lỗi xảy ra vui lòng thử lại sau");
        }

        public Extension.JsonResult ValidateInsertUpdateLanguage(LanguageModel model)
        {
            var err = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(model.RetestDateString) && !model.RetestDateString.IsValidDate())
            {
                err.Add("RetestDateString", "Ngày thi lại không hợp lệ");
            }
            if (err.Any())
            {
                return new Extension.JsonResult(HttpStatusCode.BadRequest, new
                {
                    Error = err.Select(x => new
                    {
                        key = x.Key,
                        msg = x.Value
                    })
                });
            }
            return null;
        }

        public ActionResult LanguageInfo(int id)
        {
            if (id <= 0)
                return Error("Có lỗi xảy ra vui lòng thử lại sau");

            var info = LanguageDb.Instance.Read(id);
            if (info == null)
                return Error("Không thể tìm thấy thông tin lịch sử học tập");

            // lấy thông tin và kiểm tra phân quyền theo customer
            var customer = CustomerDb.Instance.Read(info.CustomerId);
            if (customer == null || !AccountHelper.CheckCustomerPermission(AccountInfo, info.CustomerId))
                return Error("Bạn không có quyền cập nhật thông tin này");

            if (info.RetestDate.HasValue && info.RetestDate != DateTime.MinValue && info.RetestDate != ConfigMgr.DefaultDate)
                info.RetestDateString = info.RetestDate.Value.Year.ToString();

            return new Extension.JsonResult(HttpStatusCode.OK, info);
        }

        #endregion

        #region Phân quyền về cho nhân viên

        [HttpPost]
        [Permissions("Customer.AddPermission")]
        public ActionResult AddPermission(string ids, int uId)
        {
            if (string.IsNullOrEmpty(ids) || uId <= 0)
                return Error("Thông tin dữ liệu không hợp lệ");

            var title = CompanyTitleDb.Instance.Read(uId);
            if (title == null)
                return Error("Thông tin nhân viên không hợp lệ");

            if (CustomerDb.Instance.UpdateEmployee(ids, title.Id, title.Code, WebSecurity.CurrentUserId, WebSecurity.CurrentUserName))
                return Success("Cập nhật thông tin thành công");

            return Error("Có lỗi xảy ra vui lòng thử lại sau");
        }

        #endregion

        #region Delete

        [HttpPost]
        [Permissions("Customer.Customer.Delete")]
        public ActionResult DeleteCustomer(string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                LogHelper.Warning("DeleteCustomer", "IDs:" + ids);
                var lstId = ids.SplitToIntList();
                foreach (var id in lstId)
                {
                    if (AccountHelper.CheckCustomerPermission(AccountInfo, id, UserAction.Delete.Key))
                    {
                        if(CustomerDb.Instance.Delete(id.ToString(), AccountInfo.CompanyId, WebSecurity.CurrentUserId, WebSecurity.CurrentUserName))
                        {
                            var contract = ContractDb.Instance.GetByCustomer(id);
                            if (contract != null)
                                ContractDb.Instance.Delete(contract.Id.ToString(), AccountInfo.CompanyId, WebSecurity.CurrentUserId, WebSecurity.CurrentUserName);
                        }
                    }
                }
                return Success();
            }
            return Error();
        }

        [HttpPost]
        [Permissions("Customer.Parent.Delete")]
        public ActionResult DeleteParent(string ids, int cId)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                if (!AccountHelper.CheckCustomerPermission(AccountInfo, cId))
                    return Error();

                if (ParentDb.Instance.Delete(ids, AccountInfo.CompanyId, WebSecurity.CurrentUserId, WebSecurity.CurrentUserName))
                {
                    return Success();
                }
            }
            return Error();
        }

        [HttpPost]
        [Permissions("Customer.CustomerCare.Delete")]
        public ActionResult DeleteCustomerCare(string ids, int cId)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                if (!AccountHelper.CheckCustomerPermission(AccountInfo, cId))
                    return Error();

                if (CustomerCareDb.Instance.Delete(ids, AccountInfo.CompanyId, WebSecurity.CurrentUserId, WebSecurity.CurrentUserName))
                {
                    return Success();
                }
            }
            return Error();
        }

        [HttpPost]
        [Permissions("Customer.StudyHistory.Delete")]
        public ActionResult DeleteStudyHistory(string ids, int cId)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                if (!AccountHelper.CheckCustomerPermission(AccountInfo, cId))
                    return Error();

                if (StudyHistoryDb.Instance.Delete(ids, AccountInfo.CompanyId, WebSecurity.CurrentUserId, WebSecurity.CurrentUserName))
                {
                    return Success();
                }
            }
            return Error();
        }

        [HttpPost]
        [Permissions("Customer.StudyAbroad.Delete")]
        public ActionResult DeleteStudyAbroad(string ids, int cId)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                if (!AccountHelper.CheckCustomerPermission(AccountInfo, cId))
                    return Error();

                if (StudyAbroadDb.Instance.Delete(ids, AccountInfo.CompanyId, WebSecurity.CurrentUserId, WebSecurity.CurrentUserName))
                {
                    return Success();
                }
            }
            return Error();
        }

        [HttpPost]
        [Permissions("Customer.Language.Delete")]
        public ActionResult DeleteLanguage(string ids, int cId)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                if (!AccountHelper.CheckCustomerPermission(AccountInfo, cId))
                    return Error();

                if (LanguageDb.Instance.Delete(ids, AccountInfo.CompanyId, WebSecurity.CurrentUserId, WebSecurity.CurrentUserName))
                {
                    return Success();
                }
            }
            return Error();
        }

        [HttpPost]
        [Permissions("Customer.Relatives.Delete")]
        public ActionResult DeleteRelatives(string ids, int cId)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                if (!AccountHelper.CheckCustomerPermission(AccountInfo, cId))
                    return Error();

                if (RelativesDb.Instance.Delete(ids, AccountInfo.CompanyId, WebSecurity.CurrentUserId, WebSecurity.CurrentUserName))
                {
                    return Success();
                }
            }
            return Error();
        }

        [HttpPost]
        [Permissions("Customer.Guarantee.Delete")]
        public ActionResult DeleteGuarantee(string ids, int cId)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                if (!AccountHelper.CheckCustomerPermission(AccountInfo, cId))
                    return Error();

                if (GuaranteeDb.Instance.Delete(ids, AccountInfo.CompanyId, WebSecurity.CurrentUserId, WebSecurity.CurrentUserName))
                {
                    return Success();
                }
            }
            return Error();
        }

        #endregion

        #region ImportCustomer

        [Permissions("Customer.ImportCustomer")]
        public ActionResult ImportCustomer()
        {
            var model = new ImportCustomerModel();
            if(Session["ImportData"] != null)
            {
                model.ListCustomer = Session["ImportData"] as List<ImportCustomerData>;
            }
            return View(model);
        }

        [HttpPost]
        [Permissions("Customer.ImportCustomer.Post")]
        public ActionResult ImportCustomer(ImportCustomerModel model, FormCollection frmCollection)
        {
            HttpPostedFileBase file = Request.Files["inputfile"];
            if (file != null && file.ContentLength > 0 && !string.IsNullOrEmpty(file.FileName) && file.FileName.Contains(".xlsx"))
            {
                byte[] fileBytes = new byte[file.ContentLength];
                var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                using (var package = new ExcelPackage(file.InputStream))
                {
                    var index = 1;
                    var lstCustomer = new List<ImportCustomerData>();
                    var currentSheet = package.Workbook.Worksheets;
                    var workSheet = currentSheet.First();
                    var noOfCol = workSheet.Dimension.End.Column;
                    var noOfRow = workSheet.Dimension.End.Row;
                    for (int rowIterator = 3; rowIterator <= noOfRow; rowIterator++)
                    {
                        if (workSheet.Cells[rowIterator, 4].Value == null)
                            continue;

                        var user = new ImportCustomerData();
                        user.ID = index;
                        user.Gender = workSheet.Cells[rowIterator, 3].Value != null ? workSheet.Cells[rowIterator, 3].Value.ToString() : "";
                        user.FullName = workSheet.Cells[rowIterator, 4].Value != null ? workSheet.Cells[rowIterator, 4].Value.ToString() : "";
                        user.Birthday = workSheet.Cells[rowIterator, 5].Value != null ? workSheet.Cells[rowIterator, 5].Value.ToString() : "";
                        user.Phone = workSheet.Cells[rowIterator, 6].Value != null ? workSheet.Cells[rowIterator, 6].Value.ToString() : "";
                        user.Email = workSheet.Cells[rowIterator, 7].Value != null ? workSheet.Cells[rowIterator, 7].Value.ToString() : "";
                        user.Address = workSheet.Cells[rowIterator, 8].Value != null ? workSheet.Cells[rowIterator, 8].Value.ToString() : "";
                        user.Province = workSheet.Cells[rowIterator, 9].Value != null ? workSheet.Cells[rowIterator, 9].Value.ToString() : "";
                        user.Content = workSheet.Cells[rowIterator, 10].Value != null ? workSheet.Cells[rowIterator, 10].Value.ToString() : "";
                        user.Source = workSheet.Cells[rowIterator, 11].Value != null ? workSheet.Cells[rowIterator, 11].Value.ToString() : "";
                        user.Note = workSheet.Cells[rowIterator, 12].Value != null ? workSheet.Cells[rowIterator, 12].Value.ToString() : "";
                        user.Country = workSheet.Cells[rowIterator, 13].Value != null ? workSheet.Cells[rowIterator, 13].Value.ToString() : "";
                        user.EducationLevel = workSheet.Cells[rowIterator, 14].Value != null ? workSheet.Cells[rowIterator, 14].Value.ToString() : "";
                        // thêm NV tư vấn
                        user.EmployeeCode = workSheet.Cells[rowIterator, 15].Value != null ? workSheet.Cells[rowIterator, 15].Value.ToString() : "";

                        lstCustomer.Add(user);
                        index++;
                    }
                    Session["ImportData"] = lstCustomer;
                    model.ListCustomer = lstCustomer;
                }
            }
            return View(model);
        }

        [HttpPost]
        [Permissions("Customer.ImportCustomer.Save")]
        public ActionResult SaveImportData()
        {
            var result = new { Code = 1, Msg = "Fail" };
            if (Session["ImportData"] != null)
            {
                try
                {
                    var countryId = 0;
                    var lstProvince = ProvinceCompanyDb.Instance.GetByCompany(AccountInfo.CompanyId);
                    var lstCountry = CountryCompanyDb.Instance.GetByCompany(AccountInfo.CompanyId);
                    var lstCompanyTitle = CompanyTitleDb.Instance.GetByCompany(AccountInfo.CompanyId);
                    
                    var lstCustomer = Session["ImportData"] as List<ImportCustomerData>;
                    foreach (var tmp in lstCustomer)
                    {
                        tmp.Phone = string.IsNullOrWhiteSpace(tmp.Phone)
                            ? ""
                            : tmp.Phone.Trim().Replace(".", "").Replace(" ", "").Replace("p:", "").Replace("+84", "0");

                        if (!string.IsNullOrWhiteSpace(tmp.Phone))
                        {
                            // trùng thông tin thì không làm gì cả
                            var checkPhone = CustomerDb.Instance.GetByPhone(tmp.Phone, AccountInfo.CompanyId);
                            if (checkPhone != null)
                            {
                                continue;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(tmp.Email))
                        {
                            // trùng thông tin thì không làm gì cả
                            var checkEmail = CustomerDb.Instance.GetByEmail(tmp.Email, AccountInfo.CompanyId);
                            if (checkEmail != null)
                            {
                                continue;
                            }
                        }

                        var customer = new CustomerBo()
                        {
                            CompanyId = AccountInfo.CompanyId,
                            Fullname = tmp.FullName,
                            Phone = tmp.Phone,
                            Email = tmp.Email,
                            Address = tmp.Address,
                            Gender = 0,
                            Status = CustomerStatus.NotCaring.Key,
                            ProvinceId = 0,
                            CustomerNote = tmp.Content + " - " + tmp.Note,
                            EmployeeId = 0,
                            NewsUrlRef = tmp.Source,
                            CreateUserId = WebSecurity.CurrentUserId,
                            CreateUserName = WebSecurity.CurrentUserName
                        };

                        if(AccountInfo.TitleType == CompanyTitleType.Agency.Key)
                        {
                            customer.AgencyId = AccountInfo.TitleId;
                            customer.AgencyName = AccountInfo.TitleCode;
                        }

                        if (!string.IsNullOrWhiteSpace(tmp.Gender))
                        {
                            customer.Gender = tmp.Gender.ToLower().Trim().Equals("nam") ? 1 : 0;
                        }

                        if (!string.IsNullOrWhiteSpace(tmp.Birthday))
                        {
                            try
                            {
                                var myDTFI = new DateTimeFormatInfo();
                                myDTFI.ShortDatePattern = "MM/dd/yyyy";
                                var bd = DateTime.Parse(tmp.Birthday, myDTFI);
                                if (bd.Year > 1900 && bd.Year < 9999)
                                {
                                    customer.Birthday = bd;
                                }
                            }
                            catch { }
                        }

                        var provinceId = 0;
                        // phân quyền trực tiếp cho NV trước, nếu ko có nhập thì mới phân quyền về cho TVP
                        if(!string.IsNullOrWhiteSpace(tmp.EmployeeCode) && lstCompanyTitle != null && lstCompanyTitle.Any(t => t.Code == tmp.EmployeeCode.Trim().Replace(" ", "")))
                        {
                            var objTitle = lstCompanyTitle.First(t => t.Code == tmp.EmployeeCode.Trim().Replace(" ", ""));
                            if (objTitle != null)
                            {
                                customer.EmployeeId = objTitle.Id;
                                customer.EmployeeName = objTitle.Code;
                                customer.ProvinceId = objTitle.OfficeId;
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(tmp.Province) && lstProvince != null && lstProvince.Any())
                        {
                            tmp.Province = tmp.Province.Trim();
                            if (lstProvince.Any(p => p.ProvinceName.ToLower().Contains(tmp.Province.ToLower())))
                            {
                                provinceId = lstProvince.FirstOrDefault(p => p.ProvinceName.ToLower().Contains(tmp.Province.ToLower())).Id;
                                customer.ProvinceId = provinceId;

                                // phân quyền theo tỉnh thành
                                if (lstProvince != null && lstProvince.Any(p => p.Id == provinceId) &&
                                    lstCompanyTitle != null && lstCompanyTitle.Any())
                                {
                                    var provinceInfo = lstProvince.Where(p => p.Id == provinceId).FirstOrDefault();
                                    if (provinceInfo.OfficeId > 0)
                                    {
                                        var tvp = lstCompanyTitle.FirstOrDefault(p => p.OfficeId == provinceInfo.OfficeId && p.UserType == CompanyTitleType.Leader.Key);
                                        if (tvp != null)
                                        {
                                            customer.EmployeeId = tvp.Id;
                                            customer.EmployeeName = tvp.Code;
                                            customer.OfficeId = provinceInfo.OfficeId;
                                        }
                                    }
                                }
                            }
                        }

                        if(customer.EmployeeId == 0 && (AccountInfo.TitleType == CompanyTitleType.Counselor.Key || AccountInfo.TitleType == CompanyTitleType.Leader.Key))
                        {
                            // phân quyền cho chính nv import nếu chưa phân quyền cho user nào
                            customer.EmployeeId = AccountInfo.TitleId;
                            customer.EmployeeName = AccountInfo.TitleCode;
                            customer.OfficeId = AccountInfo.OfficeByTitle;
                        }

                        if (!string.IsNullOrWhiteSpace(tmp.Source))
                        {
                            tmp.Source = tmp.Source.Trim();
                            var lstSource = CustomerSource.Instant().GetAll();
                            var source = lstSource.FirstOrDefault(s => s.Value.ToLower().Equals(tmp.Source.Trim().ToLower()));
                            if (source != null)
                                customer.Source = source.Key;
                        }
                        if (!string.IsNullOrWhiteSpace(tmp.Country))
                        {
                            tmp.Country = tmp.Country.Trim();
                            var country = lstCountry.FirstOrDefault(s => s.CountryName.ToLower().Contains(tmp.Country.Trim().ToLower()));
                            if (country != null)
                            {
                                countryId = country.Id;
                            }
                            customer.CountryId = countryId.ToString();
                        }

                        var level = -1;
                        if (!string.IsNullOrEmpty(tmp.EducationLevel))
                        {
                            level = GetEducationLevel(tmp.EducationLevel);
                            customer.EducationLevelId = level.ToString();
                        }

                        var customerId = CustomerDb.Instance.Create(customer);
                        if (customerId <= 0)
                        {
                            LogHelper.Error("Lỗi insert import customer", JsonConvert.SerializeObject(tmp));
                        }

                        if (countryId > 0 && customerId > 0)
                        {
                            // thêm thông tin quốc gia dự định du học
                            var countryName = string.Empty;
                            if(lstCountry.Any(c => c.CountryId == countryId))
                                countryName = lstCountry.FirstOrDefault(c => c.Id == countryId).CountryName;

                            var abroad = new StudyAbroadBo()
                            {
                                CustomerId = customerId,
                                CountryId = countryId,
                                CountryName = countryName,
                                School = "",
                                Level = level,
                                CreateUserName = WebSecurity.CurrentUserName
                            };
                            if (StudyAbroadDb.Instance.Create(abroad) <= 0)
                            {
                                LogHelper.Error("Lỗi insert study abroad", JsonConvert.SerializeObject(tmp));
                            }
                            else
                            {
                                var lstStudyAbroad = StudyAbroadDb.Instance.GetByCustomer(customerId);
                                if(lstStudyAbroad != null && lstStudyAbroad.Any())
                                {
                                    var lstCountryId = lstStudyAbroad?.Where(h => h.CountryId > 0).Select(h => h.CountryId).ToList() ?? new List<int>();
                                    var lstLevel = lstStudyAbroad?.Where(h => h.Level >= 0).Select(h => h.Level).ToList() ?? new List<int>();
                                    
                                    CustomerSearch.Instance.Update(customerId.ToString(),
                                        new
                                        {
                                            CountryId = lstCountryId,
                                            EducationLevelId = lstLevel
                                        });
                                }
                            }
                        }
                    }
                    Session["ImportData"] = null;
                    return Success("Import dữ liệu khách hàng thành công");
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex.Message, ex);
                }
            }
            return Error("Có lỗi xảy ra vui lòng thử lại sau");
        }

        public int GetEducationLevel(string level)
        {
            if (!string.IsNullOrEmpty(level))
            {
                level = level.ToLower();
                switch (level)
                {
                    case "THPT":
                    case "thpt":
                        return 0;
                    case "HocNghe":
                    case "học nghề":
                        return 1;
                    case "TrungCap":
                    case "trung cấp":
                        return 2;
                    case "CaoDang":
                    case "cao đẳng":
                        return 3;
                    case "DaiHoc":
                    case "đại học":
                        return 4;
                    case "ThacSy":
                    case "thạc sỹ":
                        return 5;
                    case "SauDaiHoc":
                    case "sau đại học":
                        return 6;
                    
                }
            }
            return -1;
        }

        #endregion

        [Permissions("Customer.ExportExcel")]
        public ActionResult ExportExcel(int status, int country, string from, string to, int employee)
        {
            try
            {
                var employeeIds = string.Empty;
                if (employee != -1)
                {
                    employeeIds = employee.ToString();
                }
                else if (AccountInfo.IsAdminGroup())
                {
                    employeeIds = "ADMIN";
                }
                else if (AccountInfo.ListTitle != null)
                {
                    var lstEmployee = AccountInfo.ListTitle.Where(t => t.OfficeId == AccountInfo.OfficeByTitle);
                    employeeIds = string.Join(",", lstEmployee.Select(i => i.Id).ToArray());
                }
                else
                {
                    employeeIds = AccountInfo.TitleId.ToString();
                }

                var query = new CustomerQuery()
                {
                    Keyword = string.Empty,
                    From = from.ToDateTime(),
                    To = to.ToDateTime(),
                    Status = status.ToString(),
                    Country = country.ToString(),
                    Employee = employeeIds,
                    Company = AccountInfo.CompanyId.ToString(),
                    Page = 0,
                    PageSize = 500,
                    Sort = CustomerSort.CreateDate.Key
                };

                var lstCustomer = CustomerDb.Instance.Select(query);
                if (lstCustomer != null)
                {
                    var lstExcel = lstCustomer.Select(c => CustomerDb.Instance.ToCustomerExcel(c)).ToList();
                    ExportToExcel.Export<CustomerExcel>(lstExcel, Server.MapPath("~/Files/TemplateExcel/CustomerList.xls"), "DSKH.xls", "Danh sách khách hàng", "");
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("ExportExcel", ex);
            }
            
            return View();
        }

        #region SeminarRegister

        public ActionResult SeminarRegisterInfo(int id)
        {
            if (id <= 0)
                return Error("Có lỗi xảy ra vui lòng thử lại sau");

            var info = SeminarRegisterDb.Instance.Read(id);
            if (info == null)
                return Error("Không thể tìm thấy thông tin đăng ký hội thảo");

            return new Extension.JsonResult(HttpStatusCode.OK, info);
        }

        public ActionResult SeminarRegister(int id)
        {
            var model = new SeminarRegisterInsertModel();
            var customer = CustomerDb.Instance.Read(id);
            if (customer == null)
                return Error("Không tìm thấy thông tin khách hàng");

            var lstSeminar = SeminarDb.Instance.GetByCompany(AccountInfo.CompanyId);
            
            model.CustomerId = id;
            model.CustomerName = customer.Fullname;
            model.CustomerPhone = customer.Phone;
            model.CustomerEmail = customer.Email;
            if(lstSeminar != null && lstSeminar.Any())
            {
                model.ListSeminar = lstSeminar.Select(i => new SelectListItem()
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }).ToList();
                var lstPlace = SeminarPlaceDb.Instance.GetBySeminar(lstSeminar.FirstOrDefault().Id);
                if(lstPlace != null && lstPlace.Any())
                {
                    model.ListSeminarPlace = lstPlace.Select(i => new SelectListItem()
                    {
                        Text = i.Place,
                        Value = i.Id.ToString()
                    }).ToList();
                }
                var register = SeminarRegisterDb.Instance.GetByCustomer(id, lstSeminar.FirstOrDefault().Id);
                if(register != null && register.Any())
                {
                    var info = register.FirstOrDefault();
                    model.SeminarPlaceId = info.SeminarPlaceId;
                    model.CustomerNote = info.CustomerNote;
                    model.Note = info.Note;
                }
            }

            var html = RenderPartialViewToString("_SeminarRegister", model);
            return Success("Success", html);
        }

        [HttpPost]
        [ValidateModelAttribute]
        [Permissions("Customer.SeminarRegister.Post")]
        public ActionResult SeminarRegister(SeminarRegisterInsertModel model)
        {
            var customer = CustomerDb.Instance.Read(model.CustomerId);
            if (customer == null)
                return Error("Không tìm thấy thông tin khách hàng");

            if(model.Id <= 0)
            {
                var seminar = SeminarDb.Instance.Read(model.SeminarId);
                if (seminar == null)
                    return Error("Không tìm thấy thông tin hội thảo");

                var info = new SeminarRegisterBo();
                info.CompanyId = AccountInfo.CompanyId;
                info.SeminarId = model.SeminarId;
                info.SeminarName = seminar.Name;
                info.CustomerId = model.CustomerId;
                info.TicketId = model.TicketId;
                info.School1 = model.School1;
                info.School1Time = model.School1Time;
                info.School2 = model.School2;
                info.School2Time = model.School2Time;
                info.School3 = model.School3;
                info.School3Time = model.School3Time;
                info.SeminarId = model.SeminarId;
                info.SeminarPlaceId = model.SeminarPlaceId;
                info.CustomerNote = model.CustomerNote;
                info.Note = model.Note;
                info.CreateUserId = WebSecurity.CurrentUserId;
                info.CreateUserName = WebSecurity.CurrentUserName;

                var id = SeminarRegisterDb.Instance.Create(info);
                if (id > 0)
                {
                    var list = SeminarRegisterDb.Instance.GetByCustomer(model.CustomerId, -1);
                    var html = RenderPartialViewToString("_ListSeminarRegister", list);
                    return Success("Thêm mới thông tin hội thảo thành công", html);
                }
            }
            else
            {
                var seminar = SeminarDb.Instance.Read(model.SeminarId);
                if (seminar == null)
                    return Error("Không tìm thấy thông tin hội thảo");

                var info = SeminarRegisterDb.Instance.Read(model.Id);
                info.Id = model.Id;
                info.CustomerId = model.CustomerId;
                info.SeminarId = model.SeminarId;
                info.SeminarName = seminar.Name;
                info.TicketId = model.TicketId;
                info.School1 = model.School1;
                info.School1Time = model.School1Time;
                info.School2 = model.School2;
                info.School2Time = model.School2Time;
                info.School3 = model.School3;
                info.School3Time = model.School3Time;
                info.SeminarPlaceId = model.SeminarPlaceId;
                info.CustomerNote = model.CustomerNote;
                info.Note = model.Note;
                info.UpdateUserId = WebSecurity.CurrentUserId;
                info.UpdateUserName = WebSecurity.CurrentUserName;

                if (SeminarRegisterDb.Instance.Update(info))
                {
                    var list = SeminarRegisterDb.Instance.GetByCustomer(model.CustomerId, -1);
                    var html = RenderPartialViewToString("_ListSeminarRegister", list);
                    return Success("Cập nhật thông tin hội thảo thành công", html);
                }
            }

            return Error("Có lỗi xảy ra vui lòng thử lại sau");
        }

        public ActionResult LoadSeminarPlace(int id)
        {
            var lstPlace = SeminarPlaceDb.Instance.GetBySeminar(id);
            var html = RenderPartialViewToString("LoadSeminarPlace", lstPlace);
            return Success("Success", html);
        }

        //public ActionResult LoadSeminarRegister(int id, int customerId)
        //{
        //    var register = SeminarRegisterDb.Instance.GetByCustomer(customerId, id);
        //    if (register == null)
        //        return Error("Không tìm thấy thông tin đăng ký");

        //    return new Extension.JsonResult(HttpStatusCode.OK, register);
        //}

        #endregion

        #region CustomerRelatives

        [HttpPost]
        [ValidateModelAttribute]
        [Permissions("Customer.Relatives.Post")]
        public ActionResult CustomerRelatives(CustomerRelativesModel model)
        {
            if (model.Id > 0)
            {
                #region Cập nhật thông tin thân nhân

                var relatives = RelativesDb.Instance.Read(model.Id);
                if (relatives != null)
                {
                    relatives.Name = model.Name;
                    relatives.CountryName = model.CountryName;
                    relatives.Relationship = model.Relationship;
                    relatives.Address = model.Address;
                    relatives.JobName = model.JobName;
                    relatives.CompanyName = model.CompanyName;
                    relatives.Income = model.Income;
                    relatives.UpdateUserId = WebSecurity.CurrentUserId;
                    relatives.UpdateUserName = WebSecurity.CurrentUserName;

                    if (RelativesDb.Instance.Update(relatives))
                    {
                        var lstRelatives = RelativesDb.Instance.GetByCustomer(model.CustomerId);
                        var html = RenderPartialViewToString("_ListRelatives", lstRelatives);

                        return Success("Cập nhật thông tin thân nhân thành công", html);
                    }
                }

                #endregion
            }
            else
            {
                #region Thêm mới thông tin thân nhân

                var relatives = new RelativesBo
                {
                    CustomerId = model.CustomerId,
                    Name = model.Name,
                    CountryName = model.CountryName,
                    Relationship = model.Relationship,
                    Address = model.Address,
                    JobName = model.JobName,
                    CompanyName = model.CompanyName,
                    Income = model.Income,
                    CompanyId = AccountInfo.CompanyId,
                    CreateUserId = WebSecurity.CurrentUserId,
                    CreateUserName = WebSecurity.CurrentUserName
                };

                var id = RelativesDb.Instance.Create(relatives);
                if (id > 0)
                {
                    var lstRelatives = RelativesDb.Instance.GetByCustomer(model.CustomerId);
                    var html = RenderPartialViewToString("_ListRelatives", lstRelatives);
                    return Success("Thêm mới thông tin thân nhân thành công", html);
                }

                #endregion
            }
            return Error("Có lỗi xảy ra vui lòng thử lại sau");
        }

        public ActionResult RelativesInfo(int id)
        {
            if (id <= 0)
                return Error("Có lỗi xảy ra vui lòng thử lại sau");

            var relatives = RelativesDb.Instance.Read(id);
            if (relatives == null)
                return Error("Không tìm thấy thông tin thân nhân");

            return new Extension.JsonResult(HttpStatusCode.OK, relatives);
        }

        public ActionResult LoadRelatives(int id)
        {
            var lstRelatives = RelativesDb.Instance.GetByCustomer(id);
            var html = RenderPartialViewToString("LoadRelatives", lstRelatives);
            return Success("Success", html);
        }

        #endregion

        #region CustomerGuarantee

        [HttpPost]
        [ValidateModelAttribute]
        [Permissions("Customer.Guarantee.Post")]
        public ActionResult CustomerGuarantee(CustomerGuaranteeModel model)
        {
            var relative = RelativesDb.Instance.Read(model.RelativesId);
            var relativeName = relative?.Name;

            if (model.Id > 0)
            {
                #region Cập nhật thông tin bảo lãnh

                var guarantee = GuaranteeDb.Instance.Read(model.Id);
                if (guarantee != null)
                {
                    guarantee.RelativesId = model.RelativesId;
                    guarantee.RelativesName = relativeName;
                    guarantee.Person = model.Person;
                    guarantee.GuaranteeName = model.GuaranteeName;
                    guarantee.GuaranteeYear = model.GuaranteeYear;
                    guarantee.UpdateUserId = WebSecurity.CurrentUserId;
                    guarantee.UpdateUserName = WebSecurity.CurrentUserName;

                    if (GuaranteeDb.Instance.Update(guarantee))
                    {
                        var lstGuarantee = GuaranteeDb.Instance.GetByCustomer(model.CustomerId);
                        var html = RenderPartialViewToString("_ListGuarantee", lstGuarantee);

                        return Success("Cập nhật thông tin bảo lãnh thành công", html);
                    }
                }

                #endregion
            }
            else
            {
                #region Thêm mới thông tin bảo lãnh

                var guarantee = new GuaranteeBo
                {
                    CustomerId = model.CustomerId,
                    RelativesId = model.RelativesId,
                    RelativesName = relativeName,
                    Person = model.Person,
                    GuaranteeName = model.GuaranteeName,
                    GuaranteeYear = model.GuaranteeYear,
                    CompanyId = AccountInfo.CompanyId,
                    CreateUserId = WebSecurity.CurrentUserId,
                    CreateUserName = WebSecurity.CurrentUserName
                };

                var id = GuaranteeDb.Instance.Create(guarantee);
                if (id > 0)
                {
                    var lstGuarantee = GuaranteeDb.Instance.GetByCustomer(model.CustomerId);
                    var html = RenderPartialViewToString("_ListGuarantee", lstGuarantee);
                    return Success("Thêm mới thông tin bảo lãnh thành công", html);
                }

                #endregion
            }
            return Error("Có lỗi xảy ra vui lòng thử lại sau");
        }

        public ActionResult GuaranteeInfo(int id)
        {
            if (id <= 0)
                return Error("Có lỗi xảy ra vui lòng thử lại sau");

            var guarantee = GuaranteeDb.Instance.Read(id);
            if (guarantee == null)
                return Error("Không tìm thấy thông tin bảo lãnh");

            return new Extension.JsonResult(HttpStatusCode.OK, guarantee);
        }

        #endregion

        #region Comment

        [HttpPost]
        public ActionResult InsertComment(int id, int cid, string content)
        {
            if(id <= 0 || string.IsNullOrEmpty(content))
                return Error("Vui lòng nhập đầy đủ thông tin");

            var comment = new CommentBo()
            {
                CompanyId = AccountInfo.CompanyId,
                CustomerId = cid,
                ObjectId = id,
                ObjectType = CommentType.CustomerCare.Key,
                Title = "Comment chăm sóc khách hàng",
                Message = content,
                CreateUserId = WebSecurity.CurrentUserId,
                CreateUserName = WebSecurity.CurrentUserName
            };

            if(CommentDb.Instance.Create(comment) > 0)
            {
                return Success("Thêm mới thông tin bình luận thành công");
            }
            return Error("Có lỗi xảy ra vui lòng thử lại sau");
        }

        public ActionResult LoadComment(int id)
        {
            var lstComment = CommentDb.Instance.GetComment(id);
            var html = string.Empty;
            if (lstComment != null && lstComment.Any())
            {
                html = JsonConvert.SerializeObject(lstComment);
            }
            return Content(html);
        }

        [HttpPost]
        public ActionResult DeleteComment(int id)
        {
            CommentDb.Instance.Delete(id.ToString());
            return Success("Xóa bình luận thành công");
        }

        #endregion

        #region Report

        public ActionResult CustomerStatusReportMonthly(int fromMonth, int toMonth, int fromYear, int toYear)
        {
            var cacheKey = Cacher.CreateCacheKey(fromMonth, toMonth, fromYear, toYear);
            var result = Cacher.Get<LineChartModel>(cacheKey);
            if (result != null)
                return new Extension.JsonResult(HttpStatusCode.OK, result);

            var fromDate = new DateTime(fromYear, fromMonth, 1);
            var toDate = new DateTime(toYear, toMonth, 1).AddMonths(1);
            var data = CustomerDb.Instance.GetStatusReportMonthly(fromDate, toDate, AccountInfo.CompanyId);
            if (data == null || data.Count == 0)
                return new Extension.JsonResult(HttpStatusCode.NotFound);

            var model = new LineChartModel();
            model.Categories = string.Join(",", data.Select(i => i.Date.ToString("MM/yyyy")).ToArray());
            model.Series = new Series()
            {
                Items = new List<Item>()
            };
            //model.Series.Items.Add(new Item()
            //{
            //    Name = "Tổng số hợp đồng",
            //    Data = string.Join(",", result.Select(i => i.Total).ToArray())
            //});
            model.Series.Items.Add(new Item()
            {
                Name = "Tiếp tục chăm sóc",
                Data = string.Join(",", data.Select(i => i.TotalContinueCare).ToArray())
            });
            model.Series.Items.Add(new Item()
            {
                Name = "Tiềm năng",
                Data = string.Join(",", data.Select(i => i.TotalPotential).ToArray())
            });
            model.Series.Items.Add(new Item()
            {
                Name = "Không tiềm năng",
                Data = string.Join(",", data.Select(i => i.TotalNotPotential).ToArray())
            });
            model.Series.Items.Add(new Item()
            {
                Name = "Có thể ký hợp đồng",
                Data = string.Join(",", data.Select(i => i.TotalMaybeContract).ToArray())
            });
            model.Series.Items.Add(new Item()
            {
                Name = "Đã ký hợp đồng",
                Data = string.Join(",", data.Select(i => i.TotalContract).ToArray())
            });
            model.Total = data.Sum(i => i.Total);

            Cacher.Add(cacheKey, model, DateTime.Now.AddMinutes(10));

            return new Extension.JsonResult(HttpStatusCode.OK, model);
        }

        #endregion

    }
}