using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using tcs.bo;

namespace tcs.crm.Models
{
    public class AccountModel : BaseModel
    {
        public bool IsLock { get; set; }
        public List<UserBo> ListUser { get; set; }
    }

    public class AccountInsertModel : BaseInsertModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên tài khoản")]
        [MinLength(5, ErrorMessage = "Tên tài khoản tối thiểu 5 ký tự")]
        [MaxLength(50, ErrorMessage = "Tên tài khoản tối đa 50 ký tự")]
        public string UserName { get; set; }

        [MaxLength(50, ErrorMessage = "Mật khẩu tối đa 50 ký tự")]
        public string Password { get; set; }

        [MaxLength(128, ErrorMessage = "Hình đại diện tối đa 50 ký tự")]
        public string Avatar { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [MinLength(5, ErrorMessage = "Họ và tên tối thiểu 5 ký tự")]
        [MaxLength(128, ErrorMessage = "Tên tài khoản tối đa 128 ký tự")]
        public string FullName { get; set; }

        [MaxLength(15, ErrorMessage = "Số điện thoại tối đa 15 ký tự")]
        public string Phone { get; set; }

        [MaxLength(50, ErrorMessage = "Email tối đa 50 ký tự")]
        public string Email { get; set; }

        [MaxLength(255, ErrorMessage = "Địa chỉ tối đa 255 ký tự")]
        public string Address { get; set; }

        [MaxLength(500, ErrorMessage = "Thông tin ghi chú tối đa 255 ký tự")]
        public string Note { get; set; }

        public bool IsLock { get; set; }
    }

    public class LoginModel
    {
        [Required(ErrorMessage = "Nhập tên đăng nhập")]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Nhập mật khẩu đăng nhập")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Nhập mật khẩu cũ")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Nhập mật khẩu mới")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Nhập xác nhận mật khẩu")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "Mật khẩu mới và xác nhận không hợp lệ")]
        public string ConfirmPassword { get; set; }

        public int Code { get; set; }
        public string Message { get; set; }
    }

    public class UpdateInfoModel
    {
        public string UserName { get; set; }
        [Required(ErrorMessage = "Nhập họ và tên")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Nhập số điện thoại")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Nhập địa chỉ email")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        public string Email { get; set; }
        public string Address { get; set; }
        public string Avatar { get; set; }
        public HttpPostedFileBase AvatarFile { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
    }

}