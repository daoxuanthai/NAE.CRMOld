using System;
using System.Collections.Generic;
using System.Linq;

namespace tcs.bo
{
    [Serializable]
    public class AccountInfo
    {
        public int UserId { get; set; }

        /// <summary>
        /// Mọi phân quyền đều dựa trên titleId
        /// </summary>
        public int TitleId { get; set; }

        public string TitleCode { get; set; }

        /// <summary>
        /// Loại tài khoản
        /// </summary>
        public int TitleType { get; set; }

        /// <summary>
        /// ID văn phòng theo title
        /// </summary>
        public int OfficeByTitle { get; set; }

        public int[] ListOfficeByTitle { get; set; }

        /// <summary>
        /// Có quyền xem data all văn phòng
        /// </summary>
        public bool IsViewAll {  get; set; }

        public string UserName { get; set; }

        /// <summary>
        /// Thông tin công ty mà user khi đăng nhập đã chọn
        /// </summary>
        public int CompanyId { get; set; }
        
        /// <summary>
        /// Danh sách công ty mà user có tham gia
        /// </summary>
        public List<CompanyBo> ListCompany { get; set; }

        /// <summary>
        /// Danh sách vị trí trong các công ty user có tham gia
        /// </summary>
        public List<CompanyTitleBo> ListCompanyTitle { get; set; }

        /// <summary>
        /// Danh sách tài khoản mà user được phép phân quyền
        /// </summary>
        public List<CompanyTitleBo> ListTitle { get; set; }

        /// <summary>
        /// Danh sách role mà user được phân quyền
        /// </summary>
        public string[] ListRole { get; set; }

        public bool IsSysAdmin { get; set; }

        public AccountPermission Permission { get; set; }

        /// <summary>
        /// Vị trí có thể xem toàn bộ thông tin như Director, Admin, Marketing, Accountant
        /// </summary>
        /// <returns></returns>
        public bool IsAdminGroup()
        {
            return TitleType == CompanyTitleType.Director.Key ||
                TitleType == CompanyTitleType.Admin.Key ||
                TitleType == CompanyTitleType.Marketing.Key ||
                TitleType == CompanyTitleType.Accountant.Key ||
                (TitleType == CompanyTitleType.Leader.Key && IsViewAll);
        }

        public List<CompanyTitleBo> GetListTitleByOfficeId(int officeId) {
            if (ListTitle == null)
                return null;

            return ListTitle.Where(t => t.OfficeId == officeId || officeId == -1)?.ToList();
        }
    }

    [Serializable]
    public class UserAction : Constants
    {
        public static UserAction Instant()
        {
            return new UserAction();
        }

        public static ConstantsValue Insert = new ConstantsValue(0, "Thêm mới");
        public static ConstantsValue Update = new ConstantsValue(1, "Cập nhật");
        public static ConstantsValue Delete = new ConstantsValue(2, "Xóa");
    }

    /// <summary>
    /// Thông tin phân quyền của user
    /// </summary>
    [Serializable]
    public class AccountPermission
    {

        #region Customer

        #endregion

        #region Contract

        /// <summary>
        /// Quyền quản lý hợp đồng
        /// </summary>
        public bool ContractManager { get; set; }

        /// <summary>
        /// Quyền phân quyền cho admission
        /// </summary>
        public bool ContractAdmission { get; set; }

        #endregion
    }
}
