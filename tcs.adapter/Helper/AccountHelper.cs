using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using tcs.adapter.Sql;
using tcs.bo;
using tcs.lib;

namespace tcs.adapter.Helper
{
    public class AccountHelper
    {
        public static AccountInfo GetAccountInfo(int userId, string userName)
        {
            var lstCompany = CompanyDb.Instance.GetByUser(userId);
            if (lstCompany == null || !lstCompany.Any())
            {
                return null;
            }

            var company = lstCompany.FirstOrDefault().Id;
            var companySetting = ConfigMgr.Get<int>("CompanyId");
            if (companySetting > 0)
            {
                company = companySetting;
            }

            var lstCompanyTitle = CompanyTitleDb.Instance.GetByUser(userId);
            if (lstCompanyTitle == null || !lstCompanyTitle.Any())
            {
                LogHelper.Error("AccountHelper.GetAccountInfo", "company title not valid");
                return null;
            }

            var tmp = lstCompanyTitle.FirstOrDefault(c => c.CompanyId == company);

            var lstRole = AccountHelper.GetRoles(userName);
            var titleId = tmp.Id;
            var titleType = tmp.UserType;
            var officeByTitle = tmp.OfficeId;
            var listOfficeByTitle = lstCompanyTitle.Where(c => c.CompanyId == company).Select(i => i.OfficeId).Distinct().ToArray();

            if (lstCompanyTitle.Any(i => i.UserType == CompanyTitleType.Director.Key))
            {
                var allOffice = CompanyOfficeDb.Instance.GetAll(company);
                listOfficeByTitle = allOffice.Select(i => i.Id).ToArray();
            }

            var accountInfo = new AccountInfo()
            {
                UserId = userId,
                UserName = userName,
                CompanyId = company,
                ListCompany = lstCompany,
                ListCompanyTitle = lstCompanyTitle,
                ListTitle = AccountHelper.GetTitle(userName, company, listOfficeByTitle, titleType),
                OfficeByTitle = officeByTitle,
                ListOfficeByTitle = listOfficeByTitle,
                TitleId = titleId,
                TitleCode = tmp.Code,
                TitleType = titleType,
                IsViewAll = tmp.IsViewAll,
                ListRole = lstRole,
                IsSysAdmin = lstRole.Contains("Sys_Admin"),
                Permission = AccountHelper.GetPermission(userName, company, officeByTitle, titleType)
            };
            return accountInfo;
        }

        /// <summary>
        /// Lấy thông tin danh sách TVP và TV theo user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="company"></param>
        /// <param name="office">OfficeId theo title</param>
        /// <param name="titleType">Loại tài khoản theo công ty</param>
        /// <returns></returns>
        public static List<CompanyTitleBo> GetTitle(string userName, int company, int[] office, int titleType)
        {
            var lstTitle = CompanyTitleDb.Instance.GetByCompany(company);
            var specialRoles = GetRolesViewAllOffice();
            var allRoles = GetRoles(userName);

            // nếu nằm trong role được quyền xem tất cả tài khoản thuộc mọi văn phòng
            if (titleType == CompanyTitleType.Director.Key || titleType == CompanyTitleType.Admin.Key ||
                titleType == CompanyTitleType.Marketing.Key || titleType == CompanyTitleType.Accountant.Key)
            {
                lstTitle = lstTitle.Where(i =>
                    i.OfficeId > 0 && (i.UserType == CompanyTitleType.Leader.Key || i.UserType == CompanyTitleType.Counselor.Key
                    || i.UserType == CompanyTitleType.Admission.Key)).ToList();
                return lstTitle;
            }

            // nếu là tài khoản trưởng văn phòng thì được quyền xem tất cả tư vấn thuộc văn phòng và các trưởng VP khác
            if (titleType == CompanyTitleType.Leader.Key)
            {
                lstTitle = lstTitle.Where(i => (office.Contains(i.OfficeId) && (i.UserType == CompanyTitleType.Counselor.Key || i.UserType == CompanyTitleType.Admission.Key)) ||
                                               i.UserType == CompanyTitleType.Leader.Key).ToList();
                return lstTitle;
            }

            // nếu là tài khoản bình thường thì chỉ được phép xem chính tài khoản đó
            lstTitle = lstTitle.Where(i => i.UserName.ToLower().Equals(userName.ToLower())).ToList();

            lstTitle = lstTitle.OrderBy(i => i.OfficeId).ToList();

            return lstTitle;
        }

        /// <summary>
        /// Lấy phân quyền theo công ty, văn phòng và theo user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="company"></param>
        /// <param name="office"></param>
        /// <param name="titleType"></param>
        /// <returns></returns>
        public static AccountPermission GetPermission(string userName, int company, int office, int titleType)
        {
            var ret = new AccountPermission();

            #region Customer

            #endregion

            #region Contract

            ret.ContractAdmission = titleType == CompanyTitleType.Director.Key || titleType == CompanyTitleType.Admin.Key || 
                company != 1005 || (company == 1005 && titleType == CompanyTitleType.Leader.Key);

            #endregion

            return ret;
        }

        public static List<CompanyTitleBo> GetListEmployee(AccountInfo info)
        {
            return GetTitle(info.UserName, info.CompanyId, info.ListOfficeByTitle, info.TitleType);
        }

        /// <summary>
        /// Lấy thông tin danh sách tài khoản admission theo văn phòng
        /// </summary>
        /// <param name="company"></param>
        /// <param name="office"></param>
        /// <returns></returns>
        public static List<CompanyTitleBo> GetListAdmission(int company, int office = -1)
        {
            var allTitle = CompanyTitleDb.Instance.GetByCompany(company);
            if (allTitle == null || !allTitle.Any())
                return null;

            var lstTitle = allTitle.Where(t => (office == -1 || t.OfficeId == office) && t.UserType == CompanyTitleType.Admission.Key);
            return lstTitle != null && lstTitle.Any() ? lstTitle.ToList() : null;
        }

        /// <summary>
        /// Lưu thông tin phân quyền khi khởi tạo application
        /// </summary>
        public static void InitFuntionPermission()
        {
            //TODO: lưu thông tin danh sách công ty, danh sách vị trí của mỗi công ty
        }

        /// <summary>
        /// Kiểm tra xem user có được phân quyền trên chức năng này hay không
        /// </summary>
        /// <param name="acc">Thông tin tài khoản đang đăng nhập và công ty đang chọn</param>
        /// <param name="function">Tên chức năng cần kiểm tra phân quyền</param>
        /// <returns></returns>
        public static bool CheckPermission(AccountInfo acc, string function)
        {
            // TODO: kiểm tra phân quyền theo chức năng
            return true;
        }

        /// <summary>
        /// Kiểm tra phân quyền đối với id khách hàng đang cập nhật
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="id"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool CheckCustomerPermission(AccountInfo acc, int id, int action = -1)
        {
            if (id == 0)
                return true;

            #region Một số tài khoản có full quyền

            if (acc.IsAdminGroup())
                return true;

            #endregion
            
            var customer = CustomerDb.Instance.Read(id);
            if (customer == null)
                return false;

            if (acc.TitleType == CompanyTitleType.Agency.Key && action == UserAction.Delete.Key)
            {
                return customer.CreateUserId == acc.TitleId;
            }

            // kiểm tra tài khoản đại lý
            if (acc.TitleType == CompanyTitleType.Agency.Key && customer.AgencyId == acc.TitleId)
                return true;

            // kiểm tra companyId
            if (customer.CompanyId != acc.CompanyId)
                return false;

            // kiểm tra theo quyền trưởng văn phòng
            if (acc.TitleType == CompanyTitleType.Leader.Key && 
                acc.ListTitle.Any(t => t.Id == customer.EmployeeId))
                return true;

            // kiểm tra theo user được phân quyền
            if (acc.ListCompanyTitle.Any(t=>t.Id == customer.EmployeeId))
                return true;

            // nếu là nhân viên admission thì cũng có quyền xem
            if (customer.EmployeeProcessId == acc.TitleId)
                return true;

            return false;
        }
        
        /// <summary>
        /// Kiểm tra phân quyền theo công ty
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public static bool CheckCompanyPermission(AccountInfo acc, int companyId)
        {
            if (acc.CompanyId != companyId)
                return false;

            return true;
        }

        /// <summary>
        /// Lấy tất cả quyền của user
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static string[] GetRoles(string userName)
        {
            return Roles.GetRolesForUser(userName);
        }

        /// <summary>
        /// Lấy tất cả role được phép xem toàn bộ dữ liệu thuộc các văn phòng của công ty
        /// </summary>
        /// <returns></returns>
        public static string[] GetRolesViewAllOffice()
        {
            return ConfigMgr.Get<string[]>("RolesViewAllOffice");
        }

        /// <summary>
        /// Có quyền phân quyền cho user khác
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static bool AllowAddPermission(AccountInfo acc)
        {
            return acc.TitleType == CompanyTitleType.Director.Key ||
                acc.TitleType == CompanyTitleType.Leader.Key ||
                acc.TitleType == CompanyTitleType.Admin.Key ||
                acc.TitleType == CompanyTitleType.Marketing.Key;
        }
        
        public static bool AllowContractManager(AccountInfo acc)
        {
            return acc.TitleType == CompanyTitleType.Director.Key ||
                acc.TitleType == CompanyTitleType.Leader.Key ||
                acc.TitleType == CompanyTitleType.Admin.Key ||
                acc.TitleType == CompanyTitleType.Marketing.Key ||
                acc.TitleType == CompanyTitleType.Admission.Key;
        }

        public static bool AllowImportCustomer(AccountInfo acc)
        {
            return acc.TitleType == CompanyTitleType.Director.Key ||
                acc.TitleType == CompanyTitleType.Leader.Key ||
                acc.TitleType == CompanyTitleType.Marketing.Key;
        }

        public static void CacheAllUserInRoles(Dictionary<string, string[]> dic)
        {
            var key = DataKey.SystemModule.Account.ALL_USER_IN_ROLES;
            if (dic != null)
                Cacher.Add(key, dic);
        }

        public static Dictionary<string, string[]> GetAllUserInRoles()
        {
            var key = DataKey.SystemModule.Account.ALL_USER_IN_ROLES;
            return Cacher.Get<Dictionary<string, string[]>>(key);
        }

        public static List<SelectListItem> GetSelectListUser(bool useCache = true)
        {
            var query = new UserQuery() { PageSize = 1000 };
            var lstUser = UserDb.Instance.Select(query, useCache);
            if (lstUser == null || !lstUser.Any())
                return new List<SelectListItem>();

            var ret = lstUser.Select(x =>
                                  new SelectListItem()
                                  {
                                      Text = x.UserName + " - " + x.FullName,
                                      Value = x.Id.ToString()
                                  }).ToList();

            ret.Insert(0, new SelectListItem() {
                Text = "Không phân quyền",
                Value = "-1"
            });

            return ret;
        }

        /// <summary>
        /// Role tương ứng với từng loại company title
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetRoleByTitleType(int type)
        {
            if (type == CompanyTitleType.Director.Key)
                return CompanyRoles.Director.Value;
            if (type == CompanyTitleType.Accountant.Key)
                return CompanyRoles.Accountant.Value;
            if (type == CompanyTitleType.Admin.Key)
                return CompanyRoles.Admin.Value;
            if (type == CompanyTitleType.Admission.Key)
                return CompanyRoles.Admission.Value;
            if (type == CompanyTitleType.Counselor.Key)
                return CompanyRoles.Counselor.Value;
            if (type == CompanyTitleType.Leader.Key)
                return CompanyRoles.Leader.Value;
            if (type == CompanyTitleType.Marketing.Key)
                return CompanyRoles.Marketing.Value;
            return string.Empty;
        }

        /// <summary>
        /// Cập nhật lại role cho user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public static bool UpdateUserRole(string userName, string role)
        {
            var allRoles = Roles.GetRolesForUser(userName);
            if(allRoles != null)
            {
                foreach (var item in allRoles)
                {
                    if(!item.ToLower().Equals(role.ToLower()))
                    {
                        Roles.RemoveUserFromRole(userName, item);
                    }
                }
            }
            if(!Roles.IsUserInRole(userName, role))
            {
                Roles.AddUsersToRole(new string[] { userName }, role);
            }
            return true;
        }
    }
}
