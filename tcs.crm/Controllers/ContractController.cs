using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using tcs.adapter.Helper;
using tcs.adapter.Models;
using tcs.adapter.Sql;
using tcs.bo;
using tcs.crm.Models;
using tcs.lib;
using WebMatrix.WebData;

namespace tcs.crm.Controllers
{
    [Authorize]
    public class ContractController : BaseController
    {
        #region Index

        [Permissions("Contract.Index")]
        public ActionResult Index()
        {
            var model = new ContractModel();
            InitContractModel(model);

            var total = 0;
            model.ListContract = SearchContract(model, ref total);
            if (model.ListContract != null && model.ListContract.Any())
            {
                model.TotalRecord = total;
                model.TotalString = ViewHelper.GetTotalString(model.PageIndex, model.PageSize, total);
                model.PagerString = ViewHelper.BuildCMSPaging(total, model.PageIndex, 5, model.PageSize);
            }

            return View(model);
        }

        [HttpPost]
        [Permissions("Contract.Index")]
        public ActionResult Index(ContractModel model)
        {
            var total = 0;
            InitContractModel(model);
            model.ListContract = SearchContract(model, ref total);
            if (model.ListContract != null && model.ListContract.Any())
            {
                model.TotalRecord = total;
                model.TotalString = ViewHelper.GetTotalString(model.PageIndex, model.PageSize, total);
                model.PagerString = ViewHelper.BuildCMSPaging(total, model.PageIndex, 5, model.PageSize);
            }
            return View(model);
        }

        public List<ContractSE> SearchContract(ContractModel model, ref int total)
        {
            var query = new ContractQuery()
            {
                Keyword = model.Keyword,
                From = model.From,
                To = model.To,
                Status = model.Status,
                Page = model.PageIndex,
                PageSize = model.PageSize,
                Company = AccountInfo.CompanyId.ToString(),
                Office = model.OfficeId,
                CountryId = model.CountryId,
                EmployeeProcessId = model.EmployeeProcessId
            };
            var lstContract = ContractDb.Instance.Search(query);
            total = query.TotalRecord;
            return lstContract;
        }

        public void InitContractModel(ContractModel model)
        {
            if (model == null)
                model = new ContractModel();

            if (!string.IsNullOrEmpty(model.FromDate))
            {
                model.From = model.FromDate.ToDateTime();
            }
            if (!string.IsNullOrEmpty(model.ToDate))
            {
                model.To = model.ToDate.ToDateTime().AddHours(23).AddMinutes(59).AddSeconds(59);
            }

            model.AccountInfo = AccountInfo;
            model.ListStatus = ContractStatus.Instant().GetAll(true);
            // aaaa
            //var lstCompanyTitle = AccountInfo.ListTitle;
            //if (lstCompanyTitle != null)
            //{
            //    model.ListCompanyTitle = lstCompanyTitle.Select(i => new SelectListItem()
            //    {
            //        Text = (i.UserType == CompanyTitleType.Leader.Key ? "" : "---- ") + i.Code + " (" + i.UserFullName + ")",
            //        Value = i.Id.ToString()
            //    }).ToList();
            //    model.ListCompanyTitle.Insert(0, new SelectListItem()
            //    {
            //        Text = "Tất cả",
            //        Value = ""
            //    });
            //}
            var office = AccountInfo.IsAdminGroup() ? -1 : AccountInfo.OfficeByTitle;
            var lstAdmission = AccountHelper.GetListAdmission(AccountInfo.CompanyId, office);
            if (lstAdmission != null)
            {
                var employeeWithName = lstAdmission.Select(i => new SelectListItem()
                {
                    Text = (i.UserType == CompanyTitleType.Leader.Key ? "" : "---- ") + i.Code + " (" + i.UserFullName + ")",
                    Value = i.Id.ToString()
                }).ToList();
                employeeWithName.Insert(0, new SelectListItem()
                {
                    Text = "Chưa phân quyền",
                    Value = ""
                });
                model.ListCompanyTitle = employeeWithName;
            }

            var lstOffice = CompanyOfficeDb.Instance.GetByCompany(AccountInfo.CompanyId);
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
                    Value = ""
                });
            }

            var lstCountry = CountryCompanyDb.Instance.GetByCompany(AccountInfo.CompanyId, true);
            if (lstCountry != null)
            {
                model.ListCountryCompany = lstCountry.Select(i => new SelectListItem()
                {
                    Text = i.CountryName,
                    Value = i.Id.ToString()
                }).ToList();
                model.ListCountryCompany.Insert(0, new SelectListItem()
                {
                    Text = "Tất cả",
                    Value = ""
                });
            }
        }

        #endregion

        #region Detail

        [Permissions("Contract.Detail")]
        public ActionResult Detail(int id = 0, int customerId = 0)
        {
            var model = new ContractDetailModel() { Id = id };

            var contract = ContractDb.Instance.Read(id);
            if (contract == null && customerId > 0)
            {
                // thử lấy theo customerId (TH xem hợp đồng từ trang DSKH)
                contract = ContractDb.Instance.GetByCustomer(customerId);
            }

            if (contract == null)
                contract = new ContractBo()
                {
                    CustomerId = customerId,
                    CompanyId = AccountInfo.CompanyId
                };

            if (contract.EmployeeId != AccountInfo.TitleId && !AccountInfo.IsAdminGroup() && AccountInfo.TitleType != CompanyTitleType.Leader.Key)
                return Error("Bạn không có quyền cập nhật thông tin này");

            model.Id = contract.Id;
            InitInsertUpdateModel(model, contract);

            var html = RenderPartialViewToString("Detail", model);
            return Success("Success", html);
        }

        #endregion

        #region ContractInfo

        [Permissions("Contract.ContractInfo")]
        public ActionResult ContractInfo(int id = 0, int customerId = 0)
        {
            var model = new ContractInsertModel() { Id = id, CustomerId = customerId, AccountInfo = AccountInfo };

            if (!AccountHelper.CheckCustomerPermission(AccountInfo, customerId))
                return null;

            var contract = ContractDb.Instance.Read(id);
            if (contract == null && customerId > 0)
            {
                // thử lấy theo customerId (TH xem hợp đồng từ trang DSKH)
                contract = ContractDb.Instance.GetByCustomer(customerId);
            }

            if (contract == null)
                contract = new ContractBo()
                {
                    CustomerId = customerId,
                    CompanyId = AccountInfo.CompanyId
                };

            model.Id = contract.Id;
            model.CustomerId = contract.CustomerId;
            InitContractInfo(model, contract);

            var html = RenderPartialViewToString("_ContractInfo", model);
            return Success("Success", html);
        }

        [Permissions("Contract.ContractInfo.Post")]
        [HttpPost]
        [ValidateModel]
        public ActionResult ContractInfo(ContractInsertModel model)
        {
            var err = ValidateInsertUpdate(model);
            if (err != null)
                return err;

            var customer = CustomerDb.Instance.Read(model.CustomerId);
            if (customer == null)
                return Error("Không tìm thấy thông tin khách hàng");

            if (AccountInfo.TitleType != CompanyTitleType.Leader.Key && !AccountInfo.IsAdminGroup() &&
                (customer.EmployeeId != AccountInfo.TitleId || AccountInfo.TitleType == CompanyTitleType.Agency.Key) &&
                customer.EmployeeProcessId != AccountInfo.TitleId)
            {
                return Error("Bạn không có quyền cập nhật thông tin này");
            }

            // thông tin nhân viên xử lý
            var employeeId = 0;
            var employeeName = string.Empty;
            var processEmployee = CompanyTitleDb.Instance.Read(model.EmployeeId);
            if (processEmployee != null)
            {
                employeeId = processEmployee.Id;
                employeeName = processEmployee.Code;
            }

            var schoolName = string.Empty;
            var scholarshipName = string.Empty;
            if(model.SchoolId > 0)
            {
                var school = SchoolDb.Instance.Read(model.SchoolId);
                schoolName = school?.SchoolName;
            }
            if (model.ScholarshipId > 0)
            {
                var scholarship = ScholarshipDb.Instance.Read(model.ScholarshipId);
                scholarshipName = scholarship?.ScholarshipName;
            }

            if (model.Id > 0)
            {
                #region Cập nhật thông tin hợp đồng

                var contract = ContractDb.Instance.Read(model.Id);
                if (contract == null)
                    return Error("Không tìm thấy thông tin hợp đồng");

                var firstScholarshipId = contract.ScholarshipId;
                var processNote = string.Empty;
                var logInfo = new ContractLogInfo();
                var changeStatus = false;
                if (model.Status != contract.Status)
                {
                    changeStatus = true;
                    logInfo = ContractLogInfo.FromContractBo(contract);
                    processNote = string.Format("Cập nhật trạng thái từ {0} -> {1}",
                        ContractStatus.Instant().GetValueByKey(contract.Status),
                        ContractStatus.Instant().GetValueByKey(model.Status));
                }

                contract.IsVisa = model.IsVisa;
                contract.Deposit = model.Deposit;
                contract.ServiceFee = model.ServiceFee;
                contract.CollectOne = model.CollectOne;
                contract.CollectTwo = model.CollectTwo;
                contract.Currency = model.Currency;
                contract.IsRefund = model.IsRefund;
                contract.Promotion = model.Promotion;
                contract.Note = model.Note;
                contract.Status = model.Status;
                contract.TotalCommission = model.TotalCommission;
                contract.UpdateUserId = WebSecurity.CurrentUserId;
                contract.UpdateUserName = WebSecurity.CurrentUserName;
                contract.ProfileTypeId = model.ProfileTypeId;
                contract.SchoolId = model.SchoolId;
                contract.SchoolName = schoolName;
                contract.ScholarshipId = model.ScholarshipId;
                contract.ScholarshipName = scholarshipName;

                if(AccountInfo.Permission.ContractAdmission)
                {
                    contract.EmployeeId = employeeId;
                    contract.EmployeeName = employeeName;
                }

                contract.ContractDate = !string.IsNullOrWhiteSpace(model.ContractDateString)
                    ? model.ContractDateString.ToDateTime()
                    : DateTime.MinValue;
                contract.VisaDate = !string.IsNullOrWhiteSpace(model.VisaDateString)
                    ? model.VisaDateString.ToDateTime()
                    : DateTime.MinValue;
                contract.RefundDate = !string.IsNullOrWhiteSpace(model.RefundDateString)
                    ? model.RefundDateString.ToDateTime()
                    : DateTime.MinValue;

                if (ContractDb.Instance.Update(contract))
                {
                    if (AccountInfo.Permission.ContractAdmission)
                        UpdateProcessEmployee(contract.CustomerId, employeeId, employeeName);

                    if(model.ScholarshipId > 0)
                        ScholarshipDb.Instance.UpdateRegister(model.ScholarshipId, WebSecurity.CurrentUserId , WebSecurity.CurrentUserName);
                    if(firstScholarshipId > 0)
                        ScholarshipDb.Instance.UpdateRegister(firstScholarshipId, WebSecurity.CurrentUserId, WebSecurity.CurrentUserName);

                    if (changeStatus)
                    {
                        var log = new ContractProcessBo()
                        {
                            ContractId = contract.Id,
                            ContractData = JsonConvert.SerializeObject(logInfo),
                            ProcessNote = processNote,
                            CreateUserId = WebSecurity.CurrentUserId,
                            CreateUserName = WebSecurity.CurrentUserName
                        };
                        ContractProcessDb.Instance.Create(log);
                    }
                    return Success("Cập nhật thông tin hợp đồng thành công");
                }

                #endregion
            }
            else
            {
                // kiểm tra đã có thông tin hợp đồng chưa
                var tmp = ContractDb.Instance.GetByCustomer(model.CustomerId);
                if (tmp != null)
                {
                    return Error("Đã có thông tin hợp đồng");
                }

                #region Thêm mới thông tin hợp đồng

                var contract = new ContractBo
                {
                    CustomerId = model.CustomerId,
                    CompanyId = AccountInfo.CompanyId,
                    SchoolId = model.SchoolId,
                    SchoolName = schoolName,
                    ScholarshipId = model.ScholarshipId,
                    ScholarshipName = scholarshipName,
                    IsVisa = model.IsVisa,
                    Deposit = model.Deposit,
                    ServiceFee = model.ServiceFee,
                    CollectOne = model.CollectOne,
                    CollectTwo = model.CollectTwo,
                    Currency = model.Currency,
                    IsRefund = model.IsRefund,
                    Promotion = model.Promotion,
                    AttachFile = model.AttachFilePath,
                    Note = model.Note,
                    Status = model.Status,
                    TotalCommission = model.TotalCommission,
                    CreateUserId = WebSecurity.CurrentUserId,
                    CreateUserName = WebSecurity.CurrentUserName,
                    EmployeeId = employeeId,
                    EmployeeName = employeeName
                };

                if (!string.IsNullOrWhiteSpace(model.ContractDateString))
                    contract.ContractDate = model.ContractDateString.ToDateTime();
                if (!string.IsNullOrWhiteSpace(model.VisaDateString))
                    contract.VisaDate = model.VisaDateString.ToDateTime();
                if (!string.IsNullOrWhiteSpace(model.RefundDateString))
                    contract.RefundDate = model.RefundDateString.ToDateTime();

                var id = ContractDb.Instance.Create(contract);
                if (id > 0)
                {
                    if (model.ScholarshipId > 0)
                        ScholarshipDb.Instance.UpdateRegister(model.ScholarshipId, WebSecurity.CurrentUserId, WebSecurity.CurrentUserName);

                    UpdateProcessEmployee(model.CustomerId, employeeId, employeeName);
                    return Success("Thêm mới thông tin hợp đồng thành công");
                }

                #endregion
            }
            return Error("Có lỗi xảy ra vui lòng thử lại sau");
        }

        public void InitContractInfo(ContractInsertModel model, ContractBo contract)
        {
            if (contract != null && contract.Id > 0)
            {
                model.Id = contract.Id;
                if (contract.ContractDate.HasValue && contract.ContractDate != DateTime.MinValue)
                {
                    model.ContractDate = contract.ContractDate.Value;
                    model.ContractDateString = contract.ContractDate.Value.ToDateString();
                }
                if (contract.VisaDate.HasValue && contract.VisaDate != DateTime.MinValue)
                {
                    model.VisaDate = contract.VisaDate.Value;
                    model.VisaDateString = contract.VisaDate.Value.ToDateString();
                }
                if (contract.RefundDate.HasValue && contract.RefundDate != DateTime.MinValue)
                {
                    model.RefundDate = contract.RefundDate.Value;
                    model.RefundDateString = contract.RefundDate.Value.ToDateString();
                }
                model.Status = contract.Status;
                model.IsVisa = contract.IsVisa;
                model.Deposit = contract.Deposit;
                model.ServiceFee = contract.ServiceFee;
                model.CollectOne = contract.CollectOne;
                model.CollectTwo = contract.CollectTwo;
                model.Currency = contract.Currency;
                model.IsRefund = contract.IsRefund;
                model.Promotion = contract.Promotion;
                model.AttachFilePath = contract.AttachFile;
                model.Note = contract.Note;
                model.ProcessNote = contract.ProcessNote;
                model.EmployeeId = contract.EmployeeId;
                model.EmployeeName = contract.EmployeeName;
                model.TotalCommission = contract.TotalCommission;
                model.ProfileTypeId = contract.ProfileTypeId;
                model.Percent = contract.PercentProcessing;
                model.SchoolId = contract.SchoolId;
                model.ScholarshipId = contract.ScholarshipId;

                if(contract.SchoolId > 0)
                {
                    var lstScholarship = ScholarshipDb.Instance.GetBySchool(model.SchoolId);
                    if(lstScholarship != null && lstScholarship.Any())
                    {
                        var scholarships = lstScholarship.Select(i => new SelectListItem()
                        {
                            Text = i.ScholarshipName,
                            Value = i.Id.ToString()
                        }).ToList();
                        model.ListScholarship = scholarships;
                    }
                }
            }
            model.ListStatus = ContractStatus.Instant().GetAll();
            model.ListEmployee = AccountInfo.ListTitle;

            // lấy DS admission theo văn phòng
            var office = AccountInfo.IsAdminGroup() ? -1 : AccountInfo.OfficeByTitle;
            var lstAdmission = AccountHelper.GetListAdmission(AccountInfo.CompanyId, office);
            if (lstAdmission != null)
            {
                var employeeWithName = lstAdmission.Select(i => new SelectListItem()
                {
                    Text = (i.UserType == CompanyTitleType.Leader.Key ? "" : "---- ") + i.Code + " (" + i.UserFullName + ")",
                    Value = i.Id.ToString()
                }).ToList();
                employeeWithName.Insert(0, new SelectListItem()
                {
                    Text = "Chưa phân quyền",
                    Value = ""
                });
                model.ListEmployeeWithName = employeeWithName;
            }

            var lstProfileType = ProfileTypeDb.Instance.GetByCompany(AccountInfo.CompanyId);
            if (lstProfileType != null)
            {
                model.ListProfileType = lstProfileType.OrderBy(t => t.CountryId).ToList();
                model.ListProfileType.Insert(0, new ProfileTypeBo()
                {
                    Id = 0,
                    TypeName = "Khác"
                });
            }

            var lstSchool = SchoolDb.Instance.GetAll(AccountInfo.CompanyId);
            if (lstSchool != null)
            {
                var schools = lstSchool.Select(i => new SelectListItem()
                {
                    Text = i.SchoolName,
                    Value = i.Id.ToString()
                }).ToList();
                schools.Insert(0, new SelectListItem()
                {
                    Text = "Chọn trường",
                    Value = "0"
                });
                model.ListSchool = schools;
            }

            if(model.ListScholarship == null || !model.ListScholarship.Any())
            {
                model.ListScholarship = new List<SelectListItem>() {
                    new SelectListItem()
                    {
                        Text = "Chọn học bổng",
                        Value = "0"
                    }
                };
            }
        }

        public void InitInsertUpdateModel(ContractDetailModel model, ContractBo contract, bool loadMoreInfo = false)
        {
            if (model == null)
                model = new ContractDetailModel();
            if (model.ContractInfo == null)
                model.ContractInfo = new ContractInsertModel() { AccountInfo = AccountInfo};
            if (model.CommissionInfo == null)
                model.CommissionInfo = new CommissionInsertModel();
            if (model.DocumentInfo == null)
                model.DocumentInfo = new DocumentModels();

            #region ContractInfo

            if (contract != null)
            {
                model.ContractInfo.Id = contract.Id;
                model.ContractInfo.CustomerId = contract.CustomerId;
                if (contract.ContractDate.HasValue && contract.ContractDate != DateTime.MinValue)
                {
                    model.ContractInfo.ContractDate = contract.ContractDate.Value;
                    model.ContractInfo.ContractDateString = contract.ContractDate.Value.ToDateString();
                }
                if (contract.VisaDate.HasValue && contract.VisaDate != DateTime.MinValue)
                {
                    model.ContractInfo.VisaDate = contract.VisaDate.Value;
                    model.ContractInfo.VisaDateString = contract.VisaDate.Value.ToDateString();
                }
                if (contract.RefundDate.HasValue && contract.RefundDate != DateTime.MinValue)
                {
                    model.ContractInfo.RefundDate = contract.RefundDate.Value;
                    model.ContractInfo.RefundDateString = contract.RefundDate.Value.ToDateString();
                }
                model.ContractInfo.Status = contract.Status;
                model.ContractInfo.IsVisa = contract.IsVisa;
                model.ContractInfo.Deposit = contract.Deposit;
                model.ContractInfo.ServiceFee = contract.ServiceFee;
                model.ContractInfo.CollectOne = contract.CollectOne;
                model.ContractInfo.CollectTwo = contract.CollectTwo;
                model.ContractInfo.Currency = contract.Currency;
                model.ContractInfo.IsRefund = contract.IsRefund;
                model.ContractInfo.Promotion = contract.Promotion;
                model.ContractInfo.AttachFilePath = contract.AttachFile;
                model.ContractInfo.Note = contract.Note;
                model.ContractInfo.ProcessNote = contract.ProcessNote;
                model.ContractInfo.EmployeeId = contract.EmployeeId;
                model.ContractInfo.EmployeeName = contract.EmployeeName;
                model.ContractInfo.ProfileTypeId = contract.ProfileTypeId;
                model.ContractInfo.Percent = contract.PercentProcessing;
                model.ContractInfo.SchoolId = contract.SchoolId;
                model.ContractInfo.ScholarshipId = contract.ScholarshipId;

                if (model.ContractInfo.SchoolId > 0)
                {
                    var lstScholarship = ScholarshipDb.Instance.GetBySchool(model.ContractInfo.SchoolId);
                    if (lstScholarship != null && lstScholarship.Any())
                    {
                        var scholarships = lstScholarship.Select(i => new SelectListItem()
                        {
                            Text = i.ScholarshipName,
                            Value = i.Id.ToString()
                        }).ToList();
                        model.ContractInfo.ListScholarship = scholarships;
                    }
                }
            }

            model.ContractInfo.ListStatus = ContractStatus.Instant().GetAll();

            // lấy danh sách tài khoản admission theo từng văn phòng
            var office = AccountInfo.IsAdminGroup() ? -1 : AccountInfo.OfficeByTitle;
            var lstAdmission = AccountHelper.GetListAdmission(AccountInfo.CompanyId, office);
            if (lstAdmission != null)
            {
                var employeeWithName = lstAdmission.Select(i => new SelectListItem()
                {
                    Text = (i.UserType == CompanyTitleType.Leader.Key ? "" : "---- ") + i.Code + " (" + i.UserFullName + ")",
                    Value = i.Id.ToString()
                }).ToList();
                employeeWithName.Insert(0, new SelectListItem()
                {
                    Text = "Chưa phân quyền",
                    Value = ""
                });
                model.ContractInfo.ListEmployeeWithName = employeeWithName;
            }

            var lstProfileType = ProfileTypeDb.Instance.GetByCompany(AccountInfo.CompanyId);
            if (lstProfileType != null)
            {
                model.ContractInfo.ListProfileType = lstProfileType.OrderBy(t => t.CountryId).ToList();
                model.ContractInfo.ListProfileType.Insert(0, new ProfileTypeBo()
                {
                    Id = 0,
                    TypeName = "Khác"
                });
            }

            var lstSchool = SchoolDb.Instance.GetAll(AccountInfo.CompanyId);
            if (lstSchool != null)
            {
                var schools = lstSchool.Select(i => new SelectListItem()
                {
                    Text = i.SchoolName,
                    Value = i.Id.ToString()
                }).ToList();
                schools.Insert(0, new SelectListItem()
                {
                    Text = "Chọn trường",
                    Value = "0"
                });
                model.ContractInfo.ListSchool = schools;
            }

            if (model.ContractInfo.ListScholarship == null || !model.ContractInfo.ListScholarship.Any())
            {
                model.ContractInfo.ListScholarship = new List<SelectListItem>() {
                    new SelectListItem()
                    {
                        Text = "Chọn học bổng",
                        Value = "0"
                    }
                };
            }

            #endregion

            #region CommissionInfo

            if (contract != null && contract.Id > 0)
            {
                var lstCommission = CommissionDb.Instance.GetByContract(contract.Id);
                model.CommissionInfo.ContractId = contract.Id;
                model.CommissionInfo.ListCommission = lstCommission;
            }

            #endregion

            #region DocumentInfo

            if (contract.ProfileTypeId > 0)
            {
                model.DocumentInfo.ContractId = contract.Id;
                model.DocumentInfo.ProfileTypeId = contract.ProfileTypeId;
                var lstStep = ProfileStepDb.Instance.GetByProfileType(contract.ProfileTypeId);
                var lstDocument = ProfileDocumentDb.Instance.GetByProfileType(contract.ProfileTypeId);
                var lstDetail = ContractDocumentDb.Instance.GetByContractId(contract.Id, contract.ProfileTypeId);
                if (lstDetail == null || lstDetail.Count == 0)
                {
                    ContractDocumentDb.Instance.InitDocument(contract.Id, contract.ProfileTypeId);
                    lstDetail = ContractDocumentDb.Instance.GetByContractId(contract.Id, contract.ProfileTypeId);
                }
                if (lstStep != null && lstStep.Any())
                {
                    model.DocumentInfo.ListStep = new List<ProfileStepModel>();
                    foreach (var step in lstStep)
                    {
                        var tmp = new ProfileStepModel();
                        tmp.StepInfo = step;
                        if (lstDocument != null && lstDocument.Any(d => d.ProfileStepId == step.Id))
                        {
                            var documents = lstDocument.Where(d => d.ProfileStepId == step.Id)
                                .OrderBy(d => d.DocumentGroupId)
                                .ThenBy(d => d.DisplayOrder);
                            
                            tmp.ListDocumentDetail = new List<ProfileDocumentDetail>();
                            foreach (var doc in documents)
                            {
                                var detail = lstDetail.FirstOrDefault(d => d.ProfileDocumentId == doc.Id);
                                tmp.ListDocumentDetail.Add(new ProfileDocumentDetail()
                                {
                                    Id = detail != null ? detail.Id : 0,
                                    StepId = doc.ProfileStepId,
                                    DocumentId = doc.Id,
                                    DocumentName = doc.DocumentName,
                                    Note = detail != null ? detail.Note : "",
                                    IsDone = detail != null ? detail.IsDone : false,
                                    IsSkip = detail != null ? detail.IsSkip : false,
                                    UpdateDate = detail != null ? detail.UpdateDate : DateTime.MinValue
                                });
                            }
                        }
                        model.DocumentInfo.ListStep.Add(tmp);
                    }
                }
            }

            #endregion

            model.ContractProcessInfo = ContractProcessDb.Instance.GetByContract(contract.Id);
        }

        /// <summary>
        /// Kiểm tra một số thông tin nếu có nhập như ngày ký hợp đồng,...
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Extension.JsonResult ValidateInsertUpdate(ContractInsertModel model)
        {
            var err = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(model.ContractDateString) && !model.ContractDateString.IsValidDate())
            {
                err.Add("ContractDateString", "Ngày ký hợp đồng không hợp lệ");
            }
            if (!string.IsNullOrWhiteSpace(model.VisaDateString) && !model.VisaDateString.IsValidDate())
            {
                err.Add("VisaDateString", "Ngày làm visa không hợp lệ");
            }
            if (!string.IsNullOrWhiteSpace(model.RefundDateString) && !model.RefundDateString.IsValidDate())
            {
                err.Add("RefundDateString", "Ngày hoàn tiền cọc không hợp lệ");
            }
            if (err.Any())
            {
                return err.ToJsonResult();
            }
            return null;
        }

        [HttpPost]
        public Extension.JsonResult UploadFile()
        {
            var length = Request.ContentLength;
            var bytes = new byte[length];
            Request.InputStream.Read(bytes, 0, length);
            // bytes has byte content here. what do do next?

            var fileName = Request.Headers["X-File-Name"];
            var fileSize = Request.Headers["X-File-Size"];
            var contractId = Convert.ToInt32(Request.Headers["X-Contract-Id"]);

            var contract = ContractDb.Instance.Read(contractId);
            if (contract == null)
            {
                return Error("Không tìm thấy thông tin hợp đồng");
            }

            string uploadPath = string.Concat(ConfigMgr.UploadFolder, "Contract", "/", contractId.ToString(), "/");
            string absolutePath = Server.MapPath(string.Concat("~", uploadPath));

            if (!Directory.Exists(absolutePath))
            {
                Directory.CreateDirectory(absolutePath);
            }

            fileName = fileName.ToUnsignedVietnamese().Replace("  ", " ").Replace(" ", "-");

            var fullPath = absolutePath + fileName;
            var ext = Path.GetExtension(fullPath);
            var uploadAllow = ConfigMgr.ValidFileUpload;
            if (!uploadAllow.Contains(ext))
            {
                return Error("File upload không được hỗ trợ");
            }

            // save the file.
            var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.ReadWrite);
            fileStream.Write(bytes, 0, length);
            fileStream.Close();

            var path = "/Upload/Contract/" + contractId + "/" + fileName;
            contract.AttachFile = path;
            ContractDb.Instance.Update(contract);

            return Success(path);
        }

        #endregion

        #region CommissionInfo

        [HttpPost]
        [ValidateModel]
        [Permissions("Customer.CommissionInfo.Post")]
        public ActionResult CommissionInfo(CommissionInsertModel model)
        {
            var err = ValidateInsertUpdateCommission(model);
            if (err != null)
                return err;

            var contract = ContractDb.Instance.Read(model.ContractId);
            if (contract == null)
                return Error("Thông tin hợp đồng không hợp lệ");

            if (model.Id > 0)
            {
                #region Cập nhật thông tin commission

                var info = CommissionDb.Instance.Read(model.Id);
                if (info != null)
                {
                    info.Name = model.Name;
                    info.Commission = model.Commission;
                    info.CommissionDate = model.CommissionDateString.ToDateTime();
                    info.IsCollect = model.IsCollect;
                    info.Note = model.Note;
                    info.UpdateUserId = WebSecurity.CurrentUserId;
                    info.UpdateUserName = WebSecurity.CurrentUserName;

                    if (CommissionDb.Instance.Update(info))
                    {
                        var lstCommission = CommissionDb.Instance.GetByContract(model.ContractId);
                        var html = RenderPartialViewToString("_ListCommission", lstCommission);
                        return Success("Cập nhật thông tin commission thành công", html);
                    }
                }

                #endregion
            }
            else
            {
                #region Thêm mới thông tin dự định du học

                var info = new CommissionBo()
                {
                    CompanyId = AccountInfo.CompanyId,
                    CustomerId = contract.CustomerId,
                    ContractId = contract.Id,
                    Name = model.Name,
                    Commission = model.Commission,
                    CommissionDate = model.CommissionDateString.ToDateTime(),
                    IsCollect = model.IsCollect,
                    Note = model.Note,
                    CreateUserId = WebSecurity.CurrentUserId,
                    CreateUserName = WebSecurity.CurrentUserName
                };

                var id = CommissionDb.Instance.Create(info);
                if (id > 0)
                {
                    var lstCommission = CommissionDb.Instance.GetByContract(model.ContractId);

                    var html = RenderPartialViewToString("_ListCommission", lstCommission);
                    return Success("Thêm mới thông tin commission thành công", html);
                }

                #endregion
            }
            return Error("Có lỗi xảy ra vui lòng thử lại sau");
        }

        public ActionResult CommissionInfo(int id)
        {
            if (id <= 0)
                return Error("Có lỗi xảy ra vui lòng thử lại sau");

            var info = CommissionDb.Instance.Read(id);
            if (info == null)
                return Error("Không thể tìm thấy thông tin commission");

            // lấy thông tin và kiểm tra phân quyền theo customer
            var customer = CustomerDb.Instance.Read(info.CustomerId);
            if (customer == null || !AccountHelper.CheckCustomerPermission(AccountInfo, info.CustomerId))
                return Error("Bạn không có quyền cập nhật thông tin này");

            if (info.CommissionDate.HasValue && info.CommissionDate.Value != DateTime.MinValue)
                info.CommissionDateString = info.CommissionDate.Value.ToDateString();

            return new Extension.JsonResult(HttpStatusCode.OK, info);
        }

        public Extension.JsonResult ValidateInsertUpdateCommission(CommissionInsertModel model)
        {
            var err = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(model.CommissionDateString) && !model.CommissionDateString.IsValidDate())
            {
                err.Add("CommissionDateString", "Ngày thu tiền không hợp lệ");
            }
            if (err.Any())
            {
                return err.ToJsonResult();
            }
            return null;
        }

        #endregion

        #region DocumentInfo

        public ActionResult LoadContractDocument(int id)
        {
            var model = new DocumentModels();
            var contract = ContractDb.Instance.Read(id);
            if (contract == null)
                return Error("Thông tin hợp đồng không hợp lệ");

            if (contract.ProfileTypeId == 0)
                return Error("Chưa chọn loại hồ sơ");

            var totalDone = 0;
            var totalDoc = 0;
            model.ContractId = contract.Id;
            model.ProfileTypeId = contract.ProfileTypeId;
            var lstStep = ProfileStepDb.Instance.GetByProfileType(contract.ProfileTypeId);
            var lstDocument = ProfileDocumentDb.Instance.GetByProfileType(contract.ProfileTypeId);
            var lstDetail = ContractDocumentDb.Instance.GetByContractId(contract.Id, contract.ProfileTypeId);
            if (lstDetail == null || lstDetail.Count == 0)
            {
                ContractDocumentDb.Instance.InitDocument(contract.Id, contract.ProfileTypeId);
                lstDetail = ContractDocumentDb.Instance.GetByContractId(contract.Id, contract.ProfileTypeId);
            }
            if (lstDetail != null)
            {
                totalDone = lstDetail.Count(d => d.IsDone);
            }
            if (lstStep != null && lstStep.Any())
            {
                model.ListStep = new List<ProfileStepModel>();
                foreach (var step in lstStep)
                {
                    var tmp = new ProfileStepModel();
                    tmp.StepInfo = step;
                    if (lstDocument != null && lstDocument.Any(d => d.ProfileStepId == step.Id))
                    {
                        totalDoc = lstDocument.Count;
                        var documents = lstDocument.Where(d => d.ProfileStepId == step.Id)
                            .OrderBy(d => d.DocumentGroupId)
                            .ThenBy(d => d.DisplayOrder);
                        
                        tmp.ListDocumentDetail = new List<ProfileDocumentDetail>();
                        foreach (var doc in documents)
                        {
                            var detail = lstDetail.FirstOrDefault(d => d.ProfileDocumentId == doc.Id);
                            tmp.ListDocumentDetail.Add(new ProfileDocumentDetail()
                            {
                                Id = detail != null ? detail.Id : 0,
                                StepId = doc.ProfileStepId,
                                DocumentId = doc.Id,
                                DocumentName = doc.DocumentName,
                                Note = detail != null ? detail.Note : "",
                                IsDone = detail != null ? detail.IsDone : false,
                                IsSkip = detail != null ? detail.IsSkip : false,
                                UpdateDate = detail != null ? detail.UpdateDate : DateTime.MinValue
                            });
                        }
                    }
                    model.ListStep.Add(tmp);
                }
            }

            var html = RenderPartialViewToString("_ContractDocument", model);
            return Success("Success", html);
        }

        public ActionResult LoadContractDocumentByCustomer(int id)
        {
            var model = new DocumentModels();

            var contract = ContractDb.Instance.GetByCustomer(id);
            if (contract == null)
                return Error("Thông tin hợp đồng không hợp lệ");

            if (contract.ProfileTypeId == 0)
                return Error("Chưa chọn loại hồ sơ");

            var totalDone = 0;
            var totalDoc = 0;
            model.ContractId = contract.Id;
            model.ProfileTypeId = contract.ProfileTypeId;
            var lstStep = ProfileStepDb.Instance.GetByProfileType(contract.ProfileTypeId);
            var lstDocument = ProfileDocumentDb.Instance.GetByProfileType(contract.ProfileTypeId);
            var lstDetail = ContractDocumentDb.Instance.GetByContractId(contract.Id, contract.ProfileTypeId);
            if (lstDetail == null || lstDetail.Count == 0)
            {
                ContractDocumentDb.Instance.InitDocument(contract.Id, contract.ProfileTypeId);
                lstDetail = ContractDocumentDb.Instance.GetByContractId(contract.Id, contract.ProfileTypeId);
            }
            if (lstDetail != null)
            {
                totalDone = lstDetail.Count(d => d.IsDone);
            }
            if (lstStep != null && lstStep.Any())
            {
                model.ListStep = new List<ProfileStepModel>();
                foreach (var step in lstStep)
                {
                    var tmp = new ProfileStepModel();
                    if (lstDocument != null && lstDocument.Any(d => d.ProfileStepId == step.Id))
                    {
                        totalDoc = lstDocument.Count;
                        var documents = lstDocument.Where(d => d.ProfileStepId == step.Id)
                            .OrderBy(d => d.DocumentGroupId)
                            .ThenBy(d => d.DisplayOrder);
                        tmp.StepInfo = step;
                        tmp.ListDocumentDetail = new List<ProfileDocumentDetail>();
                        foreach (var doc in documents)
                        {
                            var detail = lstDetail.FirstOrDefault(d => d.ProfileDocumentId == doc.Id);
                            tmp.ListDocumentDetail.Add(new ProfileDocumentDetail()
                            {
                                Id = detail != null ? detail.Id : 0,
                                StepId = doc.ProfileStepId,
                                DocumentId = doc.Id,
                                DocumentName = doc.DocumentName,
                                Note = detail != null ? detail.Note : "",
                                IsDone = detail != null ? detail.IsDone : false,
                                IsSkip = detail != null ? detail.IsSkip : false,
                                UpdateDate = detail.UpdateDate
                            });
                        }
                    }
                    model.ListStep.Add(tmp);
                }
            }

            var html = RenderPartialViewToString("_ContractDocument", model);
            return Success("Success", html);
        }

        #endregion

        #region Function

        /// <summary>
        /// Cập nhật thông tin nhân viên xử lý hồ sơ vào khách hàng
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="employeeId"></param>
        /// <param name="employerrName"></param>
        /// <returns></returns>
        private bool UpdateProcessEmployee(int customerId, int employeeId, string employerrName)
        {
            if (customerId <= 0 || employeeId <= 0 || string.IsNullOrEmpty(employerrName))
                return false;

            var customer = CustomerDb.Instance.Read(customerId);
            if (customer == null)
                return false;

            customer.EmployeeProcessId = employeeId;
            customer.EmployeeProcessName = employerrName;

            if (CustomerDb.Instance.Update(customer))
                return true;

            return false;
        }

        private bool UploadAttachFile(int id, HttpPostedFileBase file, ref string uploadFileLink)
        {
            if (file == null)
            {
                return false;
            }
            string fileLink = string.Empty;
            string strFileName = file.FileName.Replace(" ", "");
            string strFolderName = "AttachFile";
            string strUploadPath = string.Concat(ConfigMgr.UploadFolder, strFolderName, "/", id.ToString(), "/");
            string strAbsolutePath = Server.MapPath(string.Concat("~", strUploadPath));
            fileLink = strUploadPath + strFileName;

            ViewHelper.UploadFile(strAbsolutePath, strFileName, file);

            uploadFileLink = fileLink;
            return true;
        }

        #endregion

        public ActionResult CheckCollect(int id, bool collect)
        {
            var comm = CommissionDb.Instance.Read(id);
            if (comm == null)
                return Error("Không tìm thấy thông tin");

            comm.IsCollect = collect;
            if (CommissionDb.Instance.Update(comm))
            {
                return Success("Cập nhật thông tin thành công");
            }

            return Success();
        }

        public ActionResult CompleteDocument(int id, bool done)
        {
            var doc = ContractDocumentDb.Instance.Read(id);
            if (doc == null)
                return Error("Không tìm thấy thông tin");

            if (AccountInfo.TitleType == CompanyTitleType.Agency.Key)
                return Error("Bạn không có quyền cập nhật thông tin này");

            doc.IsDone = done;
            if (done)
                doc.IsSkip = false;
            doc.UpdateUserId = AccountInfo.UserId;
            doc.UpdateUserName = AccountInfo.UserName;
            if (ContractDocumentDb.Instance.Update(doc))
            {
                // cập nhật tỉ lệ % xử lý vào hợp đồng
                int totalDone = 0, totalDoc = 0;
                var contract = ContractDb.Instance.Read(doc.ContractId);
                var lstDetail = ContractDocumentDb.Instance.GetByContractId(contract.Id, contract.ProfileTypeId);
                if (lstDetail != null)
                {
                    totalDone = lstDetail.Count(d => d.IsDone && !d.IsSkip);
                    totalDoc = lstDetail.Count(d => !d.IsSkip);
                }
                if(totalDoc > 0)
                    contract.PercentProcessing = totalDone * 100 / totalDoc;
                ContractDb.Instance.UpdatePercentProcessing(contract);

                return Success("Cập nhật thông tin thành công");
            }

            return Success();
        }

        public ActionResult SkipDocument(int id, bool skip)
        {
            var doc = ContractDocumentDb.Instance.Read(id);
            if (doc == null)
                return Error("Không tìm thấy thông tin");

            if(AccountInfo.TitleType == CompanyTitleType.Agency.Key)
                return Error("Bạn không có quyền cập nhật thông tin này");

            doc.IsSkip = skip;
            if (skip)
                doc.IsDone = false;
            doc.UpdateUserId = AccountInfo.UserId;
            doc.UpdateUserName = AccountInfo.UserName;
            if (ContractDocumentDb.Instance.Update(doc))
            {
                // cập nhật tỉ lệ % xử lý vào hợp đồng
                int totalDone = 0, totalDoc = 0;
                var contract = ContractDb.Instance.Read(doc.ContractId);
                var lstDetail = ContractDocumentDb.Instance.GetByContractId(contract.Id, contract.ProfileTypeId);
                if (lstDetail != null)
                {
                    totalDone = lstDetail.Count(d => d.IsDone && !d.IsSkip);
                    totalDoc = lstDetail.Count(d => !d.IsSkip);
                }
                if (totalDoc > 0)
                    contract.PercentProcessing = totalDone * 100 / totalDoc;
                ContractDb.Instance.UpdatePercentProcessing(contract);

                return Success("Cập nhật thông tin thành công");
            }

            return Success();
        }

        [HttpPost]
        [Permissions("Contract.Delete")]
        public ActionResult DeleteContractById(int id, int customerId)
        {
            if (id > 0)
            {
                if (!AccountHelper.CheckCustomerPermission(AccountInfo, customerId))
                    return Error();

                if (ContractDb.Instance.Delete(id.ToString(), AccountInfo.CompanyId, WebSecurity.CurrentUserId, WebSecurity.CurrentUserName))
                {
                    return Success();
                }
            }
            return Error();
        }

        [HttpPost]
        [Permissions("Contract.Delete")]
        public ActionResult DeleteContract(string ids)
        {
            if (!string.IsNullOrEmpty(ids) && 
                (AccountInfo.TitleType == CompanyTitleType.Leader.Key || AccountInfo.TitleType == CompanyTitleType.Admin.Key || AccountInfo.TitleType == CompanyTitleType.Director.Key))
            {
                LogHelper.Warning("DeleteContract", "IDs:" + ids);
                var lstId = ids.SplitToIntList();
                foreach (var id in lstId)
                {
                    ContractDb.Instance.Delete(id.ToString(), AccountInfo.CompanyId, WebSecurity.CurrentUserId, WebSecurity.CurrentUserName);
                }
                return Success();
            }
            return Error();
        }

        #region Report

        public ActionResult ContractSummaryReportMonthly(int fromMonth, int toMonth, int fromYear, int toYear)
        {
            var cacheKey = Cacher.CreateCacheKey(fromMonth, toMonth, fromYear, toYear);
            var result = Cacher.Get<LineChartModel>(cacheKey);
            if (result != null)
                return new Extension.JsonResult(HttpStatusCode.OK, result);

            var fromDate = new DateTime(fromYear, fromMonth, 1);
            var toDate = new DateTime(toYear, toMonth, 1);
            var data = ContractDb.Instance.GetSummaryReportMonthly(fromDate, toDate, AccountInfo.CompanyId);
            if (data == null || data.Count == 0)
                return new Extension.JsonResult(HttpStatusCode.NotFound);

            var model = new LineChartModel();
            model.Categories = string.Join(",", data.Select(i => i.Date.ToString("MM/yyyy")).ToArray());
            model.Series = new Series()
            {
                Items = new List<Item>()
            };
            model.Series.Items.Add(new Item()
            {
                Name = "Tổng số khách hàng",
                Data = string.Join(",", data.Select(i => i.Total).ToArray())
            });
            model.Series.Items.Add(new Item()
            {
                Name = "Đã ký hợp đồng",
                Data = string.Join(",", data.Select(i => i.TotalContract).ToArray())
            });
            model.Series.Items.Add(new Item()
            {
                Name = "Chưa ký hợp đồng",
                Data = string.Join(",", data.Select(i => i.TotalNotContract).ToArray())
            });
            model.Total = data.Sum(i => i.Total);
            model.TotalContract = data.Sum(i => i.TotalContract);

            Cacher.Add(cacheKey, model, DateTime.Now.AddMinutes(10));

            return new Extension.JsonResult(HttpStatusCode.OK, model);
        }

        #endregion
    }
}