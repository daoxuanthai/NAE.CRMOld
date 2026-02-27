using System.Net.Http;
using System.Web.Http;
using tcs.adapter.Sql;
using tcs.bo;
using tcs.lib;
using System.Web.Http.Cors;
using tcs.api.Models;
using System.Linq;

namespace tcs.api.Controllers
{
    [EnableCors(origins: "http://trienlamhocbong.com,http://namanhenglish.edu.vn,http://hocbongnamanh.edu.vn", headers: "*", methods: "*")]
    public class CustomerController : BaseApiController
    {
        [HttpPost]
        public HttpResponseMessage Post([FromBody]Customer customer)
        {
            if (customer == null)
                return APIResponseMessage.BadRequest();

            try
            {

                var data = customer.Name + ";" + customer.Phone + ";" + customer.Email + ";" + customer.Question + ";Province:" + customer.Province + ";Country:" + customer.Country + "" + customer.Url;
                LogHelper.WriteLog("", data);

                if (string.IsNullOrWhiteSpace(customer.Name) || string.IsNullOrWhiteSpace(customer.Phone))
                    return APIResponseMessage.BadRequest();

                // kiểm tra id partner và key trước khi cho đăng ký

                // kiểm tra trùng số điện thoại
                var checkCustomer = CustomerDb.Instance.GetByPhone(customer.Phone, customer.PartnerId);
                if(checkCustomer != null && checkCustomer.Status != CustomerStatus.Contracted.Key)
                {
                    checkCustomer.Status = CustomerStatus.NotContact.Key;
                    CustomerDb.Instance.Update(checkCustomer);

                    var history = new RegisterHistoryBo()
                    {
                        CustomerId = checkCustomer.Id,
                        CompanyId = customer.PartnerId,
                        Email = customer.Email,
                        Phone = customer.Phone,
                        FullName = customer.Name,
                        RegisterLink = customer.Url,
                        AdvisoryContent = customer.Question,
                        IsParent = customer.IsParent,
                        IsCallInfo = customer.IsCallInfo,
                        IsContact = customer.IsContact
                    };
                    RegisterHistoryDb.Instance.Create(history);

                    return APIResponseMessage.Success();
                }

                // kiểm tra trùng email
                if(!string.IsNullOrEmpty(customer.Email))
                {
                    var checkCustomerEmail = CustomerDb.Instance.GetByEmail(customer.Email, customer.PartnerId);
                    if (checkCustomerEmail != null && checkCustomerEmail.Status != CustomerStatus.Contracted.Key)
                    {
                        checkCustomerEmail.Status = CustomerStatus.NotContact.Key;
                        CustomerDb.Instance.Update(checkCustomerEmail);

                        var history = new RegisterHistoryBo()
                        {
                            CustomerId = checkCustomerEmail.Id,
                            CompanyId = customer.PartnerId,
                            Email = customer.Email,
                            Phone = customer.Phone,
                            FullName = customer.Name,
                            RegisterLink = customer.Url,
                            AdvisoryContent = customer.Question,
                            IsParent = customer.IsParent,
                            IsCallInfo = customer.IsCallInfo,
                            IsContact = customer.IsContact
                        };
                        RegisterHistoryDb.Instance.Create(history);

                        return APIResponseMessage.Success();
                    }
                }

                var employee = 0;
                var employeeName = string.Empty;
                var companyId = 0;

                // phân quyền theo tỉnh thành config theo mỗi công ty
                var lstCompanyProvince = ProvinceCompanyDb.Instance.GetByCompany(customer.PartnerId);
                if (lstCompanyProvince != null && lstCompanyProvince.Any(i => i.ProvinceId == customer.Province))
                {
                    var tmp = lstCompanyProvince.FirstOrDefault(i => i.ProvinceId == customer.Province);
                    var office = CompanyOfficeDb.Instance.Read(tmp.OfficeId);
                    if(office != null && office.DirectorUserId > 0)
                    {
                        var companyTitle = CompanyTitleDb.Instance.GetByCompany(customer.PartnerId);
                        if(companyTitle != null && companyTitle.Any(i=>i.Id == office.DirectorUserId))
                        {
                            employee = companyTitle.FirstOrDefault(i => i.Id == office.DirectorUserId).Id;
                            employeeName = companyTitle.FirstOrDefault(i => i.Id == office.DirectorUserId).Code;
                        }
                    }
                    companyId = tmp.CompanyId;
                }

                var fullName = customer.IsParent ? "PH: " + customer.Name : customer.Name;

                var obj = new CustomerBo()
                {
                    CompanyId = customer.PartnerId <= 0 ? ConfigMgr.DefaultCompany : customer.PartnerId,
                    Fullname = customer.Name,
                    Phone = customer.Phone,
                    Email = customer.Email,
                    ProvinceId = customer.Province,
                    CountryId = customer.Country.ToString(),
                    CustomerNote = customer.Question,
                    Source = CustomerSource.Website.Key,
                    NewsUrlRef = customer.Url,
                    EmployeeId = employee,
                    EmployeeName = employeeName,
                    CreateUserId = 0,
                    CreateUserName = "administrator"
                };

                var cId = CustomerDb.Instance.Create(obj);
                if (cId > 0)
                {
                    if (customer.Country > 0)
                    {
                        var lstCountry = CountryDb.Instance.GetListCountry();
                        var countryName = string.Empty;
                        if (lstCountry != null && lstCountry.Any(c => c.Id == customer.Country))
                        {
                            countryName = lstCountry.FirstOrDefault(c => c.Id == customer.Country).CountryName;
                        }
                        var study = new StudyAbroadBo()
                        {
                            CompanyId = customer.PartnerId <= 0 ? ConfigMgr.DefaultCompany : customer.PartnerId,
                            CustomerId = cId,
                            CountryId = customer.Country,
                            CountryName = countryName,
                            CreateUserId = 0,
                            CreateUserName = "administrator"
                        };
                        StudyAbroadDb.Instance.Create(study);
                    }
                    /// ghi log dang ky 
                    var history = new RegisterHistoryBo()
                    {
                        CustomerId = cId,
                        CompanyId = companyId,
                        Email = customer.Email,
                        Phone = customer.Phone,
                        FullName = customer.Name,
                        RegisterLink = customer.Url,
                        AdvisoryContent = customer.Question,
                        IsParent = customer.IsParent
                    };
                    RegisterHistoryDb.Instance.Create(history);
                    return APIResponseMessage.Success();
                }
            }
            catch (System.Exception ex)
            {
                LogHelper.WriteLog(ex.Message, ex);
            }

            return APIResponseMessage.BadRequest();
        }
    }
}
